using MiniNet.Buffers;
using System;
using System.Collections.Generic;
using System.IO;
using System.Linq;
using System.Net.Sockets;
using System.Runtime.CompilerServices;
using System.Text;
using System.Threading.Tasks;

namespace MiniNet.Sockets
{
    public class NetSocket : INetSocket
    {
        protected Socket BaseSocket { get; private set; }
        protected SocketEventArgPool SocketPool { get; private set; }
        protected BufferPool BufferPool { get; private set; }

        public Action<INetSocket> OnAccepted { get; set; }
        public Action<INetSocket, IBuffer> OnReceived { get; set; }
        public Action<INetSocket> OnSent { get; set; }
        public Action<INetSocket, Exception> OnFault { get; set; }

        public INetAddress Address { get; private set; }
        public Stream Stream { get { return new NetworkStream(BaseSocket); } }
        public bool IsClient { get; private set; }
        public bool IsConnected { get { return BaseSocket.Connected; } }

        public NetSocket(INetAddress address, BufferPool bufferPool, SocketEventArgPool socketPool)
        {
            this.Address = address;
            this.BufferPool = bufferPool;
            this.SocketPool = socketPool;

            this.BaseSocket = new Socket(AddressFamily.InterNetwork, SocketType.Stream, ProtocolType.Tcp);
            this.BaseSocket.LingerState = new LingerOption(true, 0);
            this.BaseSocket.SetSocketOption(SocketOptionLevel.Socket, SocketOptionName.ReuseAddress, true);

            this.IsClient = false;
            this.OnAccepted = delegate { };
            this.OnReceived = delegate { };
            this.OnSent = delegate { };
            this.OnFault = delegate { };
        }

        internal NetSocket(INetAddress address, BufferPool bufferPool, SocketEventArgPool socketPool, Socket baseSocket)
        {
            this.BufferPool = bufferPool;
            this.SocketPool = socketPool;
            this.BaseSocket = baseSocket;
            this.BaseSocket.NoDelay = true;

            this.IsClient = true;
            this.OnAccepted = delegate { };
            this.OnReceived = delegate { };
            this.OnSent = delegate { };
            this.OnFault = delegate { };
        }

        public void Listen(int backlog)
        {
            var endPoint = ((NetAddress)Address).EndPoint;
            BaseSocket.Bind(endPoint);
            BaseSocket.Listen(backlog);
        }

        public void AcceptAsync()
        {
            if (!IsClient)
            {
                var acceptArgs = SocketPool.Pop();
                acceptArgs.Completed += OnSocketCompleted;
                var willRaiseEvent = BaseSocket.AcceptAsync(acceptArgs);
                if (!willRaiseEvent) { AcceptHandle(acceptArgs); }
            }
            else
            {
                RaiseFault("Unable to create connections on a client socket.");
            }
        }

        private void AcceptHandle(SocketAsyncEventArgs args)
        {
            // Capture References
            var acceptedSocket = args.AcceptSocket;
            var socketError = args.SocketError;
            var successful = (args.SocketError == SocketError.Success);
            var aborted = (args.SocketError == SocketError.OperationAborted);

            // Free Resources
            ReleaseSocketArgs(args); // return borrowed event args.

            // Process
            if (successful)
            {
                var clientSocket = new NetSocket(Address, BufferPool, SocketPool, acceptedSocket);
                OnAccepted(clientSocket); // AcceptAsync(); Ask caller to loop.
            }
            else
            {
                Disconnect(acceptedSocket);
                if (!aborted)
                {
                    RaiseFault(socketError, "Failed to handle accepting of a client socket.");
                }
            }
        }

        public void ReceiveAsync()
        {
            if (IsClient)
            {
                var block = BufferPool.Pop();
                var readArgs = SocketPool.Pop();

                readArgs.UserToken = block;
                readArgs.AcceptSocket = BaseSocket;
                readArgs.SetBuffer(block.Bytes, block.Offset, block.Capacity);
                readArgs.Completed += OnSocketCompleted;

                var willRaiseEvent = BaseSocket.ReceiveAsync(readArgs);
                if (!willRaiseEvent) { ReceiveHandle(readArgs); }
            }
            else
            {
                RaiseFault("Only accepted client sockets may read and write.");
            }
        }

        private void ReceiveHandle(SocketAsyncEventArgs args)
        {
            // Capture References
            var clientSocket = args.AcceptSocket;
            var socketError = args.SocketError;
            var block = (IBuffer)args.UserToken;
            var successful = (args.SocketError == SocketError.Success);
            var aborted = (args.SocketError == SocketError.OperationAborted);

            // Process
            try
            {
                if (successful)
                {
                    if (clientSocket.Connected)
                    {
                        //block.Resize(args.BytesTransferred);
                        System.Console.WriteLine("args.BytesTransferred: " + args.BytesTransferred);
                        ((MiniNet.Buffers.Buffer)block).Size = args.BytesTransferred;
                        OnReceived(this, block); // ReceiveAsync(); Ask caller to loop.
                    }
                    else
                    {
                        Disconnect(clientSocket);
                    }
                }
                else
                {
                    Disconnect(clientSocket);
                    if (!aborted)
                    {
                        RaiseFault(socketError, "Failed to handle receiving of socket data.");
                    }
                }
            }
            finally
            {
                // Warning!!!
                //		This finally block is only executed on a function return (i.e. just before the exit point).
                //		This means multiple calls to OnReceived()->ReceiveAsync() without a async thread will use up the Buffer Pool and
                //		only on each function exit point will the pool re-obtain its items.
                // Free Resources
                ReleaseBufferBlock(block); // return the borrowed buffer block
                ReleaseSocketArgs(args); // return borrowed event args.
            }
        }

        public void SendAsync(byte[] bytes)
        {
            if (IsClient)
            {
                var block = BufferPool.Pop();
                var writeArgs = SocketPool.Pop();

                // TODO: loop in case byte[] is greater than block capacity!!!
                var writeSize = (bytes.Length < block.Capacity) ? bytes.Length : block.Capacity;
                System.Buffer.BlockCopy(bytes, 0, block.Bytes, block.Offset, writeSize);
                ((MiniNet.Buffers.Buffer)block).Size = writeSize;

                writeArgs.UserToken = block;
                writeArgs.AcceptSocket = BaseSocket;
                writeArgs.SetBuffer(block.Bytes, block.Offset, block.Size);
                writeArgs.Completed += OnSocketCompleted;

                var willRaiseEvent = BaseSocket.SendAsync(writeArgs);
                if (!willRaiseEvent) { SendHandle(writeArgs); }
            }
            else
            {
                RaiseFault("Only accepted client sockets may read and write.");
            }
        }

        public void SendAsync(IBuffer buffer)
        {
            SendAsync(buffer.Bytes);
        }

        private void SendHandle(SocketAsyncEventArgs args)
        {
            // Capture References
            var clientSocket = args.AcceptSocket;
            var socketError = args.SocketError;
            var block = (IBuffer)args.UserToken;
            var successful = (args.SocketError == SocketError.Success);
            var aborted = (args.SocketError == SocketError.OperationAborted);

            // Process
            try
            {
                if (successful)
                {
                    if (clientSocket.Connected)
                    {
                        ((MiniNet.Buffers.Buffer)block).Size = args.BytesTransferred;
                        OnSent(this); // SendAsync(); Ask caller to loop.
                    }
                    else
                    {
                        Disconnect(clientSocket);
                    }
                }
                else
                {
                    Disconnect(clientSocket);
                    if (!aborted)
                    {
                        RaiseFault(socketError, "Failed to handle sending of socket data.");
                    }
                }
            }
            finally
            {
                // Warning!!!
                //		This finally block is only executed on a function return (i.e. just before the exit point).
                //		This means multiple calls to OnSent()->SendAsync() without a async thread will use up the Buffer Pool and
                //		only on each function exit point will the pool re-obtain its items.
                // Free Resources
                ReleaseBufferBlock(block); // return the borrowed buffer block
                ReleaseSocketArgs(args); // return borrowed event args.
            }
        }

        public void Disconnect()
        {
            Disconnect(BaseSocket);
        }

        private void Disconnect(Socket socket)
        {
            try
            {
                if (socket.Connected)
                {
                    socket.Shutdown(SocketShutdown.Both);
                }
                socket.Close();
            }
            catch (Exception ex)
            {
                RaiseFault(ex);
            }
        }

        private void ReleaseBufferBlock(IBuffer block)
        {
            if (block != null)
            {
                BufferPool.Push(block);
            }
        }

        private void ReleaseSocketArgs(SocketAsyncEventArgs args)
        {
            if (args != null)
            {
                args.Completed -= OnSocketCompleted; // clear previously attached event.

                // On Microsoft's AcceptSocket page, it says,
                // "If not supplied (set to null) before calling the Socket.AcceptAsync method, a new socket will be created automatically."
                // The "new socket is constructed with the same AddressFamily, SocketType, and ProtocolType as the current socket", 
                // which is the listening socket.
                args.AcceptSocket = null;
                args.UserToken = null;

                SocketPool.Push(args);
            }
        }

        private void OnSocketCompleted(object sender, SocketAsyncEventArgs args)
        {
            switch (args.LastOperation)
            {
                case SocketAsyncOperation.Accept:
                    AcceptHandle(args);
                    break;
                case SocketAsyncOperation.Receive:
                    ReceiveHandle(args);
                    break;
                case SocketAsyncOperation.Send:
                    SendHandle(args);
                    break;
                default:
                    throw new Exception("Unsupported operation: " + args.LastOperation);
            }
        }

        private void RaiseFault(SocketError errorCode)
        {
            RaiseFault(new MiniNet.Exceptions.SocketException((int)errorCode));
        }

        private void RaiseFault(string message)
        {
            RaiseFault(new MiniNet.Exceptions.SocketException((int)SocketError.Fault, message));
        }

        private void RaiseFault(SocketError errorCode, string message)
        {
            RaiseFault(new MiniNet.Exceptions.SocketException((int)errorCode, message));
        }

        private void RaiseFault(Exception exception)
        {
            OnFault(this, exception);
        }

        public void Dispose()
        {
            Dispose(true);
            GC.SuppressFinalize(this);
        }

        [MethodImpl(MethodImplOptions.Synchronized)]
        protected virtual void Dispose(bool disposing)
        {
            if (disposing)
            {
                Disconnect(BaseSocket);
                BaseSocket.Dispose();
            }
        }
    }
}

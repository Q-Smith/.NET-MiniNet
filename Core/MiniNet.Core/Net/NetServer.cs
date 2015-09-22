using MiniNet.Buffers;
using MiniNet.Channels;
using MiniNet.Sockets;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Runtime.CompilerServices;
using System.Text;
using System.Threading.Tasks;

namespace MiniNet.Net
{
    public class NetServer : INetServer
    {
        protected SocketEventArgPool SocketArgPool { get; private set; }
        protected BufferPool BufferPool { get; private set; }
        protected INetSocket Socket { get; private set; }

        protected Action<INetServer> OnListen { get; set; }
        protected Action<INetChannel> OnConnect { get; set; }
        protected Action<Exception> OnFault { get; set; }

        public INetAddress Address { get; private set; }
        public INetServerOptions Options { get; private set; }

        public NetServer(INetServerOptions options = null)
        {
            this.Options = options ?? NetServerOptions.Create();
            this.SocketArgPool = new SocketEventArgPool(this.Options.MaxConnections);
            this.BufferPool = new BufferPool(this.Options.BufferSize, this.Options.MaxConnections);

            this.OnListen = delegate { };
            this.OnConnect = delegate { };
            this.OnFault = delegate { };
        }

        public void Bind(int port, string host)
        {
            Address = NetAddress.Resolve(port, host, Options.IPv4Only);
            Socket = new NetSocket(Address, BufferPool, SocketArgPool);
            Socket.OnAccepted = HandleAccept;
            Socket.Listen(Options.MaxAccept);
            OnListen(this);
            StartAccept();
        }

        private void StartAccept()
        {
            Socket.AcceptAsync();
        }

        private void HandleAccept(INetSocket clientSocket)
        {
            // FIXME: !!!Move into higher API calls outside of NetSocket
            // do we "new-up" channels or maintain existing channels?
            // how many "new" instances of objects will we be creating and is that GC efficient?
            // var buffer = new Buffer(args.Buffer, args.Offset, args.BytesTransferred);
            // var channel = new NetChannel(this, buffer); // ??? do we provide the channel or the buffer to the caller?
            var channel = new NetChannel(clientSocket);
            channel.Fault((c, exception) => { OnFault(exception); });

            StartAccept();
            OnConnect(channel);
        }

        public INetServer Listen(Action<INetServer> callback)
        {
            OnListen = callback;
            return this;
        }

        public INetServer Connect(Action<INetChannel> callback)
        {
            OnConnect = callback;
            return this;
        }

        public INetServer Fault(Action<Exception> callback)
        {
            OnFault = callback;
            return this;
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
                if (Socket != null) { Socket.Dispose(); }
            }
        }
    }
}

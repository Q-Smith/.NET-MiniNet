using MiniNet.Utils;
using System;
using System.Collections.Concurrent;
using System.Collections.Generic;
using System.Linq;
using System.Net.Sockets;
using System.Text;
using System.Threading.Tasks;

namespace MiniNet.Sockets
{
    public class SocketEventArgPool : IPool<SocketAsyncEventArgs>
    {
        private ConcurrentStack<SocketAsyncEventArgs> stack = new ConcurrentStack<SocketAsyncEventArgs>();

        public int Size
        {
            get { return stack.Count; }
        }

        public SocketEventArgPool(int initialSize)
        {
            for (var i = 0; i < initialSize; i++)
            {
                var socketArgs = new SocketAsyncEventArgs();
                stack.Push(socketArgs);
            }
        }

        public SocketAsyncEventArgs Pop()
        {
            SocketAsyncEventArgs pop;
            if (!stack.TryPop(out pop))
                throw new Exception("Out of SocketAsyncEventArgs in pool.");

            return pop;
        }

        public void Push(SocketAsyncEventArgs args)
        {
            Ensure.NotNull(args, "args");
            stack.Push(args);
        }

        public void Resize(int newSize)
        {
            throw new NotImplementedException();
        }
    }
}

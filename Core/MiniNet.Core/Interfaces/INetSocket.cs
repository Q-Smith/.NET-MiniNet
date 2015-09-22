using System;
using System.Collections.Generic;
using System.IO;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace MiniNet
{
    public interface INetSocket : IDisposable
    {
        INetAddress Address { get; }
        Stream Stream { get; }
        bool IsClient { get; }
        bool IsConnected { get; }

        void Listen(int backlog);
        void AcceptAsync();
        void ReceiveAsync();
        void SendAsync(byte[] bytes);
        void SendAsync(IBuffer buffer);

        void Disconnect();

        Action<INetSocket> OnAccepted { get; set; }
        Action<INetSocket, IBuffer> OnReceived { get; set; }
        Action<INetSocket> OnSent { get; set; }
        Action<INetSocket, Exception> OnFault { get; set; }
    }
}

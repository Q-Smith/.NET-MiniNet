using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace MiniNet
{
    public interface INetServer : IDisposable
    {
        INetAddress Address { get; }
        INetServerOptions Options { get; }

        void Bind(int port, string host);
        INetServer Listen(Action<INetServer> callback);
        INetServer Connect(Action<INetChannel> callback);
    }
}

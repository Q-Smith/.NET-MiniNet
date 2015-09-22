using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace MiniNet
{
    public interface INetChannel : IDisposable
    {
        void Start();
        void Authenticate();
        void SendAsync(IBuffer buffer);
        void Close();
        INetChannel Message(Action<INetChannel, IBuffer> callback);
        INetChannel Fault(Action<INetChannel, Exception> callback);
    }
}

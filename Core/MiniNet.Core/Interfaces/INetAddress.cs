using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace MiniNet
{
    public interface INetAddress
    {
        int Port { get; }
        string Host { get; }
    }
}

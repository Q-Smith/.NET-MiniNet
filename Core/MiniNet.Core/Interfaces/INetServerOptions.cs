using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace MiniNet
{
    public interface INetServerOptions
    {
        bool IPv4Only { get; set; }
        int MaxAccept { get; set; }
        int MaxConnections { get; set; }
        int BufferSize { get; set; }
        int ReceivePrefixLength { get; set; }
        int SendPrefixLength { get; set; }
    }
}

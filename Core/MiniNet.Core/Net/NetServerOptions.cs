using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace MiniNet.Net
{
    public class NetServerOptions : INetServerOptions
    {
        public bool IPv4Only { get; set; }

        // The listening socket keeps the backlog as a queue. The backlog allows for a certain # of excess clients 
        // waiting to be connected. If the backlog is maxed out, then the client will receive an error when trying to connect.
        public int MaxAccept { get; set; }

        // Maximum number of socket connections to maintain. Might be limited by each Operating System.
        public int MaxConnections { get; set; }

        // You would want a buffer size close to the known data size.
        public int BufferSize { get; set; }

        // TODO: Tells what size the message prefix will be. (currently not used!!)
        public int ReceivePrefixLength { get; set; }
        public int SendPrefixLength { get; set; }

        public static NetServerOptions Create()
        {
            // Set defaults;
            var result = new NetServerOptions();
            result.IPv4Only = true;
            result.MaxAccept = 100; // The size of the queue of incoming connections for the listen socket.
            result.MaxConnections = 10000;
            result.BufferSize = 1024;
            result.ReceivePrefixLength = 4; // is the length of 32 bit integer
            result.SendPrefixLength = 4; // is the length of 32 bit integer
            return result;
        }
    }
}

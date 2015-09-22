using MiniNet.Utils;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Net;
using System.Net.Sockets;
using System.Text;
using System.Threading.Tasks;

namespace MiniNet.Sockets
{
    public class NetAddress : INetAddress
    {
        internal IPEndPoint EndPoint { get; private set; }

        public int Port { get; private set; }
        public string Host { get; private set; }

        public NetAddress(int port, string host, bool ip4Only = true)
        {
            Ensure.GreaterThan(-1, port, "port");
            Ensure.NotNull(host, "host");

            this.EndPoint = ResolveInternal(port, host, ip4Only);
            this.Port = this.EndPoint.Port;
            this.Host = this.EndPoint.Address.ToString();
        }

        internal NetAddress(EndPoint endPoint)
        {
            Ensure.NotNull(endPoint, "endPoint");

            this.EndPoint = (IPEndPoint)endPoint;
            this.Port = this.EndPoint.Port;
            this.Host = this.EndPoint.Address.ToString();
        }

        public override string ToString()
        {
            return EndPoint.ToString();
        }

        // Given a host name, resolve it to an actual IP address
        // System.Net.IPAddress.Any = 0.0.0.0 in dotted-quad notation.
        // System.Net.IPAddress.Loopback = 127.0.0.1 in dotted-quad notation.
        // System.Net.IPAddress.Broadcast = 255.255.255.255 in dotted-quad notation.
        protected virtual IPEndPoint ResolveInternal(int port, string host, bool ip4Only = true)
        {
            IPAddress ipAddress;

            if (host == "*")
            {
                ipAddress = ip4Only ? IPAddress.Any : IPAddress.IPv6Any;
            }
            else if (!IPAddress.TryParse(host, out ipAddress))
            {
                var availableAddresses = Dns.GetHostEntry(host).AddressList;
                if (ip4Only)
                {
                    ipAddress = availableAddresses.FirstOrDefault(ip => ip.AddressFamily == AddressFamily.InterNetwork);
                }
                else
                {
                    ipAddress = availableAddresses.FirstOrDefault(ip => ip.AddressFamily == AddressFamily.InterNetwork || ip.AddressFamily == AddressFamily.InterNetworkV6);
                }
            }

            if (ipAddress == null)
                throw new ArgumentException(string.Format("Resolve(), unable to find an IP address for {0}", host));

            return new IPEndPoint(ipAddress, port);
        }

        public static INetAddress Resolve(int port, string host, bool ip4Only = true)
        {
            return new NetAddress(port, host, ip4Only);
        }
    }
}

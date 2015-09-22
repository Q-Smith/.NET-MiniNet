using MiniNet.Channels;
using MiniNet.Net;
using System;
using System.Collections.Generic;
using System.IO;
using System.Linq;
using System.Net.Security;
using System.Net.Sockets;
using System.Security.Cryptography.X509Certificates;
using System.Text;
using System.Threading;
using System.Threading.Tasks;

namespace SimpleConsole
{
    public class Program
    {
        public static void Main(string[] args)
        {
            try
            {
                using (var server = new NetServer())
                {
                    server.Listen((self) =>
                    {
                        System.Console.WriteLine("Server is listening on: " + self.Address.ToString());
                    });
                    server.Connect((channel) =>
                    {
                        System.Console.WriteLine("Client connected.");

                        channel.Fault((c, exception) =>
                        {
                            System.Console.WriteLine(((MiniNet.Exceptions.SocketException)exception).StatusCode.ToString());
                            System.Console.WriteLine(exception.ToString());
                        });

                        var encoding = Encoding.Unicode;
                        var decoder = new StringDecoder(encoding);
                        var encoder = new StringEncoder(encoding);

                        //channel.Authenticate();
                        channel.Start();

                        channel.Message((self, buffer) =>
                        {
                            if (buffer.Size > 0)
                            {
                                var str = decoder.Process(buffer);
                                System.Console.WriteLine(str + "-" + buffer.Size + "-");
                                System.Console.WriteLine(buffer.DumpHex(int.MaxValue));
                            }
                        });

                        /*var payload = "Server: Hello Client";
                        var message = MiniNet.Buffers.Buffer.Create(encoding.GetByteCount(payload));

                        encoder.Process(message, payload);
                        channel.SendAsync(message);*/

                        Thread.Sleep(1000);
                        //channel.Close();
                    });
                    server.Fault((exception) =>
                    {
                        System.Console.WriteLine(exception.ToString());
                    });
                    server.Bind(8080, "localhost");

                    using (var client = new TcpClient())
                    {
                        client.NoDelay = true;
                        client.Connect(server.Address.Host, server.Address.Port);
                        Thread.Sleep(100);

                        var encoding = new UnicodeEncoding(false, false, true);
                        var stream = client.GetStream();
                        //var reader = new StreamReader(stream, encoding);
                        var writer = new StreamWriter(stream, encoding) { AutoFlush = true };

                        for (int i=1; i<=4; i++)
                        {
                            /*var bytesReceived = new byte[1024];
                            var lengthReceived = client.GetStream().Read(bytesReceived, 0, bytesReceived.Length);
                            if (lengthReceived > 0)
                            {
                                var message = encoding.GetString(bytesReceived, 0, lengthReceived);
                                System.Console.WriteLine(MiniNet.Buffers.Buffer.DumpHex(bytesReceived, 0, lengthReceived, int.MaxValue));
                                System.Console.WriteLine("'" + message + "'" + lengthReceived);
                            }*/

                            writer.Write("Client: Hello Server");
                        }

                        Thread.Sleep(100);
                        System.Console.WriteLine("Press ENTER to quit");
                        System.Console.ReadLine();
                        client.Close();
                    }
                }
            }
            catch (Exception ex)
            {
                System.Console.WriteLine(ex.ToString());
                System.Console.WriteLine(ex.StackTrace);
            }
        }

        public static bool ValidateServerCertificate(
              object sender,
              X509Certificate certificate,
              X509Chain chain,
              SslPolicyErrors sslPolicyErrors)
        {
            if (sslPolicyErrors == SslPolicyErrors.None)
                return true;

            System.Console.WriteLine("Certificate error: {0}", sslPolicyErrors);

            // Do not allow this client to communicate with unauthenticated servers. 
            return false;
        }
    }
}

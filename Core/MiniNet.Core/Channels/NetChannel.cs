using System;
using System.Collections.Generic;
using System.Linq;
using System.Net.Security;
using System.Runtime.CompilerServices;
using System.Security.Cryptography.X509Certificates;
using System.Text;
using System.Threading.Tasks;

namespace MiniNet.Channels
{
    public class NetChannel : INetChannel
    {
        protected X509Certificate2 Certificate { get; private set; }
        protected INetSocket Socket { get; private set; }
        protected Action<INetChannel, IBuffer> OnMessage { get; set; }
        protected Action<INetChannel, Exception> OnFault { get; set; }

        public bool IsConnected
        {
            get
            {
                return Socket.IsConnected;
            }
        }

        public NetChannel(INetSocket Socket)
        {
            this.Socket = Socket;
            this.OnMessage = delegate { };
            this.OnFault = delegate { };

            this.Socket.OnReceived = HandleDataReceived;
            this.Socket.OnFault = (socket, exception) => { OnFault(this, exception); };

            //this.certificate = new X509Certificate2(@"C:\Programs\OpenSSL\openssl-1.0.2d\cert.pem", "password");
            //this.certificate.PrivateKey 
        }

        public void Start()
        {
            if (IsConnected)
            {
                ReceiveData();
            }
        }

        public void Authenticate()
        {
            var sslStream = new SslStream(Socket.Stream, false);
            if (!sslStream.IsAuthenticated)
            {
                sslStream.AuthenticateAsServer(Certificate);
                DisplaySecurityLevel(sslStream);
                DisplaySecurityServices(sslStream);
                DisplayCertificateInformation(sslStream);
                DisplayStreamProperties(sslStream);

                // Set timeouts for the read and write to 5 seconds.
                sslStream.ReadTimeout = 5000;
                sslStream.WriteTimeout = 5000;
            }
        }

        public void SendAsync(IBuffer buffer)
        {
            if (IsConnected && buffer.Size > 0)
            {
                Socket.SendAsync(buffer);
            }
        }

        private void ReceiveData()
        {
            if (IsConnected)
            {
                Socket.ReceiveAsync();
            }
        }

        public void Close()
        {
            Socket.Disconnect();
        }

        private void HandleDataReceived(INetSocket clientSocket, IBuffer buffer)
        {
            if (buffer.Size > 0)
            {
                ReceiveData();
            }
            OnMessage(this, buffer);
        }

        public INetChannel Message(Action<INetChannel, IBuffer> callback)
        {
            OnMessage = callback;
            return this;
        }

        public INetChannel Fault(Action<INetChannel, Exception> callback)
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
                Socket.Dispose();
            }
        }

        private static void DisplaySecurityLevel(SslStream stream)
        {
            System.Console.WriteLine("Cipher: {0} strength {1}", stream.CipherAlgorithm, stream.CipherStrength);
            System.Console.WriteLine("Hash: {0} strength {1}", stream.HashAlgorithm, stream.HashStrength);
            System.Console.WriteLine("Key exchange: {0} strength {1}", stream.KeyExchangeAlgorithm, stream.KeyExchangeStrength);
            System.Console.WriteLine("Protocol: {0}", stream.SslProtocol);
        }

        private static void DisplaySecurityServices(SslStream stream)
        {
            System.Console.WriteLine("Is authenticated: {0} as server? {1}", stream.IsAuthenticated, stream.IsServer);
            System.Console.WriteLine("IsSigned: {0}", stream.IsSigned);
            System.Console.WriteLine("Is Encrypted: {0}", stream.IsEncrypted);
        }

        private static void DisplayStreamProperties(SslStream stream)
        {
            System.Console.WriteLine("Can read: {0}, write {1}", stream.CanRead, stream.CanWrite);
            System.Console.WriteLine("Can timeout: {0}", stream.CanTimeout);
        }

        private static void DisplayCertificateInformation(SslStream stream)
        {
            System.Console.WriteLine("Certificate revocation list checked: {0}", stream.CheckCertRevocationStatus);

            X509Certificate localCertificate = stream.LocalCertificate;
            if (stream.LocalCertificate != null)
            {
                System.Console.WriteLine("Local cert was issued to {0} and is valid from {1} until {2}.",
                    localCertificate.Subject,
                    localCertificate.GetEffectiveDateString(),
                    localCertificate.GetExpirationDateString());
            }
            else
            {
                System.Console.WriteLine("Local certificate is null.");
            }
            // Display the properties of the client's certificate.
            X509Certificate remoteCertificate = stream.RemoteCertificate;
            if (stream.RemoteCertificate != null)
            {
                System.Console.WriteLine("Remote cert was issued to {0} and is valid from {1} until {2}.",
                    remoteCertificate.Subject,
                    remoteCertificate.GetEffectiveDateString(),
                    remoteCertificate.GetExpirationDateString());
            }
            else
            {
                System.Console.WriteLine("Remote certificate is null.");
            }
        }
    }
}

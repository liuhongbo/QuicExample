using System;
using System.Net;
using System.Net.Quic;
using System.Net.Security;
using System.Runtime.Versioning;
using System.Security.Cryptography.X509Certificates;
using System.Text;
#pragma warning disable CA1416

namespace QuickClientExample
{
    [RequiresPreviewFeatures]
    class Program
    {
        static async Task Main(string[] args)
        {
            var isRunning = true;

            if (!QuicListener.IsSupported)
            {
                Console.WriteLine("QUIC is not supported.");
                return;
            }
            var protocol = "quic-test";
            var pfxFilePath = "D:\\Medium\\QuicExample\\contoso.com.pfx";
            var pfxPassword = "password";
            var port = 8000;

            var serverConnectionOptions = new QuicServerConnectionOptions()
            {
                DefaultStreamErrorCode = 0x0A,
                DefaultCloseErrorCode = 0x0B,                         
                ServerAuthenticationOptions = new SslServerAuthenticationOptions
                {
                    ApplicationProtocols = new List<SslApplicationProtocol>() { new SslApplicationProtocol(protocol) },                    
                    ServerCertificate = new X509Certificate2(pfxFilePath, pfxPassword)
                }
            };            

            var listener = await QuicListener.ListenAsync(new QuicListenerOptions()
            {
                ListenEndPoint = new IPEndPoint(IPAddress.Loopback, port),
                ApplicationProtocols = new List<SslApplicationProtocol>() { new SslApplicationProtocol(protocol) },
                ConnectionOptionsCallback = (_, _, _) => ValueTask.FromResult(serverConnectionOptions)
            });           


            while (isRunning)
            {
                var connection = await listener.AcceptConnectionAsync();
                var stream = await connection.AcceptInboundStreamAsync();
                
                // Work with the incoming stream ...
                var buf = new byte[128];
                var len = await stream.ReadAsync(buf, 0, buf.Length);
                Console.WriteLine(Encoding.UTF8.GetString(buf, 0, len));                
            }


            await listener.DisposeAsync();
        }
    }
}
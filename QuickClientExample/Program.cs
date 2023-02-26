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

            if (!QuicConnection.IsSupported)
            {
                Console.WriteLine("QUIC is not supported.");
                return;
            }

            var protocol = "quic-test";
            var port = 8000;

            var clientConnectionOptions = new QuicClientConnectionOptions()
            {
                RemoteEndPoint = new IPEndPoint(IPAddress.Loopback, port),
                DefaultStreamErrorCode = 0x0A,
                DefaultCloseErrorCode = 0x0B,
                MaxInboundUnidirectionalStreams = 10,
                MaxInboundBidirectionalStreams = 100,
                ClientAuthenticationOptions = new SslClientAuthenticationOptions()
                {
                    ApplicationProtocols = new List<SslApplicationProtocol>() { new SslApplicationProtocol(protocol) }
                }
            };
            var connection = await QuicConnection.ConnectAsync(clientConnectionOptions);
            var outgoingStream = await connection.OpenOutboundStreamAsync(QuicStreamType.Bidirectional);

            var str = "hello world";
            var buf = Encoding.UTF8.GetBytes(str);
            await outgoingStream.WriteAsync(buf, 0, buf.Length);

            Console.ReadKey();

            await connection.CloseAsync(0x0C);
            await connection.DisposeAsync();
        }
    }
}
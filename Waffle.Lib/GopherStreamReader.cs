using System;
using System.Collections.Generic;
using System.Diagnostics;
using System.Linq;
using System.Net.Sockets;
using System.Text;
using System.Threading.Tasks;

namespace Waffle.Lib
{
    class GopherStreamReader : IDisposable
    {
        private Socket socc;
        private bool opened = false;

        public async Task OpenAsync(string absoluteUrl)
        {
            if (opened)
            {
                throw new InvalidOperationException("Reader already opened!");
            }

            opened = true;

            var host = ParseHost(absoluteUrl);
            var port = ParsePort(absoluteUrl);
            var path = ParsePath(absoluteUrl);

            var data = Encoding.ASCII.GetBytes($"{path}\r\n");

            socc = new Socket(SocketType.Stream, ProtocolType.Tcp);

            await socc.ConnectAsync(host, port.GetValueOrDefault(70));

            await socc.SendAsync(data, SocketFlags.None);
        }

        public async Task<string[]> ReadAllLinesAsync()
        {
            var bytes = await ReadAllBytesAsync();

            var responseString = Encoding.ASCII.GetString(bytes);

            var lines = responseString.Split("\r\n");

            return lines.ToArray();
        }

        public async Task<byte[]> ReadAllBytesAsync()
        {
            var byteLine = new List<byte>();

            while (true)
            {
                var responseBuffer = new byte[5_000_000];

                var bytesRead = await socc.ReceiveAsync(responseBuffer, SocketFlags.None);

                if (bytesRead == 0)
                {
                    break;
                }

                byteLine.AddRange(responseBuffer[..bytesRead]);
            }

            return byteLine.ToArray();
        }

        public void Dispose()
        {
            socc.Dispose();
        }

        private static string ParseHost(string absoluteUrl)
        {
            var host = absoluteUrl;

            if (host.Contains('/'))
            {
                host = host[..host.IndexOf('/')];
            }

            if (host.Contains(':'))
            {
                host = host[..host.IndexOf(':')];
            }

            return host;
        }

        private static int? ParsePort(string absoluteUrl)
        {
            int? port = null;

            var host = absoluteUrl;

            if (host.Contains('/'))
            {
                host = host[..host.IndexOf('/')];
            }

            if (host.Contains(':'))
            {
                var parts = host.Split(':');

                if (int.TryParse(parts[1], out int parsedPort))
                {
                    port = parsedPort;
                }
                else
                {
                    throw new ArgumentException($"Bad port value: {absoluteUrl}");
                }

                return port;
            }

            return null;
        }

        private static string ParsePath(string absoluteUrl)
        {
            string path = null;

            if (absoluteUrl.Contains('/'))
            {
                path = absoluteUrl[(absoluteUrl.IndexOf('/') + 1)..];
            }

            if (path == null)
            {
                return null;
            }

            if (!path.Contains('/'))
            {
                return path;
            }

            if (path[..path.IndexOf('/')].Length == 1)
            {
                path = path[(path.IndexOf('/') + 1)..];
            }

            return path;
        }
    }
}

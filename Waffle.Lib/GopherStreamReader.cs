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

            var (host, urlPart) = ParseHostAndUrl(absoluteUrl);

            urlPart = StripLeadingItemType(urlPart);

            var data = Encoding.ASCII.GetBytes($"{urlPart}\r\n");

            socc = new Socket(SocketType.Stream, ProtocolType.Tcp);

            await socc.ConnectAsync(host, 70);

            await socc.SendAsync(data, SocketFlags.None);
        }

        private string StripLeadingItemType(string urlPart)
        {
            if (!urlPart.Contains('/'))
            {
                return urlPart;
            }

            if(urlPart[..urlPart.IndexOf('/')].Length == 1)
            {
                return urlPart[(urlPart.IndexOf('/') + 1)..];
            }

            return urlPart;
        }

        public static (string Host, string Url) ParseHostAndUrl(string absoluteUrl)
        {
            if (!absoluteUrl.Contains('/'))
            {
                return (Host: absoluteUrl, Url: "");
            }

            var host = absoluteUrl[..absoluteUrl.IndexOf('/')];
            var urlPart = absoluteUrl[(absoluteUrl.IndexOf('/') + 1)..];

            return (host, urlPart);
        }

        public bool HasMoreLines
        {
            get
            {
                if (socc == null)
                {
                    return false;
                }

                var started = DateTime.Now;

                while (socc.Available == 0)
                {
                    var waitFor = TimeSpan.FromMilliseconds(200);

                    if (DateTime.Now.Subtract(started).Milliseconds > waitFor.Milliseconds)
                    {
                        return false;
                    }
                }

                return socc.Available > 0;
            }
        }

        public async Task<string[]> ReadAllLinesAsync()
        {
            var lines = new List<string>();

            while (HasMoreLines)
            {
                var line = await ReadLineAsync();

                lines.Add(line);
            }

            return lines.ToArray();
        }

        public async Task<string> ReadLineAsync()
        {
            if (!HasMoreLines)
            {
                throw new InvalidOperationException("No more lines to read! :(");
            }

            var byteLine = new List<byte>();

            while (true)
            {
                var responseBuffer = new byte[1];

                var bytesRead = await socc.ReceiveAsync(responseBuffer, SocketFlags.None);

                if (bytesRead == 0)
                {
                    break;
                }

                var currentByte = responseBuffer[0];
                byte? lastByte = byteLine.Any() ? byteLine.Last() : null;

                byteLine.Add(currentByte);

                if (currentByte == 10 && lastByte.GetValueOrDefault() == 13)
                {
                    break;
                }
            }

            var responseString = Encoding.ASCII.GetString(byteLine.ToArray());

            return responseString;
        }

        public void Dispose()
        {
            socc.Dispose();
        }
    }
}

using System;
using System.Collections.Generic;
using System.Linq;
using System.Net;
using System.Net.Sockets;
using System.Text;
using System.Threading.Tasks;

namespace Waffle
{
    public class WaffleLib
    {
        public async Task<Response> SendRequestAsync(string url)
        {
            ValidateUrl(url);

            var parsedUrl = ParseUrl(url);

            using var reader = new GopherStreamReader();

            await reader.OpenAsync(parsedUrl);

            var lines = await reader.ReadAllLinesAsync();

            return new Response()
            {
                Lines = lines.Select(line => new SelectorLine(line)).ToArray(),
            };
        }

        public async Task<string> GetTextFileAsync(string url)
        {
            ValidateUrl(url);

            var parsedUrl = ParseUrl(url);

            using var reader = new GopherStreamReader();

            await reader.OpenAsync(parsedUrl);

            var lines = await reader.ReadAllLinesAsync();

            var fileContents = new StringBuilder();

            foreach (var line in lines)
            {
                if (line != ".\r\n")
                {
                    fileContents.Append(line);
                }
            }

            return fileContents.ToString();
        }

        private static void ValidateUrl(string url)
        {
            ArgumentNullException.ThrowIfNull(url);

            if (url.Trim() == "")
            {
                throw new InvalidOperationException("Empty url not allowed.");
            }
        }

        private static string ParseUrl(string url)
        {
            const string Protocol = "gopher://";

            url = url.Trim();

            if (url.StartsWith(Protocol))
            {
                url = url[(url.IndexOf(Protocol) + Protocol.Length)..];
            }

            if (url.EndsWith("/"))
            {
                url = url[0..^1];
            }

            return url;
        }
    }
}

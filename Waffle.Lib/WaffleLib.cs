using System;
using System.Collections.Generic;
using System.Drawing;
using System.Linq;
using System.Net;
using System.Net.Sockets;
using System.Text;
using System.Threading.Tasks;

namespace Waffle.Lib
{
    public class WaffleLib
    {
        public async Task<Response> GetAsync<T>(string absoluteUrl) where T : Response
        {
            var responseType = GetLinkType(absoluteUrl);

            return responseType switch
            {
                ResponseType.Menu => await GetMenuAsync(absoluteUrl),
                ResponseType.TextFile => await GetTextFileAsync(absoluteUrl),
                ResponseType.PNG => await GetPngFileAsync(absoluteUrl),
                _ => throw new InvalidOperationException($"Unknown link type: {absoluteUrl}"),
            };
        }

        public async Task<MenuResponse> GetMenuAsync(string absoluteUrl)
        {
            ValidateUrl(absoluteUrl);

            var parsedUrl = ParseUrl(absoluteUrl);

            using var reader = new GopherStreamReader();

            await reader.OpenAsync(parsedUrl);

            var lines = await reader.ReadAllLinesAsync();

            return new MenuResponse()
            {
                Lines = lines.Select(line => new SelectorLine(line)).ToArray(),
            };
        }

        public async Task<TextResponse> GetTextFileAsync(string absoluteUrl)
        {
            ValidateUrl(absoluteUrl);

            var parsedUrl = ParseUrl(absoluteUrl);

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

            return new TextResponse()
            {
                Text = fileContents.ToString(),
            };
        }

        public async Task<PngResponse> GetPngFileAsync(string absoluteUrl)
        {
            ValidateUrl(absoluteUrl);

            var parsedUrl = ParseUrl(absoluteUrl);

            using var reader = new GopherStreamReader();

            await reader.OpenAsync(parsedUrl);

            var bytes = await reader.ReadPng();

            return new PngResponse()
            {
                Image = Image.FromStream(new MemoryStream(bytes)),
            };
        }

        public ResponseType GetLinkType(string absoluteUrl)
        {
            ValidateUrl(absoluteUrl);

            var parsedUrl = ParseUrl(absoluteUrl);

            var (_, urlPart) = GopherStreamReader.ParseHostAndUrl(parsedUrl);

            if (urlPart.StartsWith('0'))
            {
                return ResponseType.TextFile;
            }
            else if (urlPart.StartsWith('1'))
            {
                return ResponseType.Menu;
            }
            else if (urlPart.StartsWith('p'))
            {
                return ResponseType.PNG;
            }
            else
            {
                return ResponseType.Unknown;
            }
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

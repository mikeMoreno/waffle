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
            var responseType = GetItemType(absoluteUrl);

            return responseType switch
            {
                ItemType.Menu => await GetMenuAsync(absoluteUrl),
                ItemType.Text => await GetTextFileAsync(absoluteUrl),
                ItemType.PNG => await GetPngFileAsync(absoluteUrl),
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
                if (line != ".")
                {
                    fileContents.AppendLine(line);
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

            var bytes = await reader.ReadAllBytesAsync();

            return new PngResponse()
            {
                Image = Image.FromStream(new MemoryStream(bytes)),
            };
        }

        public async Task<ImageResponse> GetImageFileAsync(string absoluteUrl)
        {
            ValidateUrl(absoluteUrl);

            var parsedUrl = ParseUrl(absoluteUrl);

            using var reader = new GopherStreamReader();

            await reader.OpenAsync(parsedUrl);

            var bytes = await reader.ReadAllBytesAsync();

            return new ImageResponse()
            {
                Image = Image.FromStream(new MemoryStream(bytes)),
            };
        }

        public async Task<BinaryResponse> GetBinaryFile(string absoluteUrl)
        {
            ValidateUrl(absoluteUrl);

            var parsedUrl = ParseUrl(absoluteUrl);

            using var reader = new GopherStreamReader();

            await reader.OpenAsync(parsedUrl);

            return new BinaryResponse()
            {
                Bytes = await reader.ReadAllBytesAsync(),
            };
        }

        public ItemType GetItemType(string absoluteUrl)
        {
            ValidateUrl(absoluteUrl);

            var parsedUrl = ParseUrl(absoluteUrl);

            var itemType = ParseItemType(parsedUrl);

            if (itemType == "0")
            {
                return ItemType.Text;
            }
            else if (itemType == "1")
            {
                return ItemType.Menu;
            }
            else if (itemType == "p")
            {
                return ItemType.PNG;
            }
            else if (itemType == "I")
            {
                return ItemType.Image;
            }
            else if (itemType == "9")
            {
                return ItemType.BinaryFile;
            }
            else
            {
                return ItemType.Unknown;
            }
        }

        private static string ParseItemType(string absoluteUrl)
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

            if (path.Length <= 1)
            {
                return path;
            }

            if (!path.Contains('/'))
            {
                return path;
            }

            if (path[..path.IndexOf('/')].Length == 1)
            {
                path = path[..path.IndexOf('/')];
            }

            return path;
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

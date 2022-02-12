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
        public async Task<Response> GetAsync(string absoluteUrl)
        {
            // TODO: until / if we can just pass around SelectorLines / LinkLines
            var itemType = SelectorLine.GetItemType(absoluteUrl);

            return itemType switch
            {
                ItemType.Menu => await GetMenuAsync(absoluteUrl),
                ItemType.Text => await GetTextFileAsync(absoluteUrl),
                ItemType.PNG => await GetPngFileAsync(absoluteUrl),
                _ => await GetMenuAsync(absoluteUrl),
            };
        }

        public async Task<Response> GetAsync(SelectorLine selectorLine)
        {
            var link = selectorLine.GetLink();

            return selectorLine.ItemType switch
            {
                ItemType.Menu => await GetMenuAsync(link),
                ItemType.Text => await GetTextFileAsync(link),
                ItemType.PNG => await GetPngFileAsync(link),
                _ => await GetMenuAsync(link),
            };
        }

        public async Task<MenuResponse> GetMenuAsync(string absoluteUrl)
        {
            UrlValidator.ValidateUrl(absoluteUrl);

            var parsedUrl = UrlValidator.ParseUrl(absoluteUrl);

            using var reader = new GopherStreamReader();

            try
            {
                await reader.OpenAsync(parsedUrl);

                var lines = await reader.ReadAllLinesAsync();

                return new MenuResponse()
                {
                    Lines = lines.Select(line => new SelectorLine(line)).ToArray(),
                };
            }
            catch (Exception e)
            {
                return Response.Error<MenuResponse>(e);
            }
        }

        public async Task<TextResponse> GetTextFileAsync(string absoluteUrl)
        {
            UrlValidator.ValidateUrl(absoluteUrl);

            var parsedUrl = UrlValidator.ParseUrl(absoluteUrl);

            using var reader = new GopherStreamReader();

            string[] lines;

            try
            {
                await reader.OpenAsync(parsedUrl);

                lines = await reader.ReadAllLinesAsync();
            }
            catch (Exception e)
            {
                return Response.Error<TextResponse>(e);
            }

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
            UrlValidator.ValidateUrl(absoluteUrl);

            var parsedUrl = UrlValidator.ParseUrl(absoluteUrl);

            using var reader = new GopherStreamReader();

            try
            {
                await reader.OpenAsync(parsedUrl);

                var bytes = await reader.ReadAllBytesAsync();

                return new PngResponse()
                {
                    Image = Image.FromStream(new MemoryStream(bytes)),
                };
            }
            catch (Exception e)
            {
                return Response.Error<PngResponse>(e);
            }
        }

        public async Task<ImageResponse> GetImageFileAsync(string absoluteUrl)
        {
            UrlValidator.ValidateUrl(absoluteUrl);

            var parsedUrl = UrlValidator.ParseUrl(absoluteUrl);

            using var reader = new GopherStreamReader();

            try
            {
                await reader.OpenAsync(parsedUrl);

                var bytes = await reader.ReadAllBytesAsync();

                return new ImageResponse()
                {
                    Image = Image.FromStream(new MemoryStream(bytes)),
                };
            }
            catch (Exception e)
            {
                return Response.Error<ImageResponse>(e);
            }
        }

        public async Task<BinaryResponse> GetBinaryFile(string absoluteUrl)
        {
            UrlValidator.ValidateUrl(absoluteUrl);

            var parsedUrl = UrlValidator.ParseUrl(absoluteUrl);

            using var reader = new GopherStreamReader();

            try
            {
                await reader.OpenAsync(parsedUrl);

                return new BinaryResponse()
                {
                    Bytes = await reader.ReadAllBytesAsync(),
                };
            }
            catch (Exception e)
            {
                return Response.Error<BinaryResponse>(e);
            }
        }
    }
}

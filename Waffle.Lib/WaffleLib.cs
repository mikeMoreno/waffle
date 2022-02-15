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
        public async Task<Response> GetAsync(SelectorLine selectorLine)
        {
            var link = selectorLine.GetLink();

            return selectorLine.ItemType switch
            {
                ItemType.Menu => await GetMenuAsync(link),
                ItemType.Text => await GetTextFileAsync(link),
                ItemType.PNG => await GetPngFileAsync(link),
                ItemType.Image => await GetImageFileAsync(link),
                ItemType.BinaryFile => await GetBinaryFile(link),
                ItemType.HTML => GetHtmlLink(link),
                _ => await GetMenuAsync(link),
            };
        }

        private async Task<MenuResponse> GetMenuAsync(string absoluteUrl)
        {
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

        private async Task<TextResponse> GetTextFileAsync(string absoluteUrl)
        {
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

        private async Task<PngResponse> GetPngFileAsync(string absoluteUrl)
        {
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

        private async Task<ImageResponse> GetImageFileAsync(string absoluteUrl)
        {
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

        private async Task<BinaryResponse> GetBinaryFile(string absoluteUrl)
        {
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

        private HtmlResponse GetHtmlLink(string absoluteUrl)
        {
            return new HtmlResponse()
            {
                Url = absoluteUrl,
            };
        }
    }
}

using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace Waffle.Lib
{
    public record LinkLine : SelectorLine
    {
        public LinkLine(string line) : base(line)
        {
            Raw = Raw.Trim();
            DisplayString = Raw;
            ItemType = GetItemType(Raw);
        }

        private static ItemType GetItemType(string absoluteUrl)
        {
            absoluteUrl = absoluteUrl.Trim();

            if (absoluteUrl.StartsWith("http://") || absoluteUrl.StartsWith("https://"))
            {
                return ItemType.HTML;
            }

            var parsedUrl = UrlValidator.ParseUrl(absoluteUrl);

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
                return GetItemTypeFromFileExtension(absoluteUrl);
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

        private static ItemType GetItemTypeFromFileExtension(string absoluteUrl)
        {
            if (absoluteUrl.EndsWith(".jpg"))
            {
                return ItemType.Image;
            }

            if (absoluteUrl.EndsWith(".png"))
            {
                return ItemType.PNG;
            }

            if (absoluteUrl.EndsWith(".tar.gz"))
            {
                return ItemType.BinaryFile;
            }

            if (absoluteUrl.EndsWith(".zip"))
            {
                return ItemType.BinaryFile;
            }

            if (absoluteUrl.EndsWith(".txt"))
            {
                return ItemType.Text;
            }

            return ItemType.Unknown;
        }
    }
}

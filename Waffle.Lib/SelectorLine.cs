using Newtonsoft.Json;
using Newtonsoft.Json.Converters;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace Waffle.Lib
{
    public record SelectorLine
    {
        public string Raw { get; set; }

        public string DisplayString { get; set; }

        [JsonConverter(typeof(StringEnumConverter))]
        public ItemType ItemType { get; set; }

        public string Selector { get; set; }

        public string HostName { get; set; }

        public int Port { get; set; }

        public bool IsRenderable
        {
            get
            {
                return ItemType == ItemType.Text ||
                       ItemType == ItemType.Menu ||
                       ItemType == ItemType.PNG ||
                       ItemType == ItemType.Image ||
                       ItemType == ItemType.Unknown;
            }
        }

        public SelectorLine()
        {

        }

        public SelectorLine(string line)
        {
            Raw = line;

            line = StripNewline(line);

            if (line == ".")
            {
                DisplayString = ".";

                return;
            }

            var parts = line.Split('\t');

            if (parts.Length >= 4)
            {
                ItemType = MapItemType(parts[0][0].ToString());
                DisplayString = parts[0][1..];
                Selector = parts[1].Trim();
                HostName = parts[2].Trim();
                Port = int.Parse(parts[3]);
            }
        }

        public string GetUserFriendlyName()
        {
            var absoluteUrl = GetLink();
            absoluteUrl = absoluteUrl.Replace("gopher://", "");
            absoluteUrl = absoluteUrl.TrimEnd('/');

            return absoluteUrl;
        }

        public virtual string GetLink()
        {
            if (ItemType == ItemType.HTML)
            {
                var url = Selector;

                if (url != null && url.StartsWith("URL:"))
                {
                    url = url[4..];
                }

                return url;
            }
            else
            {
                if (string.IsNullOrWhiteSpace(HostName) && string.IsNullOrWhiteSpace(Selector))
                {
                    return Raw;
                }

                return $"gopher://{HostName}{Selector}";
            }
        }

        private static ItemType MapItemType(string itemTypePrefix)
        {
            return itemTypePrefix switch
            {
                "0" => ItemType.Text,
                "1" => ItemType.Menu,
                "2" => ItemType.Nameserver,
                "3" => ItemType.Error,
                "4" => ItemType.BinHex,
                "5" => ItemType.DOS,
                "6" => ItemType.UuencodedFile,
                "7" => ItemType.Search,
                "8" => ItemType.Telnet,
                "9" => ItemType.BinaryFile,
                "+" => ItemType.Mirror,
                "g" => ItemType.GIF,
                "I" => ItemType.Image,
                "T" => ItemType.Telnet3270,
                "d" => ItemType.Doc,
                "h" => ItemType.HTML,
                "i" => ItemType.Info,
                "s" => ItemType.SoundFile,
                "p" => ItemType.PNG,
                _ => ItemType.Unknown
            };
        }

        private static string StripNewline(string line)
        {
            if (line.EndsWith("\r\n"))
            {
                line = line[0..^2];
            }

            return line;
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

        protected static ItemType GetItemType(string absoluteUrl)
        {
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
    }
}

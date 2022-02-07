using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace Waffle.Lib
{
    public record SelectorLine
    {
        public string Raw { get; private set; }

        public string DisplayString { get; private set; }

        public ItemType ItemType { get; private set; }

        public string Selector { get; private set; }

        public string HostName { get; private set; }

        public int Port { get; private set; }


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

            if (parts.Length == 4)
            {
                ItemType = MapItemType(parts[0][0].ToString());
                DisplayString = parts[0][1..];
                Selector = parts[1].Trim();
                HostName = parts[2].Trim();
                Port = int.Parse(parts[3]);
            }
        }

        public string GetLink()
        {
            if (ItemType != ItemType.TextFile &&
                ItemType != ItemType.Menu &&
                ItemType != ItemType.Search &&
                ItemType != ItemType.PNG
            )
            {
                return null;
            }

            return $"gopher://{HostName}{Selector}";
        }

        private static ItemType MapItemType(string itemTypePrefix)
        {
            return itemTypePrefix switch
            {
                "0" => ItemType.TextFile,
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
            else
            {
                // TODO: log warning
            }

            return line;
        }
    }
}

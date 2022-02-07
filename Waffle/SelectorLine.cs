using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace Waffle
{
    public record SelectorLine
    {
        public string Raw { get; private set; }

        public string DisplayString { get; private set; }

        public string ItemType { get; private set; }

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
                ItemType = parts[0][0].ToString();
                DisplayString = parts[0][1..];
                Selector = parts[1];
                HostName = parts[2];
                Port = int.Parse(parts[3]);
            }
        }

        public string GetLink()
        {
            if (ItemType != "0" && ItemType != "1" && ItemType != "7")
            {
                return null;
            }

            return $"gopher://{HostName}{Selector}";
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

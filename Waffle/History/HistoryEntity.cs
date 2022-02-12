using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using Waffle.Lib;

namespace Waffle.History
{
    internal class HistoryEntity
    {
        public DateTime Timestamp { get; set; }

        public SelectorLine SelectorLine { get; set; }

        public override string ToString()
        {
            string link = SelectorLine.GetLink();

            if (string.IsNullOrWhiteSpace(link))
            {
                link = SelectorLine.Raw;
            }

            return $"{Timestamp} - {link}";
        }
    }
}

using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace Waffle.History
{
    internal class HistoryEntity
    {
        public DateTime Timestamp { get; set; }

        public string Url { get; set; }

        public override string ToString()
        {
            return $"{Timestamp} - {Url}";
        }
    }
}

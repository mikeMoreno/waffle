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

        public override string GetLink()
        {
            return Raw;
        }
    }
}

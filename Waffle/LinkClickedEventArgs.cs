using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using Waffle.Lib;

namespace Waffle
{
    class LinkClickedEventArgs : EventArgs
    {
        public SelectorLine SelectorLine { get; }

        //public string Link { get; }

        //public ItemType ItemType { get; }

        //public LinkClickedEventArgs(string link, ItemType itemType)
        //{
        //    Link = link;
        //    ItemType = itemType;
        //}

        public LinkClickedEventArgs(SelectorLine selectorLine)
        {
            SelectorLine = selectorLine;
        }
    }
}

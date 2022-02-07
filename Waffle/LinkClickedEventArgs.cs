using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using Waffle.Lib;

namespace Waffle
{
    public class LinkClickedEventArgs : EventArgs
    {
        public string Link { get; }
        public ItemType ItemType { get; }

        public LinkClickedEventArgs(string link, ItemType itemType)
        {
            Link = link;
            ItemType = itemType;
        }
    }
}

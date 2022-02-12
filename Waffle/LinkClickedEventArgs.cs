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

        public LinkClickedEventArgs(SelectorLine selectorLine)
        {
            SelectorLine = selectorLine;
        }
    }
}

using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using Waffle.Lib;

namespace Waffle.Navigation
{
    class NavigationLinkClickedEventArgs : EventArgs
    {
        public SelectorLine SelectorLine { get; }

        public NavigationLinkClickedEventArgs(SelectorLine selectorLine)
        {
            SelectorLine = selectorLine;
        }
    }
}

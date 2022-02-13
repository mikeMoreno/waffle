using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using Waffle.Lib;

namespace Waffle.Bookmarks
{

    class BookmarkClickedEventArgs : EventArgs
    {
        //public Bookmark Bookmark { get; private set; }
        
        public SelectorLine SelectorLine { get; }


        public BookmarkClickedEventArgs(SelectorLine selectorLine)
        {
            SelectorLine = selectorLine;
        }
    }
}

using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace Waffle.Bookmarks
{

    class BookmarkClickedEventArgs : EventArgs
    {
        public Bookmark Bookmark { get; private set; }

        public BookmarkClickedEventArgs(Bookmark bookmark)
        {
            Bookmark = bookmark;
        }
    }
}

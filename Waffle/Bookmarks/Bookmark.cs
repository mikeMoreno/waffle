using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using Waffle.Lib;

namespace Waffle.Bookmarks
{
    internal class Bookmark : BookmarkEntity
    {
        public override string BookmarkEntityType => "Bookmark";

        public SelectorLine SelectorLine { get; set; }
    }
}

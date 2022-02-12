using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace Waffle.Bookmarks
{
    internal class BookmarkFolder : BookmarkEntity
    {
        public override string BookmarkEntityType => "BookmarkFolder";

        public List<BookmarkEntity> BookmarkEntities { get; set; } = new List<BookmarkEntity>();
    }
}

using Newtonsoft.Json;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace Waffle.Bookmarks
{
    [JsonConverter(typeof(BookmarkEntityConverter))]
    internal abstract class BookmarkEntity
    {
        public string Name { get; set; }

        public abstract string BookmarkEntityType { get; }
    }
}

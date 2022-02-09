using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace Waffle
{
    internal class Bookmark : BookmarkEntity
    {
        public override string BookmarkEntityType => "Bookmark";

        public string Url { get; set; }
    }
}

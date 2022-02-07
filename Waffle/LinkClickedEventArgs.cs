using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace Waffle
{
    public class LinkClickedEventArgs : EventArgs
    {
        public string Link { get; }

        public LinkClickedEventArgs(string link)
        {
            Link = link.Trim();
        }
    }
}

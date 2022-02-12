using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace Waffle.Navigation
{
    internal class RequestTab : TabPage
    {
        public Guid Key { get; }

        public RequestTab()
        {
            Key = Guid.NewGuid();
        }
    }
}

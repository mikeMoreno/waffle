using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using Waffle.Lib;

namespace Waffle.Navigation
{
    public class ViewSourceEventArgs : EventArgs
    {
        public string CurrentlyDisplayedText { get; }

        public ViewSourceEventArgs(string currentlyDisplayedText)
        {
            CurrentlyDisplayedText = currentlyDisplayedText;
        }
    }
}

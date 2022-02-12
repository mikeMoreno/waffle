using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace Waffle.Lib
{

    // TODO: maybe not need this if we can pass SelectorLines around everywhere
    internal static class UrlValidator
    {
        public  static void ValidateUrl(string url)
        {
            ArgumentNullException.ThrowIfNull(url);

            if (url.Trim() == "")
            {
                throw new InvalidOperationException("Empty url not allowed.");
            }
        }


        // TODO: bundle with ValidateUrl?
        public static string ParseUrl(string url)
        {
            const string Protocol = "gopher://";

            url = url.Trim();

            if (url.StartsWith(Protocol))
            {
                url = url[(url.IndexOf(Protocol) + Protocol.Length)..];
            }

            if (url.EndsWith("/"))
            {
                url = url[0..^1];
            }

            return url;
        }
    }
}

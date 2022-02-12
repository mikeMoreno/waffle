using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace Waffle.Lib
{
    public abstract class Response
    {
        public bool IsSuccess { get; set; } = true;

        public string ErrorMessage { get; set; }

        internal static T Error<T>(Exception e) where T : Response, new()
        {
            return new T()
            {
                IsSuccess = false,
                ErrorMessage = e.Message,
            };
        }
    }
}

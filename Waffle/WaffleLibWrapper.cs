using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using Waffle.Lib;

namespace Waffle
{
    internal static class WaffleLibWrapper1
    {
        public static async Task<Response> GetResponseAsync(WaffleLib waffleLib, string absoluteUrl, ItemType itemType)
        {
            try
            {
                return itemType switch
                {
                    ItemType.Menu => await waffleLib.GetMenuAsync(absoluteUrl),
                    ItemType.Text => await waffleLib.GetTextFileAsync(absoluteUrl),
                    ItemType.PNG => await waffleLib.GetPngFileAsync(absoluteUrl),
                    ItemType.Image => await waffleLib.GetImageFileAsync(absoluteUrl),
                    ItemType.BinaryFile => await waffleLib.GetBinaryFile(absoluteUrl),
                    _ => await waffleLib.GetMenuAsync(absoluteUrl),
                };
            }
            catch
            {
                MessageBox.Show($"Unable to navigate to {absoluteUrl}.");

                return null;
            }
        }
    }
}

using Waffle.Lib;

namespace Waffle
{
    public partial class Main : Form
    {
        private WaffleLib WaffleLib { get; }

        public Main(WaffleLib waffleLib)
        {
            InitializeComponent();

            WaffleLib = waffleLib;

            pageRenderer.WaffleLib = WaffleLib;

            pageRenderer.LinkClicked += PageRenderer_LinkClicked;
        }

        private void PageRenderer_LinkClicked(object sender, LinkClickedEventArgs e)
        {
            txtUrl.Text = e.Link;
        }

        private async void btnGo_Click(object sender, EventArgs e)
        {
            var url = txtUrl.Text.Trim();

            if (string.IsNullOrWhiteSpace(url))
            {
                return;
            }

            var responseType = WaffleLib.GetLinkType(url);

            switch (responseType)
            {
                case ResponseType.Menu:
                    pageRenderer.Render(await WaffleLib.GetMenuAsync(url));
                    break;
                case ResponseType.TextFile:
                    pageRenderer.Render(await WaffleLib.GetTextFileAsync(url));
                    break;
                default:
                    pageRenderer.Render(await WaffleLib.GetMenuAsync(url));
                    break;
            }
        }
    }
}
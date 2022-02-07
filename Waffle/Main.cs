using Waffle.Lib;

namespace Waffle
{
    public partial class Main : Form
    {
        private WaffleLib WaffleLib { get; }

        private Stack<string> visitedUrls = new Stack<string>();

        public Main(WaffleLib waffleLib)
        {
            InitializeComponent();

            WaffleLib = waffleLib;

            pageRenderer.WaffleLib = WaffleLib;

            pageRenderer.LinkClicked += PageRenderer_LinkClicked;
        }

        private void PageRenderer_LinkClicked(object sender, LinkClickedEventArgs e)
        {
            visitedUrls.Push(txtUrl.Text.Trim());

            txtUrl.Text = e.Link;

            btnBack.Enabled = true;
        }

        private async void btnGo_Click(object sender, EventArgs e)
        {
            var url = txtUrl.Text;

            if (string.IsNullOrWhiteSpace(url))
            {
                return;
            }

            await RenderUrlAsync(url);
        }

        private async void btnBack_Click(object sender, EventArgs e)
        {
            txtUrl.Text = visitedUrls.Pop();

            if (!visitedUrls.Any())
            {
                btnBack.Enabled = false;
            }

            await RenderUrlAsync(txtUrl.Text);
        }

        private async Task RenderUrlAsync(string absoluteUrl)
        {
            absoluteUrl = absoluteUrl.Trim();

            var responseType = WaffleLib.GetLinkType(absoluteUrl);

            switch (responseType)
            {
                case ResponseType.Menu:
                    pageRenderer.Render(await WaffleLib.GetMenuAsync(absoluteUrl));
                    break;
                case ResponseType.TextFile:
                    pageRenderer.Render(await WaffleLib.GetTextFileAsync(absoluteUrl));
                    break;
                default:
                    pageRenderer.Render(await WaffleLib.GetMenuAsync(absoluteUrl));
                    break;
            }
        }
    }
}
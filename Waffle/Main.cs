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

            var pageRenderer = PageRenderer.Instance(WaffleLib);

            pageRenderer.LinkClicked += PageRenderer_LinkClicked;
            pageRenderer.ViewingSource += PageRenderer_ViewingSource;

            var defaultTab = tabSitePages.TabPages[0];
            defaultTab.Controls.Add(pageRenderer);

            tabSitePages.SelectedIndexChanged += TabSitePages_SelectedIndexChanged;
        }

        private void TabSitePages_SelectedIndexChanged(object sender, EventArgs e)
        {
            var selectedTab = tabSitePages.SelectedTab;

            if (selectedTab == null)
            {
                return;
            }

            if (selectedTab.Text == "+")
            {
                SpawnNewTab();

                return;
            }

            var pageRenderer = selectedTab.Controls.OfType<PageRenderer>().Single();

            if (pageRenderer.StandbyText != null)
            {
                txtUrl.Text = pageRenderer.StandbyText;
            }
            else
            {
                if (pageRenderer.VisitedUrls.TryPeek(out string currentUrl))
                {
                    if (currentUrl != "<home>")
                    {
                        txtUrl.Text = currentUrl;
                    }
                }
                else
                {
                    txtUrl.Text = "";
                }
            }
        }

        private TabPage SpawnNewTab()
        {
            var pageRenderer = PageRenderer.Instance(WaffleLib);

            var tabPage = new TabPage
            {
                Text = "New Tab",
                BackColor = Color.White,
            };

            tabPage.Controls.Add(pageRenderer);

            tabSitePages.TabPages.Insert(tabSitePages.TabPages.Count - 1, tabPage);
            tabSitePages.SelectedTab = tabPage;

            btnBack.Enabled = false;

            return tabPage;
        }

        private void PageRenderer_LinkClicked(object sender, LinkClickedEventArgs e)
        {
            var pageRenderer = sender as PageRenderer;
            pageRenderer.VisitedUrls.Push(txtUrl.Text.Trim());

            txtUrl.Text = e.Link;

            btnBack.Enabled = true;
        }

        private void PageRenderer_ViewingSource(object sender, ViewSourceEventArgs e)
        {
            var newTab = SpawnNewTab();
            newTab.Text = "viewing source";
            var pageRenderer = newTab.Controls.OfType<PageRenderer>().Single();

            var textResponse = new TextResponse()
            {
                Text = e.CurrentlyDisplayedText,
            };

            pageRenderer.ViewSource(textResponse);

        }

        private async void btnGo_Click(object sender, EventArgs e)
        {
            var url = txtUrl.Text;

            if (string.IsNullOrWhiteSpace(url))
            {
                return;
            }

            var selectedTab = tabSitePages.SelectedTab;

            var pageRenderer = selectedTab.Controls.OfType<PageRenderer>().Single();

            if (!pageRenderer.VisitedUrls.Any())
            {
                pageRenderer.VisitedUrls.Push("<home>");
            }
            else
            {
                pageRenderer.VisitedUrls.Push(txtUrl.Text.Trim());
            }

            btnBack.Enabled = true;

            await RenderUrlAsync(url);
        }

        private async void btnBack_Click(object sender, EventArgs e)
        {
            var selectedTab = tabSitePages.SelectedTab;
            var pageRenderer = selectedTab.Controls.OfType<PageRenderer>().Single();

            var lastVisitedUrl = pageRenderer.VisitedUrls.Pop();

            if (lastVisitedUrl != "<home>")
            {
                txtUrl.Text = lastVisitedUrl;
            }
            else
            {
                txtUrl.Text = "";
            }

            if (!pageRenderer.VisitedUrls.Any())
            {
                btnBack.Enabled = false;
            }

            if (lastVisitedUrl == "<home>")
            {
                pageRenderer.Clear();
            }
            else
            {
                await RenderUrlAsync(txtUrl.Text);
            }
        }

        private async Task RenderUrlAsync(string absoluteUrl)
        {
            var selectedTab = tabSitePages.SelectedTab;
            var pageRenderer = selectedTab.Controls.OfType<PageRenderer>().Single();

            absoluteUrl = absoluteUrl.Trim();

            var responseType = WaffleLib.GetContentType(absoluteUrl);

            switch (responseType)
            {
                case ContentType.Menu:
                    pageRenderer.Render(await WaffleLib.GetMenuAsync(absoluteUrl));
                    break;
                case ContentType.TextFile:
                    pageRenderer.Render(await WaffleLib.GetTextFileAsync(absoluteUrl));
                    break;
                default:
                    pageRenderer.Render(await WaffleLib.GetMenuAsync(absoluteUrl));
                    break;
            }
        }

        private void txtUrl_Leave(object sender, EventArgs e)
        {
            var selectedTab = tabSitePages.SelectedTab;
            var pageRenderer = selectedTab.Controls.OfType<PageRenderer>().Single();

            pageRenderer.StandbyText = txtUrl.Text;
        }
    }
}
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

            var pageRenderer = BuildPageRenderer(WaffleLib);

            var defaultTab = tabSitePages.TabPages[0];
            defaultTab.Controls.Add(pageRenderer);

            tabSitePages.SelectedIndexChanged += TabSitePages_SelectedIndexChanged;

            if (Program.CliArgs.Length > 0)
            {
                var siteToVisit = Program.CliArgs.First();

                _ = VisitSiteAsync(siteToVisit);
            }
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
                if (pageRenderer.VisitedUrls.TryPeek(out (string currentUrl, ItemType _) result))
                {
                    if (result.currentUrl != "<home>")
                    {
                        txtUrl.Text = result.currentUrl;
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
            var pageRenderer = BuildPageRenderer(WaffleLib);

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
            pageRenderer.VisitedUrls.Push((txtUrl.Text.Trim(), pageRenderer.CurrentPageType));

            txtUrl.Text = e.Link;

            btnBack.Enabled = true;

            Text = $"Waffle - {e.ItemType}";
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
            await VisitSiteAsync(txtUrl.Text);
        }

        private async Task VisitSiteAsync(string absoluteUrl)
        {
            if (string.IsNullOrWhiteSpace(absoluteUrl))
            {
                return;
            }

            txtUrl.Text = absoluteUrl;

            var itemType = WaffleLib.GetItemType(absoluteUrl);

            var selectedTab = tabSitePages.SelectedTab;

            var pageRenderer = selectedTab.Controls.OfType<PageRenderer>().Single();

            if (!pageRenderer.VisitedUrls.Any())
            {
                pageRenderer.VisitedUrls.Push(("<home>", ItemType.Unknown));
            }
            else
            {
                pageRenderer.VisitedUrls.Push((txtUrl.Text.Trim(), pageRenderer.CurrentPageType));
            }

            btnBack.Enabled = true;

            await RenderUrlAsync(absoluteUrl, itemType);
        }

        private async void btnBack_Click(object sender, EventArgs e)
        {
            var selectedTab = tabSitePages.SelectedTab;
            var pageRenderer = selectedTab.Controls.OfType<PageRenderer>().Single();

            var (lastVisitedUrl, itemType) = pageRenderer.VisitedUrls.Pop();

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
                Text = $"Waffle - {ItemType.Home}";
            }
            else
            {
                await RenderUrlAsync(txtUrl.Text, itemType);
            }
        }

        private async Task RenderUrlAsync(string absoluteUrl, ItemType itemType)
        {
            var selectedTab = tabSitePages.SelectedTab;
            var pageRenderer = selectedTab.Controls.OfType<PageRenderer>().Single();

            absoluteUrl = absoluteUrl.Trim();

            switch (itemType)
            {
                case ItemType.Menu:
                    Text = $"Waffle - {itemType}";
                    pageRenderer.Render(await WaffleLib.GetMenuAsync(absoluteUrl));
                    break;
                case ItemType.Text:
                    Text = $"Waffle - {itemType}";
                    pageRenderer.Render(await WaffleLib.GetTextFileAsync(absoluteUrl));
                    break;
                default:
                    Text = "Waffle";
                    pageRenderer.Render(await WaffleLib.GetMenuAsync(absoluteUrl));
                    break;
            }
        }

        private void txtUrl_Leave(object sender, EventArgs e)
        {
            var selectedTab = tabSitePages.SelectedTab;

            if (selectedTab == null)
            {
                return;
            }

            var pageRenderer = selectedTab.Controls.OfType<PageRenderer>().Single();

            pageRenderer.StandbyText = txtUrl.Text;
        }

        private PageRenderer BuildPageRenderer(WaffleLib waffleLib)
        {
            var pageRenderer = PageRenderer.Instance(waffleLib);

            pageRenderer.LinkClicked += PageRenderer_LinkClicked;
            pageRenderer.ViewingSource += PageRenderer_ViewingSource;
            pageRenderer.CloseTab += PageRenderer_CloseTab;

            return pageRenderer;
        }

        private void PageRenderer_CloseTab(object sender, EventArgs e)
        {
            CloseTab();
        }

        private void Main_KeyDown(object sender, KeyEventArgs e)
        {
            if (e.Modifiers == Keys.Control && e.KeyCode == Keys.W)
            {
                CloseTab();
            }

            if (e.Modifiers == Keys.Control && e.KeyCode == Keys.T)
            {
                OpenTab();
            }
        }

        private void CloseTab()
        {
            tabSitePages.SelectedIndexChanged -= TabSitePages_SelectedIndexChanged;

            var selectedTab = tabSitePages.SelectedTab;

            if (selectedTab == null)
            {
                tabSitePages.SelectedIndexChanged += TabSitePages_SelectedIndexChanged;

                return;
            }

            tabSitePages.TabPages.Remove(selectedTab);

            if (tabSitePages.TabPages.Count == 1)
            {
                Application.Exit();
            }

            tabSitePages.SelectedIndexChanged += TabSitePages_SelectedIndexChanged;
        }

        private void OpenTab()
        {
            tabSitePages.SelectedIndexChanged -= TabSitePages_SelectedIndexChanged;

            SpawnNewTab();

            tabSitePages.SelectedIndexChanged += TabSitePages_SelectedIndexChanged;
        }

        private void btnExit_Click(object sender, EventArgs e)
        {
            Application.Exit();
        }
    }
}
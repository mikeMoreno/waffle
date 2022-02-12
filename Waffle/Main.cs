using Waffle.Bookmarks;
using Waffle.History;
using Waffle.Lib;
using Waffle.UserControls;

namespace Waffle
{
    partial class Main : Form
    {
        private WaffleLib WaffleLib { get; }

        private HistoryService HistoryService { get; }

        public Main(WaffleLib waffleLib, HistoryService historyService)
        {
            InitializeComponent();

            WaffleLib = waffleLib;
            HistoryService = historyService;

            HistoryService.Consolidate();

            SpawnNewTab();

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

            SetUrlTextBoxText(selectedTab);
        }

        private void SetUrlTextBoxText(TabPage tabPage)
        {
            var pageRenderer = tabPage.Controls.OfType<PageRenderer>().SingleOrDefault();

            if (pageRenderer == null)
            {
                return;
            }

            if (pageRenderer.StandbyText != null)
            {
                txtUrl.Text = pageRenderer.StandbyText;
            }
            else
            {
                if (pageRenderer.VisitedPages.TryPeek(out SelectorLine line))
                {
                    if (line.GetLink() != "<home>")
                    {
                        txtUrl.Text = line.GetLink();
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

            var tabPage = new RequestTab
            {
                Text = "New Tab",
                BackColor = Color.White,
            };

            tabPage.Controls.Add(pageRenderer);

            /*
             * This call is necessary because of a WinForms bug :D 
             * More information can be found here:
             * https://github.com/dotnet/winforms/issues/3686
             * https://stackoverflow.com/questions/1532301/visual-studio-tabcontrol-tabpages-insert-not-working
             */
            _ = tabSitePages.Handle;

            tabSitePages.TabPages.Insert(tabSitePages.TabPages.Count - 1, tabPage);

            tabSitePages.SelectedTab = tabPage;

            btnBack.Enabled = false;

            return tabPage;
        }

        private async void PageRenderer_LinkClicked(object sender, LinkClickedEventArgs e)
        {
            await VisitSiteAsync(e.SelectorLine);
        }

        private void PageRenderer_ViewingSource(object sender, ViewSourceEventArgs e)
        {
            tabSitePages.SelectedIndexChanged -= TabSitePages_SelectedIndexChanged;

            var newTab = SpawnNewTab();
            newTab.Text = "viewing source";
            var pageRenderer = newTab.Controls.OfType<PageRenderer>().Single();

            var textResponse = new TextResponse()
            {
                Text = e.CurrentlyDisplayedText,
            };

            pageRenderer.RenderSource(textResponse);

            tabSitePages.SelectedIndexChanged += TabSitePages_SelectedIndexChanged;
        }

        private void PageRenderer_ViewingHistory(object sender, ViewHistoryEventArgs e)
        {
            var selectedTab = tabSitePages.SelectedTab as RequestTab;

            var historyForm = BuildHistoryForm(selectedTab.Key);
            historyForm.Show();
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

            absoluteUrl = absoluteUrl.Trim();

            await VisitSiteAsync(new LinkLine(absoluteUrl));
        }

        private async Task VisitSiteAsync(SelectorLine selectorLine)
        {
            var selectedTab = tabSitePages.SelectedTab as RequestTab;
            var pageRenderer = selectedTab.Controls.OfType<PageRenderer>().Single();

            LinkLine currentLink = null;

            if (!string.IsNullOrWhiteSpace(txtUrl.Text))
            {
                currentLink = new LinkLine(txtUrl.Text.Trim())
                {
                    ItemType = pageRenderer.CurrentPageType
                };
            }

            var successfullyRendered = await RenderUrlAsync(selectorLine);

            if (!successfullyRendered)
            {
                return;
            }

            txtUrl.Text = selectorLine.GetLink();
            selectedTab.Text = selectorLine.GetUserFriendlyName();
            Text = $"Waffle - {pageRenderer.CurrentPageType}";

            if (!pageRenderer.VisitedPages.Any())
            {
                pageRenderer.VisitedPages.Push(new LinkLine("<home>"));
            }
            else
            {
                if (currentLink != null)
                {
                    pageRenderer.VisitedPages.Push(currentLink);
                }
            }

            btnBack.Enabled = true;

            HistoryService.AddUrl(selectedTab.Key, selectorLine.GetLink());
        }

        private async Task<bool> RenderUrlAsync(SelectorLine selectorLine)
        {
            var selectedTab = tabSitePages.SelectedTab;
            var pageRenderer = selectedTab.Controls.OfType<PageRenderer>().Single();

            var response = await WaffleLib.GetAsync(selectorLine);

            if (!response.IsSuccess)
            {
                MessageBox.Show(response.ErrorMessage);

                return false;
            }

            switch (response)
            {
                case MenuResponse menuResponse:
                    pageRenderer.Render(menuResponse);
                    break;
                case TextResponse textResponse:
                    pageRenderer.Render(textResponse);
                    break;
                case PngResponse pngResponse:
                    pageRenderer.Render(pngResponse);
                    break;
                case ImageResponse imageResponse:
                    pageRenderer.Render(imageResponse);
                    break;
                case BinaryResponse binaryResponse:
                    var fileName = selectorLine.Selector[(selectorLine.Selector.LastIndexOf('/') + 1)..];

                    var fsDialog = new SaveFileDialog
                    {
                        FileName = fileName
                    };

                    var ans = fsDialog.ShowDialog();

                    if (ans == DialogResult.OK)
                    {
                        File.WriteAllBytes(fsDialog.FileName, binaryResponse.Bytes);
                    }

                    break;
                default:
                    pageRenderer.Render(response as MenuResponse);
                    break;
            }

            return true;
        }

        private async void btnBack_Click(object sender, EventArgs e)
        {
            var selectedTab = tabSitePages.SelectedTab;
            var pageRenderer = selectedTab.Controls.OfType<PageRenderer>().Single();

            var selectorLine = pageRenderer.VisitedPages.Pop();

            var lastVisitedUrl = selectorLine.GetLink();

            if (lastVisitedUrl == "<home>")
            {
                pageRenderer.Clear();
                txtUrl.Text = "";
                Text = $"Waffle - {ItemType.Home}";
                selectedTab.Text = "Home";
            }
            else
            {
                txtUrl.Text = lastVisitedUrl;
                selectedTab.Text = selectorLine.GetUserFriendlyName();
                Text = $"Waffle - {selectorLine.ItemType}";
            }

            if (!pageRenderer.VisitedPages.Any())
            {
                btnBack.Enabled = false;
            }

            if (lastVisitedUrl != "<home>")
            {
                await RenderUrlAsync(selectorLine);
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
            pageRenderer.ViewingHistory += PageRenderer_ViewingHistory;
            pageRenderer.CloseTab += PageRenderer_CloseTab;

            return pageRenderer;
        }

        private HistoryForm BuildHistoryForm(Guid? selectedTabKey = null)
        {
            var historyForm = new HistoryForm(HistoryService, selectedTabKey);
            historyForm.LinkClicked += HistoryForm_LinkClicked;

            return historyForm;
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

            int selectedIndex = tabSitePages.SelectedIndex;

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

            if (tabSitePages.TabPages[selectedIndex].Text == "+")
            {
                if (selectedIndex == 1)
                {
                    tabSitePages.SelectedIndex = 0;

                    selectedTab = tabSitePages.SelectedTab;
                }
            }
            else
            {
                tabSitePages.SelectedIndex = selectedIndex;

                selectedTab = tabSitePages.SelectedTab;
            }

            SetUrlTextBoxText(selectedTab);

            var pageRenderer = selectedTab.Controls.OfType<PageRenderer>().SingleOrDefault();

            if (pageRenderer != null && pageRenderer.VisitedPages.Any())
            {
                btnBack.Enabled = true;
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

        private void btnViewHistory_Click(object sender, EventArgs e)
        {
            var historyForm = BuildHistoryForm();
            historyForm.Show();
        }

        private void btnAbout_Click(object sender, EventArgs e)
        {

        }

        private void btnFavorite_Click(object sender, EventArgs e)
        {
            if (string.IsNullOrWhiteSpace(txtUrl.Text))
            {
                return;
            }

            using var favAdder = new BookmarkAdder(new LinkLine(txtUrl.Text));

            var ans = favAdder.ShowDialog();
        }

        private void btnBookmarks_Click(object sender, EventArgs e)
        {
            var bookmarkPanel = new BookmarkPanel()
            {
                Dock = DockStyle.Right,
                Size = new Size()
                {
                    Width = 325,
                }
            };

            bookmarkPanel.LinkClicked += BookmarkPanel_LinkClicked;

            Controls.Add(bookmarkPanel);
            bookmarkPanel.BringToFront();
        }

        private async void BookmarkPanel_LinkClicked(object sender, BookmarkClickedEventArgs e)
        {
            await VisitSiteAsync(e.Bookmark.Url);
        }

        private async void HistoryForm_LinkClicked(object sender, LinkClickedEventArgs e)
        {
            await VisitSiteAsync(e.SelectorLine);
        }
    }
}
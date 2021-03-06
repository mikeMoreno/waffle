using System.Diagnostics;
using Waffle.Bookmarks;
using Waffle.History;
using Waffle.Lib;

namespace Waffle.Navigation
{
    partial class Navigator : Form
    {
        private WaffleLib WaffleLib { get; }

        private HistoryService HistoryService { get; }

        public Navigator(WaffleLib waffleLib, HistoryService historyService)
        {
            InitializeComponent();

            WaffleLib = waffleLib;
            HistoryService = historyService;

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

            var pageRenderer = selectedTab.Controls.OfType<PageRenderer>().SingleOrDefault();

            btnBack.Enabled = pageRenderer.VisitedPages.Any();

            SetUrlTextBoxText(selectedTab);

            Text = $"Waffle - {pageRenderer.CurrentPageType}";
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
                var selectorLine = pageRenderer.CurrentSelectorLine;

                if (selectorLine != null)
                {
                    txtUrl.Text = selectorLine.GetLink();
                }
                else
                {
                    txtUrl.Text = "";
                }
            }
        }

        private TabPage SpawnNewTab()
        {
            var pageRenderer = BuildPageRenderer();

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
            txtUrl.Text = "";

            return tabPage;
        }

        private async void PageRenderer_LinkClicked(object sender, NavigationLinkClickedEventArgs e)
        {
            await VisitSiteAsync(e.SelectorLine);
        }

        private async void PageRenderer_OpenInNewTabClicked(object sender, NavigationLinkClickedEventArgs e)
        {
            await VisitSiteAsync(e.SelectorLine, newTab: true);
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

        private async Task VisitSiteAsync(string absoluteUrl, bool newTab = false)
        {
            if (string.IsNullOrWhiteSpace(absoluteUrl))
            {
                return;
            }

            absoluteUrl = absoluteUrl.Trim();

            await VisitSiteAsync(new LinkLine(absoluteUrl), newTab);
        }

        private async Task VisitSiteAsync(SelectorLine selectorLine, bool newTab = false)
        {
            if (newTab)
            {
                tabSitePages.SelectedIndexChanged -= TabSitePages_SelectedIndexChanged;

                SpawnNewTab();

                tabSitePages.SelectedIndexChanged += TabSitePages_SelectedIndexChanged;
            }

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

            if (!selectorLine.IsRenderable)
            {
                return;
            }

            txtUrl.Text = selectorLine.GetLink();
            selectedTab.Text = selectorLine.GetUserFriendlyName();
            Text = $"Waffle - {pageRenderer.CurrentPageType}";

            if (!newTab)
            {
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
            }

            HistoryService.AddUrl(selectedTab.Key, selectorLine);
            pageRenderer.CurrentSelectorLine = selectorLine;
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
                case HtmlResponse htmlResponse:
                    var process = new ProcessStartInfo
                    {
                        FileName = htmlResponse.Url,
                        UseShellExecute = true
                    };

                    Process.Start(process);
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

        private PageRenderer BuildPageRenderer()
        {
            var pageRenderer = PageRenderer.Instance();

            pageRenderer.LinkClicked += PageRenderer_LinkClicked;
            pageRenderer.OpenInNewTabClicked += PageRenderer_OpenInNewTabClicked;
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

        private async void Main_KeyDown(object sender, KeyEventArgs e)
        {
            if (e.Modifiers == Keys.Control && e.KeyCode == Keys.W)
            {
                CloseTab();
            }

            if (e.Modifiers == Keys.Control && e.KeyCode == Keys.T)
            {
                OpenTab();
            }

            if (e.KeyCode == Keys.Enter)
            {
                await VisitSiteAsync(txtUrl.Text);
            }

            if (e.Modifiers == Keys.Control && e.KeyCode == Keys.L)
            {
                txtUrl.Focus();
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

            if (pageRenderer != null)
            {
                Text = $"Waffle - {pageRenderer.CurrentPageType}";

                if (pageRenderer.VisitedPages.Any())
                {
                    btnBack.Enabled = true;
                }
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
            var aboutForm = new AboutForm();
            aboutForm.Show();
        }

        private void btnAddBookmark_Click(object sender, EventArgs e)
        {
            if (tabSitePages.SelectedTab is not RequestTab selectedTab)
            {
                return;
            }

            var pageRenderer = selectedTab.Controls.OfType<PageRenderer>().Single();

            if (pageRenderer == null || pageRenderer.CurrentSelectorLine == null)
            {
                return;
            }

            using var favAdder = new BookmarkAdder(pageRenderer.CurrentSelectorLine);

            var ans = favAdder.ShowDialog();
        }

        private void btnOpenBookmarkPanel_Click(object sender, EventArgs e)
        {
            var bookmarkPanel = new BookmarkPanel(WaffleLib)
            {
                Dock = DockStyle.Right,
                Size = new Size()
                {
                    Width = 325,
                }
            };

            bookmarkPanel.LinkClicked += BookmarkPanel_LinkClicked;
            bookmarkPanel.OpenInNewTabClicked += BookmarkPanel_OpenInNewTabClicked;

            Controls.Add(bookmarkPanel);
            bookmarkPanel.BringToFront();
        }

        private async void BookmarkPanel_LinkClicked(object sender, BookmarkClickedEventArgs e)
        {
            await VisitSiteAsync(e.SelectorLine);
        }

        private async void BookmarkPanel_OpenInNewTabClicked(object sender, BookmarkClickedEventArgs e)
        {
            await VisitSiteAsync(e.SelectorLine, newTab: true);
        }

        private async void HistoryForm_LinkClicked(object sender, NavigationLinkClickedEventArgs e)
        {
            // TODO: by default, newTab: false. have separate button for opening in a new tab, like with bookmarks.
            await VisitSiteAsync(e.SelectorLine, newTab: true);
        }
    }
}
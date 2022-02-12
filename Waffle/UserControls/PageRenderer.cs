using System;
using System.Collections.Generic;
using System.ComponentModel;
using System.Data;
using System.Drawing;
using System.Drawing.Imaging;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using System.Windows.Forms;
using Waffle.History;
using Waffle.Lib;

namespace Waffle.UserControls
{
    partial class PageRenderer : UserControl
    {
        private ToolTip UnknownEntity { get; }

        private string CurrentlyDisplayedText { get; set; }

        private Image CurrentlyDisplayedPng { get; set; }

        private Image CurrentlyDisplayedImage { get; set; }

        public ItemType CurrentPageType { get; set; }

        public Stack<SelectorLine> VisitedPages { get; } = new Stack<SelectorLine>();

        public string StandbyText { get; set; }

        public SelectorLine CurrentSelectorLine { get; set; }

        public delegate void LinkClickedEventHandler(object sender, LinkClickedEventArgs e);

        public event LinkClickedEventHandler LinkClicked;

        public delegate void ViewSourceEventHandler(object sender, ViewSourceEventArgs e);

        public event ViewSourceEventHandler ViewingSource;

        public delegate void ViewHistoryEventHandler(object sender, ViewHistoryEventArgs e);

        public event ViewHistoryEventHandler ViewingHistory;

        public delegate void CloseTabEventHandler(object sender, EventArgs e);

        public event CloseTabEventHandler CloseTab;

        private PageRenderer()
        {
            InitializeComponent();

            UnknownEntity = new ToolTip();
        }

        public void Clear()
        {
            Controls.Clear();
        }

        public void RenderSelectorLines(SelectorLine[] selectorLines)
        {
            Controls.Clear();

            var y = 10;

            foreach (var line in selectorLines)
            {
                Label label = BuildLabel(x: 10, y, line);
                label.Text = line.Raw;

                Controls.Add(label);

                y += 20;
            }
        }

        public void Render(MenuResponse response)
        {
            Controls.Clear();

            var text = new StringBuilder();

            var lines = response.Lines;

            var y = 10;

            foreach (var line in lines)
            {
                if (line.DisplayString == ".")
                {
                    break;
                }

                Label label;

                if (line.ItemType == ItemType.Info)
                {
                    label = BuildLabel(x: 10, y, line);
                }
                else if (line.ItemType == ItemType.Text)
                {
                    label = BuildLinkLabel(x: 10, y, line);
                }
                else if (line.ItemType == ItemType.Menu)
                {
                    label = BuildLinkLabel(x: 10, y, line);
                }
                else if (line.ItemType == ItemType.Image)
                {
                    label = BuildLinkLabel(x: 10, y, line);
                }
                else if (line.ItemType == ItemType.PNG)
                {
                    label = BuildLinkLabel(x: 10, y, line);
                }
                else if (line.ItemType == ItemType.BinaryFile)
                {
                    label = BuildLinkLabel(x: 10, y, line);
                }
                else
                {
                    label = BuildUnknownTypeLabel(x: 10, y, line);
                }

                Controls.Add(label);

                y += 20;

                text.AppendLine(line.Raw);
            }

            CurrentlyDisplayedText = text.ToString();

            CurrentPageType = ItemType.Menu;

            btnViewSource.Enabled = true;
        }

        public void Render(TextResponse response)
        {
            Controls.Clear();

            var text = new StringBuilder();

            var y = 10;

            foreach (var line in response.Text.Split("\r\n"))
            {
                Label label;

                if (IsLink(line))
                {
                    label = BuildLinkLabel(x: 10, y, new LinkLine(line));
                }
                else
                {
                    label = BuildLabel(x: 10, y, line);
                }

                Controls.Add(label);

                y += 20;

                text.AppendLine(line);
            }

            CurrentlyDisplayedText = text.ToString();

            CurrentPageType = ItemType.Text;

            btnViewSource.Enabled = false;
        }

        public void Render(PngResponse response)
        {
            Controls.Clear();

            var pictureBox = new PictureBox()
            {
                Image = response.Image,
                Dock = DockStyle.Fill,
            };

            Controls.Add(pictureBox);

            CurrentlyDisplayedPng = response.Image;

            CurrentPageType = ItemType.PNG;

            btnViewSource.Enabled = false;
        }

        public void Render(ImageResponse response)
        {
            Controls.Clear();

            var pictureBox = new PictureBox()
            {
                Image = response.Image,
                Dock = DockStyle.Fill,
            };

            Controls.Add(pictureBox);

            CurrentlyDisplayedImage = response.Image;

            CurrentPageType = ItemType.Image;

            btnViewSource.Enabled = false;
        }

        public void RenderSource(TextResponse response)
        {
            Render(response);

            CurrentPageType = ItemType.GopherSourceCode;
        }

        private bool IsLink(string line)
        {
            if (string.IsNullOrWhiteSpace(line))
            {
                return false;
            }

            line = line.Trim();

            return line.StartsWith("gopher://");
        }

        private Label BuildLabel(int x, int y, string text)
        {
            var label = new Label()
            {
                AutoSize = true,
                Text = text,
                Location = new Point()
                {
                    X = x,
                    Y = y,
                },
            };

            return label;
        }

        private Label BuildLabel(int x, int y, SelectorLine selectorLine)
        {
            var label = new Label()
            {
                AutoSize = true,
                Text = selectorLine.DisplayString,
                Location = new Point()
                {
                    X = x,
                    Y = y,
                },
                Tag = selectorLine,
            };

            return label;
        }

        private Label BuildLinkLabel(int x, int y, SelectorLine selectorLine)
        {
            var label = BuildLabel(x, y, selectorLine);
            label.ForeColor = Color.CornflowerBlue;

            label.Click += (object sender, EventArgs e) =>
            {
                var selectorLine = (sender as Label).Tag as SelectorLine;

                LinkClicked?.Invoke(this, new LinkClickedEventArgs(selectorLine));
            };

            return label;
        }

        private Label BuildUnknownTypeLabel(int x, int y, SelectorLine selectorLine)
        {
            var label = BuildLabel(x, y, selectorLine);
            label.ForeColor = Color.LightGray;
            UnknownEntity.SetToolTip(label, "I'm not yet sure how to render this type of item.");

            return label;
        }

        private void btnSavePage_Click(object sender, EventArgs e)
        {
            var saveFileDialog = new SaveFileDialog
            {
                OverwritePrompt = true,
            };

            if (CurrentPageType == ItemType.Text)
            {
                saveFileDialog.DefaultExt = ".txt";
                saveFileDialog.Filter = "Text Files (*.txt)|*.txt";
            }
            else if (CurrentPageType == ItemType.Menu)
            {
                saveFileDialog.DefaultExt = ".waffle";
                saveFileDialog.Filter = "Text Files (*.waffle)|*.waffle";
            }
            else if (CurrentPageType == ItemType.PNG)
            {
                saveFileDialog.DefaultExt = ".png";
                saveFileDialog.Filter = "PNG Files (*.png)|*.png";
            }
            else if (CurrentPageType == ItemType.Image)
            {
                saveFileDialog.DefaultExt = ".jpg";
                saveFileDialog.Filter = "JPG Files (*.jpg)|*.jpg";
            }
            else if (CurrentPageType == ItemType.GopherSourceCode)
            {
                saveFileDialog.DefaultExt = ".waffle";
                saveFileDialog.Filter = "Text Files (*.waffle)|*.waffle";
            }
            else
            {
                saveFileDialog.DefaultExt = ".waffle";
                saveFileDialog.Filter = "Waffle Files (*.waffle)|*.waffle";
            }

            var ans = saveFileDialog.ShowDialog();

            if (ans != DialogResult.OK)
            {
                return;
            }

            if (CurrentPageType == ItemType.Text)
            {
                File.WriteAllText(saveFileDialog.FileName, CurrentlyDisplayedText);
            }
            else if (CurrentPageType == ItemType.Menu)
            {
                File.WriteAllText(saveFileDialog.FileName, CurrentlyDisplayedText);
            }
            else if (CurrentPageType == ItemType.PNG)
            {
                CurrentlyDisplayedPng.Save(saveFileDialog.FileName, ImageFormat.Png);
            }
            else if (CurrentPageType == ItemType.Image)
            {
                CurrentlyDisplayedImage.Save(saveFileDialog.FileName, ImageFormat.Jpeg);
            }
            else
            {
                File.WriteAllText(saveFileDialog.FileName, CurrentlyDisplayedText);
            }
        }

        private void btnViewSource_Click(object sender, EventArgs e)
        {
            ViewingSource?.Invoke(this, new ViewSourceEventArgs(CurrentlyDisplayedText));
        }

        private void btnViewHistory_Click(object sender, EventArgs e)
        {
            ViewingHistory?.Invoke(this, new ViewHistoryEventArgs());
        }

        public static PageRenderer Instance()
        {
            var pageRenderer = new PageRenderer
            {
                AutoScroll = true,
                Dock = DockStyle.Fill
            };

            return pageRenderer;
        }

        private void btnCloseTab_Click(object sender, EventArgs e)
        {
            CloseTab?.Invoke(this, new EventArgs());
        }
    }
}

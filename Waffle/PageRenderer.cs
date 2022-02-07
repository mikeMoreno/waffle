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
using Waffle.Lib;

namespace Waffle
{
    public partial class PageRenderer : UserControl
    {
        private ToolTip UnknownEntity { get; }

        public WaffleLib WaffleLib { get; set; }

        private string CurrentlyDisplayedText { get; set; }

        private Image CurrentlyDisplayedPng { get; set; }

        private ResponseType CurrentPageType { get; set; }

        public delegate void LinkClickedEventHandler(object sender, LinkClickedEventArgs e);

        public event LinkClickedEventHandler LinkClicked;

        public PageRenderer()
        {
            InitializeComponent();

            UnknownEntity = new ToolTip();
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
                else if (line.ItemType == ItemType.TextFile)
                {
                    label = BuildLinkLabel(x: 10, y, line);
                }
                else if (line.ItemType == ItemType.Submenu)
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
                else
                {
                    label = BuildUnknownTypeLabel(x: 10, y, line);
                }

                Controls.Add(label);

                y += 20;

                text.Append(line.Raw);
            }

            CurrentlyDisplayedText = text.ToString();

            CurrentPageType = ResponseType.Menu;
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
                    label = BuildLinkLabel(x: 10, y, line);
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

            CurrentPageType = ResponseType.TextFile;
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

            CurrentPageType = ResponseType.PNG;
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

        private Label BuildLinkLabel(int x, int y, string line)
        {
            var label = BuildLabel(x, y, line);
            label.ForeColor = Color.CornflowerBlue;

            label.Click += async (object sender, EventArgs e) =>
            {
                LinkClicked?.Invoke(this, new LinkClickedEventArgs(line));

                var responseType = WaffleLib.GetLinkType(line);

                switch (responseType)
                {
                    case ResponseType.Menu:
                        Render(await WaffleLib.GetMenuAsync(line));
                        break;
                    case ResponseType.TextFile:
                        Render(await WaffleLib.GetTextFileAsync(line));
                        break;
                    case ResponseType.PNG:
                        Render(await WaffleLib.GetPngFileAsync(line));
                        break;
                    default:
                        Render(await WaffleLib.GetMenuAsync(line));
                        break;
                }
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

            label.Click += async (object sender, EventArgs e) =>
            {
                var selectorLine = (sender as Label).Tag as SelectorLine;

                if (selectorLine.ItemType == ItemType.TextFile)
                {
                    LinkClicked?.Invoke(this, new LinkClickedEventArgs(selectorLine.GetLink()));

                    var response = await WaffleLib.GetTextFileAsync(selectorLine.GetLink());

                    Render(response);
                }
                else if (selectorLine.ItemType == ItemType.Submenu)
                {
                    LinkClicked?.Invoke(this, new LinkClickedEventArgs(selectorLine.GetLink()));

                    var response = await WaffleLib.GetMenuAsync(selectorLine.GetLink());

                    Render(response);
                }
                else if (selectorLine.ItemType == ItemType.PNG)
                {
                    LinkClicked?.Invoke(this, new LinkClickedEventArgs(selectorLine.GetLink()));

                    var response = await WaffleLib.GetPngFileAsync(selectorLine.GetLink());

                    Render(response);
                }
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

            if (CurrentPageType == ResponseType.TextFile)
            {
                saveFileDialog.DefaultExt = ".txt";
                saveFileDialog.Filter = "Text Files (*.txt)|*.txt";
            }
            else if (CurrentPageType == ResponseType.Menu)
            {
                saveFileDialog.DefaultExt = ".waffle";
                saveFileDialog.Filter = "Text Files (*.waffle)|*.waffle";
            }
            else if (CurrentPageType == ResponseType.PNG)
            {
                saveFileDialog.DefaultExt = ".png";
                saveFileDialog.Filter = "PNG Files (*.png)|*.png";
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

            if (CurrentPageType == ResponseType.TextFile)
            {
                File.WriteAllText(saveFileDialog.FileName, CurrentlyDisplayedText);
            }
            else if (CurrentPageType == ResponseType.Menu)
            {
                File.WriteAllText(saveFileDialog.FileName, CurrentlyDisplayedText);
            }
            else if (CurrentPageType == ResponseType.PNG)
            {
                CurrentlyDisplayedPng.Save(saveFileDialog.FileName, ImageFormat.Png);
            }
            else
            {
                File.WriteAllText(saveFileDialog.FileName, CurrentlyDisplayedText);
            }
        }
    }
}

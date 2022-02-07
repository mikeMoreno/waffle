using System;
using System.Collections.Generic;
using System.ComponentModel;
using System.Data;
using System.Drawing;
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

        public PageRenderer()
        {
            InitializeComponent();

            UnknownEntity = new ToolTip();
        }

        public void Render(MenuResponse response)
        {
            Controls.Clear();

            var lines = response.Lines;

            var y = 10;

            foreach (var line in lines)
            {
                if (line.DisplayString == ".")
                {
                    break;
                }

                Label label;

                if (line.ItemType == "i")
                {
                    label = BuildLabel(x: 10, y, line);
                }
                else if (line.ItemType == "0")
                {
                    label = BuildLinkLabel(x: 10, y, line);
                }
                else if (line.ItemType == "1")
                {
                    label = BuildLinkLabel(x: 10, y, line);
                }
                else
                {
                    label = BuildUnknownTypeLabel(x: 10, y, line);
                }

                Controls.Add(label);

                y += 20;
            }
        }

        public void Render(TextResponse response)
        {
            Controls.Clear();

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

                //if (line.ItemType == "i")
                //{
                //    label = BuildLabel(x: 10, y, line);
                //}
                //else if (line.ItemType == "0")
                //{
                //    label = BuildLinkLabel(x: 10, y, line);
                //}
                //else if (line.ItemType == "1")
                //{
                //    label = BuildLinkLabel(x: 10, y, line);
                //}
                //else
                //{
                //    label = BuildUnknownTypeLabel(x: 10, y, line);
                //}

                Controls.Add(label);

                y += 20;
            }
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
                var responseType = WaffleLib.GetLinkType(line);

                switch (responseType)
                {
                    case ResponseType.Menu:
                        Render(await WaffleLib.GetMenuAsync(line));
                        break;
                    case ResponseType.TextFile:
                        Render(await WaffleLib.GetTextFileAsync(line));
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

                if (selectorLine.ItemType == "0")
                {
                    var response = await WaffleLib.GetTextFileAsync(selectorLine.GetLink());

                    Render(response);
                }
                else if (selectorLine.ItemType == "1")
                {
                    var response = await WaffleLib.GetMenuAsync(selectorLine.GetLink());

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
    }
}

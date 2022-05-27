using System;
using System.Collections.Generic;
using System.ComponentModel;
using System.Data;
using System.Diagnostics;
using System.Drawing;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using System.Windows.Forms;

namespace Waffle
{
    public partial class AboutForm : Form
    {
        public AboutForm()
        {
            InitializeComponent();

            Text = $"{Globals.ApplicationName} {Globals.ApplicationVersion}";
        }

        private void linkLabel1_LinkClicked(object sender, LinkLabelLinkClickedEventArgs e)
        {
            var process = new ProcessStartInfo
            {
                FileName = "https://github.com/mikeMoreno/waffle",
                UseShellExecute = true
            };

            Process.Start(process);
        }
    }
}

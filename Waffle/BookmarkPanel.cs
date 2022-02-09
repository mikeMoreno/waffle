using System;
using System.Collections.Generic;
using System.ComponentModel;
using System.Data;
using System.Drawing;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using System.Windows.Forms;

namespace Waffle
{
    public partial class BookmarkPanel : UserControl
    {
        public BookmarkPanel()
        {
            InitializeComponent();
        }

        private void btnClose_Click(object sender, EventArgs e)
        {
            this.Parent.Controls.Remove(this);
        }

        private void treeView1_DoubleClick(object sender, EventArgs e)
        {
            bookmarkTree.SelectedNode.BeginEdit();
        }
    }
}

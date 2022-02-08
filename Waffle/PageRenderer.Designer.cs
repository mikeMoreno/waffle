using System.Windows.Forms;

namespace Waffle
{
    partial class PageRenderer
    {
        /// <summary> 
        /// Required designer variable.
        /// </summary>
        private System.ComponentModel.IContainer components = null;

        /// <summary> 
        /// Clean up any resources being used.
        /// </summary>
        /// <param name="disposing">true if managed resources should be disposed; otherwise, false.</param>
        protected override void Dispose(bool disposing)
        {
            if (disposing && (components != null))
            {
                components.Dispose();
            }
            base.Dispose(disposing);
        }

        #region Component Designer generated code

        /// <summary> 
        /// Required method for Designer support - do not modify 
        /// the contents of this method with the code editor.
        /// </summary>
        private void InitializeComponent()
        {
            this.components = new System.ComponentModel.Container();
            this.pageRendererContextMenu = new System.Windows.Forms.ContextMenuStrip(this.components);
            this.btnSavePage = new System.Windows.Forms.ToolStripMenuItem();
            this.btnViewSource = new System.Windows.Forms.ToolStripMenuItem();
            this.btnCloseTab = new System.Windows.Forms.ToolStripMenuItem();
            this.toolStripSeparator1 = new System.Windows.Forms.ToolStripSeparator();
            this.pageRendererContextMenu.SuspendLayout();
            this.SuspendLayout();
            // 
            // pageRendererContextMenu
            // 
            this.pageRendererContextMenu.Items.AddRange(new System.Windows.Forms.ToolStripItem[] {
            this.btnSavePage,
            this.btnViewSource,
            this.toolStripSeparator1,
            this.btnCloseTab});
            this.pageRendererContextMenu.Name = "pageRendererContextMenu";
            this.pageRendererContextMenu.Size = new System.Drawing.Size(181, 98);
            // 
            // btnSavePage
            // 
            this.btnSavePage.Name = "btnSavePage";
            this.btnSavePage.Size = new System.Drawing.Size(180, 22);
            this.btnSavePage.Text = "Save";
            this.btnSavePage.Click += new System.EventHandler(this.btnSavePage_Click);
            // 
            // btnViewSource
            // 
            this.btnViewSource.Enabled = false;
            this.btnViewSource.Name = "btnViewSource";
            this.btnViewSource.Size = new System.Drawing.Size(180, 22);
            this.btnViewSource.Text = "View Source";
            this.btnViewSource.Click += new System.EventHandler(this.btnViewSource_Click);
            // 
            // btnCloseTab
            // 
            this.btnCloseTab.Name = "btnCloseTab";
            this.btnCloseTab.ShortcutKeys = ((System.Windows.Forms.Keys)((System.Windows.Forms.Keys.Control | System.Windows.Forms.Keys.W)));
            this.btnCloseTab.Size = new System.Drawing.Size(180, 22);
            this.btnCloseTab.Text = "Close";
            this.btnCloseTab.Click += new System.EventHandler(this.btnCloseTab_Click);
            // 
            // toolStripSeparator1
            // 
            this.toolStripSeparator1.Name = "toolStripSeparator1";
            this.toolStripSeparator1.Size = new System.Drawing.Size(177, 6);
            // 
            // PageRenderer
            // 
            this.AutoScaleDimensions = new System.Drawing.SizeF(7F, 15F);
            this.AutoScaleMode = System.Windows.Forms.AutoScaleMode.Font;
            this.ContextMenuStrip = this.pageRendererContextMenu;
            this.Font = new System.Drawing.Font("Courier New", 9F, System.Drawing.FontStyle.Regular, System.Drawing.GraphicsUnit.Point);
            this.Name = "PageRenderer";
            this.Size = new System.Drawing.Size(686, 451);
            this.pageRendererContextMenu.ResumeLayout(false);
            this.ResumeLayout(false);

        }

        #endregion

        private ContextMenuStrip pageRendererContextMenu;
        private ToolStripMenuItem btnSavePage;
        private ToolStripMenuItem btnViewSource;
        private ToolStripSeparator toolStripSeparator1;
        private ToolStripMenuItem btnCloseTab;
    }
}

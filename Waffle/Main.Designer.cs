using System.Windows.Forms;

namespace Waffle
{
    partial class Main
    {
        /// <summary>
        ///  Required designer variable.
        /// </summary>
        private System.ComponentModel.IContainer components = null;

        /// <summary>
        ///  Clean up any resources being used.
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

        #region Windows Form Designer generated code

        /// <summary>
        ///  Required method for Designer support - do not modify
        ///  the contents of this method with the code editor.
        /// </summary>
        private void InitializeComponent()
        {
            System.ComponentModel.ComponentResourceManager resources = new System.ComponentModel.ComponentResourceManager(typeof(Main));
            this.txtUrl = new System.Windows.Forms.TextBox();
            this.btnGo = new System.Windows.Forms.Button();
            this.btnBack = new System.Windows.Forms.Button();
            this.tabSitePages = new System.Windows.Forms.TabControl();
            this.tabDefaultTab = new System.Windows.Forms.TabPage();
            this.tabNewTab = new System.Windows.Forms.TabPage();
            this.tabSitePages.SuspendLayout();
            this.SuspendLayout();
            // 
            // txtUrl
            // 
            this.txtUrl.Anchor = ((System.Windows.Forms.AnchorStyles)(((System.Windows.Forms.AnchorStyles.Bottom | System.Windows.Forms.AnchorStyles.Left) 
            | System.Windows.Forms.AnchorStyles.Right)));
            this.txtUrl.Location = new System.Drawing.Point(12, 414);
            this.txtUrl.Name = "txtUrl";
            this.txtUrl.Size = new System.Drawing.Size(611, 23);
            this.txtUrl.TabIndex = 1;
            this.txtUrl.Leave += new System.EventHandler(this.txtUrl_Leave);
            // 
            // btnGo
            // 
            this.btnGo.Anchor = ((System.Windows.Forms.AnchorStyles)((System.Windows.Forms.AnchorStyles.Bottom | System.Windows.Forms.AnchorStyles.Right)));
            this.btnGo.Location = new System.Drawing.Point(713, 413);
            this.btnGo.Name = "btnGo";
            this.btnGo.Size = new System.Drawing.Size(75, 23);
            this.btnGo.TabIndex = 2;
            this.btnGo.Text = "Go";
            this.btnGo.UseVisualStyleBackColor = true;
            this.btnGo.Click += new System.EventHandler(this.btnGo_Click);
            // 
            // btnBack
            // 
            this.btnBack.Anchor = ((System.Windows.Forms.AnchorStyles)((System.Windows.Forms.AnchorStyles.Bottom | System.Windows.Forms.AnchorStyles.Right)));
            this.btnBack.Enabled = false;
            this.btnBack.Location = new System.Drawing.Point(629, 413);
            this.btnBack.Name = "btnBack";
            this.btnBack.Size = new System.Drawing.Size(75, 23);
            this.btnBack.TabIndex = 4;
            this.btnBack.Text = "<---";
            this.btnBack.UseVisualStyleBackColor = true;
            this.btnBack.Click += new System.EventHandler(this.btnBack_Click);
            // 
            // tabSitePages
            // 
            this.tabSitePages.Anchor = ((System.Windows.Forms.AnchorStyles)((((System.Windows.Forms.AnchorStyles.Top | System.Windows.Forms.AnchorStyles.Bottom) 
            | System.Windows.Forms.AnchorStyles.Left) 
            | System.Windows.Forms.AnchorStyles.Right)));
            this.tabSitePages.Controls.Add(this.tabDefaultTab);
            this.tabSitePages.Controls.Add(this.tabNewTab);
            this.tabSitePages.Location = new System.Drawing.Point(12, 12);
            this.tabSitePages.Name = "tabSitePages";
            this.tabSitePages.SelectedIndex = 0;
            this.tabSitePages.Size = new System.Drawing.Size(776, 395);
            this.tabSitePages.TabIndex = 5;
            // 
            // tabDefaultTab
            // 
            this.tabDefaultTab.BackColor = System.Drawing.Color.White;
            this.tabDefaultTab.Location = new System.Drawing.Point(4, 24);
            this.tabDefaultTab.Name = "tabDefaultTab";
            this.tabDefaultTab.Padding = new System.Windows.Forms.Padding(3);
            this.tabDefaultTab.Size = new System.Drawing.Size(768, 367);
            this.tabDefaultTab.TabIndex = 0;
            this.tabDefaultTab.Text = "New Tab";
            // 
            // tabNewTab
            // 
            this.tabNewTab.Location = new System.Drawing.Point(4, 24);
            this.tabNewTab.Name = "tabNewTab";
            this.tabNewTab.Size = new System.Drawing.Size(768, 367);
            this.tabNewTab.TabIndex = 1;
            this.tabNewTab.Text = "+";
            this.tabNewTab.UseVisualStyleBackColor = true;
            // 
            // Main
            // 
            this.AutoScaleDimensions = new System.Drawing.SizeF(7F, 15F);
            this.AutoScaleMode = System.Windows.Forms.AutoScaleMode.Font;
            this.ClientSize = new System.Drawing.Size(800, 450);
            this.Controls.Add(this.tabSitePages);
            this.Controls.Add(this.btnBack);
            this.Controls.Add(this.btnGo);
            this.Controls.Add(this.txtUrl);
            this.Icon = ((System.Drawing.Icon)(resources.GetObject("$this.Icon")));
            this.Name = "Main";
            this.StartPosition = System.Windows.Forms.FormStartPosition.CenterScreen;
            this.Text = "Waffle";
            this.tabSitePages.ResumeLayout(false);
            this.ResumeLayout(false);
            this.PerformLayout();

        }

        #endregion
        private TextBox txtUrl;
        private Button btnGo;
        private Button btnBack;
        private TabControl tabSitePages;
        private TabPage tabDefaultTab;
        private TabPage tabNewTab;
    }
}
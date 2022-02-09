namespace Waffle
{
    partial class BookmarkPanel
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
            this.btnClose = new System.Windows.Forms.Button();
            this.label1 = new System.Windows.Forms.Label();
            this.bookmarkTree = new System.Windows.Forms.TreeView();
            this.SuspendLayout();
            // 
            // btnClose
            // 
            this.btnClose.Anchor = ((System.Windows.Forms.AnchorStyles)((System.Windows.Forms.AnchorStyles.Top | System.Windows.Forms.AnchorStyles.Right)));
            this.btnClose.Location = new System.Drawing.Point(388, 0);
            this.btnClose.Name = "btnClose";
            this.btnClose.Size = new System.Drawing.Size(32, 23);
            this.btnClose.TabIndex = 0;
            this.btnClose.Text = "X";
            this.btnClose.UseVisualStyleBackColor = true;
            this.btnClose.Click += new System.EventHandler(this.btnClose_Click);
            // 
            // label1
            // 
            this.label1.AutoSize = true;
            this.label1.Font = new System.Drawing.Font("Segoe UI", 15.75F, System.Drawing.FontStyle.Regular, System.Drawing.GraphicsUnit.Point);
            this.label1.Location = new System.Drawing.Point(3, 8);
            this.label1.Name = "label1";
            this.label1.Size = new System.Drawing.Size(114, 30);
            this.label1.TabIndex = 2;
            this.label1.Text = "Bookmarks";
            // 
            // bookmarkTree
            // 
            this.bookmarkTree.Anchor = ((System.Windows.Forms.AnchorStyles)((((System.Windows.Forms.AnchorStyles.Top | System.Windows.Forms.AnchorStyles.Bottom) 
            | System.Windows.Forms.AnchorStyles.Left) 
            | System.Windows.Forms.AnchorStyles.Right)));
            this.bookmarkTree.LabelEdit = true;
            this.bookmarkTree.Location = new System.Drawing.Point(3, 41);
            this.bookmarkTree.Name = "bookmarkTree";
            this.bookmarkTree.Size = new System.Drawing.Size(417, 491);
            this.bookmarkTree.TabIndex = 3;
            this.bookmarkTree.AfterLabelEdit += new System.Windows.Forms.NodeLabelEditEventHandler(this.bookmarkTree_AfterLabelEdit);
            this.bookmarkTree.NodeMouseClick += new System.Windows.Forms.TreeNodeMouseClickEventHandler(this.bookmarkTree_NodeMouseClick);
            // 
            // BookmarkPanel
            // 
            this.AutoScaleDimensions = new System.Drawing.SizeF(7F, 15F);
            this.AutoScaleMode = System.Windows.Forms.AutoScaleMode.Font;
            this.Controls.Add(this.bookmarkTree);
            this.Controls.Add(this.label1);
            this.Controls.Add(this.btnClose);
            this.Name = "BookmarkPanel";
            this.Size = new System.Drawing.Size(420, 532);
            this.ResumeLayout(false);
            this.PerformLayout();

        }

        #endregion

        private Button btnClose;
        private Label label1;
        private TreeView bookmarkTree;
    }
}

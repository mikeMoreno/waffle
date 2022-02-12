namespace Waffle.History
{
    partial class HistoryForm
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

        #region Windows Form Designer generated code

        /// <summary>
        /// Required method for Designer support - do not modify
        /// the contents of this method with the code editor.
        /// </summary>
        private void InitializeComponent()
        {
            this.listHistoryEntities = new System.Windows.Forms.ListBox();
            this.SuspendLayout();
            // 
            // listHistoryEntities
            // 
            this.listHistoryEntities.FormattingEnabled = true;
            this.listHistoryEntities.ItemHeight = 15;
            this.listHistoryEntities.Location = new System.Drawing.Point(12, 12);
            this.listHistoryEntities.Name = "listHistoryEntities";
            this.listHistoryEntities.Size = new System.Drawing.Size(776, 424);
            this.listHistoryEntities.TabIndex = 0;
            this.listHistoryEntities.DoubleClick += new System.EventHandler(this.listHistoryEntities_DoubleClick);
            // 
            // HistoryForm
            // 
            this.AutoScaleDimensions = new System.Drawing.SizeF(7F, 15F);
            this.AutoScaleMode = System.Windows.Forms.AutoScaleMode.Font;
            this.ClientSize = new System.Drawing.Size(800, 450);
            this.Controls.Add(this.listHistoryEntities);
            this.Name = "HistoryForm";
            this.StartPosition = System.Windows.Forms.FormStartPosition.CenterScreen;
            this.Text = "History";
            this.ResumeLayout(false);

        }

        #endregion

        private ListBox listHistoryEntities;
    }
}
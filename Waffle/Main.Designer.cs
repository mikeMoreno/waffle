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
            this.txtUrl = new System.Windows.Forms.TextBox();
            this.btnGo = new System.Windows.Forms.Button();
            this.pageRenderer = new Waffle.PageRenderer();
            this.SuspendLayout();
            // 
            // txtUrl
            // 
            this.txtUrl.Location = new System.Drawing.Point(12, 414);
            this.txtUrl.Name = "txtUrl";
            this.txtUrl.Size = new System.Drawing.Size(685, 23);
            this.txtUrl.TabIndex = 1;
            this.txtUrl.Text = "gopher://gopher.floodgap.com";
            // 
            // btnGo
            // 
            this.btnGo.Location = new System.Drawing.Point(713, 413);
            this.btnGo.Name = "btnGo";
            this.btnGo.Size = new System.Drawing.Size(75, 23);
            this.btnGo.TabIndex = 2;
            this.btnGo.Text = "Go";
            this.btnGo.UseVisualStyleBackColor = true;
            this.btnGo.Click += new System.EventHandler(this.btnGo_Click);
            // 
            // pageRenderer
            // 
            this.pageRenderer.AutoScroll = true;
            this.pageRenderer.Location = new System.Drawing.Point(12, 12);
            this.pageRenderer.Name = "pageRenderer";
            this.pageRenderer.Size = new System.Drawing.Size(776, 395);
            this.pageRenderer.TabIndex = 3;
            // 
            // Main
            // 
            this.AutoScaleDimensions = new System.Drawing.SizeF(7F, 15F);
            this.AutoScaleMode = System.Windows.Forms.AutoScaleMode.Font;
            this.ClientSize = new System.Drawing.Size(800, 450);
            this.Controls.Add(this.pageRenderer);
            this.Controls.Add(this.btnGo);
            this.Controls.Add(this.txtUrl);
            this.Name = "Main";
            this.Text = "Waffle";
            this.ResumeLayout(false);
            this.PerformLayout();

        }

        #endregion
        private TextBox txtUrl;
        private Button btnGo;
        private PageRenderer pageRenderer;
    }
}
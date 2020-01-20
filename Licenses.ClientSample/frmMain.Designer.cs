namespace Licenses.ClientSample
{
    partial class frmMain
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
            this.btnManual = new System.Windows.Forms.Button();
            this.btnStress = new System.Windows.Forms.Button();
            this.SuspendLayout();
            // 
            // btnManual
            // 
            this.btnManual.Anchor = ((System.Windows.Forms.AnchorStyles)(((System.Windows.Forms.AnchorStyles.Top | System.Windows.Forms.AnchorStyles.Left) 
            | System.Windows.Forms.AnchorStyles.Right)));
            this.btnManual.Location = new System.Drawing.Point(12, 12);
            this.btnManual.Name = "btnManual";
            this.btnManual.Size = new System.Drawing.Size(310, 23);
            this.btnManual.TabIndex = 0;
            this.btnManual.Text = "Manual Testing";
            this.btnManual.UseVisualStyleBackColor = true;
            this.btnManual.Click += new System.EventHandler(this.btnManual_Click);
            // 
            // btnStress
            // 
            this.btnStress.Anchor = ((System.Windows.Forms.AnchorStyles)(((System.Windows.Forms.AnchorStyles.Top | System.Windows.Forms.AnchorStyles.Left) 
            | System.Windows.Forms.AnchorStyles.Right)));
            this.btnStress.Location = new System.Drawing.Point(12, 41);
            this.btnStress.Name = "btnStress";
            this.btnStress.Size = new System.Drawing.Size(310, 23);
            this.btnStress.TabIndex = 1;
            this.btnStress.Text = "Stress Testing";
            this.btnStress.UseVisualStyleBackColor = true;
            this.btnStress.Click += new System.EventHandler(this.btnStress_Click);
            // 
            // frmMain
            // 
            this.AutoScaleDimensions = new System.Drawing.SizeF(6F, 13F);
            this.AutoScaleMode = System.Windows.Forms.AutoScaleMode.Font;
            this.ClientSize = new System.Drawing.Size(334, 311);
            this.Controls.Add(this.btnStress);
            this.Controls.Add(this.btnManual);
            this.MinimumSize = new System.Drawing.Size(350, 350);
            this.Name = "frmMain";
            this.StartPosition = System.Windows.Forms.FormStartPosition.CenterScreen;
            this.Text = "frmMain";
            this.ResumeLayout(false);

        }

        #endregion

        private System.Windows.Forms.Button btnManual;
        private System.Windows.Forms.Button btnStress;
    }
}
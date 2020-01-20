namespace Licenses.ClientSample
{
    partial class frmStress
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
            this.btnRandom = new System.Windows.Forms.Button();
            this.btnRerun = new System.Windows.Forms.Button();
            this.txtTestNum = new System.Windows.Forms.TextBox();
            this.label1 = new System.Windows.Forms.Label();
            this.lstResults = new System.Windows.Forms.ListBox();
            this.label2 = new System.Windows.Forms.Label();
            this.txtAddress = new System.Windows.Forms.TextBox();
            this.SuspendLayout();
            // 
            // btnRandom
            // 
            this.btnRandom.Location = new System.Drawing.Point(12, 38);
            this.btnRandom.Name = "btnRandom";
            this.btnRandom.Size = new System.Drawing.Size(123, 23);
            this.btnRandom.TabIndex = 0;
            this.btnRandom.Text = "Random";
            this.btnRandom.UseVisualStyleBackColor = true;
            this.btnRandom.Click += new System.EventHandler(this.btnRandom_Click);
            // 
            // btnRerun
            // 
            this.btnRerun.Enabled = false;
            this.btnRerun.Location = new System.Drawing.Point(12, 67);
            this.btnRerun.Name = "btnRerun";
            this.btnRerun.Size = new System.Drawing.Size(123, 23);
            this.btnRerun.TabIndex = 1;
            this.btnRerun.Text = "Rerun";
            this.btnRerun.UseVisualStyleBackColor = true;
            // 
            // txtTestNum
            // 
            this.txtTestNum.Location = new System.Drawing.Point(141, 41);
            this.txtTestNum.Name = "txtTestNum";
            this.txtTestNum.Size = new System.Drawing.Size(100, 20);
            this.txtTestNum.TabIndex = 2;
            this.txtTestNum.Text = "100";
            // 
            // label1
            // 
            this.label1.AutoSize = true;
            this.label1.Location = new System.Drawing.Point(247, 44);
            this.label1.Name = "label1";
            this.label1.Size = new System.Drawing.Size(111, 13);
            this.label1.TabIndex = 3;
            this.label1.Text = "Number of tests to run";
            // 
            // lstResults
            // 
            this.lstResults.Anchor = ((System.Windows.Forms.AnchorStyles)((((System.Windows.Forms.AnchorStyles.Top | System.Windows.Forms.AnchorStyles.Bottom) 
            | System.Windows.Forms.AnchorStyles.Left) 
            | System.Windows.Forms.AnchorStyles.Right)));
            this.lstResults.FormattingEnabled = true;
            this.lstResults.IntegralHeight = false;
            this.lstResults.Location = new System.Drawing.Point(12, 96);
            this.lstResults.Name = "lstResults";
            this.lstResults.Size = new System.Drawing.Size(383, 277);
            this.lstResults.TabIndex = 4;
            // 
            // label2
            // 
            this.label2.AutoSize = true;
            this.label2.Enabled = false;
            this.label2.Location = new System.Drawing.Point(141, 72);
            this.label2.Name = "label2";
            this.label2.Size = new System.Drawing.Size(112, 13);
            this.label2.TabIndex = 5;
            this.label2.Text = "Read from last test log";
            // 
            // txtAddress
            // 
            this.txtAddress.Font = new System.Drawing.Font("Courier New", 8.25F);
            this.txtAddress.Location = new System.Drawing.Point(12, 12);
            this.txtAddress.Name = "txtAddress";
            this.txtAddress.Size = new System.Drawing.Size(383, 20);
            this.txtAddress.TabIndex = 6;
            this.txtAddress.Text = "https://10.142.32.85/Licenses/api/ClientAPI";
            // 
            // frmStress
            // 
            this.AutoScaleDimensions = new System.Drawing.SizeF(6F, 13F);
            this.AutoScaleMode = System.Windows.Forms.AutoScaleMode.Font;
            this.ClientSize = new System.Drawing.Size(407, 396);
            this.Controls.Add(this.txtAddress);
            this.Controls.Add(this.label2);
            this.Controls.Add(this.lstResults);
            this.Controls.Add(this.label1);
            this.Controls.Add(this.txtTestNum);
            this.Controls.Add(this.btnRerun);
            this.Controls.Add(this.btnRandom);
            this.Name = "frmStress";
            this.StartPosition = System.Windows.Forms.FormStartPosition.CenterParent;
            this.Text = "frmStress";
            this.ResumeLayout(false);
            this.PerformLayout();

        }

        #endregion

        private System.Windows.Forms.Button btnRandom;
        private System.Windows.Forms.Button btnRerun;
        private System.Windows.Forms.TextBox txtTestNum;
        private System.Windows.Forms.Label label1;
        private System.Windows.Forms.ListBox lstResults;
        private System.Windows.Forms.Label label2;
        private System.Windows.Forms.TextBox txtAddress;
    }
}
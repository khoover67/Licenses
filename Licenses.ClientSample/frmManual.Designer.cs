namespace Licenses.ClientSample
{
    partial class frmManual
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
            this.groupBox1 = new System.Windows.Forms.GroupBox();
            this.panel1 = new System.Windows.Forms.Panel();
            this.txtDatabasePath = new System.Windows.Forms.TextBox();
            this.label6 = new System.Windows.Forms.Label();
            this.cboDatabase = new System.Windows.Forms.ComboBox();
            this.label2 = new System.Windows.Forms.Label();
            this.txtClientName = new System.Windows.Forms.TextBox();
            this.label3 = new System.Windows.Forms.Label();
            this.txtServerAddress = new System.Windows.Forms.TextBox();
            this.label1 = new System.Windows.Forms.Label();
            this.groupBox2 = new System.Windows.Forms.GroupBox();
            this.panel2 = new System.Windows.Forms.Panel();
            this.label5 = new System.Windows.Forms.Label();
            this.txtProductName = new System.Windows.Forms.TextBox();
            this.label4 = new System.Windows.Forms.Label();
            this.btnExit = new System.Windows.Forms.Button();
            this.btnPost = new System.Windows.Forms.Button();
            this.groupBox3 = new System.Windows.Forms.GroupBox();
            this.panel3 = new System.Windows.Forms.Panel();
            this.btnDeleteUnit = new System.Windows.Forms.Button();
            this.btnAddUnit = new System.Windows.Forms.Button();
            this.lvwUnits = new System.Windows.Forms.ListView();
            this.colUnitName = ((System.Windows.Forms.ColumnHeader)(new System.Windows.Forms.ColumnHeader()));
            this.colUnitCount = ((System.Windows.Forms.ColumnHeader)(new System.Windows.Forms.ColumnHeader()));
            this.label10 = new System.Windows.Forms.Label();
            this.btnClear = new System.Windows.Forms.Button();
            this.label7 = new System.Windows.Forms.Label();
            this.txtDate = new System.Windows.Forms.TextBox();
            this.groupBox1.SuspendLayout();
            this.panel1.SuspendLayout();
            this.groupBox2.SuspendLayout();
            this.panel2.SuspendLayout();
            this.groupBox3.SuspendLayout();
            this.panel3.SuspendLayout();
            this.SuspendLayout();
            // 
            // groupBox1
            // 
            this.groupBox1.Anchor = ((System.Windows.Forms.AnchorStyles)(((System.Windows.Forms.AnchorStyles.Top | System.Windows.Forms.AnchorStyles.Left) 
            | System.Windows.Forms.AnchorStyles.Right)));
            this.groupBox1.Controls.Add(this.panel1);
            this.groupBox1.Font = new System.Drawing.Font("Tahoma", 9F, System.Drawing.FontStyle.Bold, System.Drawing.GraphicsUnit.Point, ((byte)(0)));
            this.groupBox1.ForeColor = System.Drawing.Color.Blue;
            this.groupBox1.Location = new System.Drawing.Point(12, 12);
            this.groupBox1.Name = "groupBox1";
            this.groupBox1.Size = new System.Drawing.Size(560, 180);
            this.groupBox1.TabIndex = 0;
            this.groupBox1.TabStop = false;
            this.groupBox1.Text = "Database Info";
            // 
            // panel1
            // 
            this.panel1.Anchor = ((System.Windows.Forms.AnchorStyles)((((System.Windows.Forms.AnchorStyles.Top | System.Windows.Forms.AnchorStyles.Bottom) 
            | System.Windows.Forms.AnchorStyles.Left) 
            | System.Windows.Forms.AnchorStyles.Right)));
            this.panel1.Controls.Add(this.txtDate);
            this.panel1.Controls.Add(this.label7);
            this.panel1.Controls.Add(this.txtDatabasePath);
            this.panel1.Controls.Add(this.label6);
            this.panel1.Controls.Add(this.cboDatabase);
            this.panel1.Controls.Add(this.label2);
            this.panel1.Controls.Add(this.txtClientName);
            this.panel1.Controls.Add(this.label3);
            this.panel1.Controls.Add(this.txtServerAddress);
            this.panel1.Controls.Add(this.label1);
            this.panel1.Font = new System.Drawing.Font("Microsoft Sans Serif", 8.25F);
            this.panel1.ForeColor = System.Drawing.SystemColors.ControlText;
            this.panel1.Location = new System.Drawing.Point(6, 21);
            this.panel1.Name = "panel1";
            this.panel1.Size = new System.Drawing.Size(548, 153);
            this.panel1.TabIndex = 0;
            // 
            // txtDatabasePath
            // 
            this.txtDatabasePath.Anchor = ((System.Windows.Forms.AnchorStyles)(((System.Windows.Forms.AnchorStyles.Top | System.Windows.Forms.AnchorStyles.Left) 
            | System.Windows.Forms.AnchorStyles.Right)));
            this.txtDatabasePath.Font = new System.Drawing.Font("Courier New", 8.25F);
            this.txtDatabasePath.Location = new System.Drawing.Point(98, 92);
            this.txtDatabasePath.Name = "txtDatabasePath";
            this.txtDatabasePath.Size = new System.Drawing.Size(429, 20);
            this.txtDatabasePath.TabIndex = 3;
            // 
            // label6
            // 
            this.label6.AutoSize = true;
            this.label6.Location = new System.Drawing.Point(11, 95);
            this.label6.Name = "label6";
            this.label6.Size = new System.Drawing.Size(81, 13);
            this.label6.TabIndex = 26;
            this.label6.Text = "Database Path:";
            // 
            // cboDatabase
            // 
            this.cboDatabase.Font = new System.Drawing.Font("Courier New", 8.25F, System.Drawing.FontStyle.Regular, System.Drawing.GraphicsUnit.Point, ((byte)(0)));
            this.cboDatabase.FormattingEnabled = true;
            this.cboDatabase.Location = new System.Drawing.Point(98, 13);
            this.cboDatabase.Name = "cboDatabase";
            this.cboDatabase.Size = new System.Drawing.Size(121, 22);
            this.cboDatabase.TabIndex = 0;
            this.cboDatabase.SelectedIndexChanged += new System.EventHandler(this.cboDatabase_SelectedIndexChanged);
            // 
            // label2
            // 
            this.label2.AutoSize = true;
            this.label2.Location = new System.Drawing.Point(36, 16);
            this.label2.Name = "label2";
            this.label2.Size = new System.Drawing.Size(56, 13);
            this.label2.TabIndex = 24;
            this.label2.Text = "Database:";
            // 
            // txtClientName
            // 
            this.txtClientName.Anchor = ((System.Windows.Forms.AnchorStyles)(((System.Windows.Forms.AnchorStyles.Top | System.Windows.Forms.AnchorStyles.Left) 
            | System.Windows.Forms.AnchorStyles.Right)));
            this.txtClientName.Font = new System.Drawing.Font("Courier New", 8.25F);
            this.txtClientName.Location = new System.Drawing.Point(98, 66);
            this.txtClientName.Name = "txtClientName";
            this.txtClientName.Size = new System.Drawing.Size(429, 20);
            this.txtClientName.TabIndex = 2;
            // 
            // label3
            // 
            this.label3.AutoSize = true;
            this.label3.Location = new System.Drawing.Point(25, 69);
            this.label3.Name = "label3";
            this.label3.Size = new System.Drawing.Size(67, 13);
            this.label3.TabIndex = 22;
            this.label3.Text = "Client Name:";
            // 
            // txtServerAddress
            // 
            this.txtServerAddress.Anchor = ((System.Windows.Forms.AnchorStyles)(((System.Windows.Forms.AnchorStyles.Top | System.Windows.Forms.AnchorStyles.Left) 
            | System.Windows.Forms.AnchorStyles.Right)));
            this.txtServerAddress.Font = new System.Drawing.Font("Courier New", 8.25F, System.Drawing.FontStyle.Regular, System.Drawing.GraphicsUnit.Point, ((byte)(0)));
            this.txtServerAddress.Location = new System.Drawing.Point(98, 40);
            this.txtServerAddress.Name = "txtServerAddress";
            this.txtServerAddress.Size = new System.Drawing.Size(429, 20);
            this.txtServerAddress.TabIndex = 1;
            this.txtServerAddress.Text = "https://10.142.32.85/Licenses/api/LicenseCount";
            // 
            // label1
            // 
            this.label1.AutoSize = true;
            this.label1.Location = new System.Drawing.Point(10, 43);
            this.label1.Name = "label1";
            this.label1.Size = new System.Drawing.Size(82, 13);
            this.label1.TabIndex = 18;
            this.label1.Text = "Server Address:";
            // 
            // groupBox2
            // 
            this.groupBox2.Anchor = ((System.Windows.Forms.AnchorStyles)(((System.Windows.Forms.AnchorStyles.Top | System.Windows.Forms.AnchorStyles.Left) 
            | System.Windows.Forms.AnchorStyles.Right)));
            this.groupBox2.Controls.Add(this.panel2);
            this.groupBox2.Font = new System.Drawing.Font("Tahoma", 9F, System.Drawing.FontStyle.Bold, System.Drawing.GraphicsUnit.Point, ((byte)(0)));
            this.groupBox2.ForeColor = System.Drawing.Color.Blue;
            this.groupBox2.Location = new System.Drawing.Point(12, 198);
            this.groupBox2.Name = "groupBox2";
            this.groupBox2.Size = new System.Drawing.Size(560, 75);
            this.groupBox2.TabIndex = 1;
            this.groupBox2.TabStop = false;
            this.groupBox2.Text = "Product Info";
            // 
            // panel2
            // 
            this.panel2.Anchor = ((System.Windows.Forms.AnchorStyles)((((System.Windows.Forms.AnchorStyles.Top | System.Windows.Forms.AnchorStyles.Bottom) 
            | System.Windows.Forms.AnchorStyles.Left) 
            | System.Windows.Forms.AnchorStyles.Right)));
            this.panel2.Controls.Add(this.label5);
            this.panel2.Controls.Add(this.txtProductName);
            this.panel2.Controls.Add(this.label4);
            this.panel2.Font = new System.Drawing.Font("Microsoft Sans Serif", 8.25F);
            this.panel2.ForeColor = System.Drawing.SystemColors.ControlText;
            this.panel2.Location = new System.Drawing.Point(6, 21);
            this.panel2.Name = "panel2";
            this.panel2.Size = new System.Drawing.Size(548, 48);
            this.panel2.TabIndex = 0;
            // 
            // label5
            // 
            this.label5.AutoSize = true;
            this.label5.Location = new System.Drawing.Point(10, 22);
            this.label5.Name = "label5";
            this.label5.Size = new System.Drawing.Size(78, 13);
            this.label5.TabIndex = 4;
            this.label5.Text = "Product Name:";
            // 
            // txtProductName
            // 
            this.txtProductName.Location = new System.Drawing.Point(94, 19);
            this.txtProductName.Name = "txtProductName";
            this.txtProductName.Size = new System.Drawing.Size(100, 20);
            this.txtProductName.TabIndex = 0;
            // 
            // label4
            // 
            this.label4.AutoSize = true;
            this.label4.Location = new System.Drawing.Point(3, 0);
            this.label4.Name = "label4";
            this.label4.Size = new System.Drawing.Size(49, 13);
            this.label4.TabIndex = 1;
            this.label4.Text = "Products";
            // 
            // btnExit
            // 
            this.btnExit.Anchor = ((System.Windows.Forms.AnchorStyles)((System.Windows.Forms.AnchorStyles.Bottom | System.Windows.Forms.AnchorStyles.Right)));
            this.btnExit.Location = new System.Drawing.Point(497, 442);
            this.btnExit.Name = "btnExit";
            this.btnExit.Size = new System.Drawing.Size(75, 23);
            this.btnExit.TabIndex = 5;
            this.btnExit.Text = "Exit";
            this.btnExit.UseVisualStyleBackColor = true;
            this.btnExit.Click += new System.EventHandler(this.Button_Click);
            // 
            // btnPost
            // 
            this.btnPost.Anchor = ((System.Windows.Forms.AnchorStyles)((System.Windows.Forms.AnchorStyles.Bottom | System.Windows.Forms.AnchorStyles.Left)));
            this.btnPost.Location = new System.Drawing.Point(12, 442);
            this.btnPost.Name = "btnPost";
            this.btnPost.Size = new System.Drawing.Size(132, 23);
            this.btnPost.TabIndex = 3;
            this.btnPost.Text = "Post";
            this.btnPost.UseVisualStyleBackColor = true;
            this.btnPost.Click += new System.EventHandler(this.Button_Click);
            // 
            // groupBox3
            // 
            this.groupBox3.Anchor = ((System.Windows.Forms.AnchorStyles)((((System.Windows.Forms.AnchorStyles.Top | System.Windows.Forms.AnchorStyles.Bottom) 
            | System.Windows.Forms.AnchorStyles.Left) 
            | System.Windows.Forms.AnchorStyles.Right)));
            this.groupBox3.Controls.Add(this.panel3);
            this.groupBox3.Font = new System.Drawing.Font("Tahoma", 9F, System.Drawing.FontStyle.Bold, System.Drawing.GraphicsUnit.Point, ((byte)(0)));
            this.groupBox3.ForeColor = System.Drawing.Color.Blue;
            this.groupBox3.Location = new System.Drawing.Point(12, 279);
            this.groupBox3.Name = "groupBox3";
            this.groupBox3.Size = new System.Drawing.Size(560, 157);
            this.groupBox3.TabIndex = 2;
            this.groupBox3.TabStop = false;
            this.groupBox3.Text = "Unit Info";
            // 
            // panel3
            // 
            this.panel3.Anchor = ((System.Windows.Forms.AnchorStyles)((((System.Windows.Forms.AnchorStyles.Top | System.Windows.Forms.AnchorStyles.Bottom) 
            | System.Windows.Forms.AnchorStyles.Left) 
            | System.Windows.Forms.AnchorStyles.Right)));
            this.panel3.Controls.Add(this.btnDeleteUnit);
            this.panel3.Controls.Add(this.btnAddUnit);
            this.panel3.Controls.Add(this.lvwUnits);
            this.panel3.Controls.Add(this.label10);
            this.panel3.Font = new System.Drawing.Font("Microsoft Sans Serif", 8.25F);
            this.panel3.ForeColor = System.Drawing.SystemColors.ControlText;
            this.panel3.Location = new System.Drawing.Point(6, 21);
            this.panel3.Name = "panel3";
            this.panel3.Size = new System.Drawing.Size(548, 130);
            this.panel3.TabIndex = 0;
            // 
            // btnDeleteUnit
            // 
            this.btnDeleteUnit.Location = new System.Drawing.Point(484, 45);
            this.btnDeleteUnit.Name = "btnDeleteUnit";
            this.btnDeleteUnit.Size = new System.Drawing.Size(60, 23);
            this.btnDeleteUnit.TabIndex = 1;
            this.btnDeleteUnit.Text = "Delete";
            this.btnDeleteUnit.UseVisualStyleBackColor = true;
            this.btnDeleteUnit.Click += new System.EventHandler(this.Button_Click);
            // 
            // btnAddUnit
            // 
            this.btnAddUnit.Location = new System.Drawing.Point(484, 16);
            this.btnAddUnit.Name = "btnAddUnit";
            this.btnAddUnit.Size = new System.Drawing.Size(60, 23);
            this.btnAddUnit.TabIndex = 0;
            this.btnAddUnit.Text = "Add...";
            this.btnAddUnit.UseVisualStyleBackColor = true;
            this.btnAddUnit.Click += new System.EventHandler(this.Button_Click);
            // 
            // lvwUnits
            // 
            this.lvwUnits.Anchor = ((System.Windows.Forms.AnchorStyles)((((System.Windows.Forms.AnchorStyles.Top | System.Windows.Forms.AnchorStyles.Bottom) 
            | System.Windows.Forms.AnchorStyles.Left) 
            | System.Windows.Forms.AnchorStyles.Right)));
            this.lvwUnits.Columns.AddRange(new System.Windows.Forms.ColumnHeader[] {
            this.colUnitName,
            this.colUnitCount});
            this.lvwUnits.FullRowSelect = true;
            this.lvwUnits.HideSelection = false;
            this.lvwUnits.Location = new System.Drawing.Point(3, 16);
            this.lvwUnits.MultiSelect = false;
            this.lvwUnits.Name = "lvwUnits";
            this.lvwUnits.Size = new System.Drawing.Size(475, 111);
            this.lvwUnits.TabIndex = 2;
            this.lvwUnits.UseCompatibleStateImageBehavior = false;
            this.lvwUnits.View = System.Windows.Forms.View.Details;
            // 
            // colUnitName
            // 
            this.colUnitName.Text = "Unit Name";
            this.colUnitName.Width = 300;
            // 
            // colUnitCount
            // 
            this.colUnitCount.Text = "Unit Count";
            this.colUnitCount.Width = 150;
            // 
            // label10
            // 
            this.label10.AutoSize = true;
            this.label10.Location = new System.Drawing.Point(3, 0);
            this.label10.Name = "label10";
            this.label10.Size = new System.Drawing.Size(31, 13);
            this.label10.TabIndex = 1;
            this.label10.Text = "Units";
            // 
            // btnClear
            // 
            this.btnClear.Location = new System.Drawing.Point(416, 442);
            this.btnClear.Name = "btnClear";
            this.btnClear.Size = new System.Drawing.Size(75, 23);
            this.btnClear.TabIndex = 4;
            this.btnClear.Text = "Clear";
            this.btnClear.UseVisualStyleBackColor = true;
            this.btnClear.Click += new System.EventHandler(this.Button_Click);
            // 
            // label7
            // 
            this.label7.AutoSize = true;
            this.label7.Location = new System.Drawing.Point(28, 121);
            this.label7.Name = "label7";
            this.label7.Size = new System.Drawing.Size(64, 13);
            this.label7.TabIndex = 27;
            this.label7.Text = "Count Date:";
            // 
            // txtDate
            // 
            this.txtDate.Location = new System.Drawing.Point(98, 118);
            this.txtDate.Name = "txtDate";
            this.txtDate.Size = new System.Drawing.Size(183, 20);
            this.txtDate.TabIndex = 4;
            // 
            // frmManual
            // 
            this.AutoScaleDimensions = new System.Drawing.SizeF(6F, 13F);
            this.AutoScaleMode = System.Windows.Forms.AutoScaleMode.Font;
            this.ClientSize = new System.Drawing.Size(584, 477);
            this.Controls.Add(this.btnClear);
            this.Controls.Add(this.groupBox3);
            this.Controls.Add(this.btnExit);
            this.Controls.Add(this.btnPost);
            this.Controls.Add(this.groupBox2);
            this.Controls.Add(this.groupBox1);
            this.MinimumSize = new System.Drawing.Size(600, 400);
            this.Name = "frmManual";
            this.StartPosition = System.Windows.Forms.FormStartPosition.CenterParent;
            this.Text = "Test License Server";
            this.Load += new System.EventHandler(this.frmMain_Load);
            this.groupBox1.ResumeLayout(false);
            this.panel1.ResumeLayout(false);
            this.panel1.PerformLayout();
            this.groupBox2.ResumeLayout(false);
            this.panel2.ResumeLayout(false);
            this.panel2.PerformLayout();
            this.groupBox3.ResumeLayout(false);
            this.panel3.ResumeLayout(false);
            this.panel3.PerformLayout();
            this.ResumeLayout(false);

        }

        #endregion

        private System.Windows.Forms.GroupBox groupBox1;
        private System.Windows.Forms.Panel panel1;
        private System.Windows.Forms.ComboBox cboDatabase;
        private System.Windows.Forms.Label label2;
        private System.Windows.Forms.TextBox txtClientName;
        private System.Windows.Forms.Label label3;
        private System.Windows.Forms.TextBox txtServerAddress;
        private System.Windows.Forms.Label label1;
        private System.Windows.Forms.GroupBox groupBox2;
        private System.Windows.Forms.Panel panel2;
        private System.Windows.Forms.Button btnExit;
        private System.Windows.Forms.Label label4;
        private System.Windows.Forms.TextBox txtDatabasePath;
        private System.Windows.Forms.Label label6;
        private System.Windows.Forms.Label label5;
        private System.Windows.Forms.TextBox txtProductName;
        private System.Windows.Forms.Button btnPost;
        private System.Windows.Forms.GroupBox groupBox3;
        private System.Windows.Forms.Panel panel3;
        private System.Windows.Forms.Label label10;
        private System.Windows.Forms.ListView lvwUnits;
        private System.Windows.Forms.ColumnHeader colUnitName;
        private System.Windows.Forms.ColumnHeader colUnitCount;
        private System.Windows.Forms.Button btnAddUnit;
        private System.Windows.Forms.Button btnDeleteUnit;
        private System.Windows.Forms.Button btnClear;
        private System.Windows.Forms.TextBox txtDate;
        private System.Windows.Forms.Label label7;
    }
}


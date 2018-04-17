namespace KeePassRPC.Forms
{
    partial class DatabaseSettingsUserControl
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
            this.groupBox1 = new System.Windows.Forms.GroupBox();
            this.label1 = new System.Windows.Forms.Label();
            this.radioButton3 = new System.Windows.Forms.RadioButton();
            this.radioButton2 = new System.Windows.Forms.RadioButton();
            this.radioButton1 = new System.Windows.Forms.RadioButton();
            this.groupBox2 = new System.Windows.Forms.GroupBox();
            this.label2 = new System.Windows.Forms.Label();
            this.buttonURLEdit = new System.Windows.Forms.Button();
            this.buttonURLDelete = new System.Windows.Forms.Button();
            this.buttonURLAdd = new System.Windows.Forms.Button();
            this.listView1 = new System.Windows.Forms.ListView();
            this.columnHeaderValue = ((System.Windows.Forms.ColumnHeader)(new System.Windows.Forms.ColumnHeader()));
            this.columnHeaderMethod = ((System.Windows.Forms.ColumnHeader)(new System.Windows.Forms.ColumnHeader()));
            this.groupBox1.SuspendLayout();
            this.groupBox2.SuspendLayout();
            this.SuspendLayout();
            // 
            // groupBox1
            // 
            this.groupBox1.Controls.Add(this.label1);
            this.groupBox1.Controls.Add(this.radioButton3);
            this.groupBox1.Controls.Add(this.radioButton2);
            this.groupBox1.Controls.Add(this.radioButton1);
            this.groupBox1.Location = new System.Drawing.Point(6, 7);
            this.groupBox1.Name = "groupBox1";
            this.groupBox1.Size = new System.Drawing.Size(440, 126);
            this.groupBox1.TabIndex = 0;
            this.groupBox1.TabStop = false;
            this.groupBox1.Text = "Default minimum URL matching accuracy";
            // 
            // label1
            // 
            this.label1.AutoSize = true;
            this.label1.Location = new System.Drawing.Point(6, 20);
            this.label1.Name = "label1";
            this.label1.Size = new System.Drawing.Size(411, 13);
            this.label1.TabIndex = 10;
            this.label1.Text = "This affects new entries and entries with no existing KeePassRPC (Kee) configurat" +
    "ion.";
            // 
            // radioButton3
            // 
            this.radioButton3.AutoSize = true;
            this.radioButton3.Location = new System.Drawing.Point(13, 93);
            this.radioButton3.Name = "radioButton3";
            this.radioButton3.Size = new System.Drawing.Size(361, 17);
            this.radioButton3.TabIndex = 9;
            this.radioButton3.TabStop = true;
            this.radioButton3.Text = "Exact: The URL must match exactly including full path and query string.";
            this.radioButton3.UseVisualStyleBackColor = true;
            // 
            // radioButton2
            // 
            this.radioButton2.AutoSize = true;
            this.radioButton2.Location = new System.Drawing.Point(13, 70);
            this.radioButton2.Name = "radioButton2";
            this.radioButton2.Size = new System.Drawing.Size(415, 17);
            this.radioButton2.TabIndex = 8;
            this.radioButton2.TabStop = true;
            this.radioButton2.Text = "Hostname: The URL must match the hostname (domain and subdomains) and port.";
            this.radioButton2.UseVisualStyleBackColor = true;
            // 
            // radioButton1
            // 
            this.radioButton1.AutoSize = true;
            this.radioButton1.Location = new System.Drawing.Point(13, 47);
            this.radioButton1.Name = "radioButton1";
            this.radioButton1.Size = new System.Drawing.Size(355, 17);
            this.radioButton1.TabIndex = 7;
            this.radioButton1.TabStop = true;
            this.radioButton1.Text = "Domain: The URL only needs to be part of the same domain to match.";
            this.radioButton1.UseVisualStyleBackColor = true;
            // 
            // groupBox2
            // 
            this.groupBox2.Controls.Add(this.label2);
            this.groupBox2.Controls.Add(this.buttonURLEdit);
            this.groupBox2.Controls.Add(this.buttonURLDelete);
            this.groupBox2.Controls.Add(this.buttonURLAdd);
            this.groupBox2.Controls.Add(this.listView1);
            this.groupBox2.Location = new System.Drawing.Point(6, 139);
            this.groupBox2.Name = "groupBox2";
            this.groupBox2.Size = new System.Drawing.Size(440, 178);
            this.groupBox2.TabIndex = 1;
            this.groupBox2.TabStop = false;
            this.groupBox2.Text = "Override minimum URL match accuracy";
            // 
            // label2
            // 
            this.label2.Location = new System.Drawing.Point(4, 20);
            this.label2.Name = "label2";
            this.label2.Size = new System.Drawing.Size(430, 35);
            this.label2.TabIndex = 11;
            this.label2.Text = "The match accuracy will be applied to all entries when a search URL matches the s" +
    "pecified domain name.";
            // 
            // buttonURLEdit
            // 
            this.buttonURLEdit.Enabled = false;
            this.buttonURLEdit.Location = new System.Drawing.Point(376, 88);
            this.buttonURLEdit.Name = "buttonURLEdit";
            this.buttonURLEdit.Size = new System.Drawing.Size(60, 26);
            this.buttonURLEdit.TabIndex = 9;
            this.buttonURLEdit.Text = "Edit";
            this.buttonURLEdit.UseVisualStyleBackColor = true;
            this.buttonURLEdit.Click += new System.EventHandler(this.buttonURLEdit_Click);
            // 
            // buttonURLDelete
            // 
            this.buttonURLDelete.Enabled = false;
            this.buttonURLDelete.Location = new System.Drawing.Point(376, 120);
            this.buttonURLDelete.Name = "buttonURLDelete";
            this.buttonURLDelete.Size = new System.Drawing.Size(60, 26);
            this.buttonURLDelete.TabIndex = 10;
            this.buttonURLDelete.Text = "Delete";
            this.buttonURLDelete.UseVisualStyleBackColor = true;
            this.buttonURLDelete.Click += new System.EventHandler(this.buttonURLDelete_Click);
            // 
            // buttonURLAdd
            // 
            this.buttonURLAdd.Location = new System.Drawing.Point(376, 56);
            this.buttonURLAdd.Name = "buttonURLAdd";
            this.buttonURLAdd.Size = new System.Drawing.Size(60, 26);
            this.buttonURLAdd.TabIndex = 8;
            this.buttonURLAdd.Text = "Add";
            this.buttonURLAdd.UseVisualStyleBackColor = true;
            this.buttonURLAdd.Click += new System.EventHandler(this.buttonURLAdd_Click);
            // 
            // listView1
            // 
            this.listView1.Columns.AddRange(new System.Windows.Forms.ColumnHeader[] {
            this.columnHeaderValue,
            this.columnHeaderMethod});
            this.listView1.FullRowSelect = true;
            this.listView1.HeaderStyle = System.Windows.Forms.ColumnHeaderStyle.Nonclickable;
            this.listView1.HideSelection = false;
            this.listView1.Location = new System.Drawing.Point(4, 56);
            this.listView1.MultiSelect = false;
            this.listView1.Name = "listView1";
            this.listView1.ShowItemToolTips = true;
            this.listView1.Size = new System.Drawing.Size(366, 114);
            this.listView1.TabIndex = 7;
            this.listView1.UseCompatibleStateImageBehavior = false;
            this.listView1.View = System.Windows.Forms.View.Details;
            this.listView1.SelectedIndexChanged += new System.EventHandler(this.listView1_SelectedIndexChanged);
            // 
            // columnHeaderValue
            // 
            this.columnHeaderValue.Text = "Domain";
            this.columnHeaderValue.Width = 240;
            // 
            // columnHeaderMethod
            // 
            this.columnHeaderMethod.Text = "Method";
            this.columnHeaderMethod.Width = 100;
            // 
            // DatabaseSettingsUserControl
            // 
            this.AutoScaleDimensions = new System.Drawing.SizeF(6F, 13F);
            this.AutoScaleMode = System.Windows.Forms.AutoScaleMode.Font;
            this.BackColor = System.Drawing.SystemColors.Window;
            this.Controls.Add(this.groupBox2);
            this.Controls.Add(this.groupBox1);
            this.Location = new System.Drawing.Point(4, 20);
            this.Name = "DatabaseSettingsUserControl";
            this.Size = new System.Drawing.Size(455, 320);
            this.groupBox1.ResumeLayout(false);
            this.groupBox1.PerformLayout();
            this.groupBox2.ResumeLayout(false);
            this.ResumeLayout(false);

        }

        #endregion

        private System.Windows.Forms.GroupBox groupBox1;
        private System.Windows.Forms.RadioButton radioButton2;
        private System.Windows.Forms.RadioButton radioButton1;
        private System.Windows.Forms.Label label1;
        private System.Windows.Forms.RadioButton radioButton3;
        private System.Windows.Forms.GroupBox groupBox2;
        private System.Windows.Forms.Label label2;
        private System.Windows.Forms.Button buttonURLEdit;
        private System.Windows.Forms.Button buttonURLDelete;
        private System.Windows.Forms.Button buttonURLAdd;
        private System.Windows.Forms.ListView listView1;
        private System.Windows.Forms.ColumnHeader columnHeaderValue;
        private System.Windows.Forms.ColumnHeader columnHeaderMethod;
    }
}

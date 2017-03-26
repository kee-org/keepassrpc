namespace KeePassRPC.Forms
{
    partial class KeeFoxEntryUserControl
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
            this.groupBox1 = new System.Windows.Forms.GroupBox();
            this.comboBoxAutoFill = new System.Windows.Forms.ComboBox();
            this.comboBoxAutoSubmit = new System.Windows.Forms.ComboBox();
            this.label4 = new System.Windows.Forms.Label();
            this.label3 = new System.Windows.Forms.Label();
            this.label1 = new System.Windows.Forms.Label();
            this.textBoxKeeFoxPriority = new System.Windows.Forms.TextBox();
            this.checkBoxHideFromKeeFox = new System.Windows.Forms.CheckBox();
            this.groupBox2 = new System.Windows.Forms.GroupBox();
            this.buttonFieldEdit = new System.Windows.Forms.Button();
            this.buttonFieldDelete = new System.Windows.Forms.Button();
            this.listView2 = new System.Windows.Forms.ListView();
            this.columnHeaderFName = ((System.Windows.Forms.ColumnHeader)(new System.Windows.Forms.ColumnHeader()));
            this.columnHeaderFValue = ((System.Windows.Forms.ColumnHeader)(new System.Windows.Forms.ColumnHeader()));
            this.columnHeaderFId = ((System.Windows.Forms.ColumnHeader)(new System.Windows.Forms.ColumnHeader()));
            this.columnHeaderFType = ((System.Windows.Forms.ColumnHeader)(new System.Windows.Forms.ColumnHeader()));
            this.columnHeaderFPage = ((System.Windows.Forms.ColumnHeader)(new System.Windows.Forms.ColumnHeader()));
            this.buttonFieldAdd = new System.Windows.Forms.Button();
            this.groupBox3 = new System.Windows.Forms.GroupBox();
            this.buttonURLEdit = new System.Windows.Forms.Button();
            this.buttonURLDelete = new System.Windows.Forms.Button();
            this.buttonURLAdd = new System.Windows.Forms.Button();
            this.listView1 = new System.Windows.Forms.ListView();
            this.columnHeaderValue = ((System.Windows.Forms.ColumnHeader)(new System.Windows.Forms.ColumnHeader()));
            this.columnHeaderMethod = ((System.Windows.Forms.ColumnHeader)(new System.Windows.Forms.ColumnHeader()));
            this.columnHeaderType = ((System.Windows.Forms.ColumnHeader)(new System.Windows.Forms.ColumnHeader()));
            this.tabControl1 = new System.Windows.Forms.TabControl();
            this.tabPage1 = new System.Windows.Forms.TabPage();
            this.textBoxKeeFoxRealm = new System.Windows.Forms.TextBox();
            this.labelRealm = new System.Windows.Forms.Label();
            this.tabPage2 = new System.Windows.Forms.TabPage();
            this.label2 = new System.Windows.Forms.Label();
            this.tabPage3 = new System.Windows.Forms.TabPage();
            this.toolTipRealm = new System.Windows.Forms.ToolTip(this.components);
            this.label5 = new System.Windows.Forms.Label();
            this.groupBox4 = new System.Windows.Forms.GroupBox();
            this.radioButton1 = new System.Windows.Forms.RadioButton();
            this.radioButton2 = new System.Windows.Forms.RadioButton();
            this.radioButton3 = new System.Windows.Forms.RadioButton();
            this.groupBox1.SuspendLayout();
            this.groupBox2.SuspendLayout();
            this.groupBox3.SuspendLayout();
            this.tabControl1.SuspendLayout();
            this.tabPage1.SuspendLayout();
            this.tabPage2.SuspendLayout();
            this.tabPage3.SuspendLayout();
            this.groupBox4.SuspendLayout();
            this.SuspendLayout();
            // 
            // groupBox1
            // 
            this.groupBox1.Controls.Add(this.comboBoxAutoFill);
            this.groupBox1.Controls.Add(this.comboBoxAutoSubmit);
            this.groupBox1.Controls.Add(this.label4);
            this.groupBox1.Controls.Add(this.label3);
            this.groupBox1.Location = new System.Drawing.Point(226, 19);
            this.groupBox1.Name = "groupBox1";
            this.groupBox1.Size = new System.Drawing.Size(214, 90);
            this.groupBox1.TabIndex = 0;
            this.groupBox1.TabStop = false;
            this.groupBox1.Text = "Entry behaviour";
            // 
            // comboBoxAutoFill
            // 
            this.comboBoxAutoFill.FormattingEnabled = true;
            this.comboBoxAutoFill.Items.AddRange(new object[] {
            "Use KeeFox setting",
            "Never",
            "Always"});
            this.comboBoxAutoFill.Location = new System.Drawing.Point(75, 20);
            this.comboBoxAutoFill.Name = "comboBoxAutoFill";
            this.comboBoxAutoFill.Size = new System.Drawing.Size(121, 21);
            this.comboBoxAutoFill.TabIndex = 7;
            // 
            // comboBoxAutoSubmit
            // 
            this.comboBoxAutoSubmit.FormattingEnabled = true;
            this.comboBoxAutoSubmit.Items.AddRange(new object[] {
            "Use KeeFox setting",
            "Never",
            "Always"});
            this.comboBoxAutoSubmit.Location = new System.Drawing.Point(75, 55);
            this.comboBoxAutoSubmit.Name = "comboBoxAutoSubmit";
            this.comboBoxAutoSubmit.Size = new System.Drawing.Size(121, 21);
            this.comboBoxAutoSubmit.TabIndex = 8;
            // 
            // label4
            // 
            this.label4.AutoSize = true;
            this.label4.Location = new System.Drawing.Point(6, 58);
            this.label4.Name = "label4";
            this.label4.Size = new System.Drawing.Size(65, 13);
            this.label4.TabIndex = 1;
            this.label4.Text = "Auto-submit:";
            // 
            // label3
            // 
            this.label3.AutoSize = true;
            this.label3.Location = new System.Drawing.Point(25, 23);
            this.label3.Name = "label3";
            this.label3.Size = new System.Drawing.Size(44, 13);
            this.label3.TabIndex = 0;
            this.label3.Text = "Auto-fill:";
            // 
            // label1
            // 
            this.label1.AutoSize = true;
            this.label1.Location = new System.Drawing.Point(12, 77);
            this.label1.Name = "label1";
            this.label1.Size = new System.Drawing.Size(142, 13);
            this.label1.TabIndex = 0;
            this.label1.Text = "Priority override (e.g. 1 - 100)";
            // 
            // textBoxKeeFoxPriority
            // 
            this.textBoxKeeFoxPriority.Location = new System.Drawing.Point(160, 74);
            this.textBoxKeeFoxPriority.Name = "textBoxKeeFoxPriority";
            this.textBoxKeeFoxPriority.Size = new System.Drawing.Size(39, 20);
            this.textBoxKeeFoxPriority.TabIndex = 2;
            // 
            // checkBoxHideFromKeeFox
            // 
            this.checkBoxHideFromKeeFox.AutoSize = true;
            this.checkBoxHideFromKeeFox.Location = new System.Drawing.Point(24, 19);
            this.checkBoxHideFromKeeFox.Name = "checkBoxHideFromKeeFox";
            this.checkBoxHideFromKeeFox.Size = new System.Drawing.Size(155, 17);
            this.checkBoxHideFromKeeFox.TabIndex = 1;
            this.checkBoxHideFromKeeFox.Text = "Hide this entry from KeeFox";
            this.checkBoxHideFromKeeFox.UseVisualStyleBackColor = true;
            // 
            // groupBox2
            // 
            this.groupBox2.Controls.Add(this.buttonFieldEdit);
            this.groupBox2.Controls.Add(this.buttonFieldDelete);
            this.groupBox2.Controls.Add(this.listView2);
            this.groupBox2.Controls.Add(this.buttonFieldAdd);
            this.groupBox2.Location = new System.Drawing.Point(6, 17);
            this.groupBox2.Name = "groupBox2";
            this.groupBox2.Size = new System.Drawing.Size(443, 265);
            this.groupBox2.TabIndex = 0;
            this.groupBox2.TabStop = false;
            this.groupBox2.Text = "Form fields";
            // 
            // buttonFieldEdit
            // 
            this.buttonFieldEdit.Enabled = false;
            this.buttonFieldEdit.Location = new System.Drawing.Point(312, 233);
            this.buttonFieldEdit.Name = "buttonFieldEdit";
            this.buttonFieldEdit.Size = new System.Drawing.Size(60, 26);
            this.buttonFieldEdit.TabIndex = 11;
            this.buttonFieldEdit.Text = "Edit";
            this.buttonFieldEdit.UseVisualStyleBackColor = true;
            this.buttonFieldEdit.Click += new System.EventHandler(this.buttonFieldEdit_Click);
            // 
            // buttonFieldDelete
            // 
            this.buttonFieldDelete.Enabled = false;
            this.buttonFieldDelete.Location = new System.Drawing.Point(378, 233);
            this.buttonFieldDelete.Name = "buttonFieldDelete";
            this.buttonFieldDelete.Size = new System.Drawing.Size(60, 26);
            this.buttonFieldDelete.TabIndex = 12;
            this.buttonFieldDelete.Text = "Delete";
            this.buttonFieldDelete.UseVisualStyleBackColor = true;
            this.buttonFieldDelete.Click += new System.EventHandler(this.buttonFieldDelete_Click);
            // 
            // listView2
            // 
            this.listView2.Columns.AddRange(new System.Windows.Forms.ColumnHeader[] {
            this.columnHeaderFName,
            this.columnHeaderFValue,
            this.columnHeaderFId,
            this.columnHeaderFType,
            this.columnHeaderFPage});
            this.listView2.FullRowSelect = true;
            this.listView2.HeaderStyle = System.Windows.Forms.ColumnHeaderStyle.Nonclickable;
            this.listView2.HideSelection = false;
            this.listView2.Location = new System.Drawing.Point(10, 19);
            this.listView2.MultiSelect = false;
            this.listView2.Name = "listView2";
            this.listView2.ShowItemToolTips = true;
            this.listView2.Size = new System.Drawing.Size(428, 209);
            this.listView2.TabIndex = 9;
            this.listView2.UseCompatibleStateImageBehavior = false;
            this.listView2.View = System.Windows.Forms.View.Details;
            this.listView2.SelectedIndexChanged += new System.EventHandler(this.listView2_SelectedIndexChanged);
            this.listView2.MouseDoubleClick += new System.Windows.Forms.MouseEventHandler(this.listView2_MouseDoubleClick);
            // 
            // columnHeaderFName
            // 
            this.columnHeaderFName.Text = "Name";
            this.columnHeaderFName.Width = 76;
            // 
            // columnHeaderFValue
            // 
            this.columnHeaderFValue.Text = "Value";
            this.columnHeaderFValue.Width = 115;
            // 
            // columnHeaderFId
            // 
            this.columnHeaderFId.Text = "Id";
            this.columnHeaderFId.Width = 85;
            // 
            // columnHeaderFType
            // 
            this.columnHeaderFType.Text = "Type";
            // 
            // columnHeaderFPage
            // 
            this.columnHeaderFPage.Text = "Page";
            // 
            // buttonFieldAdd
            // 
            this.buttonFieldAdd.Location = new System.Drawing.Point(246, 233);
            this.buttonFieldAdd.Name = "buttonFieldAdd";
            this.buttonFieldAdd.Size = new System.Drawing.Size(60, 26);
            this.buttonFieldAdd.TabIndex = 10;
            this.buttonFieldAdd.Text = "Add";
            this.buttonFieldAdd.UseVisualStyleBackColor = true;
            this.buttonFieldAdd.Click += new System.EventHandler(this.buttonFieldAdd_Click);
            // 
            // groupBox3
            // 
            this.groupBox3.Controls.Add(this.buttonURLEdit);
            this.groupBox3.Controls.Add(this.buttonURLDelete);
            this.groupBox3.Controls.Add(this.buttonURLAdd);
            this.groupBox3.Controls.Add(this.listView1);
            this.groupBox3.Location = new System.Drawing.Point(6, 173);
            this.groupBox3.Name = "groupBox3";
            this.groupBox3.Size = new System.Drawing.Size(443, 141);
            this.groupBox3.TabIndex = 0;
            this.groupBox3.TabStop = false;
            this.groupBox3.Text = "Additional URLs";
            // 
            // buttonURLEdit
            // 
            this.buttonURLEdit.Enabled = false;
            this.buttonURLEdit.Location = new System.Drawing.Point(377, 51);
            this.buttonURLEdit.Name = "buttonURLEdit";
            this.buttonURLEdit.Size = new System.Drawing.Size(60, 26);
            this.buttonURLEdit.TabIndex = 5;
            this.buttonURLEdit.Text = "Edit";
            this.buttonURLEdit.UseVisualStyleBackColor = true;
            this.buttonURLEdit.Click += new System.EventHandler(this.buttonURLEdit_Click);
            // 
            // buttonURLDelete
            // 
            this.buttonURLDelete.Enabled = false;
            this.buttonURLDelete.Location = new System.Drawing.Point(377, 83);
            this.buttonURLDelete.Name = "buttonURLDelete";
            this.buttonURLDelete.Size = new System.Drawing.Size(60, 26);
            this.buttonURLDelete.TabIndex = 6;
            this.buttonURLDelete.Text = "Delete";
            this.buttonURLDelete.UseVisualStyleBackColor = true;
            this.buttonURLDelete.Click += new System.EventHandler(this.buttonURLDelete_Click);
            // 
            // buttonURLAdd
            // 
            this.buttonURLAdd.Location = new System.Drawing.Point(377, 19);
            this.buttonURLAdd.Name = "buttonURLAdd";
            this.buttonURLAdd.Size = new System.Drawing.Size(60, 26);
            this.buttonURLAdd.TabIndex = 4;
            this.buttonURLAdd.Text = "Add";
            this.buttonURLAdd.UseVisualStyleBackColor = true;
            this.buttonURLAdd.Click += new System.EventHandler(this.buttonURLAdd_Click);
            // 
            // listView1
            // 
            this.listView1.Columns.AddRange(new System.Windows.Forms.ColumnHeader[] {
            this.columnHeaderValue,
            this.columnHeaderMethod,
            this.columnHeaderType});
            this.listView1.FullRowSelect = true;
            this.listView1.HeaderStyle = System.Windows.Forms.ColumnHeaderStyle.Nonclickable;
            this.listView1.HideSelection = false;
            this.listView1.Location = new System.Drawing.Point(5, 19);
            this.listView1.MultiSelect = false;
            this.listView1.Name = "listView1";
            this.listView1.ShowItemToolTips = true;
            this.listView1.Size = new System.Drawing.Size(366, 119);
            this.listView1.TabIndex = 3;
            this.listView1.UseCompatibleStateImageBehavior = false;
            this.listView1.View = System.Windows.Forms.View.Details;
            this.listView1.SelectedIndexChanged += new System.EventHandler(this.listView1_SelectedIndexChanged);
            this.listView1.MouseDoubleClick += new System.Windows.Forms.MouseEventHandler(this.listView1_MouseDoubleClick);
            // 
            // columnHeaderValue
            // 
            this.columnHeaderValue.Text = "URL / pattern";
            this.columnHeaderValue.Width = 226;
            // 
            // columnHeaderMethod
            // 
            this.columnHeaderMethod.Text = "Method";
            // 
            // columnHeaderType
            // 
            this.columnHeaderType.Text = "Type";
            // 
            // tabControl1
            // 
            this.tabControl1.Controls.Add(this.tabPage1);
            this.tabControl1.Controls.Add(this.tabPage2);
            this.tabControl1.Controls.Add(this.tabPage3);
            this.tabControl1.ItemSize = new System.Drawing.Size(152, 18);
            this.tabControl1.Location = new System.Drawing.Point(5, 3);
            this.tabControl1.Name = "tabControl1";
            this.tabControl1.SelectedIndex = 0;
            this.tabControl1.Size = new System.Drawing.Size(463, 343);
            this.tabControl1.SizeMode = System.Windows.Forms.TabSizeMode.Fixed;
            this.tabControl1.TabIndex = 3;
            // 
            // tabPage1
            // 
            this.tabPage1.Controls.Add(this.textBoxKeeFoxRealm);
            this.tabPage1.Controls.Add(this.labelRealm);
            this.tabPage1.Controls.Add(this.groupBox1);
            this.tabPage1.Controls.Add(this.checkBoxHideFromKeeFox);
            this.tabPage1.Controls.Add(this.label1);
            this.tabPage1.Controls.Add(this.textBoxKeeFoxPriority);
            this.tabPage1.Location = new System.Drawing.Point(4, 22);
            this.tabPage1.Name = "tabPage1";
            this.tabPage1.Padding = new System.Windows.Forms.Padding(3);
            this.tabPage1.Size = new System.Drawing.Size(455, 317);
            this.tabPage1.TabIndex = 0;
            this.tabPage1.Text = "General";
            this.tabPage1.ToolTipText = "Basic KeeFox settings for this entry";
            this.tabPage1.UseVisualStyleBackColor = true;
            // 
            // textBoxKeeFoxRealm
            // 
            this.textBoxKeeFoxRealm.Location = new System.Drawing.Point(115, 128);
            this.textBoxKeeFoxRealm.Name = "textBoxKeeFoxRealm";
            this.textBoxKeeFoxRealm.Size = new System.Drawing.Size(307, 20);
            this.textBoxKeeFoxRealm.TabIndex = 4;
            this.textBoxKeeFoxRealm.TextChanged += new System.EventHandler(this.textBoxKeeFoxRealm_TextChanged);
            // 
            // labelRealm
            // 
            this.labelRealm.AutoSize = true;
            this.labelRealm.Location = new System.Drawing.Point(12, 131);
            this.labelRealm.Name = "labelRealm";
            this.labelRealm.Size = new System.Drawing.Size(97, 13);
            this.labelRealm.TabIndex = 3;
            this.labelRealm.Text = "HTTP Auth Realm:";
            // 
            // tabPage2
            // 
            this.tabPage2.Controls.Add(this.groupBox4);
            this.tabPage2.Controls.Add(this.groupBox3);
            this.tabPage2.Location = new System.Drawing.Point(4, 22);
            this.tabPage2.Name = "tabPage2";
            this.tabPage2.Padding = new System.Windows.Forms.Padding(3);
            this.tabPage2.Size = new System.Drawing.Size(455, 317);
            this.tabPage2.TabIndex = 1;
            this.tabPage2.Text = "URLs";
            this.tabPage2.ToolTipText = "Which URLs should this entry match or be blocked from matching?";
            this.tabPage2.UseVisualStyleBackColor = true;
            // 
            // label2
            // 
            this.label2.Location = new System.Drawing.Point(25, 126);
            this.label2.Name = "label2";
            this.label2.Size = new System.Drawing.Size(410, 31);
            this.label2.TabIndex = 2;
            this.label2.Text = "You may want to choose Exact when you want to limit form filling to only certain " +
    "website pages, perhaps by using additional URLs below.";
            // 
            // tabPage3
            // 
            this.tabPage3.Controls.Add(this.groupBox2);
            this.tabPage3.Location = new System.Drawing.Point(4, 22);
            this.tabPage3.Name = "tabPage3";
            this.tabPage3.Padding = new System.Windows.Forms.Padding(3);
            this.tabPage3.Size = new System.Drawing.Size(459, 321);
            this.tabPage3.TabIndex = 2;
            this.tabPage3.Text = "Form fields";
            this.tabPage3.ToolTipText = "Define the web page form fields this entry applies to";
            this.tabPage3.UseVisualStyleBackColor = true;
            // 
            // label5
            // 
            this.label5.Location = new System.Drawing.Point(6, 17);
            this.label5.Name = "label5";
            this.label5.Size = new System.Drawing.Size(433, 31);
            this.label5.TabIndex = 3;
            this.label5.Text = "When more than one entry matches, KeeFox prioritises more accurately matching ent" +
    "ries. You can exclude this entry from matching at all in some cases.";
            // 
            // groupBox4
            // 
            this.groupBox4.Controls.Add(this.radioButton3);
            this.groupBox4.Controls.Add(this.radioButton2);
            this.groupBox4.Controls.Add(this.radioButton1);
            this.groupBox4.Controls.Add(this.label2);
            this.groupBox4.Controls.Add(this.label5);
            this.groupBox4.Location = new System.Drawing.Point(4, 7);
            this.groupBox4.Name = "groupBox4";
            this.groupBox4.Size = new System.Drawing.Size(445, 160);
            this.groupBox4.TabIndex = 4;
            this.groupBox4.TabStop = false;
            this.groupBox4.Text = "Minimum URL match accuracy";
            // 
            // radioButton1
            // 
            this.radioButton1.AutoSize = true;
            this.radioButton1.Location = new System.Drawing.Point(8, 51);
            this.radioButton1.Name = "radioButton1";
            this.radioButton1.Size = new System.Drawing.Size(355, 17);
            this.radioButton1.TabIndex = 4;
            this.radioButton1.TabStop = true;
            this.radioButton1.Text = "Domain: The URL only needs to be part of the same domain to match.";
            this.toolTipRealm.SetToolTip(this.radioButton1, "This is the default behaviour.");
            this.radioButton1.UseVisualStyleBackColor = true;
            this.radioButton1.CheckedChanged += new System.EventHandler(this.radioButton1_CheckedChanged);
            // 
            // radioButton2
            // 
            this.radioButton2.AutoSize = true;
            this.radioButton2.Location = new System.Drawing.Point(8, 74);
            this.radioButton2.Name = "radioButton2";
            this.radioButton2.Size = new System.Drawing.Size(415, 17);
            this.radioButton2.TabIndex = 5;
            this.radioButton2.TabStop = true;
            this.radioButton2.Text = "Hostname: The URL must match the hostname (domain and subdomains) and port.";
            this.toolTipRealm.SetToolTip(this.radioButton2, "This was the default behaviour in KeeFox 1.4 and lower.");
            this.radioButton2.UseVisualStyleBackColor = true;
            this.radioButton2.CheckedChanged += new System.EventHandler(this.radioButton2_CheckedChanged);
            // 
            // radioButton3
            // 
            this.radioButton3.AutoSize = true;
            this.radioButton3.Location = new System.Drawing.Point(8, 97);
            this.radioButton3.Name = "radioButton3";
            this.radioButton3.Size = new System.Drawing.Size(361, 17);
            this.radioButton3.TabIndex = 6;
            this.radioButton3.TabStop = true;
            this.radioButton3.Text = "Exact: The URL must match exactly including full path and query string.";
            this.radioButton3.UseVisualStyleBackColor = true;
            this.radioButton3.CheckedChanged += new System.EventHandler(this.radioButton3_CheckedChanged);
            // 
            // KeeFoxEntryUserControl
            // 
            this.AutoScaleDimensions = new System.Drawing.SizeF(6F, 13F);
            this.AutoScaleMode = System.Windows.Forms.AutoScaleMode.Font;
            this.BackColor = System.Drawing.SystemColors.Window;
            this.Controls.Add(this.tabControl1);
            this.Name = "KeeFoxEntryUserControl";
            this.Size = new System.Drawing.Size(470, 352);
            this.Load += new System.EventHandler(this.KeeFoxEntryUserControl_Load);
            this.groupBox1.ResumeLayout(false);
            this.groupBox1.PerformLayout();
            this.groupBox2.ResumeLayout(false);
            this.groupBox3.ResumeLayout(false);
            this.tabControl1.ResumeLayout(false);
            this.tabPage1.ResumeLayout(false);
            this.tabPage1.PerformLayout();
            this.tabPage2.ResumeLayout(false);
            this.tabPage3.ResumeLayout(false);
            this.groupBox4.ResumeLayout(false);
            this.groupBox4.PerformLayout();
            this.ResumeLayout(false);

        }

        #endregion

        private System.Windows.Forms.GroupBox groupBox1;
        private System.Windows.Forms.CheckBox checkBoxHideFromKeeFox;
        private System.Windows.Forms.Label label1;
        private System.Windows.Forms.TextBox textBoxKeeFoxPriority;
        private System.Windows.Forms.GroupBox groupBox2;
        private System.Windows.Forms.GroupBox groupBox3;
        private System.Windows.Forms.ComboBox comboBoxAutoSubmit;
        private System.Windows.Forms.ComboBox comboBoxAutoFill;
        private System.Windows.Forms.Label label4;
        private System.Windows.Forms.Label label3;
        private System.Windows.Forms.ListView listView1;
        private System.Windows.Forms.Button buttonURLDelete;
        private System.Windows.Forms.Button buttonURLEdit;
        private System.Windows.Forms.Button buttonURLAdd;
        private System.Windows.Forms.ColumnHeader columnHeaderValue;
        private System.Windows.Forms.ColumnHeader columnHeaderType;
        private System.Windows.Forms.ColumnHeader columnHeaderMethod;
        private System.Windows.Forms.Button buttonFieldEdit;
        private System.Windows.Forms.Button buttonFieldDelete;
        private System.Windows.Forms.ListView listView2;
        private System.Windows.Forms.Button buttonFieldAdd;
        private System.Windows.Forms.ColumnHeader columnHeaderFName;
        private System.Windows.Forms.ColumnHeader columnHeaderFValue;
        private System.Windows.Forms.ColumnHeader columnHeaderFId;
        private System.Windows.Forms.ColumnHeader columnHeaderFType;
        private System.Windows.Forms.ColumnHeader columnHeaderFPage;
        private System.Windows.Forms.TabControl tabControl1;
        private System.Windows.Forms.TabPage tabPage1;
        private System.Windows.Forms.TabPage tabPage2;
        private System.Windows.Forms.TabPage tabPage3;
        private System.Windows.Forms.TextBox textBoxKeeFoxRealm;
        private System.Windows.Forms.Label labelRealm;
        private System.Windows.Forms.ToolTip toolTipRealm;
        private System.Windows.Forms.Label label2;
        private System.Windows.Forms.GroupBox groupBox4;
        private System.Windows.Forms.Label label5;
        private System.Windows.Forms.RadioButton radioButton2;
        private System.Windows.Forms.RadioButton radioButton1;
        private System.Windows.Forms.RadioButton radioButton3;
    }
}

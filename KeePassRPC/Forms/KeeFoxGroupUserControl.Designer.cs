namespace KeePassRPC.Forms
{
    partial class KeeFoxGroupUserControl
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
            this.l_status = new System.Windows.Forms.Label();
            this.buttonMakeHome = new System.Windows.Forms.Button();
            this.l_homeExplanation = new System.Windows.Forms.Label();
            this.label1 = new System.Windows.Forms.Label();
            this.comboBoxLocation = new System.Windows.Forms.ComboBox();
            this.SuspendLayout();
            // 
            // l_status
            // 
            this.l_status.AutoSize = true;
            this.l_status.Location = new System.Drawing.Point(12, 44);
            this.l_status.Name = "l_status";
            this.l_status.Size = new System.Drawing.Size(76, 13);
            this.l_status.TabIndex = 0;
            this.l_status.Text = "Visibility Status";
            // 
            // buttonMakeHome
            // 
            this.buttonMakeHome.AutoSize = true;
            this.buttonMakeHome.Location = new System.Drawing.Point(15, 78);
            this.buttonMakeHome.Name = "buttonMakeHome";
            this.buttonMakeHome.Size = new System.Drawing.Size(151, 23);
            this.buttonMakeHome.TabIndex = 1;
            this.buttonMakeHome.Text = "Set as KeeFox Home group";
            this.buttonMakeHome.UseVisualStyleBackColor = true;
            this.buttonMakeHome.Click += new System.EventHandler(this.buttonMakeHome_Click);
            // 
            // l_homeExplanation
            // 
            this.l_homeExplanation.AutoSize = true;
            this.l_homeExplanation.Location = new System.Drawing.Point(12, 107);
            this.l_homeExplanation.Name = "l_homeExplanation";
            this.l_homeExplanation.Size = new System.Drawing.Size(120, 13);
            this.l_homeExplanation.TabIndex = 2;
            this.l_homeExplanation.Text = "home group explanation";
            // 
            // label1
            // 
            this.label1.AutoSize = true;
            this.label1.Location = new System.Drawing.Point(16, 8);
            this.label1.Name = "label1";
            this.label1.Size = new System.Drawing.Size(164, 13);
            this.label1.TabIndex = 3;
            this.label1.Text = "When KeePass is in this location:";
            // 
            // comboBoxLocation
            // 
            this.comboBoxLocation.DropDownStyle = System.Windows.Forms.ComboBoxStyle.DropDownList;
            this.comboBoxLocation.FormattingEnabled = true;
            this.comboBoxLocation.Location = new System.Drawing.Point(186, 5);
            this.comboBoxLocation.Name = "comboBoxLocation";
            this.comboBoxLocation.Size = new System.Drawing.Size(143, 21);
            this.comboBoxLocation.TabIndex = 4;
            this.comboBoxLocation.SelectedIndexChanged += new System.EventHandler(this.comboBoxLocation_SelectedIndexChanged);
            // 
            // KeeFoxGroupUserControl
            // 
            this.AutoScaleDimensions = new System.Drawing.SizeF(6F, 13F);
            this.AutoScaleMode = System.Windows.Forms.AutoScaleMode.Font;
            this.BackColor = System.Drawing.SystemColors.Window;
            this.Controls.Add(this.comboBoxLocation);
            this.Controls.Add(this.label1);
            this.Controls.Add(this.l_homeExplanation);
            this.Controls.Add(this.buttonMakeHome);
            this.Controls.Add(this.l_status);
            this.Name = "KeeFoxGroupUserControl";
            this.Size = new System.Drawing.Size(332, 150);
            this.Load += new System.EventHandler(this.KeeFoxGroupUserControl_Load);
            this.ResumeLayout(false);
            this.PerformLayout();

        }

        #endregion

        private System.Windows.Forms.Label l_status;
        private System.Windows.Forms.Button buttonMakeHome;
        private System.Windows.Forms.Label l_homeExplanation;
        private System.Windows.Forms.Label label1;
        private System.Windows.Forms.ComboBox comboBoxLocation;
    }
}

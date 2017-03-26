namespace KeePassRPC.Forms
{
	partial class KeyCreationSimpleForm
	{
		/// <summary>
		/// Erforderliche Designervariable.
		/// </summary>
		private System.ComponentModel.IContainer components = null;

		/// <summary>
		/// Verwendete Ressourcen bereinigen.
		/// </summary>
		/// <param name="disposing">True, wenn verwaltete Ressourcen gelöscht werden sollen; andernfalls False.</param>
		protected override void Dispose(bool disposing)
		{
			if(disposing && (components != null))
			{
				components.Dispose();
			}
			base.Dispose(disposing);
		}

		#region Vom Windows Form-Designer generierter Code

		/// <summary>
		/// Erforderliche Methode für die Designerunterstützung.
		/// Der Inhalt der Methode darf nicht mit dem Code-Editor geändert werden.
		/// </summary>
		private void InitializeComponent()
		{
            System.ComponentModel.ComponentResourceManager resources = new System.ComponentModel.ComponentResourceManager(typeof(KeyCreationSimpleForm));
            this.m_lblIntro = new System.Windows.Forms.Label();
            this.labelWarning = new System.Windows.Forms.Label();
            this.m_tbPassword = new System.Windows.Forms.TextBox();
            this.m_lblRepeatPassword = new System.Windows.Forms.Label();
            this.m_tbRepeatPassword = new System.Windows.Forms.TextBox();
            this.labelAdvancedTip = new System.Windows.Forms.Label();
            this.m_btnCancel = new System.Windows.Forms.Button();
            this.m_btnCreate = new System.Windows.Forms.Button();
            this.m_cbHidePassword = new System.Windows.Forms.CheckBox();
            this.m_lblSeparator = new System.Windows.Forms.Label();
            this.m_pbPasswordQuality = new KeePass.UI.QualityProgressBar();
            this.m_lblEstimatedQuality = new System.Windows.Forms.Label();
            this.m_lblQualityBits = new System.Windows.Forms.Label();
            this.m_bannerImage = new System.Windows.Forms.PictureBox();
            this.dbNameTextBox = new System.Windows.Forms.TextBox();
            this.labelMasterPassword = new System.Windows.Forms.Label();
            this.labelDBName = new System.Windows.Forms.Label();
            this.advancedKeyButton = new System.Windows.Forms.Button();
            this.labelHeading = new System.Windows.Forms.Label();
            ((System.ComponentModel.ISupportInitialize)(this.m_bannerImage)).BeginInit();
            this.SuspendLayout();
            // 
            // m_lblIntro
            // 
            this.m_lblIntro.Font = new System.Drawing.Font("Tahoma", 8.25F, System.Drawing.FontStyle.Regular, System.Drawing.GraphicsUnit.Point, ((byte)(0)));
            this.m_lblIntro.ForeColor = System.Drawing.Color.FromArgb(((int)(((byte)(40)))), ((int)(((byte)(72)))), ((int)(((byte)(152)))));
            this.m_lblIntro.Location = new System.Drawing.Point(9, 72);
            this.m_lblIntro.Name = "m_lblIntro";
            this.m_lblIntro.Size = new System.Drawing.Size(498, 13);
            this.m_lblIntro.TabIndex = 19;
            this.m_lblIntro.Text = "Please specify a master password, which will be used to protect (encrypt) your da" +
                "tabase.";
            // 
            // labelWarning
            // 
            this.labelWarning.Font = new System.Drawing.Font("Tahoma", 9F, System.Drawing.FontStyle.Bold, System.Drawing.GraphicsUnit.Point, ((byte)(0)));
            this.labelWarning.ForeColor = System.Drawing.Color.FromArgb(((int)(((byte)(40)))), ((int)(((byte)(72)))), ((int)(((byte)(152)))));
            this.labelWarning.Location = new System.Drawing.Point(9, 93);
            this.labelWarning.Name = "labelWarning";
            this.labelWarning.Size = new System.Drawing.Size(498, 42);
            this.labelWarning.TabIndex = 20;
            this.labelWarning.Text = "If you forget your password, you will not be able to open the database. There is " +
                "NO way to recover your password.";
            // 
            // m_tbPassword
            // 
            this.m_tbPassword.Font = new System.Drawing.Font("Tahoma", 8.25F, System.Drawing.FontStyle.Regular, System.Drawing.GraphicsUnit.Point, ((byte)(0)));
            this.m_tbPassword.ForeColor = System.Drawing.Color.FromArgb(((int)(((byte)(40)))), ((int)(((byte)(72)))), ((int)(((byte)(152)))));
            this.m_tbPassword.Location = new System.Drawing.Point(150, 145);
            this.m_tbPassword.Name = "m_tbPassword";
            this.m_tbPassword.Size = new System.Drawing.Size(308, 21);
            this.m_tbPassword.TabIndex = 1;
            this.m_tbPassword.UseSystemPasswordChar = true;
            // 
            // m_lblRepeatPassword
            // 
            this.m_lblRepeatPassword.AutoSize = true;
            this.m_lblRepeatPassword.Font = new System.Drawing.Font("Tahoma", 8.25F, System.Drawing.FontStyle.Regular, System.Drawing.GraphicsUnit.Point, ((byte)(0)));
            this.m_lblRepeatPassword.ForeColor = System.Drawing.Color.FromArgb(((int)(((byte)(40)))), ((int)(((byte)(72)))), ((int)(((byte)(152)))));
            this.m_lblRepeatPassword.Location = new System.Drawing.Point(49, 174);
            this.m_lblRepeatPassword.Name = "m_lblRepeatPassword";
            this.m_lblRepeatPassword.Size = new System.Drawing.Size(95, 13);
            this.m_lblRepeatPassword.TabIndex = 2;
            this.m_lblRepeatPassword.Text = "Repeat password:";
            // 
            // m_tbRepeatPassword
            // 
            this.m_tbRepeatPassword.Font = new System.Drawing.Font("Tahoma", 8.25F, System.Drawing.FontStyle.Regular, System.Drawing.GraphicsUnit.Point, ((byte)(0)));
            this.m_tbRepeatPassword.ForeColor = System.Drawing.Color.FromArgb(((int)(((byte)(40)))), ((int)(((byte)(72)))), ((int)(((byte)(152)))));
            this.m_tbRepeatPassword.Location = new System.Drawing.Point(150, 171);
            this.m_tbRepeatPassword.Name = "m_tbRepeatPassword";
            this.m_tbRepeatPassword.Size = new System.Drawing.Size(308, 21);
            this.m_tbRepeatPassword.TabIndex = 2;
            this.m_tbRepeatPassword.UseSystemPasswordChar = true;
            // 
            // labelAdvancedTip
            // 
            this.labelAdvancedTip.Font = new System.Drawing.Font("Tahoma", 8.25F, System.Drawing.FontStyle.Regular, System.Drawing.GraphicsUnit.Point, ((byte)(0)));
            this.labelAdvancedTip.ForeColor = System.Drawing.Color.FromArgb(((int)(((byte)(40)))), ((int)(((byte)(72)))), ((int)(((byte)(152)))));
            this.labelAdvancedTip.Location = new System.Drawing.Point(28, 263);
            this.labelAdvancedTip.Name = "labelAdvancedTip";
            this.labelAdvancedTip.Size = new System.Drawing.Size(479, 48);
            this.labelAdvancedTip.TabIndex = 11;
            this.labelAdvancedTip.Text = resources.GetString("labelAdvancedTip.Text");
            // 
            // m_btnCancel
            // 
            this.m_btnCancel.DialogResult = System.Windows.Forms.DialogResult.Cancel;
            this.m_btnCancel.Font = new System.Drawing.Font("Tahoma", 8.25F, System.Drawing.FontStyle.Regular, System.Drawing.GraphicsUnit.Point, ((byte)(0)));
            this.m_btnCancel.ForeColor = System.Drawing.Color.FromArgb(((int)(((byte)(40)))), ((int)(((byte)(72)))), ((int)(((byte)(152)))));
            this.m_btnCancel.Location = new System.Drawing.Point(432, 325);
            this.m_btnCancel.Name = "m_btnCancel";
            this.m_btnCancel.Size = new System.Drawing.Size(75, 23);
            this.m_btnCancel.TabIndex = 5;
            this.m_btnCancel.Text = "&Cancel";
            this.m_btnCancel.TextImageRelation = System.Windows.Forms.TextImageRelation.ImageBeforeText;
            this.m_btnCancel.UseVisualStyleBackColor = true;
            this.m_btnCancel.Click += new System.EventHandler(this.OnBtnCancel);
            // 
            // m_btnCreate
            // 
            this.m_btnCreate.DialogResult = System.Windows.Forms.DialogResult.OK;
            this.m_btnCreate.Font = new System.Drawing.Font("Tahoma", 8.25F, System.Drawing.FontStyle.Regular, System.Drawing.GraphicsUnit.Point, ((byte)(0)));
            this.m_btnCreate.ForeColor = System.Drawing.Color.FromArgb(((int)(((byte)(40)))), ((int)(((byte)(72)))), ((int)(((byte)(152)))));
            this.m_btnCreate.Location = new System.Drawing.Point(351, 324);
            this.m_btnCreate.Name = "m_btnCreate";
            this.m_btnCreate.Size = new System.Drawing.Size(75, 23);
            this.m_btnCreate.TabIndex = 4;
            this.m_btnCreate.Text = "&OK";
            this.m_btnCreate.TextImageRelation = System.Windows.Forms.TextImageRelation.ImageBeforeText;
            this.m_btnCreate.UseVisualStyleBackColor = true;
            this.m_btnCreate.Click += new System.EventHandler(this.OnBtnOK);
            // 
            // m_cbHidePassword
            // 
            this.m_cbHidePassword.Appearance = System.Windows.Forms.Appearance.Button;
            this.m_cbHidePassword.Font = new System.Drawing.Font("Tahoma", 11.25F, System.Drawing.FontStyle.Bold, System.Drawing.GraphicsUnit.Point, ((byte)(0)));
            this.m_cbHidePassword.ForeColor = System.Drawing.Color.FromArgb(((int)(((byte)(40)))), ((int)(((byte)(72)))), ((int)(((byte)(152)))));
            this.m_cbHidePassword.Location = new System.Drawing.Point(464, 145);
            this.m_cbHidePassword.Name = "m_cbHidePassword";
            this.m_cbHidePassword.Size = new System.Drawing.Size(46, 21);
            this.m_cbHidePassword.TabIndex = 6;
            this.m_cbHidePassword.Text = "***";
            this.m_cbHidePassword.UseVisualStyleBackColor = true;
            this.m_cbHidePassword.CheckedChanged += new System.EventHandler(this.OnCheckedHidePassword);
            // 
            // m_lblSeparator
            // 
            this.m_lblSeparator.BorderStyle = System.Windows.Forms.BorderStyle.Fixed3D;
            this.m_lblSeparator.Location = new System.Drawing.Point(0, 311);
            this.m_lblSeparator.Name = "m_lblSeparator";
            this.m_lblSeparator.Size = new System.Drawing.Size(519, 2);
            this.m_lblSeparator.TabIndex = 15;
            // 
            // m_pbPasswordQuality
            // 
            this.m_pbPasswordQuality.Location = new System.Drawing.Point(150, 197);
            this.m_pbPasswordQuality.Maximum = 100;
            this.m_pbPasswordQuality.Minimum = 0;
            this.m_pbPasswordQuality.Name = "m_pbPasswordQuality";
            this.m_pbPasswordQuality.Size = new System.Drawing.Size(260, 14);
            this.m_pbPasswordQuality.Style = System.Windows.Forms.ProgressBarStyle.Continuous;
            this.m_pbPasswordQuality.TabIndex = 5;
            this.m_pbPasswordQuality.TabStop = false;
            this.m_pbPasswordQuality.Value = 0;
            // 
            // m_lblEstimatedQuality
            // 
            this.m_lblEstimatedQuality.AutoSize = true;
            this.m_lblEstimatedQuality.Font = new System.Drawing.Font("Tahoma", 8.25F, System.Drawing.FontStyle.Regular, System.Drawing.GraphicsUnit.Point, ((byte)(0)));
            this.m_lblEstimatedQuality.ForeColor = System.Drawing.Color.FromArgb(((int)(((byte)(40)))), ((int)(((byte)(72)))), ((int)(((byte)(152)))));
            this.m_lblEstimatedQuality.Location = new System.Drawing.Point(51, 198);
            this.m_lblEstimatedQuality.Name = "m_lblEstimatedQuality";
            this.m_lblEstimatedQuality.Size = new System.Drawing.Size(93, 13);
            this.m_lblEstimatedQuality.TabIndex = 4;
            this.m_lblEstimatedQuality.Text = "Estimated quality:";
            // 
            // m_lblQualityBits
            // 
            this.m_lblQualityBits.Font = new System.Drawing.Font("Tahoma", 8.25F, System.Drawing.FontStyle.Regular, System.Drawing.GraphicsUnit.Point, ((byte)(0)));
            this.m_lblQualityBits.ForeColor = System.Drawing.Color.FromArgb(((int)(((byte)(40)))), ((int)(((byte)(72)))), ((int)(((byte)(152)))));
            this.m_lblQualityBits.Location = new System.Drawing.Point(413, 197);
            this.m_lblQualityBits.Name = "m_lblQualityBits";
            this.m_lblQualityBits.Size = new System.Drawing.Size(53, 13);
            this.m_lblQualityBits.TabIndex = 6;
            this.m_lblQualityBits.Text = "9999 bits";
            this.m_lblQualityBits.TextAlign = System.Drawing.ContentAlignment.MiddleRight;
            // 
            // m_bannerImage
            // 
            this.m_bannerImage.Image = global::KeePassRPC.Properties.Resources.KeeFox64;
            this.m_bannerImage.Location = new System.Drawing.Point(0, -2);
            this.m_bannerImage.Name = "m_bannerImage";
            this.m_bannerImage.Padding = new System.Windows.Forms.Padding(10, 0, 0, 10);
            this.m_bannerImage.Size = new System.Drawing.Size(75, 60);
            this.m_bannerImage.TabIndex = 15;
            this.m_bannerImage.TabStop = false;
            // 
            // dbNameTextBox
            // 
            this.dbNameTextBox.Font = new System.Drawing.Font("Tahoma", 8.25F, System.Drawing.FontStyle.Regular, System.Drawing.GraphicsUnit.Point, ((byte)(0)));
            this.dbNameTextBox.ForeColor = System.Drawing.Color.FromArgb(((int)(((byte)(40)))), ((int)(((byte)(72)))), ((int)(((byte)(152)))));
            this.dbNameTextBox.Location = new System.Drawing.Point(150, 225);
            this.dbNameTextBox.Name = "dbNameTextBox";
            this.dbNameTextBox.Size = new System.Drawing.Size(260, 21);
            this.dbNameTextBox.TabIndex = 3;
            // 
            // labelMasterPassword
            // 
            this.labelMasterPassword.AutoSize = true;
            this.labelMasterPassword.Font = new System.Drawing.Font("Tahoma", 8.25F, System.Drawing.FontStyle.Regular, System.Drawing.GraphicsUnit.Point, ((byte)(0)));
            this.labelMasterPassword.ForeColor = System.Drawing.Color.FromArgb(((int)(((byte)(40)))), ((int)(((byte)(72)))), ((int)(((byte)(152)))));
            this.labelMasterPassword.Location = new System.Drawing.Point(51, 150);
            this.labelMasterPassword.Name = "labelMasterPassword";
            this.labelMasterPassword.Size = new System.Drawing.Size(93, 13);
            this.labelMasterPassword.TabIndex = 23;
            this.labelMasterPassword.Text = "Master password:";
            // 
            // labelDBName
            // 
            this.labelDBName.AutoSize = true;
            this.labelDBName.Font = new System.Drawing.Font("Tahoma", 8.25F, System.Drawing.FontStyle.Regular, System.Drawing.GraphicsUnit.Point, ((byte)(0)));
            this.labelDBName.ForeColor = System.Drawing.Color.FromArgb(((int)(((byte)(40)))), ((int)(((byte)(72)))), ((int)(((byte)(152)))));
            this.labelDBName.Location = new System.Drawing.Point(58, 228);
            this.labelDBName.Name = "labelDBName";
            this.labelDBName.Size = new System.Drawing.Size(86, 13);
            this.labelDBName.TabIndex = 24;
            this.labelDBName.Text = "Database name:";
            // 
            // advancedKeyButton
            // 
            this.advancedKeyButton.DialogResult = System.Windows.Forms.DialogResult.No;
            this.advancedKeyButton.Font = new System.Drawing.Font("Tahoma", 8.25F, System.Drawing.FontStyle.Regular, System.Drawing.GraphicsUnit.Point, ((byte)(0)));
            this.advancedKeyButton.ForeColor = System.Drawing.Color.FromArgb(((int)(((byte)(40)))), ((int)(((byte)(72)))), ((int)(((byte)(152)))));
            this.advancedKeyButton.Location = new System.Drawing.Point(12, 325);
            this.advancedKeyButton.Name = "advancedKeyButton";
            this.advancedKeyButton.Size = new System.Drawing.Size(214, 23);
            this.advancedKeyButton.TabIndex = 7;
            this.advancedKeyButton.Text = "Switch to advanced key creation mode";
            this.advancedKeyButton.UseVisualStyleBackColor = true;
            this.advancedKeyButton.Click += new System.EventHandler(this.button1_Click);
            // 
            // labelHeading
            // 
            this.labelHeading.AutoSize = true;
            this.labelHeading.Font = new System.Drawing.Font("Tahoma", 18F, System.Drawing.FontStyle.Regular, System.Drawing.GraphicsUnit.Point, ((byte)(0)));
            this.labelHeading.ForeColor = System.Drawing.Color.FromArgb(((int)(((byte)(40)))), ((int)(((byte)(72)))), ((int)(((byte)(152)))));
            this.labelHeading.Location = new System.Drawing.Point(80, 13);
            this.labelHeading.Name = "labelHeading";
            this.labelHeading.Size = new System.Drawing.Size(231, 29);
            this.labelHeading.TabIndex = 26;
            this.labelHeading.Text = "Create a master key";
            // 
            // KeyCreationSimpleForm
            // 
            this.AcceptButton = this.m_btnCreate;
            this.AutoScaleDimensions = new System.Drawing.SizeF(6F, 13F);
            this.AutoScaleMode = System.Windows.Forms.AutoScaleMode.Font;
            this.BackColor = System.Drawing.Color.White;
            this.CancelButton = this.m_btnCancel;
            this.ClientSize = new System.Drawing.Size(519, 354);
            this.Controls.Add(this.labelHeading);
            this.Controls.Add(this.advancedKeyButton);
            this.Controls.Add(this.labelDBName);
            this.Controls.Add(this.labelMasterPassword);
            this.Controls.Add(this.dbNameTextBox);
            this.Controls.Add(this.m_lblQualityBits);
            this.Controls.Add(this.m_lblEstimatedQuality);
            this.Controls.Add(this.m_pbPasswordQuality);
            this.Controls.Add(this.m_lblSeparator);
            this.Controls.Add(this.labelAdvancedTip);
            this.Controls.Add(this.m_cbHidePassword);
            this.Controls.Add(this.m_bannerImage);
            this.Controls.Add(this.m_btnCancel);
            this.Controls.Add(this.m_btnCreate);
            this.Controls.Add(this.m_tbRepeatPassword);
            this.Controls.Add(this.m_lblRepeatPassword);
            this.Controls.Add(this.m_tbPassword);
            this.Controls.Add(this.labelWarning);
            this.Controls.Add(this.m_lblIntro);
            this.FormBorderStyle = System.Windows.Forms.FormBorderStyle.FixedDialog;
            this.MaximizeBox = false;
            this.MinimizeBox = false;
            this.Name = "KeyCreationSimpleForm";
            this.SizeGripStyle = System.Windows.Forms.SizeGripStyle.Hide;
            this.StartPosition = System.Windows.Forms.FormStartPosition.CenterParent;
            this.Text = "<>";
            this.Load += new System.EventHandler(this.OnFormLoad);
            this.FormClosed += new System.Windows.Forms.FormClosedEventHandler(this.OnFormClosed);
            this.FormClosing += new System.Windows.Forms.FormClosingEventHandler(this.OnFormClosing);
            ((System.ComponentModel.ISupportInitialize)(this.m_bannerImage)).EndInit();
            this.ResumeLayout(false);
            this.PerformLayout();

		}

		#endregion

		private System.Windows.Forms.Label m_lblIntro;
        private System.Windows.Forms.Label labelWarning;
		private System.Windows.Forms.TextBox m_tbPassword;
		private System.Windows.Forms.Label m_lblRepeatPassword;
        private System.Windows.Forms.TextBox m_tbRepeatPassword;
		private System.Windows.Forms.Button m_btnCreate;
		private System.Windows.Forms.Button m_btnCancel;
		private System.Windows.Forms.PictureBox m_bannerImage;
        private System.Windows.Forms.CheckBox m_cbHidePassword;
        private System.Windows.Forms.Label labelAdvancedTip;
		private System.Windows.Forms.Label m_lblSeparator;
		private KeePass.UI.QualityProgressBar m_pbPasswordQuality;
		private System.Windows.Forms.Label m_lblEstimatedQuality;
        private System.Windows.Forms.Label m_lblQualityBits;
        private System.Windows.Forms.TextBox dbNameTextBox;
        private System.Windows.Forms.Label labelMasterPassword;
        private System.Windows.Forms.Label labelDBName;
        private System.Windows.Forms.Button advancedKeyButton;
        private System.Windows.Forms.Label labelHeading;
	}
}
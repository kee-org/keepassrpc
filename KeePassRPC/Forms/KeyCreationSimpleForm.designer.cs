using System;
using System.ComponentModel;
using System.Drawing;
using System.Windows.Forms;
using KeePass.UI;
using KeePassRPC.Properties;

namespace KeePassRPC.Forms
{
	partial class KeyCreationSimpleForm
	{
		/// <summary>
		/// Erforderliche Designervariable.
		/// </summary>
		private IContainer components = null;

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
            ComponentResourceManager resources = new ComponentResourceManager(typeof(KeyCreationSimpleForm));
            this.m_lblIntro = new Label();
            this.labelWarning = new Label();
            this.m_tbPassword = new TextBox();
            this.m_lblRepeatPassword = new Label();
            this.m_tbRepeatPassword = new TextBox();
            this.labelAdvancedTip = new Label();
            this.m_btnCancel = new Button();
            this.m_btnCreate = new Button();
            this.m_cbHidePassword = new CheckBox();
            this.m_lblSeparator = new Label();
            this.m_pbPasswordQuality = new QualityProgressBar();
            this.m_lblEstimatedQuality = new Label();
            this.m_lblQualityBits = new Label();
            this.m_bannerImage = new PictureBox();
            this.dbNameTextBox = new TextBox();
            this.labelMasterPassword = new Label();
            this.labelDBName = new Label();
            this.advancedKeyButton = new Button();
            this.labelHeading = new Label();
            ((ISupportInitialize)(this.m_bannerImage)).BeginInit();
            this.SuspendLayout();
            // 
            // m_lblIntro
            // 
            this.m_lblIntro.Font = new Font("Tahoma", 8.25F, FontStyle.Regular, GraphicsUnit.Point, ((byte)(0)));
            this.m_lblIntro.ForeColor = Color.FromArgb(((int)(((byte)(40)))), ((int)(((byte)(72)))), ((int)(((byte)(152)))));
            this.m_lblIntro.Location = new Point(9, 72);
            this.m_lblIntro.Name = "m_lblIntro";
            this.m_lblIntro.Size = new Size(498, 13);
            this.m_lblIntro.TabIndex = 19;
            this.m_lblIntro.Text = "Please specify a master password, which will be used to protect (encrypt) your da" +
                "tabase.";
            // 
            // labelWarning
            // 
            this.labelWarning.Font = new Font("Tahoma", 9F, FontStyle.Bold, GraphicsUnit.Point, ((byte)(0)));
            this.labelWarning.ForeColor = Color.FromArgb(((int)(((byte)(40)))), ((int)(((byte)(72)))), ((int)(((byte)(152)))));
            this.labelWarning.Location = new Point(9, 93);
            this.labelWarning.Name = "labelWarning";
            this.labelWarning.Size = new Size(498, 42);
            this.labelWarning.TabIndex = 20;
            this.labelWarning.Text = "If you forget your password, you will not be able to open the database. There is " +
                "NO way to recover your password.";
            // 
            // m_tbPassword
            // 
            this.m_tbPassword.Font = new Font("Tahoma", 8.25F, FontStyle.Regular, GraphicsUnit.Point, ((byte)(0)));
            this.m_tbPassword.ForeColor = Color.FromArgb(((int)(((byte)(40)))), ((int)(((byte)(72)))), ((int)(((byte)(152)))));
            this.m_tbPassword.Location = new Point(150, 145);
            this.m_tbPassword.Name = "m_tbPassword";
            this.m_tbPassword.Size = new Size(308, 21);
            this.m_tbPassword.TabIndex = 1;
            this.m_tbPassword.UseSystemPasswordChar = true;
            // 
            // m_lblRepeatPassword
            // 
            this.m_lblRepeatPassword.AutoSize = true;
            this.m_lblRepeatPassword.Font = new Font("Tahoma", 8.25F, FontStyle.Regular, GraphicsUnit.Point, ((byte)(0)));
            this.m_lblRepeatPassword.ForeColor = Color.FromArgb(((int)(((byte)(40)))), ((int)(((byte)(72)))), ((int)(((byte)(152)))));
            this.m_lblRepeatPassword.Location = new Point(49, 174);
            this.m_lblRepeatPassword.Name = "m_lblRepeatPassword";
            this.m_lblRepeatPassword.Size = new Size(95, 13);
            this.m_lblRepeatPassword.TabIndex = 2;
            this.m_lblRepeatPassword.Text = "Repeat password:";
            // 
            // m_tbRepeatPassword
            // 
            this.m_tbRepeatPassword.Font = new Font("Tahoma", 8.25F, FontStyle.Regular, GraphicsUnit.Point, ((byte)(0)));
            this.m_tbRepeatPassword.ForeColor = Color.FromArgb(((int)(((byte)(40)))), ((int)(((byte)(72)))), ((int)(((byte)(152)))));
            this.m_tbRepeatPassword.Location = new Point(150, 171);
            this.m_tbRepeatPassword.Name = "m_tbRepeatPassword";
            this.m_tbRepeatPassword.Size = new Size(308, 21);
            this.m_tbRepeatPassword.TabIndex = 2;
            this.m_tbRepeatPassword.UseSystemPasswordChar = true;
            // 
            // labelAdvancedTip
            // 
            this.labelAdvancedTip.Font = new Font("Tahoma", 8.25F, FontStyle.Regular, GraphicsUnit.Point, ((byte)(0)));
            this.labelAdvancedTip.ForeColor = Color.FromArgb(((int)(((byte)(40)))), ((int)(((byte)(72)))), ((int)(((byte)(152)))));
            this.labelAdvancedTip.Location = new Point(28, 263);
            this.labelAdvancedTip.Name = "labelAdvancedTip";
            this.labelAdvancedTip.Size = new Size(479, 48);
            this.labelAdvancedTip.TabIndex = 11;
            this.labelAdvancedTip.Text = resources.GetString("labelAdvancedTip.Text");
            // 
            // m_btnCancel
            // 
            this.m_btnCancel.DialogResult = DialogResult.Cancel;
            this.m_btnCancel.Font = new Font("Tahoma", 8.25F, FontStyle.Regular, GraphicsUnit.Point, ((byte)(0)));
            this.m_btnCancel.ForeColor = Color.FromArgb(((int)(((byte)(40)))), ((int)(((byte)(72)))), ((int)(((byte)(152)))));
            this.m_btnCancel.Location = new Point(432, 325);
            this.m_btnCancel.Name = "m_btnCancel";
            this.m_btnCancel.Size = new Size(75, 23);
            this.m_btnCancel.TabIndex = 5;
            this.m_btnCancel.Text = "&Cancel";
            this.m_btnCancel.TextImageRelation = TextImageRelation.ImageBeforeText;
            this.m_btnCancel.UseVisualStyleBackColor = true;
            this.m_btnCancel.Click += new EventHandler(this.OnBtnCancel);
            // 
            // m_btnCreate
            // 
            this.m_btnCreate.DialogResult = DialogResult.OK;
            this.m_btnCreate.Font = new Font("Tahoma", 8.25F, FontStyle.Regular, GraphicsUnit.Point, ((byte)(0)));
            this.m_btnCreate.ForeColor = Color.FromArgb(((int)(((byte)(40)))), ((int)(((byte)(72)))), ((int)(((byte)(152)))));
            this.m_btnCreate.Location = new Point(351, 324);
            this.m_btnCreate.Name = "m_btnCreate";
            this.m_btnCreate.Size = new Size(75, 23);
            this.m_btnCreate.TabIndex = 4;
            this.m_btnCreate.Text = "&OK";
            this.m_btnCreate.TextImageRelation = TextImageRelation.ImageBeforeText;
            this.m_btnCreate.UseVisualStyleBackColor = true;
            this.m_btnCreate.Click += new EventHandler(this.OnBtnOK);
            // 
            // m_cbHidePassword
            // 
            this.m_cbHidePassword.Appearance = Appearance.Button;
            this.m_cbHidePassword.Font = new Font("Tahoma", 11.25F, FontStyle.Bold, GraphicsUnit.Point, ((byte)(0)));
            this.m_cbHidePassword.ForeColor = Color.FromArgb(((int)(((byte)(40)))), ((int)(((byte)(72)))), ((int)(((byte)(152)))));
            this.m_cbHidePassword.Location = new Point(464, 145);
            this.m_cbHidePassword.Name = "m_cbHidePassword";
            this.m_cbHidePassword.Size = new Size(46, 21);
            this.m_cbHidePassword.TabIndex = 6;
            this.m_cbHidePassword.Text = "***";
            this.m_cbHidePassword.UseVisualStyleBackColor = true;
            this.m_cbHidePassword.CheckedChanged += new EventHandler(this.OnCheckedHidePassword);
            // 
            // m_lblSeparator
            // 
            this.m_lblSeparator.BorderStyle = BorderStyle.Fixed3D;
            this.m_lblSeparator.Location = new Point(0, 311);
            this.m_lblSeparator.Name = "m_lblSeparator";
            this.m_lblSeparator.Size = new Size(519, 2);
            this.m_lblSeparator.TabIndex = 15;
            // 
            // m_pbPasswordQuality
            // 
            this.m_pbPasswordQuality.Location = new Point(150, 197);
            this.m_pbPasswordQuality.Maximum = 100;
            this.m_pbPasswordQuality.Minimum = 0;
            this.m_pbPasswordQuality.Name = "m_pbPasswordQuality";
            this.m_pbPasswordQuality.Size = new Size(260, 14);
            this.m_pbPasswordQuality.Style = ProgressBarStyle.Continuous;
            this.m_pbPasswordQuality.TabIndex = 5;
            this.m_pbPasswordQuality.TabStop = false;
            this.m_pbPasswordQuality.Value = 0;
            // 
            // m_lblEstimatedQuality
            // 
            this.m_lblEstimatedQuality.AutoSize = true;
            this.m_lblEstimatedQuality.Font = new Font("Tahoma", 8.25F, FontStyle.Regular, GraphicsUnit.Point, ((byte)(0)));
            this.m_lblEstimatedQuality.ForeColor = Color.FromArgb(((int)(((byte)(40)))), ((int)(((byte)(72)))), ((int)(((byte)(152)))));
            this.m_lblEstimatedQuality.Location = new Point(51, 198);
            this.m_lblEstimatedQuality.Name = "m_lblEstimatedQuality";
            this.m_lblEstimatedQuality.Size = new Size(93, 13);
            this.m_lblEstimatedQuality.TabIndex = 4;
            this.m_lblEstimatedQuality.Text = "Estimated quality:";
            // 
            // m_lblQualityBits
            // 
            this.m_lblQualityBits.Font = new Font("Tahoma", 8.25F, FontStyle.Regular, GraphicsUnit.Point, ((byte)(0)));
            this.m_lblQualityBits.ForeColor = Color.FromArgb(((int)(((byte)(40)))), ((int)(((byte)(72)))), ((int)(((byte)(152)))));
            this.m_lblQualityBits.Location = new Point(413, 197);
            this.m_lblQualityBits.Name = "m_lblQualityBits";
            this.m_lblQualityBits.Size = new Size(53, 13);
            this.m_lblQualityBits.TabIndex = 6;
            this.m_lblQualityBits.Text = "9999 bits";
            this.m_lblQualityBits.TextAlign = ContentAlignment.MiddleRight;
            // 
            // m_bannerImage
            // 
            this.m_bannerImage.Image = Resources.KPRPC64;
            this.m_bannerImage.Location = new Point(0, -2);
            this.m_bannerImage.Name = "m_bannerImage";
            this.m_bannerImage.Padding = new Padding(10, 0, 0, 10);
            this.m_bannerImage.Size = new Size(75, 60);
            this.m_bannerImage.TabIndex = 15;
            this.m_bannerImage.TabStop = false;
            // 
            // dbNameTextBox
            // 
            this.dbNameTextBox.Font = new Font("Tahoma", 8.25F, FontStyle.Regular, GraphicsUnit.Point, ((byte)(0)));
            this.dbNameTextBox.ForeColor = Color.FromArgb(((int)(((byte)(40)))), ((int)(((byte)(72)))), ((int)(((byte)(152)))));
            this.dbNameTextBox.Location = new Point(150, 225);
            this.dbNameTextBox.Name = "dbNameTextBox";
            this.dbNameTextBox.Size = new Size(260, 21);
            this.dbNameTextBox.TabIndex = 3;
            // 
            // labelMasterPassword
            // 
            this.labelMasterPassword.AutoSize = true;
            this.labelMasterPassword.Font = new Font("Tahoma", 8.25F, FontStyle.Regular, GraphicsUnit.Point, ((byte)(0)));
            this.labelMasterPassword.ForeColor = Color.FromArgb(((int)(((byte)(40)))), ((int)(((byte)(72)))), ((int)(((byte)(152)))));
            this.labelMasterPassword.Location = new Point(51, 150);
            this.labelMasterPassword.Name = "labelMasterPassword";
            this.labelMasterPassword.Size = new Size(93, 13);
            this.labelMasterPassword.TabIndex = 23;
            this.labelMasterPassword.Text = "Master password:";
            // 
            // labelDBName
            // 
            this.labelDBName.AutoSize = true;
            this.labelDBName.Font = new Font("Tahoma", 8.25F, FontStyle.Regular, GraphicsUnit.Point, ((byte)(0)));
            this.labelDBName.ForeColor = Color.FromArgb(((int)(((byte)(40)))), ((int)(((byte)(72)))), ((int)(((byte)(152)))));
            this.labelDBName.Location = new Point(58, 228);
            this.labelDBName.Name = "labelDBName";
            this.labelDBName.Size = new Size(86, 13);
            this.labelDBName.TabIndex = 24;
            this.labelDBName.Text = "Database name:";
            // 
            // advancedKeyButton
            // 
            this.advancedKeyButton.DialogResult = DialogResult.No;
            this.advancedKeyButton.Font = new Font("Tahoma", 8.25F, FontStyle.Regular, GraphicsUnit.Point, ((byte)(0)));
            this.advancedKeyButton.ForeColor = Color.FromArgb(((int)(((byte)(40)))), ((int)(((byte)(72)))), ((int)(((byte)(152)))));
            this.advancedKeyButton.Location = new Point(12, 325);
            this.advancedKeyButton.Name = "advancedKeyButton";
            this.advancedKeyButton.Size = new Size(214, 23);
            this.advancedKeyButton.TabIndex = 7;
            this.advancedKeyButton.Text = "Switch to advanced key creation mode";
            this.advancedKeyButton.UseVisualStyleBackColor = true;
            this.advancedKeyButton.Click += new EventHandler(this.button1_Click);
            // 
            // labelHeading
            // 
            this.labelHeading.AutoSize = true;
            this.labelHeading.Font = new Font("Tahoma", 18F, FontStyle.Regular, GraphicsUnit.Point, ((byte)(0)));
            this.labelHeading.ForeColor = Color.FromArgb(((int)(((byte)(40)))), ((int)(((byte)(72)))), ((int)(((byte)(152)))));
            this.labelHeading.Location = new Point(80, 13);
            this.labelHeading.Name = "labelHeading";
            this.labelHeading.Size = new Size(231, 29);
            this.labelHeading.TabIndex = 26;
            this.labelHeading.Text = "Create a master key";
            // 
            // KeyCreationSimpleForm
            // 
            this.AcceptButton = this.m_btnCreate;
            this.AutoScaleDimensions = new SizeF(6F, 13F);
            this.AutoScaleMode = AutoScaleMode.Font;
            this.BackColor = Color.White;
            this.CancelButton = this.m_btnCancel;
            this.ClientSize = new Size(519, 354);
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
            this.FormBorderStyle = FormBorderStyle.FixedDialog;
            this.MaximizeBox = false;
            this.MinimizeBox = false;
            this.Name = "KeyCreationSimpleForm";
            this.SizeGripStyle = SizeGripStyle.Hide;
            this.StartPosition = FormStartPosition.CenterParent;
            this.Text = "<>";
            this.Load += new EventHandler(this.OnFormLoad);
            this.FormClosed += new FormClosedEventHandler(this.OnFormClosed);
            this.FormClosing += new FormClosingEventHandler(this.OnFormClosing);
            ((ISupportInitialize)(this.m_bannerImage)).EndInit();
            this.ResumeLayout(false);
            this.PerformLayout();

		}

		#endregion

		private Label m_lblIntro;
        private Label labelWarning;
		private TextBox m_tbPassword;
		private Label m_lblRepeatPassword;
        private TextBox m_tbRepeatPassword;
		private Button m_btnCreate;
		private Button m_btnCancel;
		private PictureBox m_bannerImage;
        private CheckBox m_cbHidePassword;
        private Label labelAdvancedTip;
		private Label m_lblSeparator;
		private QualityProgressBar m_pbPasswordQuality;
		private Label m_lblEstimatedQuality;
        private Label m_lblQualityBits;
        private TextBox dbNameTextBox;
        private Label labelMasterPassword;
        private Label labelDBName;
        private Button advancedKeyButton;
        private Label labelHeading;
	}
}
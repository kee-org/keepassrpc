/*
  Modified version of KeyCreationForm.cs from...

  KeePass Password Safe - The Open-Source Password Manager
  KeePass is Copyright (C) 2003-2009 Dominik Reichl <dominik.reichl@t-online.de>
*/

using System;
using System.Diagnostics;
using System.Windows.Forms;
using KeePass;
using KeePass.App;
using KeePass.Forms;
using KeePass.Resources;
using KeePass.UI;
using KeePassLib.Cryptography;
using KeePassLib.Keys;
using KeePassLib.Serialization;
using KeePassLib.Utility;

namespace KeePassRPC.Forms
{
	public partial class KeyCreationSimpleForm : Form
	{
		private CompositeKey m_pKey;
		private bool m_bCreatingNew;
		private IOConnectionInfo m_ioInfo = new IOConnectionInfo();
        private string _databaseName;

		private SecureEdit m_secPassword = new SecureEdit();
		private SecureEdit m_secRepeat = new SecureEdit();

		public CompositeKey CompositeKey
		{
			get
			{
				Debug.Assert(m_pKey != null);
				return m_pKey;
			}
		}

        public string DatabaseName
        {
            get
            {
                return _databaseName;
            }
        }

		public KeyCreationSimpleForm()
		{
			InitializeComponent();
			//Program.Translation.ApplyTo(this);
		}

		public void InitEx(IOConnectionInfo ioInfo, bool bCreatingNew)
		{
			if(ioInfo != null) m_ioInfo = ioInfo;

			m_bCreatingNew = bCreatingNew;
		}

		private void OnFormLoad(object sender, EventArgs e)
		{
			GlobalWindowManager.AddWindow(this);

			//this.Icon = Properties.Resources.KeePass;
			Text = KPRes.CreateMasterKey;

			//m_ttRect.SetToolTip(m_cbHidePassword, KPRes.TogglePasswordAsterisks);

			if(!m_bCreatingNew)
				m_lblIntro.Text = KPRes.ChangeMasterKeyIntroShort;

			m_secPassword.Attach(m_tbPassword, ProcessTextChangedPassword, true);
			m_secRepeat.Attach(m_tbRepeatPassword, null, true);
			m_cbHidePassword.Checked = true;

			ProcessTextChangedPassword(sender, e); // Update quality estimation

			CustomizeForScreenReader();
			EnableUserControls();
		}

		private void CustomizeForScreenReader()
		{
			if(!Program.Config.UI.OptimizeForScreenReader) return;

			m_cbHidePassword.Text = KPRes.HideUsingAsterisks;
		}

		private void CleanUpEx()
		{
			m_secPassword.Detach();
			m_secRepeat.Detach();
		}

		private bool CreateCompositeKey()
		{
			m_pKey = new CompositeKey();

			if(m_secPassword.ContentsEqualTo(m_secRepeat) == false)
			{
				MessageService.ShowWarning(KPRes.PasswordRepeatFailed);
				return false;
			}

			if(m_secPassword.TextLength == 0)
			{
				if(!MessageService.AskYesNo(KPRes.EmptyMasterPw +
					MessageService.NewParagraph + KPRes.EmptyMasterPwHint +
					MessageService.NewParagraph + KPRes.EmptyMasterPwQuestion,
					null, false))
				{
					return false;
				}
			}

			uint uMinLen = Program.Config.Security.MasterPassword.MinimumLength;
			if(m_secPassword.TextLength < uMinLen)
			{
				string strML = KPRes.MasterPasswordMinLengthFailed;
				strML = strML.Replace(@"{PARAM}", uMinLen.ToString());
				MessageService.ShowWarning(strML);
				return false;
			}

			byte[] pb = m_secPassword.ToUtf8();

			uint uMinQual = Program.Config.Security.MasterPassword.MinimumQuality;
			if(QualityEstimation.EstimatePasswordBits(pb) < uMinQual)
			{
				string strMQ = KPRes.MasterPasswordMinQualityFailed;
				strMQ = strMQ.Replace(@"{PARAM}", uMinQual.ToString());
				MessageService.ShowWarning(strMQ);
				Array.Clear(pb, 0, pb.Length);
				return false;
			}

			string strValRes = Program.KeyValidatorPool.Validate(pb,
				KeyValidationType.MasterPassword);
			if(strValRes != null)
			{
				MessageService.ShowWarning(strValRes);
				Array.Clear(pb, 0, pb.Length);
				return false;
			}

			m_pKey.AddUserKey(new KcpPassword(pb));
			Array.Clear(pb, 0, pb.Length);

			return true;
		}

		private void EnableUserControls()
		{
			m_tbPassword.Enabled = m_tbRepeatPassword.Enabled = m_cbHidePassword.Enabled =
				m_lblRepeatPassword.Enabled = m_lblQualityBits.Enabled =
				m_lblEstimatedQuality.Enabled = true;


			SetHidePassword(m_cbHidePassword.Checked, false);

		}

		private void SetHidePassword(bool bHide, bool bUpdateCheckBox)
		{
			if(bUpdateCheckBox) m_cbHidePassword.Checked = bHide;

			m_secPassword.EnableProtection(bHide);
			m_secRepeat.EnableProtection(bHide);
		}

		private void OnCheckedPassword(object sender, EventArgs e)
		{
			EnableUserControls();

			m_tbPassword.Focus();
		}

		private void OnCheckedKeyFile(object sender, EventArgs e)
		{
			EnableUserControls();
		}

		private void OnCheckedHidePassword(object sender, EventArgs e)
		{
			SetHidePassword(m_cbHidePassword.Checked, false);
			m_tbPassword.Focus();
		}

		private void OnBtnOK(object sender, EventArgs e)
		{
			if(!CreateCompositeKey()) DialogResult = DialogResult.None;
            if (!string.IsNullOrEmpty(dbNameTextBox.Text))
                _databaseName = dbNameTextBox.Text;
		}

		private void OnBtnCancel(object sender, EventArgs e)
		{
			m_pKey = null;
		}

		private void ProcessTextChangedPassword(object sender, EventArgs e)
		{
			byte[] pbUTF8 = m_secPassword.ToUtf8();
			uint uBits = QualityEstimation.EstimatePasswordBits(pbUTF8);
			MemUtil.ZeroByteArray(pbUTF8);

			m_lblQualityBits.Text = uBits + " " + KPRes.Bits;
			int iPos = (int)((100 * uBits) / (256 / 2));
			if(iPos < 0) iPos = 0; else if(iPos > 100) iPos = 100;
			m_pbPasswordQuality.Value = iPos;
		}

		private void OnClickKeyFileCreate(object sender, EventArgs e)
		{
            SaveFileDialogEx sfd = UIUtil.CreateSaveFileDialog(KPRes.KeyFileCreate,
                UrlUtil.StripExtension(UrlUtil.GetFileName(m_ioInfo.Path)) + "." +
                AppDefs.FileExtension.KeyFile, UIUtil.CreateFileTypeFilter("key",
                    KPRes.KeyFiles, true), 1, "key", null);

            if (sfd.ShowDialog() == DialogResult.OK)
            {
                using (EntropyForm dlg = new EntropyForm())
                {
                    if (dlg.ShowDialog() == DialogResult.OK)
                    {
                        byte[] pbAdditionalEntropy = dlg.GeneratedEntropy;

                        try
                        {
                            KcpKeyFile.Create(sfd.FileName, pbAdditionalEntropy);
                        }
                        catch (Exception exKC)
                        {
                            MessageService.ShowWarning(exKC);
                        }
                    }
                }

                EnableUserControls();
            }
		}

		private void OnClickKeyFileBrowse(object sender, EventArgs e)
		{
            OpenFileDialogEx ofd = UIUtil.CreateOpenFileDialog(KPRes.KeyFileUseExisting,
                UIUtil.CreateFileTypeFilter("key", KPRes.KeyFiles, true), 2, null,
                false, null);

		    if (ofd.ShowDialog() == DialogResult.OK)
		    {
		        string str = ofd.FileName;
		    }

		    EnableUserControls();
		}

		private void OnWinUserCheckedChanged(object sender, EventArgs e)
		{
			EnableUserControls();
		}

		private void OnFormClosed(object sender, FormClosedEventArgs e)
		{
			GlobalWindowManager.RemoveWindow(this);
		}

		private void OnBtnHelp(object sender, EventArgs e)
		{
			AppHelp.ShowHelp(AppDefs.HelpTopics.KeySources, null);
		}

		private void OnKeyFileSelectedIndexChanged(object sender, EventArgs e)
		{
			EnableUserControls();
		}

		private void OnFormClosing(object sender, FormClosingEventArgs e)
		{
			CleanUpEx();
		}

        private void button1_Click(object sender, EventArgs e)
        {
            //TODO2: can we securly pass the user's existing master password / CompositeKey to the advanced key creation form?
        }

	}
}

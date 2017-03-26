using System;
using System.Collections.Generic;
using System.ComponentModel;
using System.Data;
using System.Drawing;
using System.Text;
using System.Windows.Forms;
using System.Diagnostics;

namespace KeePassRPC.Forms
{
    public partial class AuthForm : Form
    {
        public AuthForm()
        {
            InitializeComponent();
        }

        private string SecurityLevel;
        private string ClientName;
        private string ClientDescription;
        private string Password;
        KeePassRPCClientConnection Connection;

        public AuthForm(KeePassRPCClientConnection connection, string securityLevel, string clientName,
            string clientDescription, string password)
        {
            InitializeComponent();
            SecurityLevel = securityLevel;
            ClientName = clientName;
            ClientDescription = clientDescription;
            Password = password;
            Connection = connection;
        }

        private void AuthForm_Load(object sender, EventArgs e)
        {
            /*http://git.io/GaKFCA
             * 
"{\\rtf1\\ansi\\ansicpg1252\\deff0\\deflang2057{\\fonttbl{\\f0\\fnil\\fcharset0 Microsoft Sans Serif;}}\r\n\\viewkind4\\uc1\\pard\\f0\\fs18 This is a test\\par\r\n}\r\n"
             * */
            string secLevel = @"{\rtf1\ansi{\fonttbl\f0\fArial;}\f0\fs20KeeFox will connect using {\b " + SecurityLevel + @"} security. Please go to this web page to learn about the different levels of security and how to configure your personal security preferences:\par
";

            if (Type.GetType ("Mono.Runtime") != null)
                secLevel += @"{\fs18http://git.io/GaKFCA}\par\par

";
            else
                secLevel += @"{\fs18https://github.com/luckyrat/KeeFox/wiki/en-%7C-Technical-%7C-KeePassRPC-%7C-Security-levels}\par\par

";

            secLevel += @"If you do not know what ""{\b " + ClientName + @"}"" is or have reason to suspect that a malicious program on your computer is pretending to be ""{\b " + ClientName + @"}"" you can deny the request by clicking the button below.
}";
            richTextBoxSecurityLevel.Rtf = secLevel;
            richTextBoxSecurityLevel.LinkClicked += new System.Windows.Forms.LinkClickedEventHandler(this.richTextBoxSecurityLevel_LinkClicked);

            richTextBoxClientID.Rtf = @"{\rtf1\ansi{\fonttbl\f0\fArial;}\f0A program claiming to be ""{\b " + ClientName + @"}"" is asking you to confirm you want to allow it to access your passwords.\par
\par
""{\b " + ClientName + @"}"" claims that it is ""{\b " + ClientDescription + @"}"".\par
}";

            richTextBoxPassword.Text = Password;

            richTextBoxConfirmInstruction.Rtf = @"{\rtf1\ansi{\fonttbl\f0\fArial;}\f0To authorise {\b " + ClientName + @"} to access your passwords please enter this password into the box {\b " + ClientName + @"} has presented to you.}";
            
        }

        private void richTextBoxSecurityLevel_LinkClicked(object sender, LinkClickedEventArgs e)
        {
            Process p = new Process();
            p = Process.Start(e.LinkText);
        }

        private void buttonDeny_Click(object sender, EventArgs e)
        {
            // disconnect rpcclient (which will in turn look for any active instances of this dialog and close them whenever the underlying connection is dropped)
            Connection.WebSocketConnection.Close();
            // The underlying connection is now closed
            // this form will be closed when this event handler finishes (we've queued an invocation on this thread via the OnClose callback on a different thread)
        }

    }
}

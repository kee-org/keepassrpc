using System;
using System.Diagnostics;
using System.Linq;
using System.Windows.Forms;

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
        private KeePassRPCClientConnection Connection;

        public AuthForm(KeePassRPCClientConnection connection, string securityLevel, string clientName,
            string clientDescription, string password)
        {
            InitializeComponent();
            SecurityLevel = securityLevel;
            ClientName = SanitiseClientName(clientName);
            if (string.IsNullOrWhiteSpace(ClientName)) ClientName = "Client with an invalid name (CAUTION!!!)";
            ClientDescription = clientDescription;
            if (string.IsNullOrWhiteSpace(ClientDescription)) ClientDescription = "Client has supplied no description (CAUTION!!!)";
            Password = password;
            Connection = connection;
        }

        private string SanitiseClientName(string name)
        {
            return new String(name.Where(
                c => char.IsLetter(c) || char.IsDigit(c) || c == ' ' || c == '-')
                .ToArray());
        }

        private void AuthForm_Load(object sender, EventArgs e)
        {
            /*https://git.io/GaKFCA
             * 
"{\\rtf1\\ansi\\ansicpg1252\\deff0\\deflang2057{\\fonttbl{\\f0\\fnil\\fcharset0 Microsoft Sans Serif;}}\r\n\\viewkind4\\uc1\\pard\\f0\\fs18 This is a test\\par\r\n}\r\n"
             * */
            string secLevel = @"{\rtf1\ansi{\fonttbl\f0\fArial;}\f0\fs20" + ClientName + @" will connect using {\b " + SecurityLevel + @"} security. Please go to this web page to learn about the different levels of security and how to configure your personal security preferences:\par
";

            secLevel += @"{\fs18https://forum.kee.pm/t/connection-security-levels/1075}\par\par

";

            secLevel += @"If you do not know what ""{\b " + ClientName + @"}"" is or have reason to suspect that a malicious program on your computer is pretending to be ""{\b " + ClientName + @"}"" you can deny the request by clicking the button below.
}";
            richTextBoxSecurityLevel.Rtf = secLevel;
            richTextBoxSecurityLevel.LinkClicked += richTextBoxSecurityLevel_LinkClicked;

            richTextBoxClientID.Rtf = @"{\rtf1\ansi{\fonttbl\f0\fArial;}\f0A program claiming to be ""{\b " + ClientName + @"}"" is asking you to confirm you want to allow it to access your passwords.\par
\par
""{\b " + ClientName + @"}"" claims that it is ""{\b " + ClientDescription + @"}"".\par
}";

            richTextBoxPassword.Text = Password;

            richTextBoxConfirmInstruction.Rtf = @"{\rtf1\ansi{\fonttbl\f0\fArial;}\f0To authorise the client to access your passwords please enter this password into the box it has presented to you.}";
            
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

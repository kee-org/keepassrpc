using System.Diagnostics;
using System.Windows.Forms;

namespace KeePassRPC.Forms
{
    public partial class WelcomeForm : Form
    {
        public WelcomeForm()
        {
            InitializeComponent();
        }

        private void linkLabel1_LinkClicked(object sender, LinkLabelLinkClickedEventArgs e)
        {
            Process p = new Process();
            p = Process.Start("https://forum.kee.pm/t/help-and-support/24");
        }

    }
}

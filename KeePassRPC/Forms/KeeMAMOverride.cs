using DomainPublicSuffix;
using System;
using System.Collections.Generic;
using System.Windows.Forms;

namespace KeePassRPC.Forms
{
    public partial class KeeMAMOverrideForm : Form
    {
        public string Domain;
        public MatchAccuracyMethod MAM;
        public List<string> OtherKeys;

        public KeeMAMOverrideForm(string domain, MatchAccuracyMethod? mam, List<string> otherKeys)
        {
            InitializeComponent();
            Icon = global::KeePassRPC.Properties.Resources.kee;
            Domain = domain;
            OtherKeys = otherKeys;
            MAM = mam.GetValueOrDefault(MatchAccuracyMethod.Domain);

            textBox1.Text = Domain;

            switch (MAM)
            {
                case MatchAccuracyMethod.Exact: radioButton3.Checked = true; break;
                case MatchAccuracyMethod.Hostname: radioButton2.Checked = true; break;
                case MatchAccuracyMethod.Domain: radioButton1.Checked = true; break;
            }

            if (string.IsNullOrEmpty(domain))
                Text = "Add Override";
            else
                Text = "Edit Override";
        }

        private void button2_Click(object sender, EventArgs e)
        {
            
        }

        private void buttonOK_Click(object sender, EventArgs e)
        {
            if (textBox1.Text.Length > 0)
                Domain = textBox1.Text;
            else
                DialogResult = DialogResult.None;

            if (OtherKeys.Contains(Domain))
            {
                MessageBox.Show(this, "An override for '" + Domain + "' has already been added. Cancel, then find and Edit the existing override.");
                DialogResult = DialogResult.None;
                return;
            }

            DomainName domain;
            DomainName.TryParse(Domain, out domain);
            if (domain == null || domain.RegistrableDomain == null || domain.RegistrableDomain != Domain)
            {
                MessageBox.Show(this, "Invalid domain name");
                DialogResult = DialogResult.None;
                return;
            }

            MAM = DetermineDefaultMatchAccuracy();
        }

        private MatchAccuracyMethod DetermineDefaultMatchAccuracy()
        {
            if (radioButton3.Checked) return MatchAccuracyMethod.Exact;
            else if (radioButton2.Checked) return MatchAccuracyMethod.Hostname;
            else return MatchAccuracyMethod.Domain;
        }
    }
}

using System;
using System.Collections.Generic;
using System.ComponentModel;
using System.Drawing;
using System.Data;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using System.Windows.Forms;
using KeePassLib;

namespace KeePassRPC.Forms
{
    public partial class DatabaseSettingsUserControl : UserControl
    {
        private PwDatabase database;
        private DatabaseConfig config;

        public DatabaseSettingsUserControl(PwDatabase db)
        {
            database = db;
            config = db.GetKPRPCConfig();
            InitializeComponent();
            PresentDefaultMatchAccuracy();
            PresentDomains();
        }
        
        protected override void OnLoad(EventArgs e)
        {
            base.OnLoad(e);
            if (ParentForm != null)
            {
                ParentForm.FormClosing +=
                  delegate (object aSender, FormClosingEventArgs aEventArgs)
                  {
                      if (ParentForm.DialogResult == DialogResult.OK)
                      {
                          config.DefaultMatchAccuracy = DetermineDefaultMatchAccuracy();
                          config.MatchedURLAccuracyOverrides = DetermineMatchedURLAccuracyOverrides();
                          database.SetKPRPCConfig(config);
                      }
                  };
            }
        }

        private void PresentDefaultMatchAccuracy()
        {
            switch (config.DefaultMatchAccuracy)
            {
                case MatchAccuracyMethod.Exact: radioButton3.Checked = true; break;
                case MatchAccuracyMethod.Hostname: radioButton2.Checked = true; break;
                case MatchAccuracyMethod.Domain: radioButton1.Checked = true; break;
            }
        }

        private MatchAccuracyMethod DetermineDefaultMatchAccuracy()
        {
            if (radioButton3.Checked) return MatchAccuracyMethod.Exact;
            else if (radioButton2.Checked) return MatchAccuracyMethod.Hostname;
            else return MatchAccuracyMethod.Domain;
        }

        private Dictionary<string, MatchAccuracyMethod> DetermineMatchedURLAccuracyOverrides()
        {
            Dictionary<string, MatchAccuracyMethod> matchedURLAccuracyOverrides = 
                new Dictionary<string, MatchAccuracyMethod>();

            for (int i = 0; i < listView1.Items.Count; ++i)
            {
                matchedURLAccuracyOverrides.Add(
                    listView1.Items[i].Text, 
                    MatchAccuracyMethodFromText(listView1.Items[i].SubItems[1].Text));
            }
            return matchedURLAccuracyOverrides;
        }

        private void buttonURLAdd_Click(object sender, EventArgs e)
        {
            List<string> all = new List<string>();
            for (int i = 0; i < listView1.Items.Count; ++i)
                all.Add(listView1.Items[i].Text);

            using (var mamoForm = new KeeMAMOverrideForm(null, null, all))
            {
                if (mamoForm.ShowDialog() == DialogResult.OK)
                {
                    AddURLListItem(mamoForm.Domain, mamoForm.MAM);
                }
            }
        }

        private void buttonURLEdit_Click(object sender, EventArgs e)
        {
            ListView.SelectedListViewItemCollection lvsicSel = listView1.SelectedItems;

            List<string> all = new List<string>();
            for (int i = 0; i < listView1.Items.Count; ++i)
                all.Add(listView1.Items[i].Text);

            for (int i = 0; i < lvsicSel.Count; ++i)
            {
                List<string> others = all.GetRange(0, all.Count);
                others.Remove(lvsicSel[i].Text);

                // find the current data
                using (var mamoForm = MAMOFormForEditing(lvsicSel, i, others))
                {
                    if (mamoForm.ShowDialog() == DialogResult.OK)
                    {
                        RemoveURLListItem(lvsicSel[i]);
                        AddURLListItem(mamoForm.Domain, mamoForm.MAM);
                    }
                }
            }
        }

        private static KeeMAMOverrideForm MAMOFormForEditing(ListView.SelectedListViewItemCollection lvsicSel, int i, List<string> others)
        {
            return new KeeMAMOverrideForm(lvsicSel[i].Text, MatchAccuracyMethodFromText(lvsicSel[i].SubItems[1].Text), others);
        }

        private static MatchAccuracyMethod MatchAccuracyMethodFromText(string text)
        {
            switch (text)
            {
                case "Domain": return MatchAccuracyMethod.Domain;
                case "Hostname": return MatchAccuracyMethod.Hostname;
                case "Exact": return MatchAccuracyMethod.Exact;
                default: throw new Exception("Invalid data for existing entry.");
            }
        }

        private void buttonURLDelete_Click(object sender, EventArgs e)
        {
            ListView.SelectedListViewItemCollection lvsicSel = listView1.SelectedItems;
            for (int i = 0; i < lvsicSel.Count; ++i)
            {
                RemoveURLListItem(lvsicSel[i]);
            }
        }

        private void PresentDomains()
        {
            if (config.MatchedURLAccuracyOverrides == null || config.MatchedURLAccuracyOverrides.Count == 0)
            {
                return;
            }

            foreach (var ovrride in config.MatchedURLAccuracyOverrides)
            {
                AddURLListItem(ovrride.Key, ovrride.Value);
            }
        }

        private void AddURLListItem(string domain, MatchAccuracyMethod mam)
        {
            switch (mam)
            {
                case MatchAccuracyMethod.Domain:
                    listView1.Items.Add(new ListViewItem(new string[] { domain, "Domain" }));
                    break;
                case MatchAccuracyMethod.Hostname:
                    listView1.Items.Add(new ListViewItem(new string[] { domain, "Hostname" }));
                    break;
                case MatchAccuracyMethod.Exact:
                    listView1.Items.Add(new ListViewItem(new string[] { domain, "Exact" }));
                    break;
            }
        }

        private void RemoveURLListItem(ListViewItem lvi)
        {
            listView1.Items.Remove(lvi);
            if (listView1.Items.Count == 0)
            {
                buttonURLEdit.Enabled = false;
                buttonURLDelete.Enabled = false;
            }
        }

        private void listView1_SelectedIndexChanged(object sender, EventArgs e)
        {
            if (listView1.SelectedItems.Count > 0)
            {
                buttonURLEdit.Enabled = true;
                buttonURLDelete.Enabled = true;
            }
            else
            {
                buttonURLEdit.Enabled = false;
                buttonURLDelete.Enabled = false;
            }
        }

        private void listView1_DoubleClick(object sender, EventArgs e)
        {
            if (listView1.SelectedItems.Count > 0)
            {
                buttonURLEdit_Click(sender, e);
            }
        }
    }
}

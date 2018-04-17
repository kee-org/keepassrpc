using System;
using System.Collections.Generic;
using System.ComponentModel;
using System.Drawing;
using System.Data;
using System.Text;
using System.Windows.Forms;
using KeePassLib;
using KeePassLib.Security;
using KeePass.UI;
using KeePassRPC.DataExchangeModel;
using KeePass.Forms;
using KeePassLib.Collections;

namespace KeePassRPC.Forms
{
    /// <summary>
    /// We read and write to the GUI in the Advanced tab of the standard entry editing dialog. This allows us to cancel and commit changes when the user presses OK or cancel.
    /// </summary>
    public partial class KeeEntryUserControl : UserControl
    {
        private PwEntry _entry;
        KeePassRPCExt KeePassRPCPlugin;
        PwEntryForm _pwEntryForm;
        ProtectedStringDictionary _strings;
        EntryConfig _conf;
        DatabaseConfig _dbConf;

        public KeeEntryUserControl(KeePassRPCExt keePassRPCPlugin, PwEntry entry,
            CustomListViewEx advancedListView, PwEntryForm pwEntryForm, ProtectedStringDictionary strings)
        {
            KeePassRPCPlugin = keePassRPCPlugin;
            _entry = entry;
            InitializeComponent();
            _pwEntryForm = pwEntryForm;
            _strings = strings;
            _dbConf = KeePassRPCPlugin._host.Database.GetKPRPCConfig();
            _conf = entry.GetKPRPCConfig(strings, _dbConf.DefaultMatchAccuracy);
        }

        private void changeAdvancedString(string name, string value, bool protect)
        {
            _strings.Set(name, new ProtectedString(protect, value));
        }

        private void UpdateKPRPCJSON(EntryConfig _conf)
        {
            // if the config is identical to an empty (default) config, only update if a JSON string already exists
            if (!_conf.Equals(new EntryConfig(KeePassRPCPlugin._host.Database.GetKPRPCConfig().DefaultMatchAccuracy)) || _strings.GetKeys().Contains("KPRPC JSON"))
                changeAdvancedString("KPRPC JSON", Jayrock.Json.Conversion.JsonConvert.ExportToString(_conf), true);
        }
        
        private void checkBoxHideFromKee_CheckedChanged(object sender, EventArgs e)
        {
            if (checkBoxHideFromKee.Checked)
            {
                _conf.Hide = true;
                textBoxKeePriority.Enabled = false;
                label1.Enabled = false;
                groupBox1.Enabled = false;
                groupBox2.Enabled = false;
                groupBox3.Enabled = false;
                groupBox4.Enabled = false;
                labelRealm.Enabled = false;
                textBoxKeeRealm.Enabled = false;
            }
            else
            {
                _conf.Hide = false;
                textBoxKeePriority.Enabled = true;
                label1.Enabled = true;
                groupBox1.Enabled = true;
                groupBox2.Enabled = true;
                groupBox3.Enabled = true;
                groupBox4.Enabled = true;
                labelRealm.Enabled = true;
                textBoxKeeRealm.Enabled = true;
            }
            UpdateKPRPCJSON(_conf);
        }

        private void KeeEntryUserControl_Load(object sender, EventArgs e)
        {
            bool kfNeverAutoFill = false;
            bool kfAlwaysAutoFill = false;
            bool kfNeverAutoSubmit = false;
            bool kfAlwaysAutoSubmit = false;

            if (_conf == null)
                return;

            this.checkBoxHideFromKee.CheckedChanged += new System.EventHandler(this.checkBoxHideFromKee_CheckedChanged);

            if (_conf.Hide) { checkBoxHideFromKee.Checked = true; }
            if (_conf.GetMatchAccuracyMethod() == MatchAccuracyMethod.Exact)
            {
                radioButton3.Checked = true;
            }
            else if (_conf.GetMatchAccuracyMethod() == MatchAccuracyMethod.Hostname)
            {
                radioButton2.Checked = true;
            }
            else
            {
                radioButton1.Checked = true;
            }
            RadioButton defaultRadioButton = null;
            switch (_dbConf.DefaultMatchAccuracy)
            {
                case MatchAccuracyMethod.Exact: defaultRadioButton = radioButton3; break;
                case MatchAccuracyMethod.Hostname: defaultRadioButton = radioButton2; break;
                case MatchAccuracyMethod.Domain: defaultRadioButton = radioButton1; break;
            }
            toolTipRealm.SetToolTip(defaultRadioButton, "This is the default behaviour for new entries. Change in the Database Settings dialog.");

            if (_conf.NeverAutoFill) { kfNeverAutoFill = true; }
            if (_conf.AlwaysAutoFill) { kfAlwaysAutoFill = true; }
            if (_conf.NeverAutoSubmit) { kfNeverAutoSubmit = true; }
            if (_conf.AlwaysAutoSubmit) { kfAlwaysAutoSubmit = true; }
            if (_conf.Priority != 0)
                textBoxKeePriority.Text = _conf.Priority.ToString();
                
            listNormalURLs = new List<string>();
            listNormalBlockedURLs = new List<string>();
            listRegExURLs = new List<string>();
            listRegExBlockedURLs = new List<string>();

            if (_conf.AltURLs != null) listNormalURLs.AddRange(_conf.AltURLs);
            if (_conf.BlockedURLs != null) listNormalBlockedURLs.AddRange(_conf.BlockedURLs);
            if (_conf.RegExURLs != null) listRegExURLs.AddRange(_conf.RegExURLs);
            if (_conf.RegExBlockedURLs != null) listRegExBlockedURLs.AddRange(_conf.RegExBlockedURLs);

            textBoxKeeRealm.Text = _conf.HTTPRealm;

            bool standardPasswordFound = false;
            bool standardUsernameFound = false;

            // Read the list of form field objects and create ListViewItems for display to the user
            if (_conf.FormFieldList != null)
            {
                foreach (FormField ff in _conf.FormFieldList)
                {
                    string type = Utilities.FormFieldTypeToDisplay(ff.Type, false);

                    // Override display of standard variables 
                    string value = ff.Value;
                    if (ff.Type == FormFieldType.FFTpassword)
                        value = "********";
                    if (ff.DisplayName == "KeePass username")
                    {
                        standardUsernameFound = true;
                        value = ff.DisplayName;
                    }
                    if (ff.DisplayName == "KeePass password")
                    {
                        standardPasswordFound = true;
                        value = ff.DisplayName;
                    }
                    ListViewItem lvi = new ListViewItem(new string[] { ff.Name, value, ff.Id, type, ff.Page.ToString() });
                    lvi.Tag = ff;
                    AddFieldListItem(lvi);
                }
            }

            // if we didn't find specific details about the username and
            // password, we'll pre-populate the standard KeePass ones so
            // users can easily change things like page number and ID

            // we don't add them to the list of actual fields though - just the display list.
            if (!standardPasswordFound)
            {
                ListViewItem lvi = new ListViewItem(new string[] { "", "{PASSWORD}", "", "password", "1" });
                AddFieldListItem(lvi);
            }
            if (!standardUsernameFound)
            {
                ListViewItem lvi = new ListViewItem(new string[] { "", "{USERNAME}", "", "username", "1" });
                AddFieldListItem(lvi);
            }

            ReadURLStrings();

            comboBoxAutoSubmit.Text = "Use Kee setting";
            comboBoxAutoFill.Text = "Use Kee setting";

            // There are implicit behaviours based on single option choices so we'll make them explicit now so that the GUI accurately reflects the 
            // strings stored in the advanced tab
            if (kfNeverAutoFill)
                currentBehaviour = EntryBehaviour.NeverAutoFillNeverAutoSubmit;
            else if (kfAlwaysAutoSubmit)
                currentBehaviour = EntryBehaviour.AlwaysAutoFillAlwaysAutoSubmit;
            else if (kfAlwaysAutoFill && kfNeverAutoSubmit)
                currentBehaviour = EntryBehaviour.AlwaysAutoFillNeverAutoSubmit;
            else if (kfNeverAutoSubmit)
                currentBehaviour = EntryBehaviour.NeverAutoSubmit;
            else if (kfAlwaysAutoFill)
                currentBehaviour = EntryBehaviour.AlwaysAutoFill;
            else
                currentBehaviour = EntryBehaviour.Default;
            changeBehaviourState(currentBehaviour);

            this.comboBoxAutoSubmit.SelectedIndexChanged += new System.EventHandler(this.comboBoxAutoSubmit_SelectedIndexChanged);
            this.comboBoxAutoFill.SelectedIndexChanged += new System.EventHandler(this.comboBoxAutoFill_SelectedIndexChanged);            
            this.textBoxKeePriority.TextChanged += new System.EventHandler(this.textBoxKeePriority_TextChanged);

            string realmTooltip = "Set this to the realm (what the \"site says\") in the HTTP authentication popup dialog box for a more accurate match";
            toolTipRealm.SetToolTip(this.textBoxKeeRealm, realmTooltip);
            toolTipRealm.SetToolTip(this.labelRealm, realmTooltip);
        }

        private void textBoxKeePriority_TextChanged(object sender, EventArgs e)
        {
            string priority = ((System.Windows.Forms.TextBoxBase)sender).Text;

            if (!string.IsNullOrEmpty(priority))
            {
                try
                {
                    _conf.Priority = int.Parse(priority);
                    UpdateKPRPCJSON(_conf);
                    return;
                }
                catch (Exception)
                {
                }
            }
            _conf.Priority = 0;
            UpdateKPRPCJSON(_conf);
            return;
        }

        private void textBoxKeeRealm_TextChanged(object sender, EventArgs e)
        {
            string realm = ((System.Windows.Forms.TextBoxBase)sender).Text;

            if (!string.IsNullOrEmpty(realm))
                _conf.HTTPRealm = realm;
            else
                _conf.HTTPRealm = null;
            UpdateKPRPCJSON(_conf);
            return;
        }

        private void comboBoxAutoFill_SelectedIndexChanged(object sender, EventArgs e)
        {
            switch (comboBoxAutoFill.Text)
            {
                case "Use Kee setting":
                    if (comboBoxAutoSubmit.Text == "Never")
                        changeBehaviourState(EntryBehaviour.NeverAutoSubmit);
                    else
                        changeBehaviourState(EntryBehaviour.Default);
                    break;
                case "Never":
                    changeBehaviourState(EntryBehaviour.NeverAutoFillNeverAutoSubmit);
                    break;
                case "Always":
                    if (comboBoxAutoSubmit.Text == "Never")
                        changeBehaviourState(EntryBehaviour.AlwaysAutoFillNeverAutoSubmit);
                    else if (comboBoxAutoSubmit.Text == "Always")
                        changeBehaviourState(EntryBehaviour.AlwaysAutoFillAlwaysAutoSubmit);
                    else
                        changeBehaviourState(EntryBehaviour.AlwaysAutoFill);
                    break;
            }
        }

        private void comboBoxAutoSubmit_SelectedIndexChanged(object sender, EventArgs e)
        {
            switch (comboBoxAutoSubmit.Text)
            {
                case "Use Kee setting": 
                    if (comboBoxAutoFill.Text == "Always") 
                        changeBehaviourState(EntryBehaviour.AlwaysAutoFill); 
                    else
                        changeBehaviourState(EntryBehaviour.Default);
                    break;
                case "Never":
                    if (comboBoxAutoFill.Text == "Always")
                        changeBehaviourState(EntryBehaviour.AlwaysAutoFillNeverAutoSubmit);
                    else if (comboBoxAutoFill.Text == "Never")
                        changeBehaviourState(EntryBehaviour.NeverAutoFillNeverAutoSubmit);
                    else
                        changeBehaviourState(EntryBehaviour.NeverAutoSubmit);
                    break;
                case "Always":
                    changeBehaviourState(EntryBehaviour.AlwaysAutoFillAlwaysAutoSubmit);
                    break;
            }
        }

        enum EntryBehaviour
        {
            Default,
            NeverAutoFillNeverAutoSubmit,
            NeverAutoSubmit,
            AlwaysAutoFillAlwaysAutoSubmit,
            AlwaysAutoFill,
            AlwaysAutoFillNeverAutoSubmit
        }

        EntryBehaviour currentBehaviour = EntryBehaviour.Default;

        private void changeBehaviourState(EntryBehaviour behav)
        {
            switch (behav)
            {
                case EntryBehaviour.AlwaysAutoFill:
                    _conf.AlwaysAutoFill = true;
                    _conf.AlwaysAutoSubmit = false;
                    _conf.NeverAutoFill = false;
                    _conf.NeverAutoSubmit = false;
                    comboBoxAutoFill.Text = "Always";
                    comboBoxAutoSubmit.Text = "Use Kee setting";
                    comboBoxAutoFill.Enabled = true;
                    comboBoxAutoSubmit.Enabled = true;
                    break;
                case EntryBehaviour.NeverAutoSubmit:
                    _conf.AlwaysAutoFill = false;
                    _conf.AlwaysAutoSubmit = false;
                    _conf.NeverAutoFill = false;
                    _conf.NeverAutoSubmit = true;
                    comboBoxAutoFill.Text = "Use Kee setting";
                    comboBoxAutoSubmit.Text = "Never";
                    comboBoxAutoFill.Enabled = true;
                    comboBoxAutoSubmit.Enabled = true;
                    break;
                case EntryBehaviour.AlwaysAutoFillAlwaysAutoSubmit:
                    _conf.AlwaysAutoFill = true;
                    _conf.AlwaysAutoSubmit = true;
                    _conf.NeverAutoFill = false;
                    _conf.NeverAutoSubmit = false;
                    comboBoxAutoSubmit.Text = "Always";
                    comboBoxAutoFill.Text = "Always";
                    comboBoxAutoFill.Enabled = false;
                    comboBoxAutoSubmit.Enabled = true;
                    break;
                case EntryBehaviour.NeverAutoFillNeverAutoSubmit:
                    _conf.AlwaysAutoFill = false;
                    _conf.AlwaysAutoSubmit = false;
                    _conf.NeverAutoFill = true;
                    _conf.NeverAutoSubmit = true;
                    comboBoxAutoFill.Text = "Never";
                    comboBoxAutoSubmit.Text = "Never";
                    comboBoxAutoSubmit.Enabled = false;
                    comboBoxAutoFill.Enabled = true;
                    break;
                case EntryBehaviour.AlwaysAutoFillNeverAutoSubmit:
                    _conf.AlwaysAutoFill = true;
                    _conf.AlwaysAutoSubmit = false;
                    _conf.NeverAutoFill = false;
                    _conf.NeverAutoSubmit = true;
                    comboBoxAutoFill.Text = "Always";
                    comboBoxAutoSubmit.Text = "Never";
                    comboBoxAutoSubmit.Enabled = true;
                    comboBoxAutoFill.Enabled = true;
                    break;
                case EntryBehaviour.Default:
                    _conf.AlwaysAutoFill = false;
                    _conf.AlwaysAutoSubmit = false;
                    _conf.NeverAutoFill = false;
                    _conf.NeverAutoSubmit = false;
                    comboBoxAutoFill.Text = "Use Kee setting";
                    comboBoxAutoSubmit.Text = "Use Kee setting";
                    comboBoxAutoSubmit.Enabled = true;
                    comboBoxAutoFill.Enabled = true;
                    break;
            }
            UpdateKPRPCJSON(_conf);
            currentBehaviour = behav;
        }

        List<string> listNormalURLs = new List<string>();
        List<string> listRegExURLs = new List<string>();
        List<string> listNormalBlockedURLs = new List<string>();
        List<string> listRegExBlockedURLs = new List<string>();

        private void buttonURLAdd_Click(object sender, EventArgs e)
        {
            List<string> all = new List<string>();
            for (int i = 0; i < listView1.Items.Count; ++i)
                all.Add(listView1.Items[i].Text);

            using (KeeURLForm kfurlf = new KeeURLForm(false, false, null, null, all))
            {
                if (kfurlf.ShowDialog() == DialogResult.OK)
                {
                    //UpdateEntryStrings(false, false);
                    //ResizeColumnHeaders();

                    if (kfurlf.Match && !kfurlf.RegEx)
                    {
                        listNormalURLs.Add(kfurlf.URL);
                        ListViewItem lvi = new ListViewItem(new string[] { kfurlf.URL, "Normal", "Match" });
                        AddURLListItem(lvi);
                    }
                    if (kfurlf.Block && !kfurlf.RegEx)
                    {
                        listNormalBlockedURLs.Add(kfurlf.URL);
                        ListViewItem lvi = new ListViewItem(new string[] { kfurlf.URL, "Normal", "Block" });
                        AddURLListItem(lvi);
                    }
                    if (kfurlf.Match && kfurlf.RegEx)
                    {
                        listRegExURLs.Add(kfurlf.URL);
                        ListViewItem lvi = new ListViewItem(new string[] { kfurlf.URL, "RegEx", "Match" });
                        AddURLListItem(lvi);
                    }
                    if (kfurlf.Block && kfurlf.RegEx)
                    {
                        listRegExBlockedURLs.Add(kfurlf.URL);
                        ListViewItem lvi = new ListViewItem(new string[] { kfurlf.URL, "RegEx", "Block" });
                        AddURLListItem(lvi);
                    }
                    UpdateURLStrings();
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
                using (KeeURLForm kfurlf = URLFormForEditing(lvsicSel, i, others))
                {
                    if (kfurlf.ShowDialog() == DialogResult.OK)
                    {
                        // remove the old URL data
                        if (lvsicSel[i].SubItems[1].Text == "Normal" && lvsicSel[i].SubItems[2].Text == "Match")
                            listNormalURLs.Remove(lvsicSel[i].Text);
                        else if (lvsicSel[i].SubItems[1].Text == "Normal" && lvsicSel[i].SubItems[2].Text == "Block")
                            listNormalBlockedURLs.Remove(lvsicSel[i].Text);
                        else if (lvsicSel[i].SubItems[1].Text == "RegEx" && lvsicSel[i].SubItems[2].Text == "Match")
                            listRegExURLs.Remove(lvsicSel[i].Text);
                        else if (lvsicSel[i].SubItems[1].Text == "RegEx" && lvsicSel[i].SubItems[2].Text == "Block")
                            listRegExBlockedURLs.Remove(lvsicSel[i].Text);
                        RemoveURLListItem(lvsicSel[i]);

                        //UpdateEntryStrings(false, false);
                        //ResizeColumnHeaders();

                        if (kfurlf.Match && !kfurlf.RegEx)
                        {
                            listNormalURLs.Add(kfurlf.URL);
                            ListViewItem lvi = new ListViewItem(new string[] { kfurlf.URL, "Normal", "Match" });
                            AddURLListItem(lvi);
                        }
                        if (kfurlf.Block && !kfurlf.RegEx)
                        {
                            listNormalBlockedURLs.Add(kfurlf.URL);
                            ListViewItem lvi = new ListViewItem(new string[] { kfurlf.URL, "Normal", "Block" });
                            AddURLListItem(lvi);
                        }
                        if (kfurlf.Match && kfurlf.RegEx)
                        {
                            listRegExURLs.Add(kfurlf.URL);
                            ListViewItem lvi = new ListViewItem(new string[] { kfurlf.URL, "RegEx", "Match" });
                            AddURLListItem(lvi);
                        }
                        if (kfurlf.Block && kfurlf.RegEx)
                        {
                            listRegExBlockedURLs.Add(kfurlf.URL);
                            ListViewItem lvi = new ListViewItem(new string[] { kfurlf.URL, "RegEx", "Block" });
                            AddURLListItem(lvi);
                        }
                        UpdateURLStrings();
                    }
                }
            }
        }

        private static KeeURLForm URLFormForEditing(ListView.SelectedListViewItemCollection lvsicSel, int i, List<string> others)
        {
            KeeURLForm kfurlf = null;
            if (lvsicSel[i].SubItems[1].Text == "Normal" && lvsicSel[i].SubItems[2].Text == "Match")
                kfurlf = new KeeURLForm(true, false, null, lvsicSel[i].Text, others);
            else if (lvsicSel[i].SubItems[1].Text == "Normal" && lvsicSel[i].SubItems[2].Text == "Block")
                kfurlf = new KeeURLForm(false, true, null, lvsicSel[i].Text, others);
            else if (lvsicSel[i].SubItems[1].Text == "RegEx" && lvsicSel[i].SubItems[2].Text == "Match")
                kfurlf = new KeeURLForm(true, false, lvsicSel[i].Text, null, others);
            else if (lvsicSel[i].SubItems[1].Text == "RegEx" && lvsicSel[i].SubItems[2].Text == "Block")
                kfurlf = new KeeURLForm(false, true, lvsicSel[i].Text, null, others);
            return kfurlf;
        }

        private void buttonURLDelete_Click(object sender, EventArgs e)
        {
            ListView.SelectedListViewItemCollection lvsicSel = listView1.SelectedItems;
            for (int i = 0; i < lvsicSel.Count; ++i)
            {
                if (lvsicSel[i].SubItems[1].Text == "Normal" && lvsicSel[i].SubItems[2].Text == "Match")
                    listNormalURLs.Remove(lvsicSel[i].Text);
                else if (lvsicSel[i].SubItems[1].Text == "Normal" && lvsicSel[i].SubItems[2].Text == "Block")
                    listNormalBlockedURLs.Remove(lvsicSel[i].Text);
                else if (lvsicSel[i].SubItems[1].Text == "RegEx" && lvsicSel[i].SubItems[2].Text == "Match")
                    listRegExURLs.Remove(lvsicSel[i].Text);
                else if (lvsicSel[i].SubItems[1].Text == "RegEx" && lvsicSel[i].SubItems[2].Text == "Block")
                    listRegExBlockedURLs.Remove(lvsicSel[i].Text);
                RemoveURLListItem(lvsicSel[i]);
            }

            UpdateURLStrings();
        }

        private void UpdateURLStrings()
        {
            _conf.AltURLs = listNormalURLs.ToArray();
            _conf.BlockedURLs = listNormalBlockedURLs.ToArray();
            _conf.RegExURLs = listRegExURLs.ToArray();
            _conf.RegExBlockedURLs = listRegExBlockedURLs.ToArray();
            UpdateKPRPCJSON(_conf);
        }


        private void UpdateFieldStrings()
        {
            List<FormField> ffs = new List<FormField>();
            foreach (ListViewItem lvi in listView2.Items)
            {
                if (lvi.Tag != null)
                    ffs.Add((FormField)lvi.Tag);
            }
            _conf.FormFieldList = ffs.ToArray();
            UpdateKPRPCJSON(_conf);
        }
        

        private void ReadURLStrings()
        {
            foreach (string url in listNormalURLs)
            {
                ListViewItem lvi = new ListViewItem(new string[] { url, "Normal", "Match" });
                AddURLListItem(lvi);
            }
            foreach (string url in listNormalBlockedURLs)
            {
                ListViewItem lvi = new ListViewItem(new string[] { url, "Normal", "Block" });
                AddURLListItem(lvi);
            }
            foreach (string url in listRegExURLs)
            {
                ListViewItem lvi = new ListViewItem(new string[] { url, "RegEx", "Match" });
                AddURLListItem(lvi);
            }
            foreach (string url in listRegExBlockedURLs)
            {
                ListViewItem lvi = new ListViewItem(new string[] { url, "RegEx", "Block" });
                AddURLListItem(lvi);
            }
        }

        private void AddURLListItem(ListViewItem lvi)
        {
            listView1.Items.Add(lvi);
            //buttonURLEdit.Enabled = true;
            //buttonURLDelete.Enabled = true;
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
            } else
            {
                buttonURLEdit.Enabled = false;
                buttonURLDelete.Enabled = false;
            }
        }

        private void buttonFieldAdd_Click(object sender, EventArgs e)
        {
            using (KeeFieldForm kfff = new KeeFieldForm(null, null, null, FormFieldType.FFTtext, 1))
            {

                if (kfff.ShowDialog() == DialogResult.OK)
                {
                    FormField ff = new FormField(kfff.Name, kfff.Name, kfff.Value, kfff.Type, kfff.Id, kfff.Page);

                    string type = Utilities.FormFieldTypeToDisplay(kfff.Type, false);
                    int page = kfff.Page;

                    // We know any new passwords are not the main Entry password
                    // Also know that the display name can be same as main name
                    ListViewItem lvi = new ListViewItem(new string[]
                    {
                        kfff.Name, kfff.Type == FormFieldType.FFTpassword ? "********" : kfff.Value, kfff.Id, type,
                        page.ToString()
                    });
                    lvi.Tag = ff;
                    AddFieldListItem(lvi);
                    UpdateFieldStrings();
                }
            }
        }

        private void buttonFieldEdit_Click(object sender, EventArgs e)
        {
            ListView.SelectedListViewItemCollection lvsicSel = listView2.SelectedItems;

            FormField tag = (FormField)lvsicSel[0].Tag;
            using (KeeFieldForm kfff = FormFieldForEditing(lvsicSel, tag))
            {
                if (kfff.ShowDialog() == DialogResult.OK)
                {
                    // Update the display name. defaulting to whatever user entered as the name unless they were editing one of the standard fields
                    //Really? Why fix the name to this random string?!!
                    string displayName = kfff.Name;
                    if (kfff.Value == "{PASSWORD}")
                        displayName = "KeePass password";
                    else if (kfff.Value == "{USERNAME}")
                        displayName = "KeePass username";

                    string displayValue = kfff.Value;
                    if (kfff.Type == FormFieldType.FFTpassword)
                    {
                        displayValue = "********";
                        if (kfff.Value == "{PASSWORD}")
                            displayValue = "KeePass password";
                    }
                    string type = Utilities.FormFieldTypeToDisplay(kfff.Type, false);
                    int page = kfff.Page;

                    ListViewItem lvi = new ListViewItem(new string[] { kfff.Name, displayValue, kfff.Id, type, page.ToString() });
                    lvi.Tag = new FormField(kfff.Name, displayName, kfff.Value, kfff.Type, kfff.Id, page);
                    RemoveFieldListItem(lvsicSel[0]);
                    AddFieldListItem(lvi);
                    UpdateFieldStrings();
                }
            }
        }

        private static KeeFieldForm FormFieldForEditing(ListView.SelectedListViewItemCollection lvsicSel, FormField tag)
        {
            KeeFieldForm kfff = null;
            if (tag != null)
                kfff = new KeeFieldForm(tag);
            else if (lvsicSel[0].SubItems[1].Text == "{PASSWORD}")
                kfff = new KeeFieldForm(lvsicSel[0].SubItems[0].Text, "{PASSWORD}", lvsicSel[0].SubItems[2].Text, FormFieldType.FFTpassword, int.Parse(lvsicSel[0].SubItems[4].Text));
            else if (lvsicSel[0].SubItems[1].Text == "{USERNAME}")
                kfff = new KeeFieldForm(lvsicSel[0].SubItems[0].Text, "{USERNAME}", lvsicSel[0].SubItems[2].Text, FormFieldType.FFTusername, int.Parse(lvsicSel[0].SubItems[4].Text));
            else
                throw new Exception("Corrupt Entry found!");
            return kfff;
        }

        private void buttonFieldDelete_Click(object sender, EventArgs e)
        {
            ListView.SelectedListViewItemCollection lvsicSel = listView2.SelectedItems;
            // remove the old field data
            RemoveFieldListItem(lvsicSel[0]);
            UpdateFieldStrings();
        }

        //ReadFields function????

        private void AddFieldListItem(ListViewItem lvi)
        {
            listView2.Items.Add(lvi);
        }

        private void RemoveFieldListItem(ListViewItem lvi)
        {
            listView2.Items.Remove(lvi);
            if (listView2.Items.Count == 0)
            {
                buttonFieldEdit.Enabled = false;
                buttonFieldDelete.Enabled = false;
            }
        }

        private void listView2_SelectedIndexChanged(object sender, EventArgs e)
        {
            if (listView2.SelectedItems.Count > 0)
            {
                buttonFieldEdit.Enabled = true;
                if (listView2.SelectedItems[0].SubItems[1].Text != "KeePass username" 
                    && listView2.SelectedItems[0].SubItems[1].Text != "KeePass password")
                    buttonFieldDelete.Enabled = true;
            }
            else
            {
                buttonFieldEdit.Enabled = false;
                buttonFieldDelete.Enabled = false;
            }
        }

        private void listView1_MouseDoubleClick(object sender, MouseEventArgs e)
        {
            if (listView1.SelectedItems.Count > 0)
            {
                buttonURLEdit_Click(sender, e);
            }
        }

        private void listView2_MouseDoubleClick(object sender, MouseEventArgs e)
        {
            if (listView2.SelectedItems.Count > 0)
            {
                buttonFieldEdit_Click(sender, e);
            }
        }

        private void radioButton1_CheckedChanged(object sender, EventArgs e)
        {
            if (radioButton1.Checked)
            {
                _conf.SetMatchAccuracyMethod(MatchAccuracyMethod.Domain);
            }
            UpdateKPRPCJSON(_conf);
        }

        private void radioButton2_CheckedChanged(object sender, EventArgs e)
        {
            if (radioButton2.Checked)
            {
                _conf.SetMatchAccuracyMethod(MatchAccuracyMethod.Hostname);
            }
            UpdateKPRPCJSON(_conf);
        }

        private void radioButton3_CheckedChanged(object sender, EventArgs e)
        {
            if (radioButton3.Checked)
            {
                _conf.SetMatchAccuracyMethod(MatchAccuracyMethod.Exact);
            }
            UpdateKPRPCJSON(_conf);
        }
    }
}

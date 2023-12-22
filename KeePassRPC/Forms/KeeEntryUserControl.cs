using System;
using System.Collections.Generic;
using System.ComponentModel;
using System.Drawing;
using System.Data;
using System.Linq;
using System.Text;
using System.Windows.Forms;
using Jayrock.JsonRpc;
using KeePassLib;
using KeePassLib.Security;
using KeePass.UI;
using KeePass.Forms;
using KeePassLib.Collections;
using KeePassRPC.Models.DataExchange;
using KeePassRPC.Models.Persistent;
using KeePassRPC.Models.Shared;

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
        StringDictionaryEx _cd;
        EntryConfigv2 _conf;
        DatabaseConfig _dbConf;

        public KeeEntryUserControl(KeePassRPCExt keePassRPCPlugin, PwEntry entry,
            CustomListViewEx advancedListView, PwEntryForm pwEntryForm, ProtectedStringDictionary strings, StringDictionaryEx customData)
        {
            KeePassRPCPlugin = keePassRPCPlugin;
            _entry = entry;
            InitializeComponent();
            _pwEntryForm = pwEntryForm;
            _strings = strings;
            _cd = customData;
            _dbConf = KeePassRPCPlugin._host.Database.GetKPRPCConfig();
            _conf = entry.GetKPRPCConfigNormalised(strings, _dbConf.DefaultMatchAccuracy);
        }

        private void UpdateKPRPCJSON(EntryConfigv2 conf)
        {
            EntryConfigv2 defaultConf = (new EntryConfigv1(KeePassRPCPlugin._host.Database.GetKPRPCConfig().DefaultMatchAccuracy)).ConvertToV2(new GuidService());
            
            // if the config is identical to an empty (default) config, only update if a JSON string already exists
            if (!conf.Equals(defaultConf) || _cd.Exists("KPRPC JSON"))
            {
                _cd.Set("KPRPC JSON", Jayrock.Json.Conversion.JsonConvert.ExportToString(conf));
                if (_strings.Exists("KPRPC JSON"))
                {
                    _strings.Remove("KPRPC JSON");
                }
            }
        }
        
        private void checkBoxHideFromKee_CheckedChanged(object sender, EventArgs e)
        {
            if (checkBoxHideFromKee.Checked)
            {
                var list = _conf.MatcherConfigs.ToList();
                if (list.All(emc => emc.MatcherType != EntryMatcherType.Hide))
                {
                    list.Add(new EntryMatcherConfig() { MatcherType = EntryMatcherType.Hide });
                }
                _conf.MatcherConfigs = list.ToArray();
                groupBox1.Enabled = false;
                groupBox2.Enabled = false;
                groupBox3.Enabled = false;
                groupBox4.Enabled = false;
                labelRealm.Enabled = false;
                textBoxKeeRealm.Enabled = false;
            }
            else
            {
                var list = _conf.MatcherConfigs.ToList();
                list.RemoveAll(emc => emc.MatcherType == EntryMatcherType.Hide);
                _conf.MatcherConfigs = list.ToArray();
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
            if (_conf == null)
                return;

            this.checkBoxHideFromKee.CheckedChanged += new System.EventHandler(this.checkBoxHideFromKee_CheckedChanged);

            if (_conf.MatcherConfigs.Any(emc => emc.MatcherType == EntryMatcherType.Hide)) { checkBoxHideFromKee.Checked = true; }
            var urlMc = _conf.MatcherConfigs.First(emc => emc.MatcherType == EntryMatcherType.Url);
            if (urlMc.UrlMatchMethod == MatchAccuracyMethod.Exact)
            {
                radioButton3.Checked = true;
            }
            else if (urlMc.UrlMatchMethod == MatchAccuracyMethod.Hostname)
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
                
            listNormalURLs = new List<string>();
            listNormalBlockedURLs = new List<string>();
            listRegExURLs = new List<string>();
            listRegExBlockedURLs = new List<string>();

            if (_conf.AltUrls != null) listNormalURLs.AddRange(_conf.AltUrls);
            if (_conf.BlockedUrls != null) listNormalBlockedURLs.AddRange(_conf.BlockedUrls);
            if (_conf.RegExUrls != null) listRegExURLs.AddRange(_conf.RegExUrls);
            if (_conf.RegExBlockedUrls != null) listRegExBlockedURLs.AddRange(_conf.RegExBlockedUrls);

            textBoxKeeRealm.Text = _conf.HttpRealm;

            // Read the list of field objects and create ListViewItems for display to the user
            if (_conf.Fields != null)
            {
                foreach (Field field in _conf.Fields)
                {
                    string type = Utilities.FieldTypeToDisplay(field.Type, false);

                    // Override display of standard variables 
                    string value = field.Value;
                    if (field.Type == FieldType.Password)
                        value = "********";
                    if (field.ValuePath == PwDefs.UserNameField)
                    {
                        value = "KeePass username";
                    }
                    if (field.ValuePath == PwDefs.PasswordField)
                    {
                        value = "KeePass password";
                    }
                    if (field.Type == FieldType.Toggle)
                    {
                        value = field.Value == "KEEFOX_CHECKED_FLAG_TRUE" ? "Enabled" : "Disabled";
                    }
                    var customFmc = field.MatcherConfigs.FirstOrDefault(fmc => fmc.MatcherType.GetValueOrDefault(FieldMatcherType.Custom) == FieldMatcherType.Custom);
                    string id = "";
                    string name = "";
                    if (customFmc != null)
                    {
                        var cmId = customFmc.CustomMatcher.Ids.FirstOrDefault();
                        var cmName = customFmc.CustomMatcher.Names.FirstOrDefault();
                        if (cmId != null) id = cmId;
                        if (cmName != null) name = cmName;
                    }
                    ListViewItem lvi = new ListViewItem(new string[] { name, value, id, type, field.Page.ToString() });
                    lvi.Tag = field;
                    AddFieldListItem(lvi);
                }
            }

            ReadURLStrings();

            comboBoxAutoSubmit.Text = "Use Kee setting";
            comboBoxAutoFill.Text = "Use Kee setting";

            _currentAutomationBehaviour = _conf.Behaviour.GetValueOrDefault(EntryAutomationBehaviour.Default);
            changeBehaviourState(_currentAutomationBehaviour);

            this.comboBoxAutoSubmit.SelectedIndexChanged += new System.EventHandler(this.comboBoxAutoSubmit_SelectedIndexChanged);
            this.comboBoxAutoFill.SelectedIndexChanged += new System.EventHandler(this.comboBoxAutoFill_SelectedIndexChanged);            
            string realmTooltip = "Set this to the realm (what the \"site says\") in the HTTP authentication popup dialog box for a more accurate match";
            toolTipRealm.SetToolTip(this.textBoxKeeRealm, realmTooltip);
            toolTipRealm.SetToolTip(this.labelRealm, realmTooltip);
        }

        private void textBoxKeeRealm_TextChanged(object sender, EventArgs e)
        {
            string realm = ((System.Windows.Forms.TextBoxBase)sender).Text;

            if (!string.IsNullOrEmpty(realm))
                _conf.HttpRealm = realm;
            else
                _conf.HttpRealm = null;
            UpdateKPRPCJSON(_conf);
            return;
        }

        private void comboBoxAutoFill_SelectedIndexChanged(object sender, EventArgs e)
        {
            switch (comboBoxAutoFill.Text)
            {
                case "Use Kee setting":
                    if (comboBoxAutoSubmit.Text == "Never")
                        changeBehaviourState(EntryAutomationBehaviour.NeverAutoSubmit);
                    else
                        changeBehaviourState(EntryAutomationBehaviour.Default);
                    break;
                case "Never":
                    changeBehaviourState(EntryAutomationBehaviour.NeverAutoFillNeverAutoSubmit);
                    break;
                case "Always":
                    if (comboBoxAutoSubmit.Text == "Never")
                        changeBehaviourState(EntryAutomationBehaviour.AlwaysAutoFillNeverAutoSubmit);
                    else if (comboBoxAutoSubmit.Text == "Always")
                        changeBehaviourState(EntryAutomationBehaviour.AlwaysAutoFillAlwaysAutoSubmit);
                    else
                        changeBehaviourState(EntryAutomationBehaviour.AlwaysAutoFill);
                    break;
            }
        }

        private void comboBoxAutoSubmit_SelectedIndexChanged(object sender, EventArgs e)
        {
            switch (comboBoxAutoSubmit.Text)
            {
                case "Use Kee setting": 
                    if (comboBoxAutoFill.Text == "Always") 
                        changeBehaviourState(EntryAutomationBehaviour.AlwaysAutoFill); 
                    else
                        changeBehaviourState(EntryAutomationBehaviour.Default);
                    break;
                case "Never":
                    if (comboBoxAutoFill.Text == "Always")
                        changeBehaviourState(EntryAutomationBehaviour.AlwaysAutoFillNeverAutoSubmit);
                    else if (comboBoxAutoFill.Text == "Never")
                        changeBehaviourState(EntryAutomationBehaviour.NeverAutoFillNeverAutoSubmit);
                    else
                        changeBehaviourState(EntryAutomationBehaviour.NeverAutoSubmit);
                    break;
                case "Always":
                    changeBehaviourState(EntryAutomationBehaviour.AlwaysAutoFillAlwaysAutoSubmit);
                    break;
            }
        }

        EntryAutomationBehaviour _currentAutomationBehaviour = EntryAutomationBehaviour.Default;

        private void changeBehaviourState(EntryAutomationBehaviour behav)
        {
            switch (behav)
            {
                case EntryAutomationBehaviour.AlwaysAutoFill:
                    _conf.Behaviour = EntryAutomationBehaviour.AlwaysAutoFill;
                    comboBoxAutoFill.Text = "Always";
                    comboBoxAutoSubmit.Text = "Use Kee setting";
                    comboBoxAutoFill.Enabled = true;
                    comboBoxAutoSubmit.Enabled = true;
                    break;
                case EntryAutomationBehaviour.NeverAutoSubmit:
                    _conf.Behaviour = EntryAutomationBehaviour.NeverAutoSubmit;
                    comboBoxAutoFill.Text = "Use Kee setting";
                    comboBoxAutoSubmit.Text = "Never";
                    comboBoxAutoFill.Enabled = true;
                    comboBoxAutoSubmit.Enabled = true;
                    break;
                case EntryAutomationBehaviour.AlwaysAutoFillAlwaysAutoSubmit:
                    _conf.Behaviour = EntryAutomationBehaviour.AlwaysAutoFillAlwaysAutoSubmit;
                    comboBoxAutoSubmit.Text = "Always";
                    comboBoxAutoFill.Text = "Always";
                    comboBoxAutoFill.Enabled = false;
                    comboBoxAutoSubmit.Enabled = true;
                    break;
                case EntryAutomationBehaviour.NeverAutoFillNeverAutoSubmit:
                    _conf.Behaviour = EntryAutomationBehaviour.NeverAutoFillNeverAutoSubmit;
                    comboBoxAutoFill.Text = "Never";
                    comboBoxAutoSubmit.Text = "Never";
                    comboBoxAutoSubmit.Enabled = false;
                    comboBoxAutoFill.Enabled = true;
                    break;
                case EntryAutomationBehaviour.AlwaysAutoFillNeverAutoSubmit:
                    _conf.Behaviour = EntryAutomationBehaviour.AlwaysAutoFillNeverAutoSubmit;
                    comboBoxAutoFill.Text = "Always";
                    comboBoxAutoSubmit.Text = "Never";
                    comboBoxAutoSubmit.Enabled = true;
                    comboBoxAutoFill.Enabled = true;
                    break;
                case EntryAutomationBehaviour.Default:
                    _conf.Behaviour = null;
                    comboBoxAutoFill.Text = "Use Kee setting";
                    comboBoxAutoSubmit.Text = "Use Kee setting";
                    comboBoxAutoSubmit.Enabled = true;
                    comboBoxAutoFill.Enabled = true;
                    break;
            }
            UpdateKPRPCJSON(_conf);
            _currentAutomationBehaviour = behav;
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
            _conf.AltUrls = listNormalURLs.ToArray();
            _conf.BlockedUrls = listNormalBlockedURLs.ToArray();
            _conf.RegExUrls = listRegExURLs.ToArray();
            _conf.RegExBlockedUrls = listRegExBlockedURLs.ToArray();
            UpdateKPRPCJSON(_conf);
        }


        private void UpdateFields()
        {
            List<Field> fields = new List<Field>();
            foreach (ListViewItem lvi in listView2.Items)
            {
                if (lvi.Tag != null)
                    fields.Add((Field)lvi.Tag);
            }
            _conf.Fields = fields.ToArray();
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
            IGuidService guidService = new GuidService();
            using (KeeFieldForm kff = new KeeFieldForm(null, null, null, FieldType.Text, 1, PlaceholderHandling.Default, ".", false, null, null))
            {
                if (kff.ShowDialog() == DialogResult.OK)
                {
                    var mc = FieldMatcherConfig.ForSingleClientMatch(kff.Id, kff.Name, kff.HtmlType, kff.QuerySelector);
                    var field = new Field()
                    {
                        Name = kff.Name,
                        Page = Math.Max(kff.Page, 1),
                        ValuePath = ".",
                        Uuid = guidService.NewGuid(),
                        Type = kff.Type,
                        MatcherConfigs = new[] { mc },
                        Value = kff.Value
                    };
                    if (kff.PlaceholderHandling == PlaceholderHandling.Default)
                    {
                        field.PlaceholderHandling = null;
                    }
                    else
                    {
                        field.PlaceholderHandling = kff.PlaceholderHandling;
                    }

                    string type = Utilities.FieldTypeToDisplay(kff.Type, false);
                    int page = kff.Page;

                    // We know any new passwords are not the main Entry password
                    // Also know that the display name can be same as main name
                    string displayValue = kff.Value;
                    if (kff.Type == FieldType.Password)
                    {
                        displayValue = "********";
                    }
                    if (kff.Type == FieldType.Toggle)
                    {
                        displayValue = kff.Value == "KEEFOX_CHECKED_FLAG_TRUE" ? "Enabled" : "Disabled";
                    }

                    ListViewItem lvi = new ListViewItem(new string[]
                    {
                        kff.Name, displayValue, kff.Id, type, page.ToString()
                    });
                    lvi.Tag = field;
                    AddFieldListItem(lvi);
                    UpdateFields();
                }
            }
        }

        private void buttonFieldEdit_Click(object sender, EventArgs e)
        {
            ListView.SelectedListViewItemCollection lvsicSel = listView2.SelectedItems;

            Field tag = (Field)lvsicSel[0].Tag;
            using (KeeFieldForm kff = FormFieldForEditing(tag))
            {
                if (kff.ShowDialog() == DialogResult.OK)
                {
                    string displayValue = kff.Value;
                    if (kff.Type == FieldType.Password)
                    {
                        displayValue = "********";
                    }

                    string displayName = string.IsNullOrEmpty(kff.Name) ? null : kff.Name;
                    if (tag.ValuePath == PwDefs.PasswordField)
                    {
                        displayName = null;
                        displayValue = "KeePass password";
                    }
                    else if (tag.ValuePath == PwDefs.UserNameField)
                    {
                        displayName = null;
                        displayValue = "KeePass username";
                    }

                    if (kff.Type == FieldType.Toggle)
                    {
                        displayValue = kff.Value == "KEEFOX_CHECKED_FLAG_TRUE" ? "Enabled" : "Disabled";
                    }

                    string type = Utilities.FieldTypeToDisplay(kff.Type, false);
                    int page = kff.Page;
                    
                    var mc = FieldMatcherConfig.ForSingleClientMatch(kff.Id, kff.Name, kff.HtmlType, kff.QuerySelector);
                    var field = new Field()
                    {
                        Name = displayName,
                        Page = Math.Max(page, 1),
                        ValuePath = tag.ValuePath,
                        Uuid = tag.Uuid,
                        Type = kff.Type,
                        MatcherConfigs = new[] { mc },
                        Value = kff.Value
                    };
                    if (kff.PlaceholderHandling == PlaceholderHandling.Default)
                    {
                        field.PlaceholderHandling = null;
                    }
                    else
                    {
                        field.PlaceholderHandling = kff.PlaceholderHandling;
                    }

                    ListViewItem lvi = new ListViewItem(new string[] { kff.Name, displayValue, kff.Id, type, page.ToString() });
                    lvi.Tag = field;
                    RemoveFieldListItem(lvsicSel[0]);
                    AddFieldListItem(lvi);
                    UpdateFields();
                }
            }
        }

        private static KeeFieldForm FormFieldForEditing(Field tag)
        {
            if (tag == null) throw new Exception("Corrupt Entry found!");
            return KeeFieldForm.FromField(tag);
        }

        private void buttonFieldDelete_Click(object sender, EventArgs e)
        {
            ListView.SelectedListViewItemCollection lvsicSel = listView2.SelectedItems;
            // remove the old field data
            RemoveFieldListItem(lvsicSel[0]);
            UpdateFields();
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
                var mc = _conf.MatcherConfigs.First(emc => emc.MatcherType == EntryMatcherType.Url);
                mc.UrlMatchMethod = MatchAccuracyMethod.Domain;
            }
            UpdateKPRPCJSON(_conf);
        }

        private void radioButton2_CheckedChanged(object sender, EventArgs e)
        {
            if (radioButton2.Checked)
            {
                var mc = _conf.MatcherConfigs.First(emc => emc.MatcherType == EntryMatcherType.Url);
                mc.UrlMatchMethod = MatchAccuracyMethod.Hostname;
            }
            UpdateKPRPCJSON(_conf);
        }

        private void radioButton3_CheckedChanged(object sender, EventArgs e)
        {
            if (radioButton3.Checked)
            {
                var mc = _conf.MatcherConfigs.First(emc => emc.MatcherType == EntryMatcherType.Url);
                mc.UrlMatchMethod = MatchAccuracyMethod.Exact;
            }
            UpdateKPRPCJSON(_conf);
        }

        private void listView1_KeyUp(object sender, KeyEventArgs e)
        {
            if (e.KeyCode == Keys.Delete)
            {
                buttonURLDelete_Click(sender, e);
            }
        }

        private void listView2_KeyUp(object sender, KeyEventArgs e)
        {
            if (e.KeyCode == Keys.Delete && listView2.SelectedItems.Count > 0 && listView2.SelectedItems[0].SubItems[1].Text != "KeePass username"
                    && listView2.SelectedItems[0].SubItems[1].Text != "KeePass password")
            {
                buttonFieldDelete_Click(sender, e);
            }
        }
    }
}

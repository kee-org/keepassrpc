﻿using System;
using System.Collections.Generic;
using System.Text.RegularExpressions;
using System.Windows.Forms;
using KeePass.UI;
using KeePassRPC.Properties;

namespace KeePassRPC.Forms
{
    public partial class KeeURLForm : Form
    {
        public bool Match;
        public bool Block;
        public bool RegEx;
        public string URL;
        public List<string> OtherKeys;

        private bool _editing;
        protected override void OnLoad(EventArgs e)
        {
            GlobalWindowManager.AddWindow(this);
            base.OnLoad(e);
        }

        protected override void OnClosed(EventArgs e)
        {
            GlobalWindowManager.RemoveWindow(this);
            base.OnClosed(e);
        }

        public KeeURLForm(bool match, bool block, string regExURL, string url, List<string> otherKeys)
        {
            InitializeComponent();
            Icon = Resources.KPRPCico;
            Match = match;
            Block = block;
            OtherKeys = otherKeys;

            if (Block && Match)
                throw new ArgumentException("Can't block and match the same URL");

            if (!string.IsNullOrEmpty(regExURL))
            {
                RegEx = true;
                _editing = true;
                URL = regExURL;
            }
            else if (!string.IsNullOrEmpty(url))
            {
                _editing = true;
                URL = url;
            }

            textBox1.Text = URL;
            if (!Block && !Match)
                radioButtonMatch.Checked = true;
            else
            {
                radioButtonBlock.Checked = Block;
                radioButtonMatch.Checked = Match;
            }
            checkBoxRegEx.Checked = RegEx;

            if (_editing)
                Text = "Edit URL";
            else
                Text = "Add URL";
        }

        private void button2_Click(object sender, EventArgs e)
        {
            
        }

        private void buttonOK_Click(object sender, EventArgs e)
        {
            if (textBox1.Text.Length > 0)
                URL = textBox1.Text;
            else
                DialogResult = DialogResult.None;

            if (OtherKeys.Contains(URL))
            {
                MessageBox.Show(this, "A rule for '" + URL + "' has already been added.");
                DialogResult = DialogResult.None;
                return;
            }

            //TODO2: Check the URL follows an acceptable pattern

            if (!radioButtonBlock.Checked && !radioButtonMatch.Checked)
                DialogResult = DialogResult.None;

            RegEx = checkBoxRegEx.Checked;
            Block = radioButtonBlock.Checked;
            Match = radioButtonMatch.Checked;

            // Tell the user straight away if their Regex is bad
            if (RegEx)
            {
                try
                {
                    Regex test = new Regex(URL);
                }
                catch (ArgumentException ex)
                {
                    MessageBox.Show(this, "'" + URL + "' is not a valid regular expression. Details: " + ex.Message, "Invalid regular expression", MessageBoxButtons.OK, MessageBoxIcon.Warning);
                    DialogResult = DialogResult.None;
                }
            }
        }
    }
}

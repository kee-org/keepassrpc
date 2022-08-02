using System;
using System.Collections.Generic;
using System.ComponentModel;
using System.Data;
using System.Diagnostics;
using System.Drawing;
using System.Text;
using System.Windows.Forms;
using KeePassRPC.DataExchangeModel;

namespace KeePassRPC.Forms
{
    public partial class KeeFieldForm : Form
    {
        public new string Name;
        public string Value;
        public string Id;
        public FormFieldType Type;
        public int Page;
        public PlaceholderHandling PlaceholderHandling;

        public KeeFieldForm(FormField ff) : this(ff.Name, ff.Value, ff.Id, ff.Type, ff.Page, ff.PlaceholderHandling)
        {
        }

        public KeeFieldForm(string name, string value, string id, FormFieldType type, int page, PlaceholderHandling phh)
        {
            InitializeComponent();
            Icon = global::KeePassRPC.Properties.Resources.KPRPCico;
            if (string.IsNullOrEmpty(name))
                this.Text = "Add a form field";
            else
                this.Text = "Edit a form field";

            if (value == "{USERNAME}")
            {
                textBox2.Text = Value = value;
                comboBox1.Text = "Username";
                comboBox1.Enabled = false;
                label6.Visible = true;
            } else
            if (value == "{PASSWORD}")
            {
                textBox2.Text = Value = value;
                comboBox1.Text = "Password";
                comboBox1.Enabled = false;
                label7.Visible = true;
            }
            else
            {
                if (type == FormFieldType.FFTpassword)
                    comboBox1.Text = "Password";
                else if (type == FormFieldType.FFTselect)
                    comboBox1.Text = "Select";
                else if (type == FormFieldType.FFTradio)
                    comboBox1.Text = "Radio";
                else if (type == FormFieldType.FFTtext)
                    comboBox1.Text = "Text";
                else if (type == FormFieldType.FFTtel)
                    comboBox1.Text = "Tel";
                else if (type == FormFieldType.FFTusername)
                    comboBox1.Text = "Username";
                else if (type == FormFieldType.FFTcheckbox)
                    comboBox1.Text = "Checkbox";

                if (type == FormFieldType.FFTcheckbox)
                {
                    checkBox1.Visible = true;
                    Value = value;
                    checkBox1.Checked = Value == "KEEFOX_CHECKED_FLAG_TRUE" ? true : false;
                } else
                {
                    textBox2.Text = Value = value;
                    textBox2.Visible = true;
                    label2.Visible = true;
                }
            }
            textBox1.Text = Name = name;
            textBox3.Text = Id = id;
            Page = page;
            textBox4.Text = Page.ToString();

            switch (phh)
            {
                case PlaceholderHandling.Default: radioButton1.Checked = true; break;
                case PlaceholderHandling.Enabled: radioButton2.Checked = true; break;
                case PlaceholderHandling.Disabled: radioButton3.Checked = true; break;
            }

            comboBox1.SelectedIndexChanged += new System.EventHandler(comboBox1_SelectedIndexChanged);
        }

        private void button1_Click(object sender, EventArgs e)
        {
            if (textBox2.Visible && textBox2.Text.Length <= 0)
            {
                MessageBox.Show(this, "Please specify a value");
                this.DialogResult = DialogResult.None;
                return;
            }

            if (textBox2.Visible && (textBox2.Text == "{USERNAME}" || textBox2.Text == "{PASSWORD}"))
            {
                MessageBox.Show(this, "Please change the value of this form field - it is currently set to a value that Kee needs to reserve for internal use. Sorry, please report this on the support forums if you are inconvenienced by this choice of reserved phrase.");
                this.DialogResult = DialogResult.None;
                return;
            }

            Name = textBox1.Text;
            Id = textBox3.Text;
            if (!int.TryParse(textBox4.Text, out Page)) Page = 1;
            if (comboBox1.Text == "Password")
                Type = FormFieldType.FFTpassword;
            else if (comboBox1.Text == "Select")
                Type = FormFieldType.FFTselect;
            else if (comboBox1.Text == "Radio")
                Type = FormFieldType.FFTradio;
            else if (comboBox1.Text == "Text")
                Type = FormFieldType.FFTtext;
            else if (comboBox1.Text == "Tel")
                Type = FormFieldType.FFTtel;
            else if (comboBox1.Text == "Username")
                Type = FormFieldType.FFTusername;
            else if (comboBox1.Text == "Checkbox")
                Type = FormFieldType.FFTcheckbox;

            if (comboBox1.Text == "Checkbox")
            {
                Value = checkBox1.Checked ? "KEEFOX_CHECKED_FLAG_TRUE" : "KEEFOX_CHECKED_FLAG_FALSE";
            } else
            {
                Value = textBox2.Text;
            }

            if (radioButton1.Checked) PlaceholderHandling = PlaceholderHandling.Default;
            if (radioButton2.Checked) PlaceholderHandling = PlaceholderHandling.Enabled;
            if (radioButton3.Checked) PlaceholderHandling = PlaceholderHandling.Disabled;
        }
        
        private void comboBox1_SelectedIndexChanged(object sender, EventArgs e)
        {
            if (comboBox1.Text == "Checkbox")
            {
                textBox2.Visible = false;
                label2.Visible = false;
                checkBox1.Visible = true;
                checkBox1.Checked = false;
            }
            else
            {
                textBox2.Visible = true;
                label2.Visible = true;
                checkBox1.Visible = false;
                checkBox1.Checked = false;
            }
        }

        private void linkLabel1_LinkClicked(object sender, LinkLabelLinkClickedEventArgs e)
        {
            Process p = new Process();
            p = Process.Start("https://forum.kee.pm/t/placeholder-handling/1100");
        }
    }
}

using System;
using System.Collections.Generic;
using System.ComponentModel;
using System.Data;
using System.Drawing;
using System.Text;
using System.Windows.Forms;
using KeePassRPC.DataExchangeModel;

namespace KeePassRPC.Forms
{
    public partial class KeeFoxFieldForm : Form
    {
        public new string Name;
        public string Value;
        public string Id;
        public FormFieldType Type;
        public int Page;

        public KeeFoxFieldForm(FormField ff) : this(ff.Name, ff.Value, ff.Id, ff.Type, ff.Page)
        {
        }

        public KeeFoxFieldForm(string name, string value, string id, FormFieldType type, int page)
        {
            InitializeComponent();
            Icon = global::KeePassRPC.Properties.Resources.keefox;
            if (string.IsNullOrEmpty(name))
                this.Text = "Add a form field";
            else
                this.Text = "Edit a form field";

            if (value == "{USERNAME}")
            {
                textBox2.Enabled = false;
                comboBox1.Text = "Username";
                comboBox1.Enabled = false;
                label6.Visible = true;
            } else
            if (value == "{PASSWORD}")
            {
                textBox2.Enabled = false;
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
                else if (type == FormFieldType.FFTusername)
                    comboBox1.Text = "Username";
                else if (type == FormFieldType.FFTcheckbox)
                    comboBox1.Text = "Checkbox";                    
            }
            textBox1.Text = Name = name;
            textBox2.Text = Value = value;
            textBox3.Text = Id = id;
            Page = page;
            textBox4.Text = Page.ToString();
        }

        private void button1_Click(object sender, EventArgs e)
        {
            if (textBox2.Text.Length <= 0)
            {
                MessageBox.Show(this, "Please specify a value");
                this.DialogResult = DialogResult.None;
                return;
            }

            if (textBox2.Enabled && (textBox2.Text == "{USERNAME}" || textBox2.Text == "{PASSWORD}"))
            {
                MessageBox.Show(this, "Please change the value of this form field - it is currently set to a value that KeeFox needs to reserve for internal use. Sorry, please report this on the support forums if you are inconvienced by this choice of reserved phrase.");
                this.DialogResult = DialogResult.None;
                return;
            }

            Name = textBox1.Text;
            Value = textBox2.Text;
            Id = textBox3.Text;
            Page = int.Parse(textBox4.Text);
            if (comboBox1.Text == "Password")
                Type = FormFieldType.FFTpassword;
            else if (comboBox1.Text == "Select")
                Type = FormFieldType.FFTselect;
            else if (comboBox1.Text == "Radio")
                Type = FormFieldType.FFTradio;
            else if (comboBox1.Text == "Text")
                Type = FormFieldType.FFTtext;
            else if (comboBox1.Text == "Username")
                Type = FormFieldType.FFTusername;
            else if (comboBox1.Text == "Checkbox")
                Type = FormFieldType.FFTcheckbox;
        }
    }
}

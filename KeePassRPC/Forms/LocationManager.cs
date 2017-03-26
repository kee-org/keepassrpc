using System;
using System.Collections.Generic;
using System.ComponentModel;
using System.Data;
using System.Drawing;
using System.Text;
using System.Windows.Forms;
using KeePass.Plugins;

namespace KeePassRPC.Forms
{
    public partial class LocationManager : Form
    {
        KeePassRPCExt _plugin;

        public LocationManager(KeePassRPCExt plugin)
        {
            _plugin= plugin;
            InitializeComponent();
            button1.Enabled = false;
            button2.Enabled = false;
            Icon = global::KeePassRPC.Properties.Resources.keefox;
        }

        private void button3_Click(object sender, EventArgs e)
        {
            // save changes
            string locationsConfig = "";

            foreach (string location in listBox1.Items)
            {
                locationsConfig += location + ",";
            }
            locationsConfig = locationsConfig.Length > 0 ? locationsConfig.Substring(0, locationsConfig.Length - 1) : locationsConfig;
            _plugin._host.CustomConfig.SetString("KeePassRPC.knownLocations", locationsConfig);
            _plugin._host.MainWindow.Invoke((MethodInvoker)delegate { _plugin._host.MainWindow.SaveConfig(); });

            //string rootGroupsConfig = host.CustomConfig
            //        .GetString("KeePassRPC.knownLocations." + location + ".RootGroups", "");

            //TODO2: remove RootGroups that no longer have an associated location... how? can't interate through config entries!
        }

        private void button2_Click(object sender, EventArgs e)
        {
            // add location to list box
            listBox1.Items.Add(textBox1.Text);
            textBox1.Clear();
        }

        private void button1_Click(object sender, EventArgs e)
        {
            // remove location from list box
            if (listBox1.SelectedIndex >= 0)
                listBox1.Items.RemoveAt(listBox1.SelectedIndex);
        }

        private void listBox1_SelectedIndexChanged(object sender, EventArgs e)
        {
            if (listBox1.SelectedIndex >= 0)
                button1.Enabled = true;
            else
                button1.Enabled = false;
        }

        private void textBox1_TextChanged(object sender, EventArgs e)
        {
            if (textBox1.Text.Length > 0)
                button2.Enabled = true;
            else
                button2.Enabled = false;
        }

        private void textBox1_KeyPress(object sender, KeyPressEventArgs e)
        {
            if (e.KeyChar == '\r')
            {
                listBox1.Items.Add(textBox1.Text);
                textBox1.Clear();
            }
        }

        private void LocationManager_Load(object sender, EventArgs e)
        {
            listBox1.Items.Clear();
            string klconf = _plugin._host.CustomConfig.GetString("KeePassRPC.knownLocations");
            if (!string.IsNullOrEmpty(klconf))
            {
                string[] knownLocations = klconf.Split(new char[] { ',' });
                foreach (string location in knownLocations)
                {
                    listBox1.Items.Add(location);
                }
            }
        }
    }
}

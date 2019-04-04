using System;
using System.Collections.Generic;
using System.ComponentModel;
using System.Drawing;
using System.Data;
using System.Text;
using System.Windows.Forms;
using KeePassLib;

namespace KeePassRPC.Forms
{
    public partial class KeeGroupUserControl : UserControl
    {
        KeePassRPCExt KeePassRPCPlugin;

        PwGroup _group;

        KeeHomeStatus _status = KeeHomeStatus.Unknown;

        string _location = "";

        KeeHomeStatus Status { get {
            if (_status == KeeHomeStatus.Unknown)
            {
                PwGroup _rootGroup = KeePassRPCPlugin.RPCService.GetRootPwGroup(KeePassRPCPlugin._host.Database, _location);
                var rid = KeePassRPCPlugin._host.Database.RecycleBinUuid;

                if (_rootGroup.Uuid.Equals(_group.Uuid))
                    _status = KeeHomeStatus.Home;
                else if (rid != null && rid != PwUuid.Zero && _group.IsOrIsContainedIn(KeePassRPCPlugin._host.Database.RootGroup.FindGroup(rid, true)))
                        _status = KeeHomeStatus.Rubbish;
                else if (_group.IsContainedIn(_rootGroup)) // returns true when _group is main root and custom root group has been selected.
                    _status = KeeHomeStatus.Inside;
                else
                    _status = KeeHomeStatus.Outside;

            }
            return _status;
        
        } }

        public KeeGroupUserControl(KeePassRPCExt keePassRPCPlugin, PwGroup group)
        {
            KeePassRPCPlugin = keePassRPCPlugin;
            _group = group;
            InitializeComponent();
        }

        private void KeeGroupUserControl_Load(object sender, EventArgs e)
        {
            UpdateStatus();
            
            l_homeExplanation.Text = @"Kee will only know about the groups
and entries that are inside your Home group";

            UpdateLocations();
        }

        private void UpdateLocations()
        {
            comboBoxLocation.Items.Clear();
            comboBoxLocation.Items.Add("Anywhere");
            string klconf = KeePassRPCPlugin._host.CustomConfig.GetString("KeePassRPC.knownLocations");
            if (!string.IsNullOrEmpty(klconf))
            {
                string[] knownLocations = klconf.Split(new char[] { ',' });
                foreach (string location in knownLocations)
                {
                    comboBoxLocation.Items.Add(location);
                }
            }
            comboBoxLocation.Items.Add("Location manager...");
            comboBoxLocation.SelectedIndex = 0;
        }

        private void UpdateStatus()
        {
            switch (Status)
            {
                case KeeHomeStatus.Home:
                    l_status.Text = @"This is the Kee Home group. Kee can see and work with
this group and all groups and entries that are contained within.";
                    buttonMakeHome.Enabled = false;
                    break;
                case KeeHomeStatus.Inside:
                    l_status.Text = @"Kee can see and work with this group.";
                    buttonMakeHome.Enabled = true;
                    break;
                case KeeHomeStatus.Outside:
                    l_status.Text = @"This group is hidden from Kee. You must change your Home
group if you want Kee to work with this group.";
                    buttonMakeHome.Enabled = true;
                    break;
                case KeeHomeStatus.Rubbish:
                    l_status.Text = @"This group is hidden from Kee. You must remove it from
the recycle bin if you want Kee to work with this group.";
                    buttonMakeHome.Enabled = false;
                    break;
            }
        }

        private void buttonMakeHome_Click(object sender, EventArgs e)
        {
            if (string.IsNullOrEmpty(_location))
            {
                var conf = KeePassRPCPlugin._host.Database.GetKPRPCConfig();
                conf.RootUUID = KeePassLib.Utility.MemUtil.ByteArrayToHexString(_group.Uuid.UuidBytes);
                KeePassRPCPlugin._host.Database.SetKPRPCConfig(conf);
            }
            else
            {
                // set the root group for a particular location
                string currentLocationRoots = KeePassRPCPlugin._host.CustomConfig.GetString("KeePassRPC.knownLocations." + _location + ".RootGroups","");

                currentLocationRoots = CleanUpLocationRootGroups(currentLocationRoots);
                
                if (currentLocationRoots.Length > 0)
                    currentLocationRoots += ",";
                currentLocationRoots += KeePassLib.Utility.MemUtil.ByteArrayToHexString(_group.Uuid.UuidBytes);

                KeePassRPCPlugin._host.CustomConfig.SetString("KeePassRPC.knownLocations." + _location + ".RootGroups",
                    currentLocationRoots);
                KeePassRPCPlugin._host.MainWindow.Invoke((MethodInvoker)delegate { KeePassRPCPlugin._host.MainWindow.SaveConfig(); });
            }
            
            _status = KeeHomeStatus.Unknown;
            UpdateStatus();
            KeePassRPCPlugin._host.MainWindow.UpdateUI(false, null, true, null, true, null, true);
        }

        private string CleanUpLocationRootGroups(string currentLocationRoots)
        {
            string[] guids = currentLocationRoots.Split(',');
            string newLocationRoots = "";
            foreach (string guid in guids)
            {
                if (guid.Length <= 0)
                    continue;

                PwUuid uuid = new PwUuid(KeePassLib.Utility.MemUtil.HexStringToByteArray(guid)); ;

                if (KeePassRPCPlugin._host.Database.RootGroup.Uuid == uuid)
                    continue;

                PwGroup group = KeePassRPCPlugin._host.Database.RootGroup.FindGroup(uuid, true);
                
                // only keep this group UUID if it might be from a different database
                if (group == null)
                    newLocationRoots += guid + ",";
            }
            if (newLocationRoots.Length > 0)
                newLocationRoots = newLocationRoots.Substring(0, newLocationRoots.Length - 1);

            return newLocationRoots;
        }

        private void comboBoxLocation_SelectedIndexChanged(object sender, EventArgs e)
        {
            string selected = (string)comboBoxLocation.SelectedItem;

            if (selected == "Location manager...")
            {
                // launch location manager
                using (LocationManager lm = new LocationManager(KeePassRPCPlugin))
                {
                    if (lm.ShowDialog() == DialogResult.OK)
                        UpdateLocations();
                }
            }
            else if (selected == "Anywhere")
            {
                // use default home group
                _location = "";
                _status = KeeHomeStatus.Unknown;
                UpdateStatus();
            }
            else
            {
                _location = selected;
                _status = KeeHomeStatus.Unknown;
                UpdateStatus();
            }
        }

    }

    enum KeeHomeStatus
    {
        Unknown,
        Rubbish,
        Home,
        Inside,
        Outside
    }
}

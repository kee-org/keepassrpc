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
    public partial class KeeFoxGroupUserControl : UserControl
    {
        KeePassRPCExt KeePassRPCPlugin;

        PwGroup _group;

        KeeFoxHomeStatus _status = KeeFoxHomeStatus.Unknown;

        string _location = "";

        KeeFoxHomeStatus Status { get {
            if (_status == KeeFoxHomeStatus.Unknown)
            {
                PwGroup _rootGroup = KeePassRPCPlugin.RPCService.GetRootPwGroup(KeePassRPCPlugin._host.Database, _location);

                if (_rootGroup.Uuid.EqualsValue(_group.Uuid))
                    _status = KeeFoxHomeStatus.Home;
                else if (KeePassRPCPlugin._host.Database.RecycleBinUuid.EqualsValue(_group.Uuid))
                    _status = KeeFoxHomeStatus.Rubbish;
                else if (_group.IsContainedIn(_rootGroup)) // returns true when _group is main root and custom root group has been selected.
                    _status = KeeFoxHomeStatus.Inside;
                else
                    _status = KeeFoxHomeStatus.Outside;

            }
            return _status;
        
        } }

        public KeeFoxGroupUserControl(KeePassRPCExt keePassRPCPlugin, PwGroup group)
        {
            KeePassRPCPlugin = keePassRPCPlugin;
            _group = group;
            InitializeComponent();
        }

        private void KeeFoxGroupUserControl_Load(object sender, EventArgs e)
        {
            UpdateStatus();
            
            l_homeExplanation.Text = @"KeeFox will only know about the groups
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
                case KeeFoxHomeStatus.Home:
                    l_status.Text = @"This is the KeeFox Home group. KeeFox can see and work with
this group and all groups and entries that are contained within.";
                    buttonMakeHome.Enabled = false;
                    break;
                case KeeFoxHomeStatus.Inside:
                    l_status.Text = @"KeeFox can see and work with this group.";
                    buttonMakeHome.Enabled = true;
                    break;
                case KeeFoxHomeStatus.Outside:
                    l_status.Text = @"This group is hidden from KeeFox. You must change your Home
group if you want KeeFox to work with this group.";
                    buttonMakeHome.Enabled = true;
                    break;
                case KeeFoxHomeStatus.Rubbish:
                    l_status.Text = @"This group is hidden from KeeFox. You must remove it from
the recycle bin if you want KeeFox to work with this group.";
                    buttonMakeHome.Enabled = false;
                    break;
            }
        }

        private void buttonMakeHome_Click(object sender, EventArgs e)
        {
            if (string.IsNullOrEmpty(_location))
            {
                KeePassRPCPlugin._host.Database.CustomData.Set("KeePassRPC.KeeFox.rootUUID",
                    KeePassLib.Utility.MemUtil.ByteArrayToHexString(_group.Uuid.UuidBytes));
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
            
            _status = KeeFoxHomeStatus.Unknown;
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
                LocationManager lm = new LocationManager(KeePassRPCPlugin);

                if (lm.ShowDialog() == DialogResult.OK)
                    UpdateLocations();
            }
            else if (selected == "Anywhere")
            {
                // use default home group
                _location = "";
                _status = KeeFoxHomeStatus.Unknown;
                UpdateStatus();
            }
            else
            {
                // use custom home group for this location if it's been set
                //string klrgs = KeePassRPCPlugin._host.CustomConfig.GetString("KeePassRPC.knownLocations." + selected + ".RootGroups","");
                //if (!string.IsNullOrEmpty(klrgs))
                //{
                //    string[] rootGroups = new string[0];

                //    rootGroups = klrgs.Split(',');
                //foreach (string rootGroupId in rootGroups)
                //{
                //    PwUuid pwuuid = new PwUuid(KeePassLib.Utility.MemUtil.HexStringToByteArray(rootGroupId));
                //    PwGroup matchedGroup = KeePassRPCPlugin._host.Database.RootGroup.Uuid == pwuuid ? KeePassRPCPlugin._host.Database.RootGroup : KeePassRPCPlugin._host.Database.RootGroup.FindGroup(pwuuid, true);

                //    if (matchedGroup == null)
                //        continue;

                _location = selected;

                    // update our idea of what the root group is so we can then refresh the dialog details and update the required behaviour if the user clicks the "make root" button
                    _status = KeeFoxHomeStatus.Unknown;
                    UpdateStatus();
                    //host.Database.CustomData.Set("KeePassRPC.KeeFox.rootUUID", rootGroupId);
                    //host.MainWindow.UpdateUI(false, null, true, null, true, null, true);
                    //break;
               // }
           // }
            }
        }

    }

    enum KeeFoxHomeStatus
    {
        Unknown,
        Rubbish,
        Home,
        Inside,
        Outside
    }
}

using System;
using System.Collections.Generic;
using System.Collections;
using System.ComponentModel;
using System.Drawing;
using System.Windows.Forms;
using System.Diagnostics;
using System.Threading;
using System.IO;

using KeePass.Plugins;
using KeePass.Forms;
using KeePass.Resources;

using KeePassLib;
using KeePassLib.Security;
using KeePass.App;
using KeePass.UI;
using KeePassRPC.Forms;
using System.Reflection;
using KeePassLib.Collections;
using KeePassRPC.DataExchangeModel;
using Fleck2.Interfaces;
using DomainPublicSuffix;
using KeePassLib.Utility;
using KeePass.Util.Spr;

namespace KeePassRPC
{
    /// <summary>
    /// The main class - starts the RPC service and server
    /// </summary>
    public sealed class KeePassRPCExt : Plugin
    {
        // version information
        public static readonly Version PluginVersion = new Version(1, 10, 0);

        public override string UpdateUrl
        {
            get
            {
                return "https://raw.github.com/kee-org/keepassrpc/master/versionInfo.txt";
            }
        }

        private BackgroundWorker _BackgroundWorker; // used to invoke main thread from other threads
        private AutoResetEvent _BackgroundWorkerAutoResetEvent;
        private KeePassRPCServer _RPCServer;
        private KeePassRPCService _RPCService;

        public TextWriter logger;

        /// <summary>
        /// Listens for requests from RPC clients such as Kee
        /// </summary>
        public KeePassRPCServer RPCServer
        {
            get { return _RPCServer; }
        }

        /// <summary>
        /// Provides an externally accessible API for common KeePass operations
        /// </summary>
        public KeePassRPCService RPCService
        {
            get { return _RPCService; }
        }

        internal IPluginHost _host;

        private ToolStripMenuItem _keePassRPCOptions = null;
        private ToolStripSeparator _tsSeparator1 = null;
        private ToolStripMenuItem _keeRootMenu = null;

        private EventHandler<GwmWindowEventArgs> GwmWindowAddedHandler;

        private static LockManager _lockRPCClientManagers = new LockManager();
        private Dictionary<string, KeePassRPCClientManager> _RPCClientManagers = new Dictionary<string, KeePassRPCClientManager>(3);
        public volatile bool terminating = false;

        private int FindKeePassRPCPort(IPluginHost host)
        {
            bool allowCommandLineOverride = host.CustomConfig.GetBool("KeePassRPC.connection.allowCommandLineOverride", true);
            int port = (int)host.CustomConfig.GetULong("KeePassRPC.webSocket.port", 12546);

            if (allowCommandLineOverride)
            {
                string portStr = host.CommandLineArgs["KeePassRPCWebSocketPort"];
                if (portStr != null)
                {
                    try
                    {
                        port = int.Parse(portStr);
                    }
                    catch
                    {
                        // just stick with what we had already decided
                    }

                }
            }

            if (port <= 0 || port > 65535)
                port = 12546;

            return port;
        }

        /// <summary>
        /// The <c>Initialize</c> function is called by KeePass when
        /// you should initialize your plugin (create menu items, etc.).
        /// </summary>
        /// <param name="host">Plugin host interface. By using this
        /// interface, you can access the KeePass main window and the
        /// currently opened database.</param>
        /// <returns>true if channel registered correctly, otherwise false</returns>
        public override bool Initialize(IPluginHost host)
        {
            try
            {

                Debug.Assert(host != null);
                if (host == null)
                    return false;
                _host = host;

                // Reduce Fleck library logging verbosity
                Fleck2.FleckLog.Level = Fleck2.LogLevel.Error;

                // Enable update checks
                KeePass.Util.UpdateCheckEx.SetFileSigKey(UpdateUrl, "<RSAKeyValue><Modulus>t2jki5ttRkT7D110Q5n/ZdgFZ7JGdlRDme0NvcG1Uz7CnGF40NOqWtuzW4a9p0xUN05I5JegaJ20Nu6ejuxMfOhn0jUALHYe6F2wn4yGbPHJvXLXYyc3fU7W75eWJwQabup2vKhrAjvPMSQfy05JgPcfDmLk0txuKkrPO0u3d9ZzZsYrW+GLyJAQAT9Lt87A04iQsPxB30gXv4JX7iOqtKVsWfKEzanX/zuA5XB8JEd2I7bh2u0AgUA2rnwjSkI01tb6BheruwWm5GLZhe+k/wQkgiTxLAi/HNX9BjebWvVgd7B2OpDWAq4QFLrdSlBqT8d+V1IuJeztcjKhe5lHxHDiE6/5ajmBs4/c0EmKN7bXC+fF7xbVLa+aiKQCW7rzldXx0aqP/6/+VYAXrre55nIWzuArciLT43G1DzDRTyWz+KtNm9CYd07bn1QA9a3bvQxpuM3KSo2fyfBQTcxapBNDoMnM4gKUNd3rTdDmC0j2bHN9Ikyef9ohWzgIkmLleh8Ey1TpGbWS3Y2B3AD2bmqxWgzUBUMkenmp1GglHtc448BuusPPAcibIntZMQqmaHoJ1zeNJQKGNUKCJFjbe/aeHBm/jJ7izPfR8W27D+NMhdvFOZjprmh1AVa97yQ5Zqbh1zH/gsL0XCEuNOobVaVjAsOBhXMiFnl4U4sjknE=</Modulus><Exponent>AQAB</Exponent></RSAKeyValue>");

                _BackgroundWorker = new BackgroundWorker();
                _BackgroundWorker.WorkerReportsProgress = true;
                _BackgroundWorker.ProgressChanged += _BackgroundWorker_ProgressChanged;
                _BackgroundWorkerAutoResetEvent = new AutoResetEvent(false);
                _BackgroundWorker.DoWork += delegate (object sender, DoWorkEventArgs e)
                {
                    _BackgroundWorkerAutoResetEvent.WaitOne();
                };
                _BackgroundWorker.RunWorkerAsync();

                string debugFileName = host.CommandLineArgs["KPRPCDebug"];
                if (debugFileName != null)
                {
                    try
                    {
                        logger = new StreamWriter(debugFileName);
                        ((StreamWriter)logger).AutoFlush = true;
                    }
                    catch (Exception ex)
                    {
                        MessageBox.Show("KeePassRPC debug logger failed to initialise. No logging will be performed until KeePass is restarted with a valid debug log file location. Reason: " + ex.ToString());
                    }
                }
                if (logger != null) logger.WriteLine("Logger initialised.");

                TLDRulesCache.Init(host.CustomConfig.GetString(
                    "KeePassRPC.publicSuffixDomainCache.path",
                    GetLocalConfigLocation() + "publicSuffixDomainCache.txt"));

                // The Kee client manager holds objects relating to the web socket connections managed by the Fleck2 library
                CreateClientManagers();

                if (logger != null) logger.WriteLine("Client managers started.");
                //TODO2: set up language services

                _RPCService = new KeePassRPCService(host,
                    getStandardIconsBase64(host.MainWindow.ClientIcons), this);
                if (logger != null) logger.WriteLine("RPC service started.");
                int portNew = FindKeePassRPCPort(host);

                try
                {
                    _RPCServer = new KeePassRPCServer(RPCService, this, portNew,
                        host.CustomConfig.GetBool("KeePassRPC.webSocket.bindOnlyToLoopback", true));
                }
                catch (System.Net.Sockets.SocketException ex)
                {
                    if (ex.SocketErrorCode == System.Net.Sockets.SocketError.AddressAlreadyInUse)
                    {
                        MessageBox.Show(@"KeePassRPC is already listening for connections. To allow KeePassRPC clients (e.g. Kee in your web browser) to connect to this instance of KeePass, please close all other running instances of KeePass and restart this KeePass. If you want multiple instances of KeePass to be running at the same time, you'll need to configure some of them to connect using a different communication port.

See https://forum.kee.pm/t/connection-security-levels/1075

KeePassRPC requires this port to be available: " + portNew + ". Technical detail: " + ex.ToString());
                        if (logger != null) logger.WriteLine("Socket (port) already in use. KeePassRPC requires this port to be available: " + portNew + ". Technical detail: " + ex.ToString());
                    }
                    else
                    {
                        MessageBox.Show(@"KeePassRPC could not start listening for connections. To allow KeePassRPC clients (e.g. Kee in your web browser) to connect to this instance of KeePass, please fix the problem indicated in the technical detail below and restart KeePass.

KeePassRPC requires this port to be available: " + portNew + ". Technical detail: " + ex.ToString());
                        if (logger != null) logger.WriteLine("Socket error. KeePassRPC requires this port to be available: " + portNew + ". Maybe check that you have no firewall or other third party security software interfering with your system. Technical detail: " + ex.ToString());
                    }
                    if (logger != null) logger.WriteLine("KPRPC startup failed: " + ex.ToString());
                    _BackgroundWorkerAutoResetEvent.Set(); // terminate _BackgroundWorker
                    return false;
                }
                if (logger != null) logger.WriteLine("RPC server started.");

                // register to recieve events that we need to deal with

                _host.MainWindow.FileOpened += OnKPDBOpen;
                _host.MainWindow.FileClosed += OnKPDBClose;
                _host.MainWindow.FileCreated += OnKPDBCreated;
                _host.MainWindow.FileSaving += OnKPDBSaving;
                _host.MainWindow.FileSaved += OnKPDBSaved;

                _host.MainWindow.DocumentManager.ActiveDocumentSelected += OnKPDBSelected;

                // Get a reference to the 'Tools' menu item container
                ToolStripItemCollection tsMenu = _host.MainWindow.ToolsMenu.DropDownItems;

                // Add menu item for options
                _keePassRPCOptions = new DPIScaledToolStripMenuItem("KeePassRPC (Kee) Options...");
                _keePassRPCOptions.Click += OnToolsOptions;
                tsMenu.Add(_keePassRPCOptions);

                // Add a seperator and menu item to the group context menu
                ContextMenuStrip gcm = host.MainWindow.GroupContextMenu;
                _tsSeparator1 = new ToolStripSeparator();
                gcm.Items.Add(_tsSeparator1);
                _keeRootMenu = new DPIScaledToolStripMenuItem("Set as Kee home group");
                _keeRootMenu.Click += OnMenuSetRootGroup;
                gcm.Items.Add(_keeRootMenu);

                // not acting on upgrade info just yet but we need to track it for future proofing
                bool upgrading = refreshVersionInfo(host);

                // for debug only:
                //WelcomeForm wf = new WelcomeForm();
                //DialogResult dr = wf.ShowDialog();
                //if (dr == DialogResult.Yes)
                //    CreateNewDatabase();

                GwmWindowAddedHandler = new EventHandler<GwmWindowEventArgs>(GlobalWindowManager_WindowAdded);
                GlobalWindowManager.WindowAdded += GwmWindowAddedHandler;
            }
            catch (Exception ex)
            {
                if (logger != null) logger.WriteLine("KPRPC startup failed: " + ex.ToString());
                _BackgroundWorkerAutoResetEvent.Set(); // terminate _BackgroundWorker
                return false;
            }
            if (logger != null) logger.WriteLine("KPRPC startup succeeded.");
            return true; // Initialization successful
        }

        string GetLocalConfigLocation()
        {
            string strBaseDirName = PwDefs.ShortProductName;
            if (!string.IsNullOrEmpty(KeePass.App.Configuration.AppConfigSerializer.BaseName))
                strBaseDirName = KeePass.App.Configuration.AppConfigSerializer.BaseName;

            string strUserDir;
            try
            {
                strUserDir = Environment.GetFolderPath(
                    Environment.SpecialFolder.ApplicationData);
            }
            catch (Exception)
            {
                strUserDir = UrlUtil.GetFileDirectory(UrlUtil.FileUrlToPath(
                    Assembly.GetExecutingAssembly().GetName().CodeBase), true, false);
            }
            strUserDir = UrlUtil.EnsureTerminatingSeparator(strUserDir, false);

            return strUserDir + strBaseDirName + Path.DirectorySeparatorChar;
        }

        void GlobalWindowManager_WindowAdded(object sender, GwmWindowEventArgs e)
        {
            var ef = e.Form as PwEntryForm;
            if (ef != null)
            {
                ef.Shown += new EventHandler(editEntryFormShown);
                return;
            }

            var gf = e.Form as GroupForm;
            if (gf != null)
            {
                gf.Shown += new EventHandler(editGroupFormShown);
                return;
            }

            var dsf = e.Form as DatabaseSettingsForm;
            if (dsf != null)
            {
                dsf.Shown += new EventHandler(databaseSettingsFormShown);
            }
        }

        void databaseSettingsFormShown(object sender, EventArgs e)
        {
            TabControl mainTabControl = null;
            var dsf = sender as DatabaseSettingsForm;

            //This might not work, but might as well use the feature if possible.
            try
            {
                Control[] cs = dsf.Controls.Find("m_tabMain", true);
                if (cs.Length == 0)
                    return;
                mainTabControl = cs[0] as TabControl;
            }
            catch
            {
                // that's life, just move on.
                return;
            }

            if (mainTabControl == null) return;

            var dbSettingsUserControl = new DatabaseSettingsUserControl(_host.MainWindow.ActiveDatabase);

            TabPage keeTabPage = new TabPage("Kee");
            dbSettingsUserControl.Dock = DockStyle.Fill;
            keeTabPage.Controls.Add(dbSettingsUserControl);
            if (mainTabControl.ImageList == null)
                mainTabControl.ImageList = new ImageList();
            int imageIndex = mainTabControl.ImageList.Images.Add(global::KeePassRPC.Properties.Resources.KPRPC64, Color.Transparent);
            keeTabPage.ImageIndex = imageIndex;
            mainTabControl.TabPages.Add(keeTabPage);
        }

        void editGroupFormShown(object sender, EventArgs e)
        {
            GroupForm form = sender as GroupForm;
            PwGroup group = null;
            TabControl mainTabControl = null;
            //This might not work, especially in .NET 2.0 RTM, a shame but more
            //up to date users might as well use the feature if possible.
            try
            {
                FieldInfo fi = typeof(GroupForm).GetField("m_pwGroup", BindingFlags.NonPublic | BindingFlags.Instance);
                group = (PwGroup)fi.GetValue(form);

                Control[] cs = form.Controls.Find("m_tabMain", true);
                if (cs.Length == 0)
                    return;
                mainTabControl = cs[0] as TabControl;
            }
            catch
            {
                // that's life, just move on.
                return;
            }

            if (group == null)
                return;

            lock (_lockRPCClientManagers)
            {
                _lockRPCClientManagers.HeldBy = Thread.CurrentThread.ManagedThreadId;
                _RPCClientManagers["general"].AttachToGroupDialog(this, group, mainTabControl);
            }
        }

        void editEntryFormShown(object sender, EventArgs e)
        {
            PwEntryForm form = sender as PwEntryForm;
            PwEntry entry = null;
            TabControl mainTabControl = null;
            CustomListViewEx advancedListView = null;
            ProtectedStringDictionary strings = null;

            //This might not work, but might as well use the feature if possible.
            try
            {
                // reflection doesn't seem to be needed for 2.10 and above
                entry = form.EntryRef;
                strings = form.EntryStrings;

                Control[] cs = form.Controls.Find("m_tabMain", true);
                if (cs.Length == 0)
                    return;
                mainTabControl = cs[0] as TabControl;

                Control[] cs2 = form.Controls.Find("m_lvStrings", true);
                if (cs2.Length == 0)
                    return;
                advancedListView = cs2[0] as CustomListViewEx;
            }
            catch
            {
                // that's life, just move on.
                return;
            }

            if (entry == null)
                return;

            lock (_lockRPCClientManagers)
            {
                _lockRPCClientManagers.HeldBy = Thread.CurrentThread.ManagedThreadId;
                _RPCClientManagers["general"].AttachToEntryDialog(this, entry, mainTabControl, form, advancedListView, strings);
            }
        }

        // still useful for tracking server versions I reckon...
        bool refreshVersionInfo(IPluginHost host)
        {
            bool upgrading = false;
            int majorOld = (int)host.CustomConfig.GetULong("KeePassRPC.version.major", 0);
            int minorOld = (int)host.CustomConfig.GetULong("KeePassRPC.version.minor", 0);
            int buildOld = (int)host.CustomConfig.GetULong("KeePassRPC.version.build", 0);
            Version versionCurrent = PluginVersion;

            if (majorOld != 0 || minorOld != 0 || buildOld != 0)
            {
                Version versionOld = new Version(majorOld, minorOld, buildOld);
                if (versionCurrent.CompareTo(versionOld) > 0)
                    upgrading = true;
            }

            host.CustomConfig.SetULong("KeePassRPC.version.major", (ulong)versionCurrent.Major);
            host.CustomConfig.SetULong("KeePassRPC.version.minor", (ulong)versionCurrent.Minor);
            host.CustomConfig.SetULong("KeePassRPC.version.build", (ulong)versionCurrent.Build);

            return upgrading;
        }

        void OnToolsOptions(object sender, EventArgs e)
        {
            using (KeePassRPC.Forms.OptionsForm ofDlg = new KeePassRPC.Forms.OptionsForm(_host, this))
                ofDlg.ShowDialog();
        }

        void OnMenuSetRootGroup(object sender, EventArgs e)
        {
            PwGroup pg = _host.MainWindow.GetSelectedGroup();
            Debug.Assert(pg != null);
            if (pg == null || pg.Uuid == null || pg.Uuid == PwUuid.Zero)
                return;

            var rid = _host.Database.RecycleBinUuid;
            if (rid != null && rid != PwUuid.Zero && pg.IsOrIsContainedIn(_host.Database.RootGroup.FindGroup(rid, true)))
            {
                MessageBox.Show(@"You can not set this to be the Kee Home Group. Choose a group outside of the Recycle Bin instead.");
                return;
            }
            var conf = _host.Database.GetKPRPCConfig();
            conf.RootUUID = MemUtil.ByteArrayToHexString(pg.Uuid.UuidBytes);
            _host.Database.SetKPRPCConfig(conf);

            _host.MainWindow.UpdateUI(false, null, true, null, true, null, true);
        }

        private string[] getStandardIconsBase64(ImageList il)
        {
            string[] icons = new string[il.Images.Count];

            for (int i = 0; i < il.Images.Count; i++)
            {
                Image image = il.Images[i];
                using (MemoryStream ms = new MemoryStream())
                {
                    image.Save(ms, System.Drawing.Imaging.ImageFormat.Png);
                    icons[i] = Convert.ToBase64String(ms.ToArray());
                }
            }
            return icons;
        }

        public delegate object WelcomeKeeUserDelegate();


        public object WelcomeKeeUser()
        {
            using (WelcomeForm wf = new WelcomeForm())
            {
                DialogResult dr = wf.ShowDialog(_host.MainWindow);
                if (dr == DialogResult.Yes)
                    CreateNewDatabase();
                if (dr == DialogResult.Yes || dr == DialogResult.No)
                    return 0;
                return 1;
            }
        }

        public delegate object GetIconDelegate(int iconIndex);


        public Image GetIcon(int iconIndex)
        {
            Image im = _host.MainWindow.ClientIcons.Images[(int)iconIndex];
            // can't do this until we drop support for KP <2.28: if (DpiUtil.ScalingRequired)
            im = DpiFix.ScaleImageTo16x16(im, false);
            return im;
        }


        public delegate object GetCustomIconDelegate(PwUuid uuid);


        public Image GetCustomIcon(PwUuid uuid)
        {
            return _host.Database.GetCustomIcon(uuid);
        }

        private void EnsureDBIconIsInKPRPCIconCache()
        {
            string cachedBase64 = IconCache<string>
                .GetIconEncoding(_host.Database.IOConnectionInfo.Path);
            if (string.IsNullOrEmpty(cachedBase64))
            {
                // the icon wasn't in the cache so lets calculate its base64 encoding and then add it to the cache
                using (MemoryStream ms = new MemoryStream())
                using (Image originalImage = _host.MainWindow.Icon.ToBitmap())
                using (Image imgNew = new Bitmap(originalImage, new Size(16, 16)))
                {
                    imgNew.Save(ms, System.Drawing.Imaging.ImageFormat.Png);
                    string imageData = Convert.ToBase64String(ms.ToArray());
                    IconCache<string>
                        .AddIcon(_host.Database.IOConnectionInfo.Path, imageData);
                }
            }
        }

        /// <summary>
        /// Called when [file new].
        /// </summary>
        /// <remarks>Review whenever private KeePass.MainForm.OnFileNew method changes. Last reviewed 20180416</remarks>
        /// <param name="sender">The sender.</param>
        /// <param name="e">The <see cref="System.EventArgs"/> instance containing the event data.</param>
        internal void CreateNewDatabase()
        {
            if (!AppPolicy.Try(AppPolicyId.SaveFile)) return;

            DialogResult dr;
            string strPath;
            using (SaveFileDialog sfd = UIUtil.CreateSaveFileDialog(KPRes.CreateNewDatabase,
                KPRes.NewDatabaseFileName, UIUtil.CreateFileTypeFilter(
                    AppDefs.FileExtension.FileExt, KPRes.KdbxFiles, true), 1,
                AppDefs.FileExtension.FileExt, false))
            {
                GlobalWindowManager.AddDialog(sfd);
                dr = sfd.ShowDialog(_host.MainWindow);
                GlobalWindowManager.RemoveDialog(sfd);
                strPath = sfd.FileName;
            }

            if (dr != DialogResult.OK) return;

            KeePassLib.Keys.CompositeKey key = null;
            bool showUsualKeePassKeyCreationDialog = false;
            using (KeyCreationSimpleForm kcsf = new KeyCreationSimpleForm())
            {
                // Don't show the simple key creation form if the user has set
                // security policies that restrict the allowable composite key sources
                if (KeePass.Program.Config.UI.KeyCreationFlags == 0)
                {
                    kcsf.InitEx(KeePassLib.Serialization.IOConnectionInfo.FromPath(strPath), true);
                    dr = kcsf.ShowDialog(_host.MainWindow);
                    if ((dr == DialogResult.Cancel) || (dr == DialogResult.Abort)) return;
                    if (dr == DialogResult.No)
                    {
                        showUsualKeePassKeyCreationDialog = true;
                    }
                    else
                    {
                        key = kcsf.CompositeKey;
                    }
                }
                else
                {
                    showUsualKeePassKeyCreationDialog = true;
                }

                if (showUsualKeePassKeyCreationDialog)
                {
                    using (KeyCreationForm kcf = new KeyCreationForm())
                    {
                        kcf.InitEx(KeePassLib.Serialization.IOConnectionInfo.FromPath(strPath), true);
                        dr = kcf.ShowDialog(_host.MainWindow);
                        if ((dr == DialogResult.Cancel) || (dr == DialogResult.Abort)) return;
                        key = kcf.CompositeKey;
                    }
                }

                PwDocument dsPrevActive = _host.MainWindow.DocumentManager.ActiveDocument;
                PwDatabase pd = _host.MainWindow.DocumentManager.CreateNewDocument(true).Database;
                pd.New(KeePassLib.Serialization.IOConnectionInfo.FromPath(strPath), key);

                if (!string.IsNullOrEmpty(kcsf.DatabaseName))
                {
                    pd.Name = kcsf.DatabaseName;
                    pd.NameChanged = DateTime.Now;
                }

                InsertStandardKeePassData(pd);

                var conf = pd.GetKPRPCConfig();
                pd.SetKPRPCConfig(conf);

                // save the new database & update UI appearance
                pd.Save(_host.MainWindow.CreateStatusBarLogger());
            }
            _host.MainWindow.UpdateUI(true, null, true, null, true, null, false);
        }

        private void InsertStandardKeePassData(PwDatabase pd)
        {
            PwGroup pg = new PwGroup(true, true, KPRes.General, PwIcon.Folder);
            pd.RootGroup.AddGroup(pg, true);

            pg = new PwGroup(true, true, KPRes.WindowsOS, PwIcon.DriveWindows);
            pd.RootGroup.AddGroup(pg, true);

            pg = new PwGroup(true, true, KPRes.Network, PwIcon.NetworkServer);
            pd.RootGroup.AddGroup(pg, true);

            pg = new PwGroup(true, true, KPRes.Internet, PwIcon.World);
            pd.RootGroup.AddGroup(pg, true);

            pg = new PwGroup(true, true, KPRes.EMail, PwIcon.EMail);
            pd.RootGroup.AddGroup(pg, true);

            pg = new PwGroup(true, true, KPRes.Homebanking, PwIcon.Homebanking);
            pd.RootGroup.AddGroup(pg, true);
        }

        internal PwGroup GetAndInstallKeePasswordBackupGroup(PwDatabase pd)
        {
            PwUuid groupUuid = new PwUuid(new byte[] {
                0xea, 0x9f, 0xf2, 0xed, 0x05, 0x12, 0x47, 0x47,
                0xb6, 0x3e, 0xaf, 0xa5, 0x15, 0xa3, 0x04, 0x30});

            var keeGroup = GetKeeGroup(pd);

            PwGroup kfpbg = pd.RootGroup.FindGroup(groupUuid, true);
            if (kfpbg == null)
            {
                kfpbg = new PwGroup(false, true, "Kee Generated Password Backups", PwIcon.Folder);
                kfpbg.Uuid = groupUuid;
                kfpbg.CustomIconUuid = GetKPRPCIcon();
                keeGroup.AddGroup(kfpbg, true);
            }
            else if (kfpbg.Name == "KeeFox Generated Password Backups")
            {
                kfpbg.Name = "Kee Generated Password Backups";
            }
            return kfpbg;
        }

        /// <summary>
        /// Gets the kee group and renames it from KeeFox if necessary.
        /// </summary>
        /// <param name="pd">The database</param>
        /// <returns>The Kee group or the root group if the group does not exist</returns>
        internal PwGroup GetKeeGroup(PwDatabase pd)
        {
            PwUuid groupUuid = new PwUuid(new byte[] {
                0xea, 0x9f, 0xf2, 0xed, 0x05, 0x12, 0x47, 0x47,
                0xb6, 0x3e, 0xaf, 0xa5, 0x15, 0xa3, 0x04, 0x23});

            PwGroup kfpg = pd.RootGroup.FindGroup(groupUuid, true);
            if (kfpg == null)
            {
                return pd.RootGroup;
            }
            else if (kfpg.Name == "KeeFox")
            {
                kfpg.Name = "Kee";
            }
            return kfpg;
        }

        private PwUuid GetKPRPCIcon()
        {
            //return null;

            // {EB9FF2ED-0512-4747-B83E-AFA515A30422}
            PwUuid kprpcIconUuid = new PwUuid(new byte[] {
                0xeb, 0x9f, 0xf2, 0xed, 0x05, 0x12, 0x47, 0x47,
                0xb8, 0x3e, 0xaf, 0xa5, 0x15, 0xa3, 0x04, 0x22});

            PwCustomIcon icon = null;

            foreach (PwCustomIcon testIcon in _host.Database.CustomIcons)
            {
                if (testIcon.Uuid == kprpcIconUuid)
                {
                    icon = testIcon;
                    break;
                }
            }

            if (icon == null)
            {
                using (MemoryStream ms = new MemoryStream())
                {
                    global::KeePassRPC.Properties.Resources.KPRPC64.Save(ms, System.Drawing.Imaging.ImageFormat.Png);

                    // Create a new custom icon for use with this entry
                    icon = new PwCustomIcon(kprpcIconUuid,
                        ms.ToArray());
                    _host.Database.CustomIcons.Add(icon);
                }
            }
            return kprpcIconUuid;


            //string keeFoxIcon = @"iVBORw0KGgoAAAANSUhEUgAAABAAAAAQCAMAAAFfKj/FAAAABGdBTUEAAK/INwWK6QAAABl0RVh0U29mdHdhcmUAQWRvYmUgSW1hZ2VSZWFkeXHJZTwAAABpUExURf///wAAAAAAAFpaWl5eXm5ubnh4eICAgIeHh5GRkaCgoKOjo66urq+vr8jIyMnJycvLy9LS0uDg4Ovr6+zs7O3t7e7u7u/v7/X19fb29vf39/j4+Pn5+fr6+vv7+/z8/P39/f7+/v///5goWdMAAAADdFJOUwAxTTRG/kEAAACRSURBVBjTTY2JEoMgDESDaO0h9m5DUZT9/49sCDLtzpB5eQwLkSTkwb0cOBnJksYxiHqORHZG3gFc88WReTzvBFoOMbUCVkN/ATw3CnwHmwLjpYCfYoF5TQphAUztMfp5zsm5phY6MEsV+LapYRPAoC/ooOLxfL33RXQifJjjsnZFWPBniksCbBU+6F4FmV+IvtrgDOmaq+PeAAAAAElFTkSuQmCC";

            //byte[] msByteArray = ms.ToArray();

            //foreach (PwCustomIcon item in _host.Database.CustomIcons)
            //{
            //    *var* t = item.Image.[1][2];
            //    // re-use existing custom icon if it's already in the database
            //    // (This will probably fail if database is used on 
            //    // both 32 bit and 64 bit machines - not sure why...)
            //    if (KeePassLib.Utility.MemUtil.ArraysEqual(msByteArray, item.ImageDataPng))
            //    {
            //        pwe.CustomIconUuid = item.Uuid;
            //        m_host.Database.UINeedsIconUpdate = true;
            //        return;
            //    }
            //}

            //    // Create a new custom icon for use with this entry
            //    PwCustomIcon pwci = new PwCustomIcon(new PwUuid(true),
            //        ms.ToArray());
            //    m_host.Database.CustomIcons.Add(pwci);

            //    return pwci.Uuid;
        }

        private void CreateClientManagers()
        {
            lock (_lockRPCClientManagers)
            {
                _lockRPCClientManagers.HeldBy = Thread.CurrentThread.ManagedThreadId;
                _RPCClientManagers.Add("general", new GeneralRPCClientManager());

                //TODO2: load managers from plugins, etc.
                _RPCClientManagers.Add("KeeFox", new KeeFoxRPCClientManager());
            }
        }

        private void PromoteGeneralRPCClient(KeePassRPCClientConnection connection, KeePassRPCClientManager destination)
        {
            lock (_lockRPCClientManagers)
            {
                _lockRPCClientManagers.HeldBy = Thread.CurrentThread.ManagedThreadId;
                ((GeneralRPCClientManager)_RPCClientManagers["general"]).RemoveRPCClientConnection(connection);
                destination.AddRPCClientConnection(connection);
            }
        }

        internal void PromoteGeneralRPCClient(KeePassRPCClientConnection connection, string clientName)
        {
            string managerName = "general";
            switch (clientName)
            {
                case "KeeFox": managerName = "KeeFox"; break;
            }

            PromoteGeneralRPCClient(connection, _RPCClientManagers[managerName]);
        }

        /// <summary>
        /// Free resources
        /// </summary>
        public override void Terminate()
        {
            this.terminating = true;
            lock (_lockRPCClientManagers)
            {
                _lockRPCClientManagers.HeldBy = Thread.CurrentThread.ManagedThreadId;
                foreach (KeePassRPCClientManager manager in _RPCClientManagers.Values)
                    manager.Terminate();
                _RPCClientManagers.Clear();
            }

            // remove event listeners
            _host.MainWindow.FileOpened -= OnKPDBOpen;
            _host.MainWindow.FileClosed -= OnKPDBClose;
            _host.MainWindow.FileCreated -= OnKPDBCreated;
            _host.MainWindow.FileSaving -= OnKPDBSaving;
            _host.MainWindow.FileSaved -= OnKPDBSaved;
            _host.MainWindow.DocumentManager.ActiveDocumentSelected -= OnKPDBSelected;

            GlobalWindowManager.WindowAdded -= GwmWindowAddedHandler;

            // terminate _BackgroundWorker
            _BackgroundWorkerAutoResetEvent.Set();

            // Remove 'Tools' menu items
            ToolStripItemCollection tsMenu = _host.MainWindow.ToolsMenu.DropDownItems;
            tsMenu.Remove(_keePassRPCOptions);

            // Remove group context menu items
            ContextMenuStrip gcm = _host.MainWindow.GroupContextMenu;
            gcm.Items.Remove(_tsSeparator1);
            gcm.Items.Remove(_keeRootMenu);

            if (logger != null)
                logger.Close();
        }

        private void OnKPDBSelected(object sender, EventArgs e)
        {
            SignalAllManagedRPCClients(Signal.DATABASE_SELECTED);
        }

        private void OnKPDBCreated(object sender, FileCreatedEventArgs e)
        {
            var conf = e.Database.GetKPRPCConfig();
            e.Database.SetKPRPCConfig(conf);
            EnsureDBIconIsInKPRPCIconCache();
            //KeePassRPCService.ensureDBisOpenEWH.Set(); // signal that DB is now open so any waiting JSONRPC thread can go ahead
            SignalAllManagedRPCClients(Signal.DATABASE_OPEN);
        }

        private delegate void dlgSaveDB(PwDatabase databaseToSave);

        void saveDB(PwDatabase databaseToSave)
        {
            // save active database & update UI appearance
            if (_host.MainWindow.UIFileSave(true))
                _host.MainWindow.UpdateUI(false, null, true, null, true, null, false);

        }

        private void OnKPDBOpen(object sender, FileOpenedEventArgs e)
        {
            EnsureDBIconIsInKPRPCIconCache();

            if (GetConfigVersionForLegacyMigration(e.Database) > 0)
            {
                // Version 0 could indicate this DB contains KeeFox data
                // from many years ago, no config data or post 2016 config data. It's 
                // usefulness has therefore expired and we delete it to keep the kdbx
                // file contents tidy
                e.Database.CustomData.Remove("KeePassRPC.KeeFox.configVersion");
            }

            // Database config versions < 3 will be lazily updated when a v2 config
            // is first accessed and the DB next saved. This only affects the RootUUID though.

            SignalAllManagedRPCClients(Signal.DATABASE_OPEN);
        }


        private int GetConfigVersionForLegacyMigration(PwDatabase db)
        {
            if (db.CustomData.Exists("KeePassRPC.KeeFox.configVersion"))
            {
                int configVersion = 0;
                string configVersionString = db.CustomData.Get("KeePassRPC.KeeFox.configVersion");
                if (int.TryParse(configVersionString, out configVersion))
                    return configVersion;
            }
            return 0;
        }

        private void OnKPDBClose(object sender, FileClosedEventArgs e)
        {
            SignalAllManagedRPCClients(Signal.DATABASE_CLOSED);
        }

        private void OnKPDBSaving(object sender, FileSavingEventArgs e)
        {
            SignalAllManagedRPCClients(Signal.DATABASE_SAVING);
        }

        private void OnKPDBSaved(object sender, FileSavedEventArgs e)
        {
            EnsureDBIconIsInKPRPCIconCache();
            SignalAllManagedRPCClients(Signal.DATABASE_SAVED);
        }

        internal void SignalAllManagedRPCClients(KeePassRPC.DataExchangeModel.Signal signal)
        {
            lock (_lockRPCClientManagers)
            {
                _lockRPCClientManagers.HeldBy = Thread.CurrentThread.ManagedThreadId;
                foreach (KeePassRPCClientManager manager in _RPCClientManagers.Values)
                    manager.SignalAll(signal);
            }
        }

        internal void AddRPCClientConnection(KeePassRPCClientConnection keePassRPCClient)
        {
            lock (_lockRPCClientManagers)
            {
                _lockRPCClientManagers.HeldBy = Thread.CurrentThread.ManagedThreadId;
                _RPCClientManagers["general"].AddRPCClientConnection(keePassRPCClient);
            }
        }

        internal void RemoveRPCClientConnection(KeePassRPCClientConnection keePassRPCClient)
        {
            lock (_lockRPCClientManagers)
            {
                _lockRPCClientManagers.HeldBy = Thread.CurrentThread.ManagedThreadId;
                // this generally only happens at connection shutdown time so think we get away with a search like this
                foreach (KeePassRPCClientManager manager in _RPCClientManagers.Values)
                    foreach (KeePassRPCClientConnection connection in manager.CurrentRPCClientConnections)
                        if (connection == keePassRPCClient)
                            manager.RemoveRPCClientConnection(keePassRPCClient);
            }
        }

        internal void AddRPCClientConnection(IWebSocketConnection webSocket)
        {
            lock (_lockRPCClientManagers)
            {
                _lockRPCClientManagers.HeldBy = Thread.CurrentThread.ManagedThreadId;
                _RPCClientManagers["general"].AddRPCClientConnection(new KeePassRPCClientConnection(webSocket, false, this));
            }
        }

        internal void RemoveRPCClientConnection(IWebSocketConnection webSocket)
        {
            lock (_lockRPCClientManagers)
            {
                _lockRPCClientManagers.HeldBy = Thread.CurrentThread.ManagedThreadId;
                // this generally only happens at conenction shutdown time so think we get away with a search like this
                foreach (KeePassRPCClientManager manager in _RPCClientManagers.Values)
                    foreach (KeePassRPCClientConnection connection in manager.CurrentRPCClientConnections)
                        if (connection.WebSocketConnection == webSocket)
                        {
                            manager.RemoveRPCClientConnection(connection);
                            return;
                        }
            }
        }

        internal void MessageRPCClientConnection(IWebSocketConnection webSocket, string message, KeePassRPCService service)
        {
            KeePassRPCClientConnection connection = null;

            lock (_lockRPCClientManagers)
            {
                _lockRPCClientManagers.HeldBy = Thread.CurrentThread.ManagedThreadId;
                foreach (KeePassRPCClientManager manager in _RPCClientManagers.Values)
                {
                    foreach (KeePassRPCClientConnection conn in manager.CurrentRPCClientConnections)
                    {
                        if (conn.WebSocketConnection == webSocket)
                        {
                            connection = conn;
                            break;
                        }
                    }
                    if (connection != null)
                        break;
                }
            }

            if (connection != null)
                connection.ReceiveMessage(message, service);
            else
                webSocket.Close();
        }

        internal List<KeePassRPCClientConnection> GetConnectedRPCClients()
        {
            List<KeePassRPCClientConnection> clients = new List<KeePassRPCClientConnection>();
            lock (_lockRPCClientManagers)
            {
                _lockRPCClientManagers.HeldBy = Thread.CurrentThread.ManagedThreadId;
                foreach (KeePassRPCClientManager manager in _RPCClientManagers.Values)
                    foreach (KeePassRPCClientConnection connection in manager.CurrentRPCClientConnections)
                        if (connection.Authorised)
                            clients.Add(connection);
            }
            return clients;
        }

        public string GetPwEntryString(PwEntry pwe, string name, PwDatabase db)
        {
            return pwe.Strings.ReadSafe(name);
        }

        public string GetPwEntryStringFromDereferencableValue(PwEntry pwe, string name, PwDatabase db)
        {
            return SprEngine.Compile(name, new SprContext(pwe, db, SprCompileFlags.All & ~SprCompileFlags.Run, false, false));
        }

        public void InvokeMainThread(Delegate method, params object[] args)
        {
            _BackgroundWorker.ReportProgress(0, (MethodInvoker)delegate
            {
                method.DynamicInvoke(args);
            });
        }

        private void _BackgroundWorker_ProgressChanged(object sender, ProgressChangedEventArgs e)
        {
            ((MethodInvoker)e.UserState).Invoke();
        }
    }

    public class LockManager
    {
        public int HeldBy;
    }

}
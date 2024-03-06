using System;
using System.Collections.Generic;
using System.Linq;
using System.Windows.Forms;
using KeePass;
using KeePass.Forms;
using KeePass.Plugins;
using KeePass.UI;
using KeePassLib;
using KeePassLib.Security;
using KeePassLib.Serialization;
using KeePassLib.Utility;
using KeePassRPC.JsonRpc;
using KeePassRPC.Models.DataExchange;
using KeePassRPC.Models.Persistent;
using KeePassRPC.Models.Shared;

namespace KeePassRPC
{
    /// <summary>
    /// Provides an externally accessible API for common KeePass operations
    /// </summary>
    public partial class KeePassRPCService : KprpcJsonRpcService
    {
        #region Class variables, constructor and destructor

        private readonly KeePassRPCExt _keePassRpcPlugin;
        private readonly IPluginHost _host;
        private readonly IconConverter _iconConverter;

        public KeePassRPCService(IPluginHost host, string[] standardIconsBase64, KeePassRPCExt plugin)
        {
            _keePassRpcPlugin = plugin;
            _host = host;
            _iconConverter = new IconConverter(host, _keePassRpcPlugin, standardIconsBase64);
        }

        #endregion

        #region KeePass GUI routines

        /// <summary>
        /// Halts thread until a DB is open in the KeePass application
        /// </summary>
        private bool EnsureDBisOpen()
        {
            if (!_host.Database.IsOpen)
            {
                _host.MainWindow.BeginInvoke((MethodInvoker)delegate { PromptUserToOpenDb(null, true); });
                if (!_host.Database.IsOpen)
                    return false;
            }

            return true;
        }

        private void PromptUserToOpenDb(IOConnectionInfo ioci, bool returnFocus)
        {
            //TODO: find out z-index of firefox and push keepass just behind it rather than right to the back
            //TODO: focus open DB dialog box if it's there

            IntPtr ffWindow = IntPtr.Zero;
            if (returnFocus) ffWindow = Native.GetForegroundWindowHandle();
            bool minimised = Program.MainForm.WindowState == FormWindowState.Minimized;
            bool trayed = Program.MainForm.IsTrayed();

            if (ioci == null)
                ioci = Program.Config.Application.LastUsedFile;

            if (!Native.IsUnix())
            {
                // We can't just Native.EnsureForegroundWindow() because most recent
                // Windows versions just flash the taskbar rather than actually focus 
                // the window without these shenanigans. We don't need to this.showOpenDB(ioci)
                // because any appropriate "enter master key" dialog is triggered after 
                // minimising and restoring the window
                Native.AttachToActiveAndBringToForeground(Program.MainForm.Handle);
            }
            else
            {
                Native.EnsureForegroundWindow(Program.MainForm.Handle);
                if (Program.MainForm.IsFileLocked(null) && !Program.MainForm.UIIsAutoUnlockBlocked())
                {
                    showOpenDB(ioci);
                }
            }

            Program.MainForm.Activate();

            // refresh the UI in case user cancelled the dialog box and/or KeePass 
            // native calls have left us in a bit of a weird state
            _host.MainWindow.UpdateUI(true, null, true, null, true, null, false);

            // Set the program state back to what is was unless the user has
            // configured "lock on minimise" in which case we always set it to Normal
            if (!Program.Config.Security.WorkspaceLocking.LockOnWindowMinimize)
            {
                minimised = false;
                trayed = false;
            }

            Program.MainForm.WindowState = minimised ? FormWindowState.Minimized : FormWindowState.Normal;

            if (trayed)
            {
                Program.MainForm.Visible = false;
                Program.MainForm.UpdateTrayIcon();
            }

            // Make Firefox active again
            if (returnFocus) Native.EnsureForegroundWindow(ffWindow);
        }

        private bool showOpenDB(IOConnectionInfo ioci)
        {
            // KeePass does this on "show window" keypress. Not sure what it does but most likely does no harm to check here too
            if (Program.MainForm.UIIsInteractionBlocked())
            {
                return false;
            }

            // Make sure the login dialog (or options and other windows) are not already visible. Same behaviour as KP.
            if (GlobalWindowManager.WindowCount != 0) return false;

            // Prompt user to open database
            Program.MainForm.OpenDatabase(ioci, null, false);
            return true;
        }

        private delegate void dlgUpdateUINoSave();

        private void updateUINoSave()
        {
            _host.MainWindow.UpdateUI(false, null, true, null, true, null, true);
        }

        private delegate void dlgSaveDB(PwDatabase databaseToSave);

        private void saveDB(PwDatabase databaseToSave)
        {
            // store current active tab/db
            PwDocument currentActiveDoc = _host.MainWindow.DocumentManager.ActiveDocument;

            // change active tab
            PwDocument doc = _host.MainWindow.DocumentManager.FindDocument(databaseToSave);
            _host.MainWindow.DocumentManager.ActiveDocument = doc;

            if (_host.CustomConfig.GetBool("KeePassRPC.KeeFox.autoCommit", true))
            {
                // save active database & update UI appearance
                if (_host.MainWindow.UIFileSave(false))
                    _host.MainWindow.UpdateUI(false, null, true, null, true, null, false);
            }
            else
            {
                // update ui with "changed" flag                
                _host.MainWindow.UpdateUI(false, null, true, null, true, null, true);
            }

            // change tab back
            _host.MainWindow.DocumentManager.ActiveDocument = currentActiveDoc;
        }

        private void openGroupEditorWindow(PwGroup pg, PwDatabase db)
        {
            using (GroupForm gf = new GroupForm())
            {
                gf.InitEx(pg, false, _host.MainWindow.ClientIcons, _host.Database);

                gf.BringToFront();
                gf.ShowInTaskbar = true;

                _host.MainWindow.Focus();
                gf.TopMost = true;
                gf.Focus();
                gf.Activate();
                if (gf.ShowDialog() == DialogResult.OK)
                    saveDB(db);
            }
        }

        private delegate void dlgOpenGroupEditorWindow(PwGroup pg, PwDatabase db);

        private void OpenLoginEditorWindow(PwEntry pe, PwDatabase db)
        {
            using (PwEntryForm ef = new PwEntryForm())
            {
                ef.InitEx(pe, PwEditMode.EditExistingEntry, _host.Database, _host.MainWindow.ClientIcons, false, false);

                ef.BringToFront();
                ef.ShowInTaskbar = true;

                _host.MainWindow.Focus();
                ef.TopMost = true;
                ef.Focus();
                ef.Activate();

                if (ef.ShowDialog() == DialogResult.OK)
                    saveDB(db);
            }
        }

        private delegate void dlgOpenLoginEditorWindow(PwEntry pg, PwDatabase db);

        // A similar function is defined in KeePass MainForm_functions.cs but it's internal
        private IOConnectionInfo CompleteConnectionInfoUsingMru(IOConnectionInfo ioc)
        {
            if (string.IsNullOrEmpty(ioc.UserName) && string.IsNullOrEmpty(ioc.Password))
            {
                for (uint u = 0; u < _host.MainWindow.FileMruList.ItemCount; ++u)
                {
                    IOConnectionInfo iocMru = (_host.MainWindow.FileMruList.GetItem(u).Value as IOConnectionInfo);
                    if (iocMru == null)
                    {
                        continue;
                    }

                    if (iocMru.Path.Equals(ioc.Path, StrUtil.CaseIgnoreCmp))
                    {
                        ioc = iocMru.CloneDeep();
                        break;
                    }
                }
            }

            return ioc;
        }

        #endregion

        public static bool IsNet35OrNewer()
        {
            return Type.GetType("System.GCCollectionMode", false) != null;
        }

        public static bool IsNet45OrNewer()
        {
            return Type.GetType("System.Reflection.ReflectionContext", false) != null;
        }

        public static bool IsNet451OrNewer()
        {
            return Type.GetType("System.Runtime.GCLargeObjectHeapCompactionMode", false) != null;
        }

        private IOConnectionInfo SelectActiveDatabase(string fileName)
        {
            IOConnectionInfo ioci = null;

            if (!string.IsNullOrEmpty(fileName))
            {
                ioci = new IOConnectionInfo();
                ioci.Path = fileName;
                ioci = CompleteConnectionInfoUsingMru(ioci);
            }

            // Set the current document / database to be the one we've been asked to display (may already be the case)
            // This is because the minimise/restore trick utilised a few frames later prompts KeePass into raising an
            // "enter key" dialog for the currently active database. This little check makes sure that the user sees
            // the database they've asked for first (assuming the database they want is already open but locked)
            // We can't stop an unneccessary prompt being seen if the user has asked for a new database to be opened
            // and the current workspace is locked
            //
            // We do this regardless of whether the DB is already open or locked
            //
            //TODO: Need to verify this works OK with unusual circumstances like one DB open but others locked
            if (ioci != null)
                foreach (PwDocument doc in _host.MainWindow.DocumentManager.Documents)
                    if (doc.LockedIoc.Path == fileName ||
                        (doc.Database.IsOpen && doc.Database.IOConnectionInfo.Path == fileName))
                        _host.MainWindow.DocumentManager.ActiveDocument = doc;

            // before explicitly asking user to log into the correct DB we'll set up a "fake" document in KeePass
            // in the hope that the minimise/restore trick will get KeePass to prompt the user on our behalf
            // (regardless of state of existing documents and newly requested document)
            if (ioci != null
                && !(_host.MainWindow.DocumentManager.ActiveDocument.Database.IsOpen &&
                     _host.MainWindow.DocumentManager.ActiveDocument.Database.IOConnectionInfo.Path == fileName)
                && !(!_host.MainWindow.DocumentManager.ActiveDocument.Database.IsOpen &&
                     _host.MainWindow.DocumentManager.ActiveDocument.LockedIoc.Path == fileName))
            {
                PwDocument doc = _host.MainWindow.DocumentManager.CreateNewDocument(true);
                doc.LockedIoc = ioci;
            }

            return ioci;
        }

        private void OpenIfRequired(IOConnectionInfo ioci, bool returnFocus)
        {
            _host.MainWindow.BeginInvoke((MethodInvoker)delegate { PromptUserToOpenDb(ioci, returnFocus); });
        }

        private void AddPasswordBackupLogin(string password, string url)
        {
            if (!_host.Database.IsOpen)
                return;

            PwDatabase chosenDB = SelectDatabase("");
            var parentGroup = _keePassRpcPlugin.GetAndInstallKeePasswordBackupGroup(chosenDB);

            string explanatoryNote = "This entry is a backup of a password generated by Kee. " +
                                     "It is not visible to Kee. You can edit it to make it visible but we recommend " +
                                     "instead saving a new entry after you next sign in to the website. You can " +
                                     "delete this backup when you are sure that you can sign-in to the website " +
                                     "correctly using your new password.";
            PwEntry newLogin = new PwEntry(true, true);
            newLogin.Strings.Set(PwDefs.TitleField, new ProtectedString(
                chosenDB.MemoryProtection.ProtectTitle, "Kee generated password at: " + DateTime.Now));
            newLogin.Strings.Set(PwDefs.UrlField, new ProtectedString(
                chosenDB.MemoryProtection.ProtectUrl, url));
            newLogin.Strings.Set(PwDefs.PasswordField, new ProtectedString(
                chosenDB.MemoryProtection.ProtectPassword, password));
            newLogin.Strings.Set(PwDefs.NotesField,
                new ProtectedString(chosenDB.MemoryProtection.ProtectNotes, explanatoryNote));
            EntryConfigv2 conf = (new EntryConfigv1(MatchAccuracyMethod.Hostname)).ConvertToV2(new GuidService());
            var list = conf.MatcherConfigs.ToList();
            list.Add(new EntryMatcherConfig { MatcherType = EntryMatcherType.Hide });
            conf.MatcherConfigs = list.ToArray();
            newLogin.SetKPRPCConfig(conf);
            parentGroup.AddEntry(newLogin, true);

            // We can't save the database at this point because KeePass steals
            // window focus while saving; that breaks Firefox's Australis UI panels.
            _host.MainWindow.BeginInvoke(new dlgUpdateUINoSave(updateUINoSave));
        }

        // We can't just use the MatchAccuracyMethod found for the entry (in the conf parameter)
        // because the actual MAM to apply may have been modified based upon the specific URL(s) that
        // we're being asked to match against (the URLs shown in the browser rather than those 
        // contained within the entry)
        public static int BestMatchAccuracyForAnyURL(PwEntry pwe, EntryConfigv2 conf, string url, URLSummary urlSummary,
            MatchAccuracyMethod mam)
        {
            int bestMatchSoFar = MatchAccuracy.None;

            List<string> URLs = new List<string>(3);
            URLs.Add(pwe.Strings.ReadSafe("URL"));
            if (conf.AltUrls != null)
                URLs.AddRange(conf.AltUrls);

            foreach (string entryURL in URLs)
            {
                if (entryURL == url)
                    return MatchAccuracy.Best;

                // If we require very accurate matches, we can skip the more complex assessment below
                if (mam == MatchAccuracyMethod.Exact)
                    continue;

                int entryUrlQSStartIndex = entryURL.IndexOf('?');
                int urlQSStartIndex = url.IndexOf('?');
                string entryUrlExcludingQS = entryURL.Substring(0,
                    entryUrlQSStartIndex > 0 ? entryUrlQSStartIndex : entryURL.Length);
                string urlExcludingQS = url.Substring(0,
                    urlQSStartIndex > 0 ? urlQSStartIndex : url.Length);
                if (entryUrlExcludingQS == urlExcludingQS)
                    return MatchAccuracy.Close;

                // If we've already found a reasonable match, we can skip the rest of the assessment for subsequent URLs
                // apart from the check for matches against a hostname excluding query string
                if (bestMatchSoFar >= MatchAccuracy.HostnameAndPort)
                    continue;

                URLSummary entryUrlSummary = URLSummary.FromURL(entryURL);

                if (entryUrlSummary.HostnameAndPort == urlSummary.HostnameAndPort)
                    bestMatchSoFar = MatchAccuracy.HostnameAndPort;

                // If we need at least a matching hostname and port (equivelent to
                // KeeFox <1.5) or we are missing the information needed to match
                // more loose components of the URL we have to skip these last tests
                if (mam == MatchAccuracyMethod.Hostname) continue;

                if (entryUrlSummary.Domain == null || urlSummary.Domain == null)
                {
                    if (bestMatchSoFar < MatchAccuracy.HostnameExcludingPort
                        && !string.IsNullOrWhiteSpace(entryUrlSummary.HostnameOnly)
                        && entryUrlSummary.HostnameOnly == urlSummary.HostnameOnly)
                    {
                        return MatchAccuracy.HostnameExcludingPort;
                    }

                    continue;
                }

                if (bestMatchSoFar < MatchAccuracy.HostnameExcludingPort
                    && entryUrlSummary.Domain.Hostname == urlSummary.Domain.Hostname)
                    bestMatchSoFar = MatchAccuracy.HostnameExcludingPort;

                if (bestMatchSoFar < MatchAccuracy.Domain
                    && entryUrlSummary.Domain.RegistrableDomain == urlSummary.Domain.RegistrableDomain)
                    bestMatchSoFar = MatchAccuracy.Domain;
            }

            return bestMatchSoFar;
        }

        // hostname-wide blocks are not natively supported but can be emulated using an appropriate regex
        private bool matchesAnyBlockedURL(PwEntry pwe, EntryConfigv2 conf, string url)
        {
            if (conf.BlockedUrls != null)
                foreach (string altURL in conf.BlockedUrls)
                    if (altURL.Contains(url))
                        return true;
            return false;
        }

        /// <summary>
        /// Finds the group that matches a UUID, else return the root group
        /// </summary>
        /// <param name="uuid">the unique ID of the group we're interested in.</param>
        /// <returns>Group that matches the UUID, else the root group.</returns>
        private PwGroup findMatchingGroup(string uuid)
        {
            PwGroup matchedGroup;
            if (!string.IsNullOrEmpty(uuid))
            {
                PwUuid pwuuid = new PwUuid(MemUtil.HexStringToByteArray(uuid));

                matchedGroup = _host.Database.RootGroup.FindGroup(pwuuid, true);
            }
            else
            {
                matchedGroup = GetRootPwGroup(_host.Database);
            }

            if (matchedGroup == null)
                throw new Exception(
                    "Could not find requested group. Have you deleted your Kee home group? Set a new one and try again.");

            return matchedGroup;
        }

        private bool EntryIsInRecycleBin(PwEntry pwe, PwDatabase db)
        {
            PwGroup parent = pwe.ParentGroup;
            while (parent != null)
            {
                if (db.RecycleBinUuid.Equals(parent.Uuid))
                    return true;
                parent = parent.ParentGroup;
            }

            return false;
        }


        private bool ConfigIsCorrectVersion(PwDatabase db)
        {
            if (db.GetKPRPCConfig().Version == 3)
            {
                return true;
            }

            return false;
        }

        /// <summary>
        /// Return the root group of the active database
        /// </summary>
        /// <param name="location">Selects an alternative root group based on KeePass location; null or empty string = default root group</param>
        /// <returns>the root group</returns>
        public PwGroup GetRootPwGroup(PwDatabase pwd, string location)
        {
            if (pwd == null)
                pwd = _host.Database;

            if (!string.IsNullOrEmpty(location))
            {
                // If any listed group UUID is found in this database, set it as the Kee home group
                string rootGroupsConfig = _host.CustomConfig
                    .GetString("KeePassRPC.knownLocations." + location + ".RootGroups", "");
                string[] rootGroups = new string[0];

                if (!string.IsNullOrEmpty(rootGroupsConfig))
                {
                    rootGroups = rootGroupsConfig.Split(',');
                    foreach (string rootGroupId in rootGroups)
                    {
                        PwUuid pwuuid = new PwUuid(MemUtil.HexStringToByteArray(rootGroupId));
                        PwGroup matchedGroup = _host.Database.RootGroup.FindGroup(pwuuid, true);

                        if (matchedGroup == null)
                            continue;

                        return matchedGroup;
                    }
                    // If no match found we'll just return the default root group
                }
                // If no locations found we'll just return the default root group
            }

            var conf = pwd.GetKPRPCConfig();
            if (!string.IsNullOrWhiteSpace(conf.RootUUID) && conf.RootUUID.Length == 32)
            {
                string uuid = conf.RootUUID;

                PwUuid pwuuid = new PwUuid(MemUtil.HexStringToByteArray(uuid));

                PwGroup matchedGroup = pwd.RootGroup.FindGroup(pwuuid, true);

                if (matchedGroup == null)
                    throw new Exception(
                        "Could not find requested group. Have you deleted your Kee home group? Set a new one and try again.");

                var rid = _host.Database.RecycleBinUuid;
                if (rid != null && !ReferenceEquals(rid, PwUuid.Zero) &&
                    matchedGroup.IsOrIsContainedIn(_host.Database.RootGroup.FindGroup(rid, true)))
                    throw new Exception(
                        "Kee home group is in the Recycle Bin. Restore the group or set a new home group and try again.");

                return matchedGroup;
            }

            return pwd.RootGroup;
        }

        private void MergeEntries(PwEntry destination, PwEntry source, int urlMergeMode, PwDatabase db)
        {
            // We're updating the entry so can always convert the entry to v2 config format
            EntryConfigv2 destConfig = destination.GetKPRPCConfigNormalised(db.GetKPRPCConfig().DefaultMatchAccuracy);
            if (destConfig == null)
                return;

            EntryConfigv2 sourceConfig = source.GetKPRPCConfigNormalised(db.GetKPRPCConfig().DefaultMatchAccuracy);
            if (sourceConfig == null)
                return;

            destination.CreateBackup(db);

            destination.Strings.Set("Title", new ProtectedString(
                _host.Database.MemoryProtection.ProtectTitle, source.Strings.ReadSafe("Title")));
            destConfig.HttpRealm = sourceConfig.HttpRealm;
            destination.IconId = source.IconId;
            destination.CustomIconUuid = source.CustomIconUuid;
            destination.Strings.Set("UserName", new ProtectedString(
                _host.Database.MemoryProtection.ProtectUserName, source.Strings.ReadSafe("UserName")));
            destination.Strings.Set("Password", new ProtectedString(
                _host.Database.MemoryProtection.ProtectPassword, source.Strings.ReadSafe("Password")));
            destConfig.Fields = sourceConfig.Fields;

            // This algorithm could probably be made more efficient (lots of O(n) operations
            // but we're dealing with pretty small n so I've gone with the conceptually
            // easiest approach for now).

            List<string> destURLs = new List<string>();
            destURLs.Add(destination.Strings.ReadSafe("URL"));
            if (destConfig.AltUrls != null)
                destURLs.AddRange(destConfig.AltUrls);

            List<string> sourceURLs = new List<string>();
            sourceURLs.Add(source.Strings.ReadSafe("URL"));
            if (sourceConfig.AltUrls != null)
                sourceURLs.AddRange(sourceConfig.AltUrls);

            switch (urlMergeMode)
            {
                case 1:
                    MergeInNewURLs(destURLs, sourceURLs);
                    break;
                case 2:
                    destURLs.RemoveAt(0);
                    MergeInNewURLs(destURLs, sourceURLs);
                    break;
                case 3:
                    if (sourceURLs.Count > 0)
                    {
                        foreach (string sourceUrl in sourceURLs)
                            if (!destURLs.Contains(sourceUrl))
                                destURLs.Add(sourceUrl);
                    }

                    break;
                case 4:
                    // No changes to URLs
                    break;
                case 5:
                    destURLs = sourceURLs;
                    break;
            }

            // These might not have changed but meh
            destination.Strings.Set("URL",
                new ProtectedString(_host.Database.MemoryProtection.ProtectUrl, destURLs[0]));
            destConfig.AltUrls = new string[0];
            if (destURLs.Count > 1)
                destConfig.AltUrls = destURLs.GetRange(1, destURLs.Count - 1).ToArray();

            destination.SetKPRPCConfig(destConfig);
            destination.Touch(true);
        }

        private static void MergeInNewURLs(List<string> destURLs, List<string> sourceURLs)
        {
            if (sourceURLs.Count > 0)
            {
                for (int i = sourceURLs.Count - 1; i >= 0; i--)
                {
                    string sourceUrl = sourceURLs[i];

                    if (!destURLs.Contains(sourceUrl))
                    {
                        destURLs.Insert(0, sourceUrl);
                    }
                    else if (i == 0)
                    {
                        // Promote the URL from alternative URL list to primary URL
                        destURLs.Remove(sourceUrl);
                        destURLs.Insert(0, sourceUrl);
                    }
                }
            }
        }
        
        private PwDatabase SelectDatabase(string dbFileName)
        {
            PwDatabase chosenDB = _host.Database;
            if (!string.IsNullOrEmpty(dbFileName))
            {
                try
                {
                    List<PwDatabase> allDBs = _host.MainWindow.DocumentManager.GetOpenDatabases();
                    foreach (PwDatabase db in allDBs)
                        if (db.IOConnectionInfo.Path == dbFileName)
                        {
                            chosenDB = db;
                            break;
                        }
                }
                catch (Exception)
                {
                    // If we fail to find a suitable DB for any reason we'll just continue as if no restriction had been requested
                }
            }

            return chosenDB;
        }

        /// <summary>
        /// Return the root group of the active database for the current location
        /// </summary>
        /// <returns>the root group</returns>
        private PwGroup GetRootPwGroup(PwDatabase pwd)
        {
            string locationId = _host.CustomConfig
                .GetString("KeePassRPC.currentLocation", "");
            return GetRootPwGroup(pwd, locationId);
        }
    }
}
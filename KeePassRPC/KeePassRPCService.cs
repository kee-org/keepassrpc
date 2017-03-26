/*
  KeePassRPC - Uses JSON-RPC to provide RPC facilities to KeePass.
  Example usage includes the KeeFox firefox extension.
  
  Copyright 2010-2015 Chris Tomlinson <keefox@christomlinson.name>

  This program is free software; you can redistribute it and/or modify
  it under the terms of the GNU General Public License as published by
  the Free Software Foundation; either version 2 of the License, or
  (at your option) any later version.

  This program is distributed in the hope that it will be useful,
  but WITHOUT ANY WARRANTY; without even the implied warranty of
  MERCHANTABILITY or FITNESS FOR A PARTICULAR PURPOSE.  See the
  GNU General Public License for more details.

  You should have received a copy of the GNU General Public License
  along with this program; if not, write to the Free Software
  Foundation, Inc., 51 Franklin St, Fifth Floor, Boston, MA  02110-1301  USA
*/

using System;
using System.Collections.Generic;
using Jayrock.JsonRpc;
using KeePassRPC.DataExchangeModel;
using System.Windows.Forms;
using KeePass.Forms;
using KeePassLib;
using System.Collections;
using System.Drawing;
using KeePass.Resources;
using KeePassLib.Serialization;
using System.IO;
using KeePassLib.Security;
using KeePass.Plugins;
using KeePassLib.Cryptography.PasswordGenerator;
using System.Diagnostics;
using System.Reflection;
using KeePass.UI;

namespace KeePassRPC
{
    /// <summary>
    /// Provides an externally accessible API for common KeePass operations
    /// </summary>
    public class KeePassRPCService : JsonRpcService
    {
        #region Class variables, constructor and destructor

        KeePassRPCExt KeePassRPCPlugin;
        Version PluginVersion;
        IPluginHost host;

        private string[] _standardIconsBase64;
        private bool _restartWarningShown = false;

        public KeePassRPCService(IPluginHost host, string[] standardIconsBase64, KeePassRPCExt plugin)
        {
            KeePassRPCPlugin = plugin;
            PluginVersion = KeePassRPCExt.PluginVersion;
            this.host = host;
            _standardIconsBase64 = standardIconsBase64;
        }
        #endregion

        #region PLGX install detection 
        //TODO:1.6: (this stopped working in KeeFox 1.3 - maybe we can re-enable it now?)

        public void ListenForPLGXChanges()
        {
            Type mwType = host.MainWindow.GetType();
            FieldInfo fiPM = mwType.GetField("m_pluginManager", BindingFlags.Instance | BindingFlags.NonPublic);
            object pm = fiPM.GetValue(host.MainWindow);


            Type pmType = pm.GetType();
            FieldInfo fiPI = pmType.GetField("m_vPlugins", BindingFlags.Instance | BindingFlags.NonPublic);
            IList ilist = (IList)fiPI.GetValue(pm);


            foreach (object pl in ilist)
            {
                Type pluginInfoType = pl.GetType();
                PropertyInfo piName = pluginInfoType.GetProperty("Name", BindingFlags.Instance | BindingFlags.Public);
                string name = (string)piName.GetValue(pl, null);

                if (name == "KeePassRPC")
                {
                    PropertyInfo piDisplayFilePath = pluginInfoType.GetProperty("DisplayFilePath", BindingFlags.Instance | BindingFlags.Public);
                    string displayFilePath = (string)piDisplayFilePath.GetValue(pl, null);
                    PropertyInfo piFileVersion = pluginInfoType.GetProperty("FileVersion", BindingFlags.Instance | BindingFlags.Public);
                    string fileVersion = (string)piFileVersion.GetValue(pl, null);
                    ListenForPLGXChanges(displayFilePath, fileVersion);
                    break;
                }
            }
        }

        private void ListenForPLGXChanges(string displayFilePath, string fileVersion)
        {
            if (!displayFilePath.EndsWith(".plgx"))
                return;

            FileInfo fi = new FileInfo(displayFilePath);
            FileSystemWatcher watcher = new FileSystemWatcher();
            watcher.Path = fi.DirectoryName;
            watcher.NotifyFilter = NotifyFilters.LastWrite | NotifyFilters.FileName | NotifyFilters.DirectoryName;
            watcher.Filter = fi.Name;

            watcher.Changed += new FileSystemEventHandler(OnPLGXFileChanged);
            watcher.Created += new FileSystemEventHandler(OnPLGXFileChanged);

            watcher.EnableRaisingEvents = true;
        }

        private void OnPLGXFileChanged(object source, FileSystemEventArgs e)
        {
            if (!_restartWarningShown)
                KeePassLib.Utility.MessageService.ShowInfo("Please restart KeePass for the upgrade to take effect. If KeeFox does not detect KeePass within 10 seconds of KeePass restarting, please also restart Firefox.");
            _restartWarningShown = true;
            ((FileSystemWatcher)source).EnableRaisingEvents = false;
        }

        #endregion

        #region KeePass GUI routines

        /// <summary>
        /// Halts thread until a DB is open in the KeePass application
        /// </summary>
        /// <remarks>This simple thread sync may not work if more than one RPC client gets involved.</remarks>
        private bool ensureDBisOpen()
        {

            if (!host.Database.IsOpen)
            {
                //ensureDBisOpenEWH.Reset(); // ensures we will wait even if DB has been opened previously.
                // maybe tiny opportunity for deadlock if user opens DB exactly between DB.IsOpen and this statement?
                // TODO2: consider moving above statement to top of method - shouldn't do any harm and could rule out rare deadlock?
                host.MainWindow.BeginInvoke((MethodInvoker)delegate { promptUserToOpenDB(null); });
                //ensureDBisOpenEWH.WaitOne(15000, false); // wait until DB has been opened

                if (!host.Database.IsOpen)
                    return false;
            }
            return true;
        }

        void promptUserToOpenDB(IOConnectionInfo ioci)
        {
            //TODO: find out z-index of firefox and push keepass just behind it rather than right to the back
            //TODO: focus open DB dialog box if it's there

            IntPtr ffWindow = Native.GetForegroundWindow();
            bool minimised = KeePass.Program.MainForm.WindowState == FormWindowState.Minimized;
            bool trayed = KeePass.Program.MainForm.IsTrayed();

            if (ioci == null)
                ioci = KeePass.Program.Config.Application.LastUsedFile;

            Native.AttachToActiveAndBringToForeground(KeePass.Program.MainForm.Handle);
            KeePass.Program.MainForm.Activate();

            // refresh the UI in case user cancelled the dialog box and/or KeePass native calls have left us in a bit of a weird state
            host.MainWindow.UpdateUI(true, null, true, null, true, null, false);

            // Set the program state back to what is was unless the user has
            // configured "lock on minimise" in which case we always set it to Normal
            if (!KeePass.Program.Config.Security.WorkspaceLocking.LockOnWindowMinimize)
            {
                minimised = false;
                trayed = false;
            }

            KeePass.Program.MainForm.WindowState = minimised ? FormWindowState.Minimized : FormWindowState.Normal;

            if (trayed)
            {
                KeePass.Program.MainForm.Visible = false;
                KeePass.Program.MainForm.UpdateTrayIcon();
            }

            // Make Firefox active again
            Native.EnsureForegroundWindow(ffWindow);
        }

        bool showOpenDB(IOConnectionInfo ioci)
        {
            // KeePass does this on "show window" keypress. Not sure what it does but most likely does no harm to check here too
            if (KeePass.Program.MainForm.UIIsInteractionBlocked()) { return false; }

            // Make sure the login dialog (or options and other windows) are not already visible. Same behaviour as KP.
            if (KeePass.UI.GlobalWindowManager.WindowCount != 0) return false;

            // Prompt user to open database
            KeePass.Program.MainForm.OpenDatabase(ioci, null, false);
            return true;
        }

        private delegate void dlgUpdateUINoSave();

        void updateUINoSave()
        {
            host.MainWindow.UpdateUI(false, null, true, null, true, null, true);
        }

        private delegate void dlgSaveDB(PwDatabase databaseToSave);

        void saveDB(PwDatabase databaseToSave)
        {
            // store current active tab/db
            PwDocument currentActiveDoc = host.MainWindow.DocumentManager.ActiveDocument;

            // change active tab
            PwDocument doc = host.MainWindow.DocumentManager.FindDocument(databaseToSave);
            host.MainWindow.DocumentManager.ActiveDocument = doc;

            if (host.CustomConfig.GetBool("KeePassRPC.KeeFox.autoCommit", true))
            {
                // save active database & update UI appearance
                if (host.MainWindow.UIFileSave(false))
                    host.MainWindow.UpdateUI(false, null, true, null, true, null, false);
            }
            else
            {
                // update ui with "changed" flag                
                host.MainWindow.UpdateUI(false, null, true, null, true, null, true);
            }
            // change tab back
            host.MainWindow.DocumentManager.ActiveDocument = currentActiveDoc;
        }

        void openGroupEditorWindow(PwGroup pg, PwDatabase db)
        {
            GroupForm gf = new GroupForm();
            gf.InitEx(pg, host.MainWindow.ClientIcons, host.Database);

            gf.BringToFront();
            gf.ShowInTaskbar = true;

            host.MainWindow.Focus();
            gf.TopMost = true;
            gf.Focus();
            gf.Activate();
            if (gf.ShowDialog() == DialogResult.OK)
                saveDB(db);
        }

        private delegate void dlgOpenGroupEditorWindow(PwGroup pg, PwDatabase db);

        /// <summary>
        /// Launches the group editor.
        /// </summary>
        /// <param name="uuid">The UUID of the group to edit.</param>
        [JsonRpcMethod]
        public void LaunchGroupEditor(string uuid, string dbFileName)
        {
            // Make sure there is an active database
            if (!ensureDBisOpen()) return;

            // find the database
            PwDatabase db = SelectDatabase(dbFileName);

            if (uuid != null && uuid.Length > 0)
            {
                PwUuid pwuuid = new PwUuid(KeePassLib.Utility.MemUtil.HexStringToByteArray(uuid));

                PwGroup matchedGroup = GetRootPwGroup(db).FindGroup(pwuuid, true);

                if (matchedGroup == null)
                    throw new Exception("Could not find requested entry.");

                host.MainWindow.BeginInvoke(new dlgOpenGroupEditorWindow(openGroupEditorWindow), matchedGroup, db);
            }

        }

        void OpenLoginEditorWindow(PwEntry pe, PwDatabase db)
        {
            PwEntryForm ef = new PwEntryForm();
            ef.InitEx(pe, PwEditMode.EditExistingEntry, host.Database, host.MainWindow.ClientIcons, false, false);

            ef.BringToFront();
            ef.ShowInTaskbar = true;

            host.MainWindow.Focus();
            ef.TopMost = true;
            ef.Focus();
            ef.Activate();

            if (ef.ShowDialog() == DialogResult.OK)
                saveDB(db);
        }

        private delegate void dlgOpenLoginEditorWindow(PwEntry pg, PwDatabase db);

        /// <summary>
        /// Launches the login editor.
        /// </summary>
        /// <param name="uuid">The UUID of the entry to edit.</param>
        [JsonRpcMethod]
        public void LaunchLoginEditor(string uuid, string dbFileName)
        {
            // Make sure there is an active database
            if (!ensureDBisOpen()) return;

            // find the database
            PwDatabase db = SelectDatabase(dbFileName);

            if (uuid != null && uuid.Length > 0)
            {
                PwUuid pwuuid = new PwUuid(KeePassLib.Utility.MemUtil.HexStringToByteArray(uuid));

                PwEntry matchedLogin = GetRootPwGroup(db).FindEntry(pwuuid, true);

                if (matchedLogin == null)
                    throw new Exception("Could not find requested entry.");

                host.MainWindow.BeginInvoke(new dlgOpenLoginEditorWindow(OpenLoginEditorWindow), matchedLogin, db);
            }

        }

        // A similar function is defined in KeePass MainForm_functions.cs but only from 2.18 so to retain compatibility with 2.15 we can't use it
        //TODO:1.6: Don't support < 2.15 anymore
        IOConnectionInfo CompleteConnectionInfoUsingMru(IOConnectionInfo ioc)
        {
            if (string.IsNullOrEmpty(ioc.UserName) && string.IsNullOrEmpty(ioc.Password))
            {
                for (uint u = 0; u < host.MainWindow.FileMruList.ItemCount; ++u)
                {
                    IOConnectionInfo iocMru = (host.MainWindow.FileMruList.GetItem(u).Value as IOConnectionInfo);
                    if (iocMru == null) { continue; }

                    if (iocMru.Path.Equals(ioc.Path, KeePassLib.Utility.StrUtil.CaseIgnoreCmp))
                    {
                        ioc = iocMru.CloneDeep();
                        break;
                    }
                }
            }

            return ioc;
        }

        #endregion

        #region Utility functions to convert between KeePassRPC object schema and KeePass schema

        private LightEntry GetEntryFromPwEntry(PwEntry pwe, int matchAccuracy, bool fullDetails, PwDatabase db)
        {
            return GetEntryFromPwEntry(pwe, matchAccuracy, fullDetails, db, false);
        }

        private LightEntry GetEntryFromPwEntry(PwEntry pwe, int matchAccuracy, bool fullDetails, PwDatabase db, bool abortIfHidden)
        {
            EntryConfig conf = pwe.GetKPRPCConfig();
            if (conf == null)
                return null;
            return GetEntryFromPwEntry(pwe, conf, matchAccuracy, fullDetails, db, abortIfHidden);
        }

        private LightEntry GetEntryFromPwEntry(PwEntry pwe, EntryConfig conf, int matchAccuracy, bool fullDetails, PwDatabase db, bool abortIfHidden)
        {
            ArrayList formFieldList = new ArrayList();
            ArrayList URLs = new ArrayList();
            URLs.Add(pwe.Strings.ReadSafe("URL"));
            bool usernameFound = false;
            bool passwordFound = false;
            bool alwaysAutoFill = false;
            bool neverAutoFill = false;
            bool alwaysAutoSubmit = false;
            bool neverAutoSubmit = false;
            int priority = 0;
            string usernameName = "";
            string usernameValue = "";

            if (abortIfHidden && conf.Hide)
                return null;

            if (!fullDetails)
            {

            }
            else
            {
                if (conf.FormFieldList != null)
                {
                    foreach (FormField ff in conf.FormFieldList)
                    {
                        if (ff.Type == FormFieldType.FFTpassword)
                        {
                            string ffValue = KeePassRPCPlugin.GetPwEntryStringFromDereferencableValue(pwe, ff.Value, db);
                            if (!string.IsNullOrEmpty(ffValue))
                            {
                                formFieldList.Add(new FormField(ff.Name, "KeePass password", ffValue, ff.Type, ff.Id, ff.Page));
                                passwordFound = true;
                            }
                        }
                        else if (ff.Type == FormFieldType.FFTusername)
                        {
                            string ffValue = KeePassRPCPlugin.GetPwEntryStringFromDereferencableValue(pwe, ff.Value, db);
                            if (!string.IsNullOrEmpty(ffValue))
                            {
                                formFieldList.Add(new FormField(ff.Name, "KeePass username", ffValue, ff.Type, ff.Id, ff.Page));
                                usernameFound = true;
                            }
                        }
                        else
                            formFieldList.Add(new FormField(ff.Name, ff.Name, ff.Value, ff.Type, ff.Id, ff.Page));
                    }
                }
            }
            if (conf.AltURLs != null)
                URLs.AddRange(conf.AltURLs);

            // If we didn't find an explicit password field, we assume any value
            // in the KeePass "password" box is what we are looking for
            if (fullDetails && !passwordFound)
            {
                string ffValue = KeePassRPCPlugin.GetPwEntryString(pwe, "Password", db);
                ffValue = KeePassRPCPlugin.GetPwEntryStringFromDereferencableValue(pwe, ffValue, db);
                if (!string.IsNullOrEmpty(ffValue))
                {
                    formFieldList.Add(new FormField("password",
                        "KeePass password", ffValue, FormFieldType.FFTpassword, "password", 1));
                }
            }

            // If we didn't find an explicit username field, we assume any value
            // in the KeePass "username" box is what we are looking for
            if (!usernameFound)
            {
                string ffValue = KeePassRPCPlugin.GetPwEntryString(pwe, "UserName", db);
                ffValue = KeePassRPCPlugin.GetPwEntryStringFromDereferencableValue(pwe, ffValue, db);
                if (!string.IsNullOrEmpty(ffValue))
                {
                    formFieldList.Add(new FormField("username",
                        "KeePass username", ffValue, FormFieldType.FFTusername, "username", 1));
                    usernameName = "username";
                    usernameValue = ffValue;
                }
            }

            string imageData = iconToBase64(pwe.CustomIconUuid, pwe.IconId);
            //Debug.WriteLine("GetEntryFromPwEntry icon converted: " + sw.Elapsed);

            if (fullDetails)
            {
                alwaysAutoFill = conf.AlwaysAutoFill;
                alwaysAutoSubmit = conf.AlwaysAutoSubmit;
                neverAutoFill = conf.NeverAutoFill;
                neverAutoSubmit = conf.NeverAutoSubmit;
                priority = conf.Priority;

            }

            //sw.Stop();
            //Debug.WriteLine("GetEntryFromPwEntry execution time: " + sw.Elapsed);
            //Debug.Unindent();

            if (fullDetails)
            {
                string realm = "";
                if (!string.IsNullOrEmpty(conf.HTTPRealm))
                    realm = conf.HTTPRealm;

                FormField[] temp = (FormField[])formFieldList.ToArray(typeof(FormField));
                Entry kpe = new Entry(
                (string[])URLs.ToArray(typeof(string)), realm,
                pwe.Strings.ReadSafe(PwDefs.TitleField), temp,
                KeePassLib.Utility.MemUtil.ByteArrayToHexString(pwe.Uuid.UuidBytes),
                alwaysAutoFill, neverAutoFill, alwaysAutoSubmit, neverAutoSubmit, priority,
                GetGroupFromPwGroup(pwe.ParentGroup), imageData,
                GetDatabaseFromPwDatabase(db, false, true),matchAccuracy);
                return kpe;
            }
            else
            {
                return new LightEntry((string[])URLs.ToArray(typeof(string)),
                    pwe.Strings.ReadSafe(PwDefs.TitleField),
                    KeePassLib.Utility.MemUtil.ByteArrayToHexString(pwe.Uuid.UuidBytes),
                    imageData, usernameName, usernameValue);
            }
        }

        private Group GetGroupFromPwGroup(PwGroup pwg)
        {
            //Debug.Indent();
            //Stopwatch sw = Stopwatch.StartNew();

            string imageData = iconToBase64(pwg.CustomIconUuid, pwg.IconId);

            Group kpg = new Group(pwg.Name, KeePassLib.Utility.MemUtil.ByteArrayToHexString(pwg.Uuid.UuidBytes), imageData, pwg.GetFullPath("/", false));

            //sw.Stop();
            //Debug.WriteLine("GetGroupFromPwGroup execution time: " + sw.Elapsed);
            //Debug.Unindent();
            return kpg;
        }

        private Database GetDatabaseFromPwDatabase(PwDatabase pwd, bool fullDetail, bool noDetail)
        {
            //Debug.Indent();
            // Stopwatch sw = Stopwatch.StartNew();
            if (fullDetail && noDetail)
                throw new ArgumentException("Don't be silly");

            PwGroup pwg = GetRootPwGroup(pwd);
            Group rt = GetGroupFromPwGroup(pwg);
            if (fullDetail)
                rt.ChildEntries = (Entry[])GetChildEntries(pwd, pwg, fullDetail);
            else if (!noDetail)
                rt.ChildLightEntries = GetChildEntries(pwd, pwg, fullDetail);

            if (!noDetail)
                rt.ChildGroups = GetChildGroups(pwd, pwg, true, fullDetail);

            Database kpd = new Database(pwd.Name, pwd.IOConnectionInfo.Path, rt, (pwd == host.Database) ? true : false,
                DataExchangeModel.IconCache<string>.GetIconEncoding(pwd.IOConnectionInfo.Path) ?? "");
            //  sw.Stop();
            //  Debug.WriteLine("GetDatabaseFromPwDatabase execution time: " + sw.Elapsed);
            //  Debug.Unindent();
            return kpd;
        }

        private void setPwEntryFromEntry(PwEntry pwe, Entry login)
        {
            bool firstPasswordFound = false;
            EntryConfig conf = new EntryConfig();
            List<FormField> ffl = new List<FormField>();

            // Go through each form field, mostly just making a copy but with occasional tweaks such as default username and password selection
            // by convention, we'll always have the first text field as the username when both reading and writing from the EntryConfig
            foreach (FormField kpff in login.FormFieldList)
            {
                if (kpff.Type == FormFieldType.FFTpassword && !firstPasswordFound)
                {
                    ffl.Add(new FormField(kpff.Name, "KeePass password", "{PASSWORD}", kpff.Type, kpff.Id, kpff.Page));
                    pwe.Strings.Set("Password", new ProtectedString(host.Database.MemoryProtection.ProtectPassword, kpff.Value));
                    firstPasswordFound = true;
                }
                else if (kpff.Type == FormFieldType.FFTusername)
                {
                    ffl.Add(new FormField(kpff.Name, "KeePass username", "{USERNAME}", kpff.Type, kpff.Id, kpff.Page));
                    pwe.Strings.Set("UserName", new ProtectedString(host.Database.MemoryProtection.ProtectUserName, kpff.Value));
                }
                else
                {
                    ffl.Add(new FormField(kpff.Name, kpff.Name, kpff.Value, kpff.Type, kpff.Id, kpff.Page));
                }
            }
            conf.FormFieldList = ffl.ToArray();

            List<string> altURLs = new List<string>();

            for (int i = 0; i < login.URLs.Length; i++)
            {
                string url = login.URLs[i];
                if (i == 0)
                {
                    URLSummary urlsum = URLSummary.FromURL(url);

                    // Require more strict default matching for entries that come
                    // with a port configured (user can override in the rare case
                    // that they want the loose domain-level matching)
                    if (!string.IsNullOrEmpty(urlsum.Port))
                        conf.BlockDomainOnlyMatch = true;

                    pwe.Strings.Set("URL", new ProtectedString(host.Database.MemoryProtection.ProtectUrl, url ?? ""));
                }
                else
                    altURLs.Add(url);
            }
            conf.AltURLs = altURLs.ToArray();
            conf.HTTPRealm = login.HTTPRealm;
            conf.Version = 1;

            // Set some of the string fields
            pwe.Strings.Set(PwDefs.TitleField, new ProtectedString(host.Database.MemoryProtection.ProtectTitle, login.Title ?? ""));

            // update the icon for this entry (in most cases we'll 
            // just detect that it is the same standard icon as before)
            PwUuid customIconUUID = PwUuid.Zero;
            PwIcon iconId = PwIcon.Key;
            if (login.IconImageData != null
                && login.IconImageData.Length > 0
                && base64ToIcon(login.IconImageData, ref customIconUUID, ref iconId))
            {
                if (customIconUUID == PwUuid.Zero)
                    pwe.IconId = iconId;
                else
                    pwe.CustomIconUuid = customIconUUID;
            }

            pwe.SetKPRPCConfig(conf);
        }

        private string dbIconToBase64(PwDatabase db)
        {
            string cachedBase64 = DataExchangeModel.IconCache<string>.GetIconEncoding(db.IOConnectionInfo.Path);
            if (string.IsNullOrEmpty(cachedBase64))
            {
                // Don't think this should ever happen but we'll return a null icon if we have to
                return "";
            }
            else
            {
                return cachedBase64;
            }
        }

        /// <summary>
        /// extract the current icon information for this entry
        /// </summary>
        /// <param name="customIconUUID"></param>
        /// <param name="iconId"></param>
        /// <returns></returns>
        private string iconToBase64(PwUuid customIconUUID, PwIcon iconId)
        {
            Image icon = null;
            PwUuid uuid = null;

            string imageData = "";
            if (customIconUUID != PwUuid.Zero)
            {
                string cachedBase64 = DataExchangeModel.IconCache<PwUuid>.GetIconEncoding(customIconUUID);
                if (string.IsNullOrEmpty(cachedBase64))
                {
                    object[] delParams = { customIconUUID };
                    object invokeResult = host.MainWindow.Invoke(
                        new KeePassRPCExt.GetCustomIconDelegate(
                            KeePassRPCPlugin.GetCustomIcon), delParams);
                    if (invokeResult != null)
                    {
                        icon = (Image)invokeResult;
                    }
                    if (icon != null)
                    {
                        uuid = customIconUUID;
                    }
                }
                else
                {
                    return cachedBase64;
                }
            }

            // this happens if we didn't want to or couldn't find a custom icon
            if (icon == null)
            {
                int iconIdInt = (int)iconId;
                uuid = new PwUuid(new byte[]{
                    (byte)(iconIdInt & 0xFF), (byte)(iconIdInt & 0xFF),
                    (byte)(iconIdInt & 0xFF), (byte)(iconIdInt & 0xFF),
                    (byte)(iconIdInt >> 8 & 0xFF), (byte)(iconIdInt >> 8 & 0xFF),
                    (byte)(iconIdInt >> 8 & 0xFF), (byte)(iconIdInt >> 8 & 0xFF),
                    (byte)(iconIdInt >> 16 & 0xFF), (byte)(iconIdInt >> 16 & 0xFF),
                    (byte)(iconIdInt >> 16 & 0xFF), (byte)(iconIdInt >> 16 & 0xFF),
                    (byte)(iconIdInt >> 24 & 0xFF), (byte)(iconIdInt >> 24 & 0xFF),
                    (byte)(iconIdInt >> 24 & 0xFF), (byte)(iconIdInt >> 24 & 0xFF)
                });

                string cachedBase64 = DataExchangeModel.IconCache<PwUuid>.GetIconEncoding(uuid);
                if (string.IsNullOrEmpty(cachedBase64))
                {
                    object[] delParams = { (int)iconId };
                    object invokeResult = host.MainWindow.Invoke(
                        new KeePassRPCExt.GetIconDelegate(
                            KeePassRPCPlugin.GetIcon), delParams);
                    if (invokeResult != null)
                    {
                        icon = (Image)invokeResult;
                    }
                }
                else
                {
                    return cachedBase64;
                }
            }


            if (icon != null)
            {
                // we found an icon but it wasn't in the cache so lets
                // calculate its base64 encoding and then add it to the cache
                MemoryStream ms = new MemoryStream();
                icon.Save(ms, System.Drawing.Imaging.ImageFormat.Png);
                imageData = Convert.ToBase64String(ms.ToArray());
                DataExchangeModel.IconCache<PwUuid>.AddIcon(uuid, imageData);
            }

            return imageData;
        }

        /// <summary>
        /// converts a string to the relevant icon for this entry
        /// </summary>
        /// <param name="imageData">base64 representation of the image</param>
        /// <param name="customIconUUID">UUID of the generated custom icon; may be Zero</param>
        /// <param name="iconId">PwIcon of the matched standard icon; ignore if customIconUUID != Zero</param>
        /// <returns>true if the supplied imageData was converted into a customIcon 
        /// or matched with a standard icon.</returns>
        private bool base64ToIcon(string imageData, ref PwUuid customIconUUID, ref PwIcon iconId)
        {
            iconId = PwIcon.Key;
            customIconUUID = PwUuid.Zero;

            for (int i = 0; i < _standardIconsBase64.Length; i++)
            {
                string item = _standardIconsBase64[i];
                if (item == imageData)
                {
                    iconId = (PwIcon)i;
                    return true;
                }
            }

            try
            {
                //MemoryStream id = new MemoryStream();
                //icon.Save(ms, System.Drawing.Imaging.ImageFormat.Png);

                Image img = KeePass.UI.UIUtil.LoadImage(Convert.FromBase64String(imageData));

                Image imgNew = new Bitmap(img, new Size(16, 16));

                MemoryStream ms = new MemoryStream();
                imgNew.Save(ms, System.Drawing.Imaging.ImageFormat.Png);

                byte[] msByteArray = ms.ToArray();

                foreach (PwCustomIcon item in host.Database.CustomIcons)
                {
                    // re-use existing custom icon if it's already in the database
                    // (This will probably fail if database is used on 
                    // both 32 bit and 64 bit machines - not sure why...)
                    if (KeePassLib.Utility.MemUtil.ArraysEqual(msByteArray, item.ImageDataPng))
                    {
                        customIconUUID = item.Uuid;
                        host.Database.UINeedsIconUpdate = true;
                        return true;
                    }
                }

                PwCustomIcon pwci = new PwCustomIcon(new PwUuid(true),
                    msByteArray);
                host.Database.CustomIcons.Add(pwci);

                customIconUUID = pwci.Uuid;
                host.Database.UINeedsIconUpdate = true;

                return true;
            }
            catch
            {
                return false;
            }
        }

        #endregion

        #region Configuration of KeePass/KeeFox and databases

        [JsonRpcMethod]
        public Configuration GetCurrentKFConfig()
        {
            bool autoCommit = host.CustomConfig.GetBool("KeePassRPC.KeeFox.autoCommit", true);
            string[] MRUList = new string[host.MainWindow.FileMruList.ItemCount];
            for (uint i = 0; i < host.MainWindow.FileMruList.ItemCount; i++)
                MRUList[i] = ((IOConnectionInfo)host.MainWindow.FileMruList.GetItem(i).Value).Path;

            Configuration currentConfig = new Configuration(MRUList, autoCommit);
            return currentConfig;
        }

        [JsonRpcMethod]
        public ApplicationMetadata GetApplicationMetadata()
        {
            string KeePassVersion;
            bool IsMono = false;
            string NETCLR;
            string NETversion;
            string MonoVersion = "unknown";
            // No point in outputting KeePassRPC version here since we know it has
            // to match in order to be able to call this function
            
            NETCLR = Environment.Version.Major.ToString();
            KeePassVersion = PwDefs.VersionString;

            Type type = Type.GetType("Mono.Runtime");
            if (type != null)
            {
                IsMono = true;
                NETversion = "";
                try
                {
                    MethodInfo displayName = type.GetMethod("GetDisplayName",
                        BindingFlags.NonPublic | BindingFlags.Static);
                    if (displayName != null)
                        MonoVersion = (string)displayName.Invoke(null, null);
                }
                catch (Exception)
                {
                    MonoVersion = "unknown";
                }
            }
            else
            {
                // Normally looking in the registry is the thing to try here but that means pulling
                // in lots of Win32 libraries into Mono so this alternative gets us some useful,
                // albeit incomplete, information. There shouldn't be any need to call this service
                // on a regular basis so it shouldn't matter that the use of reflection is a little inefficient

                // v3.0 is of no interest to us and difficult to detect so we ignore
                // it and bundle those users in the v2 group
                NETversion = 
                    IsNet451OrNewer() ? "4.5.1" : 
                    IsNet45OrNewer() ? "4.5" : 
                    NETCLR == "4" ? "4.0" : 
                    IsNet35OrNewer() ? "3.5" :
                    NETCLR == "2" ? "2.0" : 
                    "unknown";
            }

            ApplicationMetadata appMetadata = new ApplicationMetadata(KeePassVersion, IsMono, NETCLR, NETversion, MonoVersion);
            return appMetadata;
        }

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
        //TODO:1.6: Newer .NET versions

        #endregion

        #region Retrival and manipulation of databases and the KeePass app

        [JsonRpcMethod]
        public string GetDatabaseName()
        {
            if (!host.Database.IsOpen)
                return "";
            return (host.Database.Name.Length > 0 ? host.Database.Name : "no name");
        }

        [JsonRpcMethod]
        public string GetDatabaseFileName()
        {
            return host.Database.IOConnectionInfo.Path;
        }

        /// <summary>
        /// changes current active database
        /// </summary>
        /// <param name="fileName">Path to database to open. If empty, user is prompted to choose a file</param>
        /// <param name="closeCurrent">if true, currently active database is closed first. if false,
        /// both stay open with fileName DB active</param>
        [JsonRpcMethod]
        public void ChangeDatabase(string fileName, bool closeCurrent)
        {
            if (closeCurrent && host.MainWindow.DocumentManager.ActiveDatabase != null && host.MainWindow.DocumentManager.ActiveDatabase.IsOpen)
            {
                host.MainWindow.DocumentManager.CloseDatabase(host.MainWindow.DocumentManager.ActiveDatabase);
            }

            KeePassLib.Serialization.IOConnectionInfo ioci = null;

            if (fileName != null && fileName.Length > 0)
            {
                ioci = new KeePassLib.Serialization.IOConnectionInfo();
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
                foreach (PwDocument doc in host.MainWindow.DocumentManager.Documents)
                    if (doc.LockedIoc.Path == fileName ||
                        (doc.Database.IsOpen && doc.Database.IOConnectionInfo.Path == fileName))
                        host.MainWindow.DocumentManager.ActiveDocument = doc;

            // Going to take a new approach for a bit to see how it works out...
            //
            // before explicitly asking user to log into the correct DB we'll set up a "fake" document in KeePass
            // in the hope that the minimise/restore trick will get KeePass to prompt the user on our behalf
            // (regardless of state of existing documents and newly requested document)
            if (ioci != null
                && !(host.MainWindow.DocumentManager.ActiveDocument.Database.IsOpen && host.MainWindow.DocumentManager.ActiveDocument.Database.IOConnectionInfo.Path == fileName)
                && !(!host.MainWindow.DocumentManager.ActiveDocument.Database.IsOpen && host.MainWindow.DocumentManager.ActiveDocument.LockedIoc.Path == fileName))
            {
                PwDocument doc = host.MainWindow.DocumentManager.CreateNewDocument(true);
                //IOConnectionInfo ioci = new IOConnectionInfo();
                //ioci.Path = fileName;
                doc.LockedIoc = ioci;
            }

            // NB: going to modify implementation of the following function call so that only KeePass initiates the prompt (need to verify cross-platform, etc. even if it seems to work on win7x64)
            // if it works on some platforms, I will make it work on all platforms that support it and fall back to the old clunky method for others.
            host.MainWindow.BeginInvoke((MethodInvoker)delegate { promptUserToOpenDB(ioci); });
            return;
        }

        /// <summary>
        /// notifies KeePass of a change in current location. The location in the KeePass config file
        /// is updated and current databse state is modified if applicable
        /// </summary>
        /// <param name="locationId">New location identifier (e.g. "work", "home") Case insensitive</param>
        [JsonRpcMethod]
        public void ChangeLocation(string locationId)
        {
            if (string.IsNullOrEmpty(locationId))
                return;
            locationId = locationId.ToLower();

            host.CustomConfig.SetString("KeePassRPC.currentLocation", locationId);
            host.MainWindow.Invoke((MethodInvoker)delegate { host.MainWindow.SaveConfig(); });

            // tell all RPC clients they need to refresh their representation of the KeePass data
            if (host.Database.IsOpen)
                KeePassRPCPlugin.SignalAllManagedRPCClients(Signal.DATABASE_SELECTED);

            return;
        }

        /// <summary>
        /// Gets a list of all password profiles available in the current KeePass instance
        /// </summary>
        [JsonRpcMethod]
        public string[] GetPasswordProfiles()
        {
            List<PwProfile> profiles = KeePass.Util.PwGeneratorUtil.GetAllProfiles(true);
            List<string> profileNames = new List<string>(profiles.Count);
            foreach (PwProfile prof in profiles)
                profileNames.Add(prof.Name);

            return profileNames.ToArray();
        }

        [JsonRpcMethod]
        public string GeneratePassword(string profileName, string url)
        {
            PwProfile profile = null;
            
            if (string.IsNullOrEmpty(profileName))
                profile = KeePass.Program.Config.PasswordGenerator.LastUsedProfile;
            else
            {
                foreach (PwProfile pp in KeePass.Util.PwGeneratorUtil.GetAllProfiles(false))
                {
                    if (pp.Name == profileName)
                    {
                        profile = pp;
                        KeePass.Program.Config.PasswordGenerator.LastUsedProfile = pp;
                        break;
                    }
                }
            }

            if (profile == null)
                return "";

            ProtectedString newPassword = new ProtectedString();
            PwgError result = PwgError.Unknown; // PwGenerator.Generate(out newPassword, profile, null, null);

            MethodInfo mi;

            // Generate method signature changed in KP 2.18 so we use
            // reflection to enable support for both 2.18 and earlier versions
            //TODO:1.6: drop support for 2.18
            Type[] mitypes218 = new Type[] { typeof(ProtectedString).MakeByRefType(), typeof(PwProfile), typeof(byte[]), typeof(CustomPwGeneratorPool) };

            try
            {
                mi = typeof(PwGenerator).GetMethod(
                    "Generate",
                    BindingFlags.Public | BindingFlags.Static,
                    Type.DefaultBinder,
                    mitypes218,
                    null
                );

                object[] inputParameters = new object[] { null, profile, null, this.host.PwGeneratorPool };
                result = (PwgError)mi.Invoke(null, inputParameters);
                newPassword = (ProtectedString)inputParameters[0];
            }
            catch (Exception)
            {
                Type[] mitypes217 = new Type[] { typeof(ProtectedString), typeof(PwProfile), typeof(byte[]), typeof(CustomPwGeneratorPool) };
                // can't find the 2.18 method definition so try for an earlier version
                mi = typeof(PwGenerator).GetMethod(
                    "Generate",
                    BindingFlags.Public | BindingFlags.Static,
                    Type.DefaultBinder,
                    mitypes217,
                    null
                );

                object[] inputParameters = new object[] { newPassword, profile, null, this.host.PwGeneratorPool };
                result = (PwgError)mi.Invoke(null, inputParameters);

                // If an exception is thrown here it would be unexpected and
                // require a new version of the application to be released
            }

            var password = newPassword.ReadString();
            if (host.CustomConfig.GetBool("KeePassRPC.KeeFox.backupNewPasswords", true))
                AddPasswordBackupLogin(password, url);

            if (result == PwgError.Success)
                return password;
            else
                return "";
        }

        private void AddPasswordBackupLogin(string password, string url)
        {
            if (!host.Database.IsOpen)
                return;

            PwDatabase chosenDB = SelectDatabase("");
            var parentGroup = KeePassRPCPlugin.GetAndInstallKeeFoxPasswordBackupGroup(chosenDB);

            PwEntry newLogin = new PwEntry(true, true);
            newLogin.Strings.Set(PwDefs.TitleField, new ProtectedString(
                chosenDB.MemoryProtection.ProtectTitle, "KeeFox generated password at: " + DateTime.Now));
            newLogin.Strings.Set(PwDefs.UrlField, new ProtectedString(
                chosenDB.MemoryProtection.ProtectUrl, url));
            newLogin.Strings.Set(PwDefs.PasswordField, new ProtectedString(
                chosenDB.MemoryProtection.ProtectPassword, password));
            EntryConfig conf = new EntryConfig();
            conf.BlockDomainOnlyMatch = true;
            conf.Hide = true;
            newLogin.SetKPRPCConfig(conf);
            parentGroup.AddEntry(newLogin, true);

            // We can't save the database at this point because KeePass steals
            // window focus while saving; that breaks Firefox's Australis UI panels.
            host.MainWindow.BeginInvoke(new dlgUpdateUINoSave(updateUINoSave));

            return;
        }

        #endregion

        #region Retrival and manipulation of entries and groups

        /// <summary>
        /// removes a single entry from the database
        /// </summary>
        /// <param name="uuid">The unique indentifier of the entry we want to remove</param>
        /// <returns>true if entry removed successfully, false if it failed</returns>
        [JsonRpcMethod]
        public bool RemoveEntry(string uuid)
        {
            // Make sure there is an active database
            if (!ensureDBisOpen()) return false;

            if (uuid != null && uuid.Length > 0)
            {
                PwUuid pwuuid = new PwUuid(KeePassLib.Utility.MemUtil.HexStringToByteArray(uuid));

                PwEntry matchedLogin = GetRootPwGroup(host.Database).FindEntry(pwuuid, true);

                if (matchedLogin == null)
                    throw new Exception("Could not find requested entry.");

                PwGroup matchedLoginParent = matchedLogin.ParentGroup;
                if (matchedLoginParent == null) return false; // Can't remove

                matchedLoginParent.Entries.Remove(matchedLogin);

                PwGroup recycleBin = host.Database.RootGroup.FindGroup(host.Database.RecycleBinUuid, true);

                if (host.Database.RecycleBinEnabled == false)
                {
                    if (!KeePassLib.Utility.MessageService.AskYesNo(KPRes.DeleteEntriesQuestionSingle, KPRes.DeleteEntriesTitleSingle))
                        return false;

                    PwDeletedObject pdo = new PwDeletedObject();
                    pdo.Uuid = matchedLogin.Uuid;
                    pdo.DeletionTime = DateTime.Now;
                    host.Database.DeletedObjects.Add(pdo);
                }
                else
                {
                    if (recycleBin == null)
                    {
                        recycleBin = new PwGroup(true, true, KPRes.RecycleBin, PwIcon.TrashBin);
                        recycleBin.EnableAutoType = false;
                        recycleBin.EnableSearching = false;
                        host.Database.RootGroup.AddGroup(recycleBin, true);

                        host.Database.RecycleBinUuid = recycleBin.Uuid;
                    }

                    recycleBin.AddEntry(matchedLogin, true);
                    matchedLogin.Touch(false);
                }

                //matchedLogin.ParentGroup.Entries.Remove(matchedLogin);
                host.MainWindow.BeginInvoke(new dlgSaveDB(saveDB), host.Database);
                return true;
            }
            return false;
        }

        /// <summary>
        /// removes a single group and its contents from the database
        /// </summary>
        /// <param name="uuid">The unique indentifier of the group we want to remove</param>
        /// <returns>true if group removed successfully, false if it failed</returns>
        [JsonRpcMethod]
        public bool RemoveGroup(string uuid)
        {
            // Make sure there is an active database
            if (!ensureDBisOpen()) return false;

            if (uuid != null && uuid.Length > 0)
            {
                PwUuid pwuuid = new PwUuid(KeePassLib.Utility.MemUtil.HexStringToByteArray(uuid));

                PwGroup matchedGroup = GetRootPwGroup(host.Database).FindGroup(pwuuid, true);

                if (matchedGroup == null)
                    throw new Exception("Could not find requested entry.");

                PwGroup matchedGroupParent = matchedGroup.ParentGroup;
                if (matchedGroupParent == null) return false; // Can't remove

                matchedGroupParent.Groups.Remove(matchedGroup);

                PwGroup recycleBin = host.Database.RootGroup.FindGroup(host.Database.RecycleBinUuid, true);

                if (host.Database.RecycleBinEnabled == false)
                {
                    if (!KeePassLib.Utility.MessageService.AskYesNo(KPRes.DeleteGroupQuestion, KPRes.DeleteGroupTitle))
                        return false;

                    PwDeletedObject pdo = new PwDeletedObject();
                    pdo.Uuid = matchedGroup.Uuid;
                    pdo.DeletionTime = DateTime.Now;
                    host.Database.DeletedObjects.Add(pdo);
                }
                else
                {
                    if (recycleBin == null)
                    {
                        recycleBin = new PwGroup(true, true, KPRes.RecycleBin, PwIcon.TrashBin);
                        recycleBin.EnableAutoType = false;
                        recycleBin.EnableSearching = false;
                        host.Database.RootGroup.AddGroup(recycleBin, true);

                        host.Database.RecycleBinUuid = recycleBin.Uuid;
                    }

                    recycleBin.AddGroup(matchedGroup, true);
                    matchedGroup.Touch(false);
                }

                host.MainWindow.BeginInvoke(new dlgSaveDB(saveDB), host.Database);

                return true;
            }
            return false;
        }

        private PwDatabase SelectDatabase(string dbFileName)
        {
            PwDatabase chosenDB = host.Database;
            if (!string.IsNullOrEmpty(dbFileName))
            {
                try
                {
                    List<PwDatabase> allDBs = host.MainWindow.DocumentManager.GetOpenDatabases();
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
        /// Add a new password/login to the active KeePass database
        /// </summary>
        /// <param name="login">The KeePassRPC representation of the login to be added</param>
        /// <param name="parentUUID">The UUID of the parent group for the new login. If null, the root group will be used.</param>
        /// <param name="dbFileName">The file name of the database we want to save this entry to;
        ///                         if empty or null, the currently active database is used</param>
        [JsonRpcMethod]
        public Entry AddLogin(Entry login, string parentUUID, string dbFileName)
        {
            // Make sure there is an active database
            if (!ensureDBisOpen()) return null;

            PwEntry newLogin = new PwEntry(true, true);

            setPwEntryFromEntry(newLogin, login);

            // find the database
            PwDatabase chosenDB = SelectDatabase(dbFileName);

            PwGroup parentGroup = GetRootPwGroup(chosenDB); // if in doubt we'll stick it in the root folder

            if (parentUUID != null && parentUUID.Length > 0)
            {
                PwUuid pwuuid = new PwUuid(KeePassLib.Utility.MemUtil.HexStringToByteArray(parentUUID));

                PwGroup matchedGroup = GetRootPwGroup(chosenDB).FindGroup(pwuuid, true);

                if (matchedGroup != null)
                    parentGroup = matchedGroup;
            }

            parentGroup.AddEntry(newLogin, true);

            if (host.CustomConfig.GetBool("KeePassRPC.KeeFox.editNewEntries", false))
                host.MainWindow.BeginInvoke(new dlgOpenLoginEditorWindow(OpenLoginEditorWindow), newLogin, chosenDB);
            else
                host.MainWindow.BeginInvoke(new dlgSaveDB(saveDB), chosenDB);

            Entry output = (Entry)GetEntryFromPwEntry(newLogin, MatchAccuracy.Best, true, chosenDB);

            return output;
        }

        /// <summary>
        /// Add a new group/folder to the active KeePass database
        /// </summary>
        /// <param name="name">The name of the group to be added</param>
        /// <param name="parentUUID">The UUID of the parent group for the new group. If null, the root group will be used.</param>
        /// <param name="current__"></param>
        [JsonRpcMethod]
        public Group AddGroup(string name, string parentUUID)
        {
            // Make sure there is an active database
            if (!ensureDBisOpen()) return null;

            PwGroup newGroup = new PwGroup(true, true);
            newGroup.Name = name;

            PwGroup parentGroup = GetRootPwGroup(host.Database); // if in doubt we'll stick it in the root folder

            if (parentUUID != null && parentUUID.Length > 0)
            {
                PwUuid pwuuid = new PwUuid(KeePassLib.Utility.MemUtil.HexStringToByteArray(parentUUID));

                PwGroup matchedGroup = host.Database.RootGroup.Uuid == pwuuid ? host.Database.RootGroup : host.Database.RootGroup.FindGroup(pwuuid, true);

                if (matchedGroup != null)
                    parentGroup = matchedGroup;
            }

            parentGroup.AddGroup(newGroup, true);

            host.MainWindow.BeginInvoke(new dlgSaveDB(saveDB), host.Database);

            Group output = GetGroupFromPwGroup(newGroup);

            return output;
        }

        /// <summary>
        /// Updates an existing login
        /// </summary>
        /// <param name="login">A login that contains data to be copied into the existing login</param>
        /// <param name="oldLoginUUID">The UUID that identifies the login we want to update</param>
        /// <param name="urlMergeMode">1= Replace the entry's URL (but still fill forms if you visit the old URL)
        ///2= Replace the entry's URL (delete the old URL completely)
        ///3= Keep the old entry's URL (but still fill forms if you visit the new URL)
        ///4= Keep the old entry's URL (don't add the new URL to the entry)</param>
        /// <param name="dbFileName">Database that contains the login to update</param>
        /// <returns>The updated login</returns>
        [JsonRpcMethod]
        public Entry UpdateLogin(Entry login, string oldLoginUUID, int urlMergeMode, string dbFileName)
        {
            if (login == null)
                throw new ArgumentException("(new) login was not passed to the updateLogin function");
            if (string.IsNullOrEmpty(oldLoginUUID))
                throw new ArgumentException("oldLoginUUID was not passed to the updateLogin function");
            if (string.IsNullOrEmpty(dbFileName))
                throw new ArgumentException("dbFileName was not passed to the updateLogin function");
            
            // Make sure there is an active database
            if (!ensureDBisOpen()) return null;

            // There are odd bits of the resulting new login that we don't
            // need but the vast majority is going to be useful
            PwEntry newLoginData = new PwEntry(true, true);
            setPwEntryFromEntry(newLoginData, login);

            // find the database
            PwDatabase chosenDB = SelectDatabase(dbFileName);

            PwUuid pwuuid = new PwUuid(KeePassLib.Utility.MemUtil.HexStringToByteArray(oldLoginUUID));
            PwEntry entryToUpdate = GetRootPwGroup(chosenDB).FindEntry(pwuuid, true);
            if (entryToUpdate == null)
                throw new Exception("oldLoginUUID could not be resolved to an existing entry.");

            MergeEntries(entryToUpdate, newLoginData, urlMergeMode, chosenDB);

            host.MainWindow.BeginInvoke(new dlgSaveDB(saveDB), chosenDB);

            Entry updatedEntry = (Entry)GetEntryFromPwEntry(entryToUpdate, MatchAccuracy.Best, true, chosenDB);

            return updatedEntry;
        }

        private void MergeEntries(PwEntry destination, PwEntry source, int urlMergeMode, PwDatabase db)
        {
            EntryConfig destConfig = destination.GetKPRPCConfig();
            if (destConfig == null)
                return;

            EntryConfig sourceConfig = source.GetKPRPCConfig();
            if (sourceConfig == null)
                return;

            destination.CreateBackup(db);

            destConfig.HTTPRealm = sourceConfig.HTTPRealm;
            destination.IconId = source.IconId;
            destination.CustomIconUuid = source.CustomIconUuid;
            destination.Strings.Set("UserName", new ProtectedString(
                host.Database.MemoryProtection.ProtectUserName, source.Strings.ReadSafe("UserName")));
            destination.Strings.Set("Password", new ProtectedString(
                host.Database.MemoryProtection.ProtectPassword, source.Strings.ReadSafe("Password")));
            destConfig.FormFieldList = sourceConfig.FormFieldList;

            // This algorithm could probably be made more efficient (lots of O(n) operations
            // but we're dealing with pretty small n so I've gone with the conceptually
            // easiest approach for now).

            List<string> destURLs = new List<string>();
            destURLs.Add(destination.Strings.ReadSafe("URL"));
            if (destConfig.AltURLs != null)
                destURLs.AddRange(destConfig.AltURLs);

            List<string> sourceURLs = new List<string>();
            sourceURLs.Add(source.Strings.ReadSafe("URL"));
            if (sourceConfig.AltURLs != null)
                sourceURLs.AddRange(sourceConfig.AltURLs);

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
                default:
                    // No changes to URLs
                    break;
            }

            // These might not have changed but meh
            destination.Strings.Set("URL", new ProtectedString(host.Database.MemoryProtection.ProtectUrl, destURLs[0]));
            destConfig.AltURLs = new string[0];
            if (destURLs.Count > 1)
                destConfig.AltURLs = destURLs.GetRange(1,destURLs.Count-1).ToArray();

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

        /// <summary>
        /// Return the parent group of the object with the supplied UUID
        /// </summary>
        /// <param name="uuid">the UUID of the object we want to find the parent of</param>
        /// <param name="current__"></param>
        /// <returns>the parent group</returns>
        [JsonRpcMethod]
        public Group GetParent(string uuid)
        {
            Group output;

            // Make sure there is an active database
            if (!ensureDBisOpen()) return null;

            PwUuid pwuuid = new PwUuid(KeePassLib.Utility.MemUtil.HexStringToByteArray(uuid));
            PwGroup rootGroup = GetRootPwGroup(host.Database);

            try
            {

                PwEntry thisEntry = rootGroup.FindEntry(pwuuid, true);
                if (thisEntry != null && thisEntry.ParentGroup != null)
                {
                    output = GetGroupFromPwGroup(thisEntry.ParentGroup);
                    return output;
                }

                PwGroup thisGroup = rootGroup.FindGroup(pwuuid, true);
                if (thisGroup != null && thisGroup.ParentGroup != null)
                {
                    output = GetGroupFromPwGroup(thisGroup.ParentGroup);
                    return output;
                }
            }
            catch (Exception)
            {
                return null;
            }
            output = GetGroupFromPwGroup(rootGroup);
            return output;
        }

        /// <summary>
        /// Return the root group of the active database
        /// </summary>
        /// <param name="current__"></param>
        /// <returns>the root group</returns>
        [JsonRpcMethod]
        public Group GetRoot()
        {
            return GetGroupFromPwGroup(GetRootPwGroup(host.Database));
        }

        /// <summary>
        /// Return the root group of the active database
        /// </summary>
        /// <param name="location">Selects an alternative root group based on KeePass location; null or empty string = default root group</param>
        /// <returns>the root group</returns>
        public PwGroup GetRootPwGroup(PwDatabase pwd, string location)
        {
            if (pwd == null)
                pwd = host.Database;

            if (!string.IsNullOrEmpty(location))
            {
                // If any listed group UUID is found in this database, set it as the KeeFox home group
                string rootGroupsConfig = host.CustomConfig
                    .GetString("KeePassRPC.knownLocations." + location + ".RootGroups", "");
                string[] rootGroups = new string[0];

                if (!string.IsNullOrEmpty(rootGroupsConfig))
                {
                    rootGroups = rootGroupsConfig.Split(',');
                    foreach (string rootGroupId in rootGroups)
                    {
                        PwUuid pwuuid = new PwUuid(KeePassLib.Utility.MemUtil.HexStringToByteArray(rootGroupId));
                        PwGroup matchedGroup = host.Database.RootGroup.Uuid == pwuuid ? host.Database.RootGroup : host.Database.RootGroup.FindGroup(pwuuid, true);

                        if (matchedGroup == null)
                            continue;

                        return matchedGroup;
                    }
                    // If no match found we'll just return the default root group
                }
                // If no locations found we'll just return the default root group
            }

            if (pwd.CustomData.Exists("KeePassRPC.KeeFox.rootUUID") && pwd.CustomData.Get("KeePassRPC.KeeFox.rootUUID").Length == 32)
            {
                string uuid = pwd.CustomData.Get("KeePassRPC.KeeFox.rootUUID");

                PwUuid pwuuid = new PwUuid(KeePassLib.Utility.MemUtil.HexStringToByteArray(uuid));

                PwGroup matchedGroup = pwd.RootGroup.Uuid == pwuuid ? pwd.RootGroup : pwd.RootGroup.FindGroup(pwuuid, true);

                if (matchedGroup == null)
                    throw new Exception("Could not find requested group. Have you deleted your KeeFox start/home group? Set a new one and try again.");

                return matchedGroup;
            }
            else
            {
                return pwd.RootGroup;
            }
        }

        /// <summary>
        /// Return the root group of the active database for the current location
        /// </summary>
        /// <returns>the root group</returns>
        [JsonRpcMethod]
        public PwGroup GetRootPwGroup(PwDatabase pwd)
        {
            string locationId = host.CustomConfig
               .GetString("KeePassRPC.currentLocation", "");
            return GetRootPwGroup(pwd, locationId);
        }

        [JsonRpcMethod]
        public Database[] GetAllDatabases(bool fullDetails)
        {
            Debug.Indent();
            Stopwatch sw = Stopwatch.StartNew();

            List<PwDatabase> dbs = host.MainWindow.DocumentManager.GetOpenDatabases();
            // unless the DB is the wrong version
            dbs = dbs.FindAll(ConfigIsCorrectVersion);
            List<Database> output = new List<Database>(1);

            foreach (PwDatabase db in dbs)
            {
                output.Add(GetDatabaseFromPwDatabase(db, fullDetails, false));
            }
            Database[] dbarray = output.ToArray();
            sw.Stop();
            Debug.WriteLine("GetAllDatabases execution time: " + sw.Elapsed);
            Debug.Unindent();
            return dbarray;
        }

        private bool ConfigIsCorrectVersion(PwDatabase t)
        {
            if (t.CustomData.Exists("KeePassRPC.KeeFox.configVersion") 
                && t.CustomData.Get("KeePassRPC.KeeFox.configVersion") == KeePassRPCPlugin.CurrentConfigVersion)
            {
                return true;
            }
            else
            {
                return false;
            }
        }

        /// <summary>
        /// Return a list of every login in the database
        /// </summary>
        /// <param name="logins">the list of all logins</param>
        /// <param name="current__"></param>
        /// <returns>the number of logins in the list</returns>
        [JsonRpcMethod]
        public Entry[] GetAllLogins()
        {
            int count = 0;
            List<Entry> allEntries = new List<Entry>();

            // Make sure there is an active database
            if (!ensureDBisOpen()) { return null; }

            KeePassLib.Collections.PwObjectList<PwEntry> output;
            output = GetRootPwGroup(host.Database).GetEntries(true);

            foreach (PwEntry pwe in output)
            {
                if (EntryIsInRecycleBin(pwe, host.Database))
                    continue; // ignore if it's in the recycle bin

                if (string.IsNullOrEmpty(pwe.Strings.ReadSafe("URL")))
                    continue; // ignore if it has no URL

                Entry kpe = (Entry)GetEntryFromPwEntry(pwe, MatchAccuracy.None, true, host.Database, true);
                if (kpe != null) // is null if entry is marked as hidden from KPRPC
                {
                    allEntries.Add(kpe);
                    count++;
                }
            }

            allEntries.Sort(delegate(Entry e1, Entry e2)
            {
                return e1.Title.CompareTo(e2.Title);
            });

            return allEntries.ToArray();
        }

        private bool EntryIsInRecycleBin(PwEntry pwe, PwDatabase db)
        {
            PwGroup parent = pwe.ParentGroup;
            while (parent != null)
            {
                if (db.RecycleBinUuid.EqualsValue(parent.Uuid))
                    return true;
                parent = parent.ParentGroup;
            }
            return false;
        }

        /// <summary>
        /// Returns a list of every entry contained within a group (not recursive)
        /// </summary>
        /// <param name="uuid">the unique ID of the group we're interested in.</param>
        /// <param name="current__"></param>
        /// <returns>the list of every entry directly inside the group.</returns>
        [JsonRpcMethod]
        public Entry[] GetChildEntries(string uuid)
        {
            PwGroup matchedGroup;
            if (uuid != null && uuid.Length > 0)
            {
                PwUuid pwuuid = new PwUuid(KeePassLib.Utility.MemUtil.HexStringToByteArray(uuid));

                matchedGroup = host.Database.RootGroup.Uuid == pwuuid ? host.Database.RootGroup : host.Database.RootGroup.FindGroup(pwuuid, true);
            }
            else
            {
                matchedGroup = GetRootPwGroup(host.Database);
            }

            if (matchedGroup == null)
                throw new Exception("Could not find requested group. Have you deleted your KeeFox start/home group? Set a new one and try again.");

            return (Entry[])GetChildEntries(host.Database, matchedGroup, true);
        }

        /// <summary>
        /// Returns a list of every entry contained within a group (not recursive)
        /// </summary>
        /// <param name="uuid">the unique ID of the group we're interested in.</param>
        /// <param name="current__"></param>
        /// <returns>the list of every entry directly inside the group.</returns>
        private LightEntry[] GetChildEntries(PwDatabase pwd, PwGroup group, bool fullDetails)
        {
            List<Entry> allEntries = new List<Entry>();
            List<LightEntry> allLightEntries = new List<LightEntry>();

            if (group != null)
            {

                KeePassLib.Collections.PwObjectList<PwEntry> output;
                output = group.GetEntries(false);

                foreach (PwEntry pwe in output)
                {
                    if (EntryIsInRecycleBin(pwe, pwd))
                        continue; // ignore if it's in the recycle bin

                    if (string.IsNullOrEmpty(pwe.Strings.ReadSafe("URL")))
                        continue;
                    if (fullDetails)
                    {
                        Entry kpe = (Entry)GetEntryFromPwEntry(pwe, MatchAccuracy.None, true, pwd, true);
                        if (kpe != null) // is null if entry is marked as hidden from KPRPC
                            allEntries.Add(kpe);
                    }
                    else
                    {
                        LightEntry kpe = GetEntryFromPwEntry(pwe, MatchAccuracy.None, false, pwd, true);
                        if (kpe != null) // is null if entry is marked as hidden from KPRPC
                            allLightEntries.Add(kpe);
                    }
                }

                if (fullDetails)
                {
                    allEntries.Sort(delegate(Entry e1, Entry e2)
                    {
                        return e1.Title.CompareTo(e2.Title);
                    });
                    return allEntries.ToArray();
                }
                else
                {
                    allLightEntries.Sort(delegate(LightEntry e1, LightEntry e2)
                    {
                        return e1.Title.CompareTo(e2.Title);
                    });
                    return allLightEntries.ToArray();
                }


            }

            return null;
        }

        /// <summary>
        /// Returns a list of every group contained within a group (not recursive)
        /// </summary>
        /// <param name="uuid">the unique ID of the group we're interested in.</param>
        /// <param name="current__"></param>
        /// <returns>the list of every group directly inside the group.</returns>
        [JsonRpcMethod]
        public Group[] GetChildGroups(string uuid)
        {
            PwGroup matchedGroup;
            if (uuid != null && uuid.Length > 0)
            {
                PwUuid pwuuid = new PwUuid(KeePassLib.Utility.MemUtil.HexStringToByteArray(uuid));

                matchedGroup = host.Database.RootGroup.Uuid == pwuuid ? host.Database.RootGroup : host.Database.RootGroup.FindGroup(pwuuid, true);
            }
            else
            {
                matchedGroup = GetRootPwGroup(host.Database);
            }

            if (matchedGroup == null)
                throw new Exception("Could not find requested group. Have you deleted your KeeFox start/home group? Set a new one and try again.");

            return GetChildGroups(host.Database, matchedGroup, false, true);
        }

        /// <summary>
        /// Returns a list of every group contained within a group
        /// </summary>
        /// <param name="group">the unique ID of the group we're interested in.</param>
        /// <param name="complete">true = recursive, including Entries too (direct child entries are not included)</param>
        /// <param name="fullDetails">true = all details; false = some details ommitted (e.g. password)</param>
        /// <returns>the list of every group directly inside the group.</returns>
        private Group[] GetChildGroups(PwDatabase pwd, PwGroup group, bool complete, bool fullDetails)
        {
            List<Group> allGroups = new List<Group>();

            if (pwd == null || group == null) { return null; }

            KeePassLib.Collections.PwObjectList<PwGroup> output;
            output = group.Groups;

            foreach (PwGroup pwg in output)
            {
                if (pwd.RecycleBinUuid.EqualsValue(pwg.Uuid))
                    continue; // ignore if it's the recycle bin

                Group kpg = GetGroupFromPwGroup(pwg);

                if (complete)
                {
                    kpg.ChildGroups = GetChildGroups(pwd, pwg, true, fullDetails);
                    if (fullDetails)
                        kpg.ChildEntries = (Entry[])GetChildEntries(pwd, pwg, fullDetails);
                    else
                        kpg.ChildLightEntries = GetChildEntries(pwd, pwg, fullDetails);
                }
                allGroups.Add(kpg);
            }

            allGroups.Sort(delegate(Group g1, Group g2)
            {
                return g1.Title.CompareTo(g2.Title);
            });

            return allGroups.ToArray();
        }

        /// <summary>
        /// Return a list of groups. If uuid is supplied, the list will have a maximum of one entry. Otherwise it could have any number. TODO2: KeePass doesn't have an easy way to search groups by name so postponing that functionality until really needed (or implemented by KeePass API anyway) - for now, name IS COMPLETELY IGNORED
        /// </summary>
        /// <param name="name">IGNORED! The name of a groups we are looking for. Must be an exact match.</param>
        /// <param name="uuid">The UUID of the group we are looking for.</param>
        /// <param name="groups">The output result (a list of Groups)</param>
        /// <param name="current__"></param>
        /// <returns>The number of items in the list of groups.</returns>
        [JsonRpcMethod]
        public int FindGroups(string name, string uuid, out Group[] groups)
        {
            // if uniqueID is supplied, match just that one group. if not found, move on to search the content of the logins...
            if (uuid != null && uuid.Length > 0)
            {
                // Make sure there is an active database
                if (!ensureDBisOpen()) { groups = null; return -1; }

                PwUuid pwuuid = new PwUuid(KeePassLib.Utility.MemUtil.HexStringToByteArray(uuid));

                PwGroup matchedGroup = host.Database.RootGroup.Uuid == pwuuid ? host.Database.RootGroup : host.Database.RootGroup.FindGroup(pwuuid, true);

                if (matchedGroup == null)
                    throw new Exception("Could not find requested group. Have you deleted your KeeFox start/home group? Set a new one and try again.");

                groups = new Group[1];
                groups[0] = GetGroupFromPwGroup(matchedGroup);
                if (groups[0] != null)
                    return 1;
            }


            groups = null;

            return 0;
        }

        // Must match host name; if allowHostnameOnlyMatch is false, exact URL must be matched
        private int bestMatchAccuracyForAnyURL(PwEntry pwe, EntryConfig conf, string url, URLSummary urlSummary)
        {
            bool requireExactURLMatch = conf.BlockHostnameOnlyMatch;
            bool requireAtLeastHostnameAndPortURLMatch = conf.BlockDomainOnlyMatch;

            int bestMatchSoFar = MatchAccuracy.None;

            List<string> URLs = new List<string>(3);
            URLs.Add(pwe.Strings.ReadSafe("URL"));
            if (conf.AltURLs != null)
                URLs.AddRange(conf.AltURLs);

            foreach (string entryURL in URLs)
            {
                if (entryURL == url)
                    return MatchAccuracy.Best;

                // If we require very accurate matches, we can skip the more complex assessment below
                if (requireExactURLMatch)
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
                if (requireAtLeastHostnameAndPortURLMatch || entryUrlSummary.Domain == null || urlSummary.Domain == null)
                    continue;

                if (bestMatchSoFar < MatchAccuracy.Hostname
                    && entryUrlSummary.Domain.Hostname == urlSummary.Domain.Hostname)
                    bestMatchSoFar = MatchAccuracy.Hostname;

                if (bestMatchSoFar < MatchAccuracy.Domain
                    && entryUrlSummary.Domain.RegistrableDomain == urlSummary.Domain.RegistrableDomain)
                    bestMatchSoFar = MatchAccuracy.Domain;
            }
            return bestMatchSoFar;
        }

        private bool matchesAnyBlockedURL(PwEntry pwe, EntryConfig conf, string url) // hostname-wide blocks are not natively supported but can be emulated using an appropriate regex
        {
            if (conf.BlockedURLs != null)
                foreach (string altURL in conf.BlockedURLs)
                    if (altURL.Contains(url))
                        return true;
            return false;
        }

        /// <summary>
        /// Finds entries. Presence of certain parameters dictates type of search performed in the following priority order: uniqueId; freeTextSearch; URL, realm, etc.. Searching stops as soon as one of the different types of search results in a successful match. Supply a username to limit results from URL and realm searches (to search for username regardless of URL/realm, do a free text search and filter results in your client).
        /// </summary>
        /// <param name="unsanitisedURLs">The URLs to search for. Host must be lower case as per the URI specs. Other parts are case sensitive.</param>
        /// <param name="actionURL">The action URL.</param>
        /// <param name="httpRealm">The HTTP realm.</param>
        /// <param name="lst">The type of login search to perform. E.g. look for form matches or HTTP Auth matches.</param>
        /// <param name="requireFullURLMatches">if set to <c>true</c> require full URL matches - host name match only is unacceptable.</param>
        /// <param name="uniqueID">The unique ID of a particular entry we want to retrieve.</param>
        /// <param name="dbRootID">The unique ID of the root group of the database we want to search. Empty string = search all DBs</param>
        /// <param name="freeTextSearch">A string to search for in all entries. E.g. title, username (may change)</param>
        /// /// <param name="username">Limit a search for URL to exact username matches only</param>
        /// <returns>An entry suitable for use by a JSON-RPC client.</returns>
        [JsonRpcMethod]
        public Entry[] FindLogins(string[] unsanitisedURLs, string actionURL,
            string httpRealm, LoginSearchType lst, bool requireFullURLMatches, 
            string uniqueID, string dbFileName, string freeTextSearch, string username)
        {
            List<PwDatabase> dbs = null;
            int count = 0;
            List<Entry> allEntries = new List<Entry>();

            if (!string.IsNullOrEmpty(dbFileName))
            {
                // find the database
                PwDatabase db = SelectDatabase(dbFileName);
                dbs = new List<PwDatabase>();
                dbs.Add(db);
            }
            else
            {
                // if DB list is not populated, look in all open DBs
                dbs = host.MainWindow.DocumentManager.GetOpenDatabases();
                // unless the DB is the wrong version
                dbs = dbs.FindAll(ConfigIsCorrectVersion);
            }

            //string hostname = URLs[0];
            string actionHost = actionURL;

            // Make sure there is an active database
            if (!ensureDBisOpen()) { return null; }

            // if uniqueID is supplied, match just that one login. if not found, move on to search the content of the logins...
            if (uniqueID != null && uniqueID.Length > 0)
            {
                PwUuid pwuuid = new PwUuid(KeePassLib.Utility.MemUtil.HexStringToByteArray(uniqueID));

                //foreach DB...
                foreach (PwDatabase db in dbs)
                {
                    PwEntry matchedLogin = GetRootPwGroup(db).FindEntry(pwuuid, true);

                    if (matchedLogin == null)
                        continue;

                    Entry[] logins = new Entry[1];
                    logins[0] = (Entry)GetEntryFromPwEntry(matchedLogin, MatchAccuracy.Best, true, db);
                    if (logins[0] != null)
                        return logins;
                }
            }

            if (!string.IsNullOrEmpty(freeTextSearch))
            {
                //foreach DB...
                foreach (PwDatabase db in dbs)
                {
                    KeePassLib.Collections.PwObjectList<PwEntry> output = new KeePassLib.Collections.PwObjectList<PwEntry>();

                    PwGroup searchGroup = GetRootPwGroup(db);
                    //output = searchGroup.GetEntries(true);
                    SearchParameters sp = new SearchParameters();
                    sp.ComparisonMode = StringComparison.InvariantCultureIgnoreCase;
                    sp.SearchString = freeTextSearch;
                    sp.SearchInUserNames = true;
                    sp.SearchInTitles = true;
                    sp.SearchInTags = true;
                    MethodInfo mi;

                    // SearchEntries method signature changed in KP 2.17 so we use
                    // reflection to enable support for both 2.17 and earlier versions
                    //TODO:1.6: drop support for 2.17
                    try
                    {
                        mi = typeof(PwGroup).GetMethod("SearchEntries", new Type[] { typeof(SearchParameters), typeof(KeePassLib.Collections.PwObjectList<PwEntry>) });
                        mi.Invoke(searchGroup, new object[] { sp, output });
                    }
                    catch (AmbiguousMatchException)
                    {
                        // can't find the 2.17 method definition so try for an earlier version
                        mi = typeof(PwGroup).GetMethod("SearchEntries", new Type[] { typeof(SearchParameters), typeof(KeePassLib.Collections.PwObjectList<PwEntry>), typeof(bool) });
                        mi.Invoke(searchGroup, new object[] { sp, output, false });

                        // If an exception is thrown here it would be unexpected and
                        // require a new version of the application to be released
                    }

                    foreach (PwEntry pwe in output)
                    {
                        Entry kpe = (Entry)GetEntryFromPwEntry(pwe, MatchAccuracy.None, true, db);
                        if (kpe != null)
                        {
                            allEntries.Add(kpe);
                            count++;
                        }
                    }
                }



            }
            // else we search for the URLs

            // First, we remove any data URIs from the list - there aren't any practical use cases 
            // for this which can trump the security risks introduced by attempting to support their use.
            var santisedURLs = new List<string>(unsanitisedURLs);
            santisedURLs.RemoveAll(u => u.StartsWith("data:"));
            var URLs = santisedURLs.ToArray();

            if (count == 0 && URLs.Length > 0 && !string.IsNullOrEmpty(URLs[0]))
            {
                Dictionary<string, URLSummary> URLHostnameAndPorts = new Dictionary<string, URLSummary>();

                // make sure that hostname and actionURL always represent only the hostname portion
                // of the URL
                // It's tempting to demand that the protocol must match too (e.g. http forms won't
                // match a stored https login) but best not to define such a restriction in KeePassRPC
                // - the RPC client (e.g. KeeFox) can decide to penalise protocol mismatches, 
                // potentially dependant on user configuration options in the client.
                for (int i = 0; i < URLs.Length; i++)
                {
                    URLHostnameAndPorts.Add(URLs[i], URLSummary.FromURL(URLs[i]));
                }

                //foreach DB...
                foreach (PwDatabase db in dbs)
                {
                    KeePassLib.Collections.PwObjectList<PwEntry> output = new KeePassLib.Collections.PwObjectList<PwEntry>();

                    PwGroup searchGroup = GetRootPwGroup(db);
                    output = searchGroup.GetEntries(true);
                    List<string> configErrors = new List<string>(1);

                    // Search every entry in the DB
                    foreach (PwEntry pwe in output)
                    {
                        string entryUserName = pwe.Strings.ReadSafe(PwDefs.UserNameField);
                        entryUserName = KeePassRPCPlugin.GetPwEntryStringFromDereferencableValue(pwe, entryUserName, db);
                        if (EntryIsInRecycleBin(pwe, db))
                            continue; // ignore if it's in the recycle bin

                        //if (string.IsNullOrEmpty(pwe.Strings.ReadSafe("URL")))
                        //    continue; // entries must have a standard URL entry
                        
                        EntryConfig conf = pwe.GetKPRPCConfig(null, ref configErrors);

                        if (conf == null || conf.Hide)
                            continue;

                        bool entryIsAMatch = false;
                        int bestMatchAccuracy = MatchAccuracy.None;


                        if (conf.RegExURLs != null)
                            foreach (string URL in URLs)
                                foreach (string regexPattern in conf.RegExURLs)
                                {
                                    try
                                    {
                                        if (!string.IsNullOrEmpty(regexPattern) && System.Text.RegularExpressions.Regex.IsMatch(URL, regexPattern))
                                        {
                                            entryIsAMatch = true;
                                            bestMatchAccuracy = MatchAccuracy.Best;
                                            break;
                                        }
                                    }
                                    catch (ArgumentException)
                                    {
                                        MessageBox.Show("'" + regexPattern + "' is not a valid regular expression. This error was found in an entry in your database called '" + pwe.Strings.ReadSafe(PwDefs.TitleField) + "'. You need to fix or delete this regular expression to prevent this warning message appearing.", "Warning: Broken regular expression", MessageBoxButtons.OK, MessageBoxIcon.Warning);
                                        break;
                                    }
                                }

                        // Check for matching URLs for the page containing the form
                        if (!entryIsAMatch && lst != LoginSearchType.LSTnoForms
                                && (string.IsNullOrEmpty(username) || username == entryUserName))
                        {
                            foreach (string URL in URLs)
                        {
                            
                                int accuracy = bestMatchAccuracyForAnyURL(pwe, conf, URL, URLHostnameAndPorts[URL]);
                                if (accuracy > bestMatchAccuracy)
                                    bestMatchAccuracy = accuracy;

                            }
                        }

                        // Check for matching URLs for the HTTP Auth containing the form
                        if (!entryIsAMatch && lst != LoginSearchType.LSTnoRealms
                                && (string.IsNullOrEmpty(username) || username == entryUserName)) 
                            
                        {
                            foreach (string URL in URLs)
                            {
                                int accuracy = bestMatchAccuracyForAnyURL(pwe, conf, URL, URLHostnameAndPorts[URL]);
                                if (accuracy > bestMatchAccuracy)
                                    bestMatchAccuracy = accuracy;
                            }
                        }

                        if (bestMatchAccuracy == MatchAccuracy.Best
                            || (!requireFullURLMatches && bestMatchAccuracy > MatchAccuracy.None))
                            entryIsAMatch = true;

                        foreach (string URL in URLs)
                        {
                            // If we think we found a match, check it's not on a block list
                            if (entryIsAMatch && matchesAnyBlockedURL(pwe, conf, URL))
                            {
                                entryIsAMatch = false;
                                break;
                            }
                            if (conf.RegExBlockedURLs != null)
                                foreach (string pattern in conf.RegExBlockedURLs)
                                {
                                    try
                                    {
                                        if (!string.IsNullOrEmpty(pattern) && System.Text.RegularExpressions.Regex.IsMatch(URL, pattern))
                                        {
                                            entryIsAMatch = false;
                                            break;
                                        }
                                    }
                                    catch (ArgumentException)
                                    {
                                        MessageBox.Show("'" + pattern + "' is not a valid regular expression. This error was found in an entry in your database called '" + pwe.Strings.ReadSafe(PwDefs.TitleField) + "'. You need to fix or delete this regular expression to prevent this warning message appearing.", "Warning: Broken regular expression", MessageBoxButtons.OK, MessageBoxIcon.Warning);
                                        break;
                                    }
                                }
                        }

                        if (entryIsAMatch)
                        {
                            Entry kpe = (Entry)GetEntryFromPwEntry(pwe, bestMatchAccuracy, true, db);
                            if (kpe != null)
                            {
                                allEntries.Add(kpe);
                                count++;
                            }
                        }

                    }
                    if (configErrors.Count > 0)
                        MessageBox.Show("There are configuration errors in your database called '" + db.Name + "'. To fix the entries listed below and prevent this warning message appearing, please edit the value of the 'KeePassRPC JSON config' advanced string. Please ask for help on http://keefox.org/help/forum if you're not sure how to fix this. These entries are affected:" + Environment.NewLine + string.Join(Environment.NewLine, configErrors.ToArray()), "Warning: Configuration errors", MessageBoxButtons.OK, MessageBoxIcon.Warning);
                }
            }
            allEntries.Sort(delegate(Entry e1, Entry e2)
            {
                return e1.Title.CompareTo(e2.Title);
            });

            return allEntries.ToArray();
        }

        [JsonRpcMethod]
        public int CountLogins(string URL, string actionURL, string httpRealm, LoginSearchType lst, bool requireFullURLMatches)
        {
            throw new NotImplementedException();
        }

        #endregion

    }
}

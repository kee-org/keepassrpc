using System;
using System.Collections.Generic;
using System.Diagnostics;
using System.Linq;
using System.Reflection;
using System.Windows.Forms;
using Jayrock.JsonRpc;
using KeePass.Resources;
using KeePassLib;
using KeePassLib.Cryptography.PasswordGenerator;
using KeePassLib.Security;
using KeePassLib.Serialization;
using KeePassRPC.Models.DataExchange;
using KeePassRPC.Models.DataExchange.V2;
using KeePassRPC.Models.Persistent;
using KeePassRPC.Models.Shared;

namespace KeePassRPC
{
    public partial class KeePassRPCService
    {
        
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

        #region Configuration of KeePass/Kee and databases

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

            ApplicationMetadata appMetadata =
                new ApplicationMetadata(KeePassVersion, IsMono, NETCLR, NETversion, MonoVersion);
            return appMetadata;
        }

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
        /// Focuses KeePass with a database, opening it first if required
        /// </summary>
        /// <param name="fileName">Path to database to open. If empty, user is prompted to choose a file</param>
        [JsonRpcMethod]
        public void OpenAndFocusDatabase(string fileName)
        {
            IOConnectionInfo ioci = SelectActiveDatabase(fileName);
            OpenIfRequired(ioci, false);
            return;
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
            if (closeCurrent && host.MainWindow.DocumentManager.ActiveDatabase != null &&
                host.MainWindow.DocumentManager.ActiveDatabase.IsOpen)
            {
                host.MainWindow.DocumentManager.CloseDatabase(host.MainWindow.DocumentManager.ActiveDatabase);
            }

            IOConnectionInfo ioci = SelectActiveDatabase(fileName);
            OpenIfRequired(ioci, true);
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
            PwGenerator.Generate(out newPassword, profile, null, host.PwGeneratorPool);
            var password = newPassword.ReadString();

            if (host.CustomConfig.GetBool("KeePassRPC.KeeFox.backupNewPasswords", true))
                AddPasswordBackupLogin(password, url);

            return password;
        }

        #endregion

        #region Removal of entries and groups by UUID (config V1 and V2)

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
                    if (!KeePassLib.Utility.MessageService.AskYesNo(KPRes.DeleteEntriesQuestionSingle,
                            KPRes.DeleteEntriesTitleSingle))
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


        #endregion
        
        
        #region V1 Retrival and manipulation of entries and groups

        
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

                PwGroup matchedGroup = host.Database.RootGroup.Uuid == pwuuid
                    ? host.Database.RootGroup
                    : host.Database.RootGroup.FindGroup(pwuuid, true);

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
        ///4= Keep the old entry's URL (don't add the new URL to the entry)
        ///5= No merge. Delete all URLs and replace with those supplied in the new entry data</param>
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

        /// <summary>
        /// Return a list of every entry in the database that has a URL
        /// </summary>
        /// <returns>all logins in the database that have a URL</returns>
        [JsonRpcMethod]
        public Entry[] GetEntries()
        {
            return getAllLogins(true);
        }

        /// <summary>
        /// Return a list of every entry in the database that has a URL
        /// </summary>
        /// <returns>all logins in the database that have a URL</returns>
        /// <remarks>GetAllLogins is deprecated. Use GetEntries instead.</remarks>
        [JsonRpcMethod]
        public Entry[] GetAllLogins()
        {
            return getAllLogins(true);
        }

        /// <summary>
        /// Return a list of every entry in the database - this includes entries without an URL
        /// </summary>
        /// <returns>all logins in the database</returns>
        [JsonRpcMethod]
        public Entry[] GetAllEntries()
        {
            return getAllLogins(false);
        }


        /// <summary>
        /// Returns a list of every entry contained within a group (not recursive)
        /// </summary>
        /// <param name="uuid">the unique ID of the group we're interested in.</param>
        /// <param name="current__"></param>
        /// <returns>the list of every entry with a URL directly inside the group.</returns>
        [JsonRpcMethod]
        public Entry[] GetChildEntries(string uuid)
        {
            PwGroup matchedGroup;
            matchedGroup = findMatchingGroup(uuid);

            return (Entry[])GetChildEntries(host.Database, matchedGroup, true, true);
        }

        /// <summary>
        /// Returns a list of all the entry contained within a group - including ones missing a URL (not recursive)
        /// </summary>
        /// <param name="uuid">the unique ID of the group we're interested in.</param>
        /// <param name="current__"></param>
        /// <returns>the list of every entry directly inside the group.</returns>
        [JsonRpcMethod]
        public Entry[] GetAllChildEntries(string uuid)
        {
            PwGroup matchedGroup;
            matchedGroup = findMatchingGroup(uuid);

            return (Entry[])GetChildEntries(host.Database, matchedGroup, true, false);
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
            matchedGroup = findMatchingGroup(uuid);

            return GetChildGroups(host.Database, matchedGroup, false, true);
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
                if (!ensureDBisOpen())
                {
                    groups = null;
                    return -1;
                }

                PwUuid pwuuid = new PwUuid(KeePassLib.Utility.MemUtil.HexStringToByteArray(uuid));

                PwGroup matchedGroup = host.Database.RootGroup.Uuid == pwuuid
                    ? host.Database.RootGroup
                    : host.Database.RootGroup.FindGroup(pwuuid, true);

                if (matchedGroup == null)
                    throw new Exception(
                        "Could not find requested group. Have you deleted your Kee home group? Set a new one and try again.");

                groups = new Group[1];
                groups[0] = GetGroupFromPwGroup(matchedGroup);
                if (groups[0] != null)
                    return 1;
            }


            groups = null;

            return 0;
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
            if (!ensureDBisOpen())
            {
                return null;
            }

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
                    KeePassLib.Collections.PwObjectList<PwEntry> output =
                        new KeePassLib.Collections.PwObjectList<PwEntry>();

                    PwGroup searchGroup = GetRootPwGroup(db);
                    //output = searchGroup.GetEntries(true);
                    SearchParameters sp = new SearchParameters();
                    sp.ComparisonMode = StringComparison.InvariantCultureIgnoreCase;
                    sp.SearchString = freeTextSearch;
                    sp.SearchInUserNames = true;
                    sp.SearchInTitles = true;
                    sp.SearchInTags = true;

                    searchGroup.SearchEntries(sp, output);

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
                    var dbConf = db.GetKPRPCConfig();

                    KeePassLib.Collections.PwObjectList<PwEntry> output =
                        new KeePassLib.Collections.PwObjectList<PwEntry>();

                    PwGroup searchGroup = GetRootPwGroup(db);
                    output = searchGroup.GetEntries(true);
                    List<string> configErrors = new List<string>(1);

                    // Search every entry in the DB
                    foreach (PwEntry pwe in output)
                    {
                        string entryUserName = pwe.Strings.ReadSafe(PwDefs.UserNameField);
                        entryUserName =
                            KeePassRPCPlugin.GetPwEntryStringFromDereferencableValue(pwe, entryUserName, db);
                        if (EntryIsInRecycleBin(pwe, db))
                            continue; // ignore if it's in the recycle bin

                        EntryConfigv2 conf =
                            pwe.GetKPRPCConfigNormalised(null, ref configErrors, dbConf.DefaultMatchAccuracy);

                        if (conf == null || conf.MatcherConfigs.Any(mc => mc.MatcherType == EntryMatcherType.Hide))
                            continue;

                        var urlMatcher =
                            conf.MatcherConfigs.FirstOrDefault(mc => mc.MatcherType == EntryMatcherType.Url);
                        if (urlMatcher == null)
                        {
                            // Ignore entries with no Url matcher type. Shouldn't ever happen but maybe loading a newer DB into an old version will cause it so this just protects us against unexpected matches in case of that user error.
                            continue;
                        }

                        bool entryIsAMatch = false;
                        int bestMatchAccuracy = MatchAccuracy.None;


                        if (conf.RegExUrls != null)
                            foreach (string URL in URLs)
                            foreach (string regexPattern in conf.RegExUrls)
                            {
                                try
                                {
                                    if (!string.IsNullOrEmpty(regexPattern) &&
                                        System.Text.RegularExpressions.Regex.IsMatch(URL, regexPattern))
                                    {
                                        entryIsAMatch = true;
                                        bestMatchAccuracy = MatchAccuracy.Best;
                                        break;
                                    }
                                }
                                catch (ArgumentException)
                                {
                                    Utils.ShowMonoSafeMessageBox(
                                        "'" + regexPattern +
                                        "' is not a valid regular expression. This error was found in an entry in your database called '" +
                                        pwe.Strings.ReadSafe(PwDefs.TitleField) +
                                        "'. You need to fix or delete this regular expression to prevent this warning message appearing.",
                                        "Warning: Broken regular expression", MessageBoxButtons.OK,
                                        MessageBoxIcon.Warning);
                                    break;
                                }
                            }

                        if (!entryIsAMatch && (string.IsNullOrEmpty(username) || username == entryUserName))
                        {
                            foreach (string URL in URLs)
                            {
                                var mam = pwe.GetMatchAccuracyMethod(URLHostnameAndPorts[URL], dbConf);
                                int accuracy =
                                    BestMatchAccuracyForAnyURL(pwe, conf, URL, URLHostnameAndPorts[URL], mam);
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

                            if (conf.RegExBlockedUrls != null)
                                foreach (string pattern in conf.RegExBlockedUrls)
                                {
                                    try
                                    {
                                        if (!string.IsNullOrEmpty(pattern) &&
                                            System.Text.RegularExpressions.Regex.IsMatch(URL, pattern))
                                        {
                                            entryIsAMatch = false;
                                            break;
                                        }
                                    }
                                    catch (ArgumentException)
                                    {
                                        Utils.ShowMonoSafeMessageBox(
                                            "'" + pattern +
                                            "' is not a valid regular expression. This error was found in an entry in your database called '" +
                                            pwe.Strings.ReadSafe(PwDefs.TitleField) +
                                            "'. You need to fix or delete this regular expression to prevent this warning message appearing.",
                                            "Warning: Broken regular expression", MessageBoxButtons.OK,
                                            MessageBoxIcon.Warning);
                                        break;
                                    }
                                }
                        }

                        if (entryIsAMatch)
                        {
                            //TODO: Update dozens of JSONRPC method signatures to accept either Entry or Entry2, etc.? Or duplicate them all? Thousands of lines of code though!!!! Also have previously used and deprecated some useful method names...
                            // Maybe first step is to find out exactly which methods work with the entities that will be upgraded and identify suitable names for the new methods to replace them.
                            Entry kpe = (Entry)GetEntryFromPwEntry(pwe, bestMatchAccuracy, true, db);
                            if (kpe != null)
                            {
                                allEntries.Add(kpe);
                                count++;
                            }
                        }
                    }

                    if (configErrors.Count > 0)
                        Utils.ShowMonoSafeMessageBox(
                            "There are configuration errors in your database called '" + db.Name +
                            "'. To fix the entries listed below and prevent this warning message appearing, please edit the value of the 'KeePassRPC JSON' custom data. Please ask for help on https://forum.kee.pm if you're not sure how to fix this. These entries are affected:" +
                            Environment.NewLine + string.Join(Environment.NewLine, configErrors.ToArray()),
                            "Warning: Configuration errors", MessageBoxButtons.OK, MessageBoxIcon.Warning);
                }
            }

            allEntries.Sort(delegate(Entry e1, Entry e2) { return e1.Title.CompareTo(e2.Title); });

            return allEntries.ToArray();
        }
        
        #endregion

        #region V2 Retrival and manipulation of entries and groups

        
        /// <summary>
        /// Add a new entry to the active KeePass database
        /// </summary>
        /// <param name="entry">The KeePassRPC representation of the entry to be added</param>
        /// <param name="parentUuid">The UUID of the parent group for the new entry. If null, the root group will be used.</param>
        /// <param name="dbFileName">The file name of the database we want to save this entry to;
        ///                         if empty or null, the currently active database is used</param>
        /// <returns>The new entry, after having passed through the conversion to and from a KeePass Entry</returns>
        [JsonRpcMethod]
        public Entry2 AddEntry(Entry2 entry, string parentUuid, string dbFileName)
        {
            // Make sure there is an active database
            if (!ensureDBisOpen()) return null;

            PwEntry newLogin = new PwEntry(true, true);

            setPwEntryFromEntry2(newLogin, entry);

            // find the database
            PwDatabase chosenDb = SelectDatabase(dbFileName);

            PwGroup parentGroup = GetRootPwGroup(chosenDb); // if in doubt we'll stick it in the root folder

            if (!string.IsNullOrEmpty(parentUuid))
            {
                PwUuid pwuuid = new PwUuid(KeePassLib.Utility.MemUtil.HexStringToByteArray(parentUuid));

                PwGroup matchedGroup = GetRootPwGroup(chosenDb).FindGroup(pwuuid, true);

                if (matchedGroup != null)
                    parentGroup = matchedGroup;
            }

            parentGroup.AddEntry(newLogin, true);

            if (host.CustomConfig.GetBool("KeePassRPC.KeeFox.editNewEntries", false))
                host.MainWindow.BeginInvoke(new dlgOpenLoginEditorWindow(OpenLoginEditorWindow), newLogin, chosenDb);
            else
                host.MainWindow.BeginInvoke(new dlgSaveDB(saveDB), chosenDb);

            return (Entry2)GetEntry2FromPwEntry(newLogin, MatchAccuracy.Best, true, chosenDb);

        }

        /// <summary>
        /// Updates an existing entry
        /// </summary>
        /// <param name="entry">A entry that contains data to be copied into the existing entry</param>
        /// <param name="oldLoginUuid">The UUID that identifies the entry we want to update</param>
        /// <param name="urlMergeMode">1= Replace the entry's URL (but still fill forms if you visit the old URL)
        ///2= Replace the entry's URL (delete the old URL completely)
        ///3= Keep the old entry's URL (but still fill forms if you visit the new URL)
        ///4= Keep the old entry's URL (don't add the new URL to the entry)
        ///5= No merge. Delete all URLs and replace with those supplied in the new entry data</param>
        /// <param name="dbFileName">Database that contains the entry to update</param>
        /// <returns>The updated entry, after having passed through the conversion to and from a KeePass Entry</returns>
        [JsonRpcMethod]
        public Entry2 UpdateEntry(Entry2 entry, string oldLoginUuid, int urlMergeMode, string dbFileName)
        {
            if (entry == null)
                throw new ArgumentException("(new) entry was not passed to the updateEntry function");
            if (string.IsNullOrEmpty(oldLoginUuid))
                throw new ArgumentException("oldLoginUUID was not passed to the updateEntry function");
            if (string.IsNullOrEmpty(dbFileName))
                throw new ArgumentException("dbFileName was not passed to the updateEntry function");

            // Make sure there is an active database
            if (!ensureDBisOpen()) return null;

            // There are odd bits of the resulting new entry that we don't
            // need but the vast majority is going to be useful
            PwEntry newPwEntryData = new PwEntry(true, true);
            setPwEntryFromEntry2(newPwEntryData, entry);

            // find the database
            PwDatabase chosenDb = SelectDatabase(dbFileName);

            PwUuid pwuuid = new PwUuid(KeePassLib.Utility.MemUtil.HexStringToByteArray(oldLoginUuid));
            PwEntry entryToUpdate = GetRootPwGroup(chosenDb).FindEntry(pwuuid, true);
            if (entryToUpdate == null)
                throw new Exception("oldLoginUUID could not be resolved to an existing entry.");

            MergeEntries(entryToUpdate, newPwEntryData, urlMergeMode, chosenDb);

            host.MainWindow.BeginInvoke(new dlgSaveDB(saveDB), chosenDb);

            return (Entry2)GetEntry2FromPwEntry(entryToUpdate, MatchAccuracy.Best, true, chosenDb);
        }

        [JsonRpcMethod]
        public Database2[] AllDatabases(bool fullDetails)
        {
            Debug.Indent();
            Stopwatch sw = Stopwatch.StartNew();

            List<PwDatabase> dbs = host.MainWindow.DocumentManager.GetOpenDatabases();
            // unless the DB is the wrong version
            dbs = dbs.FindAll(ConfigIsCorrectVersion);
            List<Database2> output = new List<Database2>(1);

            foreach (PwDatabase db in dbs)
            {
                output.Add(GetDatabase2FromPwDatabase(db, fullDetails, false));
            }

            Database2[] dbarray = output.ToArray();
            sw.Stop();
            Debug.WriteLine("GetAllDatabases execution time: " + sw.Elapsed);
            Debug.Unindent();
            return dbarray;
        }


        /// <summary>
        /// Finds entries. Presence of certain parameters dictates type of search performed in the following priority order: uniqueId; freeTextSearch; URL, etc.. Searching stops as soon as one of the different types of search results in a successful match. Supply a username to limit results from URL searches (to search for username regardless of URL, do a free text search and filter results in your client).
        /// </summary>
        /// <param name="unsanitisedUrls">The URLs to search for. Host must be lower case as per the URI specs. Other parts are case sensitive.</param>
        /// <param name="requireFullUrlMatches">if set to <c>true</c> require full URL matches - host name match only is unacceptable.</param>
        /// <param name="uuid">The unique ID of a particular entry we want to retrieve.</param>
        /// <param name="dbFileName">The file name of the database we want to search. Empty string = search all DBs</param>
        /// <param name="freeTextSearch">A string to search for in all entries. E.g. title, username (may change)</param>
        /// <param name="username">Limit a search for URL to exact username matches only</param>
        /// <returns>A list of entries suitable for use by a JSON-RPC client.</returns>
        [JsonRpcMethod]
        public Entry2[] FindEntries(string[] unsanitisedUrls, bool requireFullUrlMatches,
            string uuid, string dbFileName, string freeTextSearch, string username)
        {
            List<PwDatabase> dbs = null;
            int count = 0;
            List<Entry2> allEntries = new List<Entry2>();

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

            // Make sure there is an active database
            if (!ensureDBisOpen())
            {
                return null;
            }

            // if uniqueID is supplied, match just that one login. if not found, move on to search the content of the logins...
            if (!string.IsNullOrEmpty(uuid))
            {
                PwUuid pwuuid = new PwUuid(KeePassLib.Utility.MemUtil.HexStringToByteArray(uuid));

                //foreach DB...
                foreach (PwDatabase db in dbs)
                {
                    PwEntry matchedLogin = GetRootPwGroup(db).FindEntry(pwuuid, true);

                    if (matchedLogin == null)
                        continue;

                    var logins = new Entry2[1];
                    logins[0] = (Entry2)GetEntry2FromPwEntry(matchedLogin, MatchAccuracy.Best, true, db);
                    if (logins[0] != null)
                        return logins;
                }
            }

            if (!string.IsNullOrEmpty(freeTextSearch))
            {
                //foreach DB...
                foreach (PwDatabase db in dbs)
                {
                    KeePassLib.Collections.PwObjectList<PwEntry> output =
                        new KeePassLib.Collections.PwObjectList<PwEntry>();

                    PwGroup searchGroup = GetRootPwGroup(db);
                    //output = searchGroup.GetEntries(true);
                    SearchParameters sp = new SearchParameters();
                    sp.ComparisonMode = StringComparison.InvariantCultureIgnoreCase;
                    sp.SearchString = freeTextSearch;
                    sp.SearchInUserNames = true;
                    sp.SearchInTitles = true;
                    sp.SearchInTags = true;

                    searchGroup.SearchEntries(sp, output);

                    foreach (PwEntry pwe in output)
                    {
                        Entry2 kpe = (Entry2)GetEntry2FromPwEntry(pwe, MatchAccuracy.None, true, db);
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
            var santisedUrls = new List<string>(unsanitisedUrls);
            santisedUrls.RemoveAll(u => u.StartsWith("data:"));
            var urls = santisedUrls.ToArray();

            if (count == 0 && urls.Length > 0 && !string.IsNullOrEmpty(urls[0]))
            {
                Dictionary<string, URLSummary> urlHostnameAndPorts = new Dictionary<string, URLSummary>();

                // make sure that hostname and actionURL always represent only the hostname portion
                // of the URL
                // It's tempting to demand that the protocol must match too (e.g. http forms won't
                // match a stored https login) but best not to define such a restriction in KeePassRPC
                // - the RPC client (e.g. KeeFox) can decide to penalise protocol mismatches, 
                // potentially dependant on user configuration options in the client.
                for (int i = 0; i < urls.Length; i++)
                {
                    urlHostnameAndPorts.Add(urls[i], URLSummary.FromURL(urls[i]));
                }

                //foreach DB...
                foreach (PwDatabase db in dbs)
                {
                    var dbConf = db.GetKPRPCConfig();

                    KeePassLib.Collections.PwObjectList<PwEntry> output =
                        new KeePassLib.Collections.PwObjectList<PwEntry>();

                    PwGroup searchGroup = GetRootPwGroup(db);
                    output = searchGroup.GetEntries(true);
                    List<string> configErrors = new List<string>(1);

                    // Search every entry in the DB
                    foreach (PwEntry pwe in output)
                    {
                        string entryUserName = pwe.Strings.ReadSafe(PwDefs.UserNameField);
                        entryUserName =
                            KeePassRPCPlugin.GetPwEntryStringFromDereferencableValue(pwe, entryUserName, db);
                        if (EntryIsInRecycleBin(pwe, db))
                            continue; // ignore if it's in the recycle bin

                        EntryConfigv2 conf =
                            pwe.GetKPRPCConfigNormalised(null, ref configErrors, dbConf.DefaultMatchAccuracy);

                        if (conf == null || conf.MatcherConfigs.Any(mc => mc.MatcherType == EntryMatcherType.Hide))
                            continue;

                        var urlMatcher =
                            conf.MatcherConfigs.FirstOrDefault(mc => mc.MatcherType == EntryMatcherType.Url);
                        if (urlMatcher == null)
                        {
                            // Ignore entries with no Url matcher type. Shouldn't ever happen but maybe loading a newer DB into an old version will cause it so this just protects us against unexpected matches in case of that user error.
                            continue;
                        }

                        bool entryIsAMatch = false;
                        int bestMatchAccuracy = MatchAccuracy.None;


                        if (conf.RegExUrls != null)
                            foreach (string url in urls)
                            foreach (string regexPattern in conf.RegExUrls)
                            {
                                try
                                {
                                    if (!string.IsNullOrEmpty(regexPattern) &&
                                        System.Text.RegularExpressions.Regex.IsMatch(url, regexPattern))
                                    {
                                        entryIsAMatch = true;
                                        bestMatchAccuracy = MatchAccuracy.Best;
                                        break;
                                    }
                                }
                                catch (ArgumentException)
                                {
                                    Utils.ShowMonoSafeMessageBox(
                                        "'" + regexPattern +
                                        "' is not a valid regular expression. This error was found in an entry in your database called '" +
                                        pwe.Strings.ReadSafe(PwDefs.TitleField) +
                                        "'. You need to fix or delete this regular expression to prevent this warning message appearing.",
                                        "Warning: Broken regular expression", MessageBoxButtons.OK,
                                        MessageBoxIcon.Warning);
                                    break;
                                }
                            }

                        if (!entryIsAMatch && (string.IsNullOrEmpty(username) || username == entryUserName))
                        {
                            foreach (string url in urls)
                            {
                                var mam = pwe.GetMatchAccuracyMethod(urlHostnameAndPorts[url], dbConf);
                                int accuracy =
                                    BestMatchAccuracyForAnyURL(pwe, conf, url, urlHostnameAndPorts[url], mam);
                                if (accuracy > bestMatchAccuracy)
                                    bestMatchAccuracy = accuracy;
                            }
                        }

                        if (bestMatchAccuracy == MatchAccuracy.Best
                            || (!requireFullUrlMatches && bestMatchAccuracy > MatchAccuracy.None))
                            entryIsAMatch = true;

                        foreach (string url in urls)
                        {
                            // If we think we found a match, check it's not on a block list
                            if (entryIsAMatch && matchesAnyBlockedURL(pwe, conf, url))
                            {
                                entryIsAMatch = false;
                                break;
                            }

                            if (conf.RegExBlockedUrls != null)
                                foreach (string pattern in conf.RegExBlockedUrls)
                                {
                                    try
                                    {
                                        if (!string.IsNullOrEmpty(pattern) &&
                                            System.Text.RegularExpressions.Regex.IsMatch(url, pattern))
                                        {
                                            entryIsAMatch = false;
                                            break;
                                        }
                                    }
                                    catch (ArgumentException)
                                    {
                                        Utils.ShowMonoSafeMessageBox(
                                            "'" + pattern +
                                            "' is not a valid regular expression. This error was found in an entry in your database called '" +
                                            pwe.Strings.ReadSafe(PwDefs.TitleField) +
                                            "'. You need to fix or delete this regular expression to prevent this warning message appearing.",
                                            "Warning: Broken regular expression", MessageBoxButtons.OK,
                                            MessageBoxIcon.Warning);
                                        break;
                                    }
                                }
                        }

                        if (entryIsAMatch)
                        {
                            //TODO: Update dozens of JSONRPC method signatures to accept either Entry or Entry2, etc.? Or duplicate them all? Thousands of lines of code though!!!! Also have previously used and deprecated some useful method names...
                            // Maybe first step is to find out exactly which methods work with the entities that will be upgraded and identify suitable names for the new methods to replace them.
                            Entry2 kpe = (Entry2)GetEntry2FromPwEntry(pwe, bestMatchAccuracy, true, db);
                            if (kpe != null)
                            {
                                allEntries.Add(kpe);
                                count++;
                            }
                        }
                    }

                    if (configErrors.Count > 0)
                        Utils.ShowMonoSafeMessageBox(
                            "There are configuration errors in your database called '" + db.Name +
                            "'. To fix the entries listed below and prevent this warning message appearing, please edit the value of the 'KeePassRPC JSON' custom data. Please ask for help on https://forum.kee.pm if you're not sure how to fix this. These entries are affected:" +
                            Environment.NewLine + string.Join(Environment.NewLine, configErrors.ToArray()),
                            "Warning: Configuration errors", MessageBoxButtons.OK, MessageBoxIcon.Warning);
                }
            }

            allEntries.Sort(delegate(Entry2 e1, Entry2 e2) { return e1.Title.CompareTo(e2.Title); });

            return allEntries.ToArray();
        }
        
        #endregion

    }
}
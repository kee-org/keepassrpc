using System;
using System.Collections;
using System.Collections.Generic;
using System.Linq;
using KeePassLib;
using KeePassLib.Security;
using KeePassRPC.Models.DataExchange;
using KeePassRPC.Models.Persistent;
using KeePassRPC.Models.Shared;

namespace KeePassRPC
{
    public partial class KeePassRPCService
    {
        #region Utility functions to convert between KeePassRPC object schema and KeePass schema

        private LightEntry GetEntryFromPwEntry(PwEntry pwe, int matchAccuracy, bool fullDetails, PwDatabase db)
        {
            return GetEntryFromPwEntry(pwe, matchAccuracy, fullDetails, db, false);
        }

        private LightEntry GetEntryFromPwEntry(PwEntry pwe, int matchAccuracy, bool fullDetails, PwDatabase db,
            bool abortIfHidden)
        {
            EntryConfigv2 conf = pwe.GetKPRPCConfigNormalised(db.GetKPRPCConfig().DefaultMatchAccuracy);
            if (conf == null)
                return null;
            return GetEntryFromPwEntry(pwe, conf, matchAccuracy, fullDetails, db, abortIfHidden);
        }

        private LightEntry GetEntryFromPwEntry(PwEntry pwe, EntryConfigv2 conf, int matchAccuracy, bool fullDetails,
            PwDatabase db, bool abortIfHidden)
        {
            ArrayList formFieldList = new ArrayList();
            ArrayList URLs = new ArrayList();
            bool alwaysAutoFill = false;
            bool neverAutoFill = false;
            bool alwaysAutoSubmit = false;
            bool neverAutoSubmit = false;
            int priority = 0;
            string usernameValue = "";

            if (!string.IsNullOrEmpty(pwe.Strings.ReadSafe("URL")))
            {
                URLs.Add(pwe.Strings.ReadSafe("URL"));
            }

            // Hide always blocks if matcher is even present
            if (abortIfHidden && conf.MatcherConfigs.Any(mc => mc.MatcherType == EntryMatcherType.Hide))
                return null;

            if (conf.AltUrls != null)
                URLs.AddRange(conf.AltUrls);

            bool dbDefaultPlaceholderHandlingEnabled =
                db.GetKPRPCConfig().DefaultPlaceholderHandling == PlaceholderHandling.Enabled;

            foreach (Field ff in conf.Fields)
            {
                if (!fullDetails && ff.ValuePath != PwDefs.UserNameField)
                    continue;

                bool enablePlaceholders = false;
                string displayName = ff.Name;
                string ffValue = ff.Value;
                string htmlName = "";
                string htmlId = "";
                FormFieldType htmlType = Utilities.FieldTypeToFormFieldType(ff.Type);

                // Currently we can only have one custommatcher. If that changes and someone tries to use this old version with a newer DB things will break so they will have to upgrade again to fix it.
                var customMatcherConfig =
                    ff.MatcherConfigs.FirstOrDefault(mc => mc != null && mc.CustomMatcher != null);
                if (customMatcherConfig != null)
                {
                    if (customMatcherConfig.CustomMatcher.Names != null)
                    {
                        htmlName = customMatcherConfig.CustomMatcher.Names.FirstOrDefault() ?? "";
                    }

                    if (customMatcherConfig.CustomMatcher.Ids != null)
                    {
                        htmlId = customMatcherConfig.CustomMatcher.Ids.FirstOrDefault() ?? "";
                    }

                    if (customMatcherConfig.CustomMatcher.Types != null)
                    {
                        htmlType = FormField.FormFieldTypeFromHtmlTypeOrFieldType(
                            customMatcherConfig.CustomMatcher.Types.FirstOrDefault() ?? "", ff.Type);
                    }
                }

                if (ff.PlaceholderHandling == PlaceholderHandling.Enabled ||
                    (ff.PlaceholderHandling == PlaceholderHandling.Default &&
                     dbDefaultPlaceholderHandlingEnabled))
                {
                    enablePlaceholders = true;
                }

                if (ff.Type == FieldType.Password && ff.ValuePath == PwDefs.PasswordField)
                {
                    displayName = "KeePass password";
                    htmlType = FormFieldType.FFTpassword;
                }
                else if (ff.Type == FieldType.Text && ff.ValuePath == PwDefs.UserNameField)
                {
                    displayName = "KeePass username";
                    htmlType = FormFieldType.FFTusername;
                }

                ffValue = ff.ValuePath == "." ? ff.Value : _keePassRpcPlugin.GetPwEntryString(pwe, ff.ValuePath, db);

                string derefValue = enablePlaceholders
                    ? _keePassRpcPlugin.GetPwEntryStringFromDereferencableValue(pwe, ffValue, db)
                    : ffValue;

                if (fullDetails)
                {
                    if (!string.IsNullOrEmpty(ffValue))
                    {
                        formFieldList.Add(new FormField(htmlName, displayName, derefValue, htmlType, htmlId, ff.Page,
                            ff.PlaceholderHandling.GetValueOrDefault(PlaceholderHandling.Default)));
                    }
                }
                else
                {
                    usernameValue = derefValue;
                }
            }

            string imageData = _iconConverter.iconToBase64(pwe.CustomIconUuid, pwe.IconId);
            //Debug.WriteLine("GetEntryFromPwEntry icon converted: " + sw.Elapsed);

            if (fullDetails)
            {
                switch (conf.Behaviour)
                {
                    case EntryAutomationBehaviour.AlwaysAutoFill:
                        alwaysAutoFill = true;
                        alwaysAutoSubmit = false;
                        neverAutoFill = false;
                        neverAutoSubmit = false;
                        break;
                    case EntryAutomationBehaviour.NeverAutoSubmit:
                        alwaysAutoFill = false;
                        alwaysAutoSubmit = false;
                        neverAutoFill = false;
                        neverAutoSubmit = true;
                        break;
                    case EntryAutomationBehaviour.AlwaysAutoFillAlwaysAutoSubmit:
                        alwaysAutoFill = true;
                        alwaysAutoSubmit = true;
                        neverAutoFill = false;
                        neverAutoSubmit = false;
                        break;
                    case EntryAutomationBehaviour.NeverAutoFillNeverAutoSubmit:
                        alwaysAutoFill = false;
                        alwaysAutoSubmit = false;
                        neverAutoFill = true;
                        neverAutoSubmit = true;
                        break;
                    case EntryAutomationBehaviour.AlwaysAutoFillNeverAutoSubmit:
                        alwaysAutoFill = true;
                        alwaysAutoSubmit = false;
                        neverAutoFill = false;
                        neverAutoSubmit = true;
                        break;
                    case EntryAutomationBehaviour.Default:
                        alwaysAutoFill = false;
                        alwaysAutoSubmit = false;
                        neverAutoFill = false;
                        neverAutoSubmit = false;
                        break;
                }

                priority = 0;
            }

            //sw.Stop();
            //Debug.WriteLine("GetEntryFromPwEntry execution time: " + sw.Elapsed);
            //Debug.Unindent();

            if (fullDetails)
            {
                string realm = "";
                if (!string.IsNullOrEmpty(conf.HttpRealm))
                    realm = conf.HttpRealm;

                FormField[] temp = (FormField[])formFieldList.ToArray(typeof(FormField));
                Entry kpe = new Entry(
                    (string[])URLs.ToArray(typeof(string)), realm,
                    pwe.Strings.ReadSafe(PwDefs.TitleField), temp,
                    KeePassLib.Utility.MemUtil.ByteArrayToHexString(pwe.Uuid.UuidBytes),
                    alwaysAutoFill, neverAutoFill, alwaysAutoSubmit, neverAutoSubmit, priority,
                    GetGroupFromPwGroup(pwe.ParentGroup), imageData,
                    GetDatabaseFromPwDatabase(db, false, true), matchAccuracy);
                return kpe;
            }
            else
            {
                return new LightEntry((string[])URLs.ToArray(typeof(string)),
                    pwe.Strings.ReadSafe(PwDefs.TitleField),
                    KeePassLib.Utility.MemUtil.ByteArrayToHexString(pwe.Uuid.UuidBytes),
                    imageData, "username", usernameValue);
            }
        }

        private Group GetGroupFromPwGroup(PwGroup pwg)
        {
            //Debug.Indent();
            //Stopwatch sw = Stopwatch.StartNew();

            string imageData = _iconConverter.iconToBase64(pwg.CustomIconUuid, pwg.IconId);

            Group kpg = new Group(pwg.Name, KeePassLib.Utility.MemUtil.ByteArrayToHexString(pwg.Uuid.UuidBytes),
                imageData, pwg.GetFullPath("/", false));

            //sw.Stop();
            //Debug.WriteLine("GetGroupFromPwGroup execution time: " + sw.Elapsed);
            //Debug.Unindent();
            return kpg;
        }

        private Database GetDatabaseFromPwDatabase(PwDatabase pwd, bool fullDetail, bool noDetail)
        {
            try
            {
                //Debug.Indent();
                // Stopwatch sw = Stopwatch.StartNew();
                if (fullDetail && noDetail)
                    throw new ArgumentException("Don't be silly");

                PwGroup pwg = GetRootPwGroup(pwd);
                Group rt = GetGroupFromPwGroup(pwg);
                if (fullDetail)
                    rt.ChildEntries = (Entry[])GetChildEntries(pwd, pwg, fullDetail, true);
                else if (!noDetail)
                    rt.ChildLightEntries = GetChildEntries(pwd, pwg, fullDetail, true);

                if (!noDetail)
                    rt.ChildGroups = GetChildGroups(pwd, pwg, true, fullDetail);

                Database kpd = new Database(pwd.Name, pwd.IOConnectionInfo.Path, rt,
                    (pwd == _host.Database) ? true : false,
                    IconCache<string>.GetIconEncoding(pwd.IOConnectionInfo.Path) ?? "");
                //  sw.Stop();
                //  Debug.WriteLine("GetDatabaseFromPwDatabase execution time: " + sw.Elapsed);
                //  Debug.Unindent();
                return kpd;
            }
            catch (Exception ex)
            {
                if (_keePassRpcPlugin.logger != null)
                    _keePassRpcPlugin.logger.WriteLine("Failed to parse database. Exception: " + ex);
                return null;
            }
        }

        private void setPwEntryFromEntry(PwEntry pwe, Entry login)
        {
            IGuidService guidService = new GuidService();
            bool firstPasswordFound = false;
            EntryConfigv2 conf =
                (new EntryConfigv1(_host.Database.GetKPRPCConfig().DefaultMatchAccuracy))
                .ConvertToV2(new GuidService());
            List<Field> fields = new List<Field>();

            // Go through each form field, mostly just making a copy but with occasional tweaks such as default username and password selection
            // by convention, we'll always have the first text field as the username when both reading and writing from the EntryConfig
            foreach (FormField kpff in login.FormFieldList)
            {
                if (kpff.Type == FormFieldType.FFTpassword && !firstPasswordFound)
                {
                    var mc = string.IsNullOrEmpty(kpff.Id) && string.IsNullOrEmpty(kpff.Name)
                        ? new FieldMatcherConfig() { MatcherType = FieldMatcherType.PasswordDefaultHeuristic }
                        : FieldMatcherConfig.ForSingleClientMatch(kpff.Id, kpff.Name, kpff.Type);
                    fields.Add(new Field()
                    {
                        Page = Math.Max(kpff.Page, 1),
                        ValuePath = PwDefs.PasswordField,
                        Uuid = Convert.ToBase64String(guidService.NewGuid().ToByteArray()),
                        Type = FieldType.Password,
                        MatcherConfigs = new[] { mc }
                    });
                    pwe.Strings.Set(PwDefs.PasswordField,
                        new ProtectedString(_host.Database.MemoryProtection.ProtectPassword, kpff.Value));
                    firstPasswordFound = true;
                }
                else if (kpff.Type == FormFieldType.FFTusername)
                {
                    var mc = string.IsNullOrEmpty(kpff.Id) && string.IsNullOrEmpty(kpff.Name)
                        ? new FieldMatcherConfig() { MatcherType = FieldMatcherType.UsernameDefaultHeuristic }
                        : FieldMatcherConfig.ForSingleClientMatch(kpff.Id, kpff.Name, kpff.Type);
                    fields.Add(new Field()
                    {
                        Page = Math.Max(kpff.Page, 1),
                        ValuePath = PwDefs.UserNameField,
                        Uuid = Convert.ToBase64String(guidService.NewGuid().ToByteArray()),
                        Type = FieldType.Text,
                        MatcherConfigs = new[] { mc }
                    });
                    pwe.Strings.Set(PwDefs.UserNameField,
                        new ProtectedString(_host.Database.MemoryProtection.ProtectUserName, kpff.Value));
                }
                else
                {
                    var mc = FieldMatcherConfig.ForSingleClientMatch(kpff.Id, kpff.Name, kpff.Type);
                    fields.Add(new Field()
                    {
                        Name = !string.IsNullOrEmpty(kpff.DisplayName) ? kpff.DisplayName : kpff.Name,
                        Page = Math.Max(kpff.Page, 1),
                        ValuePath = ".",
                        Uuid = Convert.ToBase64String(guidService.NewGuid().ToByteArray()),
                        Type = Utilities.FormFieldTypeToFieldType(kpff.Type),
                        MatcherConfigs = new[] { mc },
                        Value = kpff.Value
                    });
                }
            }

            conf.Fields = fields.ToArray();

            List<string> altURLs = new List<string>();

            for (int i = 0; i < login.URLs.Length; i++)
            {
                string url = login.URLs[i];
                if (i == 0)
                {
                    // We can't use the framework Uri.Port property here because
                    // we are interested in whether it is explicit or not - the 
                    // Port property returns the default port for a protocol if 
                    // one is not explicitly included in the URL
                    URLSummary urlsum = URLSummary.FromURL(url);

                    // Require more strict default matching for entries that come
                    // with a port configured (user can override in the rare case
                    // that they want the loose domain-level matching)
                    if (!string.IsNullOrEmpty(urlsum.Port))
                    {
                        var mc = conf.MatcherConfigs.First(emc => emc.MatcherType == EntryMatcherType.Url);
                        mc.UrlMatchMethod = MatchAccuracyMethod.Hostname;
                    }

                    pwe.Strings.Set("URL", new ProtectedString(_host.Database.MemoryProtection.ProtectUrl, url ?? ""));
                }
                else
                    altURLs.Add(url);
            }

            conf.AltUrls = altURLs.ToArray();
            conf.HttpRealm = string.IsNullOrEmpty(login.HTTPRealm) ? null : login.HTTPRealm;
            conf.Version = 2;

            // Set some of the string fields
            pwe.Strings.Set(PwDefs.TitleField,
                new ProtectedString(_host.Database.MemoryProtection.ProtectTitle, login.Title ?? ""));

            // update the icon for this entry (in most cases we'll 
            // just detect that it is the same standard icon as before)
            PwUuid customIconUUID = PwUuid.Zero;
            PwIcon iconId = PwIcon.Key;
            if (login.IconImageData != null
                && login.IconImageData.Length > 0
                && _iconConverter.base64ToIcon(ClientMetadata, login.IconImageData, ref customIconUUID, ref iconId))
            {
                if (customIconUUID == PwUuid.Zero)
                    pwe.IconId = iconId;
                else
                    pwe.CustomIconUuid = customIconUUID;
            }

            pwe.SetKPRPCConfig(conf);
        }

        #endregion


        /// <summary>
        /// Returns a list of every entry in the database
        /// </summary>
        /// <param name="urlRequired">true = URL field must exist for a child entry to be returned, false = all entries are returned</param>
        /// <param name="current__"></param>
        /// <returns>all logins in the database subject to the urlRequired setting</returns>
        public Entry[] getAllLogins(bool urlRequired)
        {
            int count = 0;
            List<Entry> allEntries = new List<Entry>();

            // Make sure there is an active database
            if (!EnsureDBisOpen())
            {
                return null;
            }

            KeePassLib.Collections.PwObjectList<PwEntry> output;
            output = GetRootPwGroup(_host.Database).GetEntries(true);

            foreach (PwEntry pwe in output)
            {
                if (EntryIsInRecycleBin(pwe, _host.Database))
                    continue; // ignore if it's in the recycle bin

                if (urlRequired && string.IsNullOrEmpty(pwe.Strings.ReadSafe("URL")))
                    continue; // ignore if it has no URL

                Entry kpe = (Entry)GetEntryFromPwEntry(pwe, MatchAccuracy.None, true, _host.Database, true);
                if (kpe != null) // is null if entry is marked as hidden from KPRPC
                {
                    allEntries.Add(kpe);
                    count++;
                }
            }

            allEntries.Sort(delegate(Entry e1, Entry e2) { return e1.Title.CompareTo(e2.Title); });

            return allEntries.ToArray();
        }


        /// <summary>
        /// Returns a list of every entry contained within a group (not recursive)
        /// </summary>
        /// <param name="pwd">the database to search in</param>
        /// <param name="group">the group to search in</param>
        /// <param name="fullDetails">true = all details; false = some details ommitted (e.g. password)</param>
        /// <param name="urlRequired">true = URL field must exist for a child entry to be returned, false = all entries are returned</param>
        /// <param name="current__"></param>
        /// <returns>the list of every entry directly inside the group.</returns>
        private LightEntry[] GetChildEntries(PwDatabase pwd, PwGroup group, bool fullDetails, bool urlRequired)
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

                    if (urlRequired && string.IsNullOrEmpty(pwe.Strings.ReadSafe("URL")))
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
                    allEntries.Sort(delegate(Entry e1, Entry e2) { return e1.Title.CompareTo(e2.Title); });
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
        /// Returns a list of every group contained within a group
        /// </summary>
        /// <param name="group">the unique ID of the group we're interested in.</param>
        /// <param name="complete">true = recursive, including Entries too (direct child entries are not included)</param>
        /// <param name="fullDetails">true = all details; false = some details ommitted (e.g. password)</param>
        /// <returns>the list of every group directly inside the group.</returns>
        private Group[] GetChildGroups(PwDatabase pwd, PwGroup group, bool complete, bool fullDetails)
        {
            List<Group> allGroups = new List<Group>();

            if (pwd == null || group == null)
            {
                return null;
            }

            KeePassLib.Collections.PwObjectList<PwGroup> output;
            output = group.Groups;

            foreach (PwGroup pwg in output)
            {
                if (pwd.RecycleBinUuid.Equals(pwg.Uuid))
                    continue; // ignore if it's the recycle bin

                Group kpg = GetGroupFromPwGroup(pwg);

                if (complete)
                {
                    kpg.ChildGroups = GetChildGroups(pwd, pwg, true, fullDetails);
                    if (fullDetails)
                        kpg.ChildEntries = (Entry[])GetChildEntries(pwd, pwg, fullDetails, true);
                    else
                        kpg.ChildLightEntries = GetChildEntries(pwd, pwg, fullDetails, true);
                }

                allGroups.Add(kpg);
            }

            allGroups.Sort(delegate(Group g1, Group g2) { return g1.Title.CompareTo(g2.Title); });

            return allGroups.ToArray();
        }
    }
}
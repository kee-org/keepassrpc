using System;
using System.Collections.Generic;
using System.Linq;
using KeePassLib;
using KeePassLib.Collections;
using KeePassLib.Security;
using KeePassLib.Utility;
using KeePassRPC.Models.DataExchange;
using KeePassRPC.Models.DataExchange.V2;
using KeePassRPC.Models.Persistent;
using KeePassRPC.Models.Shared;

namespace KeePassRPC
{
    public partial class KeePassRPCService
    {
        #region Utility functions to convert between KeePassRPC object schema and KeePass schema

        private LightEntry2 GetEntry2FromPwEntry(PwEntry pwe, int matchAccuracy, bool fullDetails, PwDatabase db,
            bool urlRequired)
        {
            return GetEntry2FromPwEntry(pwe, matchAccuracy, fullDetails, db, false, urlRequired);
        }

        private LightEntry2 GetEntry2FromPwEntry(PwEntry pwe, int matchAccuracy, bool fullDetails, PwDatabase db,
            bool abortIfHidden, bool urlRequired)
        {
            EntryConfigv2 conf = pwe.GetKPRPCConfigNormalised(db.GetKPRPCConfig().DefaultMatchAccuracy);
            if (conf == null)
                return null;
            return GetEntry2FromPwEntry(pwe, conf, matchAccuracy, fullDetails, db, abortIfHidden, urlRequired);
        }

        private LightEntry2 GetEntry2FromPwEntry(PwEntry pwe, EntryConfigv2 conf, int matchAccuracy, bool fullDetails,
            PwDatabase db, bool abortIfHidden, bool urlRequired)
        {
            var fields = new List<ResolvedField>();
            var urls = new List<string>();
            string usernameValue = "";
            string usernameName = "";

            if (!string.IsNullOrEmpty(pwe.Strings.ReadSafe("URL")))
            {
                urls.Add(pwe.Strings.ReadSafe("URL"));
            }

            // Hide always blocks if matcher is even present
            if (abortIfHidden && conf.MatcherConfigs.Any(mc => mc.MatcherType == EntryMatcherType.Hide))
                return null;

            if (conf.AltUrls != null)
                urls.AddRange(conf.AltUrls);

            bool dbDefaultPlaceholderHandlingEnabled =
                db.GetKPRPCConfig().DefaultPlaceholderHandling == PlaceholderHandling.Enabled;

            foreach (Field field in conf.Fields)
            {
                if (!fullDetails && field.ValuePath != PwDefs.UserNameField)
                    continue;

                bool enablePlaceholders = field.PlaceholderHandling == PlaceholderHandling.Enabled ||
                                          (field.PlaceholderHandling == PlaceholderHandling.Default &&
                                           dbDefaultPlaceholderHandlingEnabled);

                string ffValue = field.ValuePath == "."
                    ? field.Value
                    : _keePassRpcPlugin.GetPwEntryString(pwe, field.ValuePath, db);

                string derefValue = enablePlaceholders
                    ? _keePassRpcPlugin.GetPwEntryStringFromDereferencableValue(pwe, ffValue, db)
                    : ffValue;

                if (fullDetails)
                {
                    if (!string.IsNullOrEmpty(ffValue))
                    {
                        fields.Add(new ResolvedField
                        {
                            ResolvedValue = derefValue,
                            ValuePath = field.ValuePath,
                            Value = field.Value,
                            Uuid = field.Uuid,
                            MatcherConfigs = field.MatcherConfigs,
                            PlaceholderHandling = field.PlaceholderHandling,
                            Name = field.Name,
                            Page = field.Page,
                            Type = field.Type
                        });
                    }
                }
                else
                {
                    usernameName = !string.IsNullOrWhiteSpace(field.Name) ? field.Name : "username";
                    usernameValue = derefValue;
                }
            }

            Icon icon = _iconConverter.iconToDto(ClientMetadata, pwe.CustomIconUuid, pwe.IconId);

            if (fullDetails)
            {
                string realm = "";
                if (!string.IsNullOrEmpty(conf.HttpRealm))
                    realm = conf.HttpRealm;

                var temp = fields.ToArray();
                var mc = (ClientMetadata != null && ClientMetadata.Features != null &&
                          ClientMetadata.Features.Contains("KPRPC_FEATURE_ENTRY_CLIENT_MATCHERS"))
                    ? conf.MatcherConfigs
                    : null;
                Entry2 kpe = new Entry2(
                    urls.ToArray(), realm,
                    pwe.Strings.ReadSafe(PwDefs.TitleField), temp,
                    conf.Behaviour,
                    MemUtil.ByteArrayToHexString(pwe.Uuid.UuidBytes),
                    GetGroup2FromPwGroup(pwe.ParentGroup), icon,
                    GetDatabase2FromPwDatabase(db, false, true, urlRequired), matchAccuracy, mc,
                    conf.AuthenticationMethods);

                return kpe;
            }

            return new LightEntry2(urls.ToArray(),
                pwe.Strings.ReadSafe(PwDefs.TitleField),
                MemUtil.ByteArrayToHexString(pwe.Uuid.UuidBytes),
                icon, usernameName, usernameValue, conf.AuthenticationMethods);
        }

        private Group2 GetGroup2FromPwGroup(PwGroup pwg)
        {
            Icon icon = _iconConverter.iconToDto(ClientMetadata, pwg.CustomIconUuid, pwg.IconId);

            Group2 kpg = new Group2(pwg.Name, MemUtil.ByteArrayToHexString(pwg.Uuid.UuidBytes),
                icon, pwg.GetFullPath("/", false));

            return kpg;
        }

        private Database2 GetDatabase2FromPwDatabase(PwDatabase pwd, bool fullDetail, bool noDetail, bool urlRequired)
        {
            try
            {
                //Debug.Indent();
                // Stopwatch sw = Stopwatch.StartNew();
                if (fullDetail && noDetail)
                    throw new ArgumentException("Don't be silly");

                PwGroup pwg = GetRootPwGroup(pwd);
                Group2 rt = GetGroup2FromPwGroup(pwg);
                if (fullDetail)
                    rt.ChildEntries = (Entry2[])GetChildEntries2(pwd, pwg, fullDetail, urlRequired);
                else if (!noDetail)
                    rt.ChildLightEntries = GetChildEntries2(pwd, pwg, fullDetail, urlRequired);

                if (!noDetail)
                    rt.ChildGroups = GetChildGroups2(pwd, pwg, true, fullDetail);

                // Can just send a null icon if we know the client can get it elsewhere
                var icon = (ClientMetadata != null && ClientMetadata.Features != null &&
                            ClientMetadata.Features.Contains("KPRPC_FEATURE_ICON_REFERENCES"))
                    ? null
                    : new Icon
                    {
                        Base64 = IconCache<string>.GetIconEncoding(pwd.IOConnectionInfo.Path) ?? ""
                    };

                Database2 kpd = new Database2(pwd.Name, pwd.IOConnectionInfo.Path, rt,
                    (pwd == _host.Database) ? true : false, icon);
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

        private void setPwEntryFromEntry2(PwEntry pwe, Entry2 entry)
        {
            EntryConfigv2 conf =
                (new EntryConfigv1(_host.Database.GetKPRPCConfig().DefaultMatchAccuracy))
                .ConvertToV2(new GuidService());
            List<Field> fields = new List<Field>();

            foreach (ResolvedField incomingField in entry.Fields)
            {
                if (incomingField.ValuePath == PwDefs.PasswordField)
                {
                    pwe.Strings.Set(PwDefs.PasswordField,
                        new ProtectedString(_host.Database.MemoryProtection.ProtectPassword, incomingField.Value));
                }
                else if (incomingField.ValuePath == PwDefs.UserNameField)
                {
                    pwe.Strings.Set(PwDefs.UserNameField,
                        new ProtectedString(_host.Database.MemoryProtection.ProtectUserName, incomingField.Value));
                }

                fields.Add(new Field
                {
                    Name = incomingField.Name,
                    Page = Math.Max(incomingField.Page, 1),
                    ValuePath = incomingField.ValuePath,
                    Uuid = incomingField.Uuid,
                    Type = incomingField.Type,
                    MatcherConfigs = incomingField.MatcherConfigs,
                    Value = incomingField.ValuePath == "." ? incomingField.Value : null
                });
            }

            conf.Fields = fields.ToArray();

            List<string> altUrls = new List<string>();

            for (int i = 0; i < entry.Urls.Length; i++)
            {
                string url = entry.Urls[i];
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
                    altUrls.Add(url);
            }

            conf.AltUrls = altUrls.ToArray();
            conf.HttpRealm = string.IsNullOrEmpty(entry.Realm) ? null : entry.Realm;
            conf.Version = 2;

            // Set some of the string fields
            pwe.Strings.Set(PwDefs.TitleField,
                new ProtectedString(_host.Database.MemoryProtection.ProtectTitle, entry.Title ?? ""));

            // update the icon for this entry (in most cases we'll 
            // just detect that it is the same standard icon as before)
            PwUuid customIconUuid = PwUuid.Zero;
            PwIcon iconId = PwIcon.Key;
            if (entry.Icon != null
                && _iconConverter.dtoToIcon(ClientMetadata, entry.Icon, ref customIconUuid, ref iconId))
            {
                if (ReferenceEquals(customIconUuid, PwUuid.Zero))
                    pwe.IconId = iconId;
                else
                    pwe.CustomIconUuid = customIconUuid;
            }

            pwe.SetKPRPCConfig(conf);
        }

        #endregion


        /// <summary>
        /// Returns a list of every entry contained within a group (not recursive)
        /// </summary>
        /// <param name="pwd">the database to search in</param>
        /// <param name="group">the group to search in</param>
        /// <param name="fullDetails">true = all details; false = some details ommitted (e.g. password)</param>
        /// <param name="urlRequired">true = URL field must exist for a child entry to be returned, false = all entries are returned</param>
        /// <returns>the list of every entry directly inside the group.</returns>
        private LightEntry2[] GetChildEntries2(PwDatabase pwd, PwGroup group, bool fullDetails, bool urlRequired)
        {
            List<Entry2> allEntries = new List<Entry2>();
            List<LightEntry2> allLightEntries = new List<LightEntry2>();

            if (group != null)
            {
                PwObjectList<PwEntry> output;
                output = group.GetEntries(false);

                foreach (PwEntry pwe in output)
                {
                    if (EntryIsInRecycleBin(pwe, pwd))
                        continue; // ignore if it's in the recycle bin

                    if (urlRequired && string.IsNullOrEmpty(pwe.Strings.ReadSafe("URL")))
                        continue;
                    if (fullDetails)
                    {
                        Entry2 kpe = (Entry2)GetEntry2FromPwEntry(pwe, MatchAccuracy.None, true, pwd, true);
                        if (kpe != null) // is null if entry is marked as hidden from KPRPC
                            allEntries.Add(kpe);
                    }
                    else
                    {
                        LightEntry2 kpe = GetEntry2FromPwEntry(pwe, MatchAccuracy.None, false, pwd, true);
                        if (kpe != null) // is null if entry is marked as hidden from KPRPC
                            allLightEntries.Add(kpe);
                    }
                }

                if (fullDetails)
                {
                    allEntries.Sort(delegate(Entry2 e1, Entry2 e2) { return e1.Title.CompareTo(e2.Title); });
                    return allEntries.ToArray();
                }

                allLightEntries.Sort(delegate(LightEntry2 e1, LightEntry2 e2)
                {
                    return e1.Title.CompareTo(e2.Title);
                });
                return allLightEntries.ToArray();
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
        private Group2[] GetChildGroups2(PwDatabase pwd, PwGroup group, bool complete, bool fullDetails)
        {
            List<Group2> allGroups = new List<Group2>();

            if (pwd == null || group == null)
            {
                return null;
            }

            PwObjectList<PwGroup> output;
            output = group.Groups;

            foreach (PwGroup pwg in output)
            {
                if (pwd.RecycleBinUuid.Equals(pwg.Uuid))
                    continue; // ignore if it's the recycle bin

                Group2 kpg = GetGroup2FromPwGroup(pwg);

                if (complete)
                {
                    kpg.ChildGroups = GetChildGroups2(pwd, pwg, true, fullDetails);
                    if (fullDetails)
                        kpg.ChildEntries = (Entry2[])GetChildEntries2(pwd, pwg, fullDetails, true);
                    else
                        kpg.ChildLightEntries = GetChildEntries2(pwd, pwg, fullDetails, true);
                }

                allGroups.Add(kpg);
            }

            allGroups.Sort(delegate(Group2 g1, Group2 g2) { return g1.Title.CompareTo(g2.Title); });

            return allGroups.ToArray();
        }
    }
}
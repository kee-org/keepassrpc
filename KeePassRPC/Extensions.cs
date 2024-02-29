using KeePassLib;
using KeePassLib.Collections;
using KeePassLib.Security;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Windows.Forms;
using KeePassRPC.Models.DataExchange;
using KeePassRPC.Models.Persistent;
using KeePassRPC.Models.Shared;
using KeePassRPC.Models.Transient;

namespace KeePassRPC
{
    public static class Extensions
    {
        public static EntryConfig GetKPRPCConfig(this PwEntry entry, ProtectedStringDictionary strings,
            ref List<string> configErrors, MatchAccuracyMethod mam)
        {
            if (strings == null)
                strings = entry.Strings;
            // We try to load both versions. Use normalised variant if you want auto conversion to V2 and non-null return (excepting errors)
            var v1 = strings.Exists("KPRPC JSON") ? entry.GetKPRPCConfigV1(strings, ref configErrors, mam) : null;
            var v2 = entry.CustomData.Exists("KPRPC JSON") ? entry.GetKPRPCConfigV2(ref configErrors, mam) : null;
            return new EntryConfig(v1, v2);
        }
        
        public static EntryConfigv2 GetKPRPCConfigNormalised(this PwEntry entry, ProtectedStringDictionary strings,
            ref List<string> configErrors, MatchAccuracyMethod mam)
        {
            if (strings == null)
                strings = entry.Strings;
            // return either V2 or a converted version of V1. We need to leave most entries with a V1 config for quite a long time since modifications result in new entry versions and potential need to save the database at unexpected and impossible times.
            // It's inefficient since we will often be creating a new V1 config and instantly converting it to V2 before use or persistence but this keeps the logic through this transitional period much simpler and is unlikely to cause critical performance problems that can't wait for the full transition to V2 in a year or two.
            var v2 = entry.CustomData.Exists("KPRPC JSON") ? entry.GetKPRPCConfigV2(ref configErrors, mam) : null;
            return v2 ?? entry.GetKPRPCConfigV1(strings, ref configErrors, mam).ConvertToV2(new GuidService());
        }

    public static EntryConfigv1 GetKPRPCConfigV1(this PwEntry entry, ProtectedStringDictionary strings, ref List<string> configErrors, MatchAccuracyMethod mam)
        {
            if (strings == null)
                strings = entry.Strings;
            EntryConfigv1 conf = null;
            string json = strings.ReadSafe("KPRPC JSON");
            if (string.IsNullOrEmpty(json))
                conf = new EntryConfigv1(mam);
            else
            {
                try
                {
                    conf = (EntryConfigv1)Jayrock.Json.Conversion.JsonConvert.Import(typeof(EntryConfigv1), json);
                }
                catch (Exception)
                {
                    var url = strings.ReadSafe("URL");
                    if (string.IsNullOrEmpty(url))
                        url = "<unknown>";
                    if (configErrors != null)
                    {
                        string entryUserName = strings.ReadSafe(PwDefs.UserNameField);
                        //entryUserName = KeePassRPCPlugin.GetPwEntryStringFromDereferencableValue(entry, entryUserName, db);
                        configErrors.Add("Username: " + entryUserName + ". URL: " + url);
                    }
                    else
                    {
                        Utils.ShowMonoSafeMessageBox("There are configuration errors in this entry. To fix the entry and prevent this warning message appearing, please edit the value of the 'KPRPC JSON' advanced string. Please ask for help on https://forum.kee.pm if you're not sure how to fix this. The URL of the entry is: " + url + " and the full configuration data is: " + json, "Warning: Configuration errors", MessageBoxButtons.OK, MessageBoxIcon.Warning);
                    }
                    return null;
                }
            }
            return conf;
        }
        
        public static EntryConfigv2 GetKPRPCConfigV2(this PwEntry entry, ref List<string> configErrors, MatchAccuracyMethod mam)
        {
            EntryConfigv2 conf = null;
            string json = entry.CustomData.Get("KPRPC JSON");
            if (string.IsNullOrEmpty(json))
                conf = new EntryConfigv2(mam);
            else
            {
                try
                {
                    conf = (EntryConfigv2)Jayrock.Json.Conversion.JsonConvert.Import(typeof(EntryConfigv2), json);
                }
                catch (Exception ex)
                {
                    var url = entry.Strings.ReadSafe("URL");
                    if (string.IsNullOrEmpty(url))
                        url = "<unknown>";
                    if (configErrors != null)
                    {
                        string entryUserName = entry.Strings.ReadSafe(PwDefs.UserNameField);
                        configErrors.Add("Username: " + entryUserName + ". URL: " + url);
                    }
                    else
                    {
                        Utils.ShowMonoSafeMessageBox("There are configuration errors in this entry. To fix the entry and prevent this warning message appearing, please edit the value of the 'KPRPC JSON' custom data. Please ask for help on https://forum.kee.pm if you're not sure how to fix this. The URL of the entry is: " + url + " and the full configuration data is: " + json, "Warning: Configuration errors", MessageBoxButtons.OK, MessageBoxIcon.Warning);
                    }
                    return null;
                }
            }
            return conf;
        }

        public static EntryConfig GetKPRPCConfig(this PwEntry entry, ProtectedStringDictionary strings, MatchAccuracyMethod mam)
        {
            List<string> dummy = null;
            return entry.GetKPRPCConfig(strings, ref dummy, mam);
        }

        public static EntryConfig GetKPRPCConfig(this PwEntry entry, MatchAccuracyMethod mam)
        {
            List<string> dummy = null;
            return entry.GetKPRPCConfig(null, ref dummy, mam);
        }

        public static EntryConfigv2 GetKPRPCConfigNormalised(this PwEntry entry, ProtectedStringDictionary strings, MatchAccuracyMethod mam)
        {
            List<string> dummy = null;
            return entry.GetKPRPCConfigNormalised(strings, ref dummy, mam);
        }

        public static EntryConfigv2 GetKPRPCConfigNormalised(this PwEntry entry, MatchAccuracyMethod mam)
        {
            List<string> dummy = null;
            return entry.GetKPRPCConfigNormalised(null, ref dummy, mam);
        }

        
        
        public static EntryConfigv1 GetKPRPCConfigV1(this PwEntry entry, ProtectedStringDictionary strings, MatchAccuracyMethod mam)
        {
            List<string> dummy = null;
            return entry.GetKPRPCConfigV1(strings, ref dummy, mam);
        }

        public static EntryConfigv1 GetKPRPCConfigV1(this PwEntry entry, MatchAccuracyMethod mam)
        {
            List<string> dummy = null;
            return entry.GetKPRPCConfigV1(null, ref dummy, mam);
        }

        public static EntryConfigv2 GetKPRPCConfigV2(this PwEntry entry, MatchAccuracyMethod mam)
        {
            List<string> dummy = null;
            return entry.GetKPRPCConfigV2(ref dummy, mam);
        }
        
        public static void SetKPRPCConfig(this PwEntry entry, EntryConfigv1 newConfig)
        {
            entry.Strings.Set("KPRPC JSON", new ProtectedString(
                true, Jayrock.Json.Conversion.JsonConvert.ExportToString(newConfig)));
        }
        
        public static void SetKPRPCConfig(this PwEntry entry, EntryConfigv2 newConfig)
        {
            entry.CustomData.Set("KPRPC JSON", Jayrock.Json.Conversion.JsonConvert.ExportToString(newConfig));
        }
        
        public static MatchAccuracyMethod GetMatchAccuracyMethod(this PwEntry entry, URLSummary urlsum, DatabaseConfig dbConf)
        {
            var conf = entry.GetKPRPCConfigNormalised(dbConf.DefaultMatchAccuracy);
            MatchAccuracyMethod overridenMethod;
            if (urlsum.Domain != null && urlsum.Domain.RegistrableDomain != null &&
                dbConf.MatchedURLAccuracyOverrides.TryGetValue(urlsum.Domain.RegistrableDomain, out overridenMethod))
                return overridenMethod;
            else
                return conf.MatcherConfigs.First(mc => mc.MatcherType == EntryMatcherType.Url).UrlMatchMethod ?? MatchAccuracyMethod.Domain;
        }

        public static DatabaseConfig GetKPRPCConfig(this PwDatabase db)
        {
            if (!db.CustomData.Exists("KeePassRPC.Config"))
            {
                // Set custom data and migrate the old config custom data to this
                // version (but don't save the DB - we can do this again and again until
                // user decides to save a change for another reason)
                var newConfig = new DatabaseConfig();

                // This migration can be removed in 2021
                if (db.CustomData.Exists("KeePassRPC.KeeFox.rootUUID"))
                    newConfig.RootUUID = db.CustomData.Get("KeePassRPC.KeeFox.rootUUID");

                db.SetKPRPCConfig(newConfig);
                return newConfig;
            }
            else
            {
                try
                {
                    return (DatabaseConfig)Jayrock.Json.Conversion.JsonConvert.Import(typeof(DatabaseConfig), db.CustomData.Get("KeePassRPC.Config"));
                }
                catch (Exception)
                {
                    // Reset to default config because the current stored config is corrupt
                    var newConfig = new DatabaseConfig();
                    db.SetKPRPCConfig(newConfig);
                    return newConfig;
                }
            }
        }

        public static void SetKPRPCConfig(this PwDatabase db, DatabaseConfig newConfig)
        {
            db.CustomData.Set("KeePassRPC.Config", Jayrock.Json.Conversion.JsonConvert.ExportToString(newConfig));
        }

        public static bool IsOrIsContainedIn(this PwGroup gp, PwGroup hostGroup)
        {
            if (gp == hostGroup) return true;
            return gp.IsContainedIn(hostGroup);
        }

    }
}

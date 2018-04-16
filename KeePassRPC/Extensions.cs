using KeePassLib;
using KeePassLib.Collections;
using KeePassLib.Security;
using KeePassRPC.DataExchangeModel;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Windows.Forms;

namespace KeePassRPC
{
    public static class Extensions
    {
        public static EntryConfig GetKPRPCConfig(this PwEntry entry, ProtectedStringDictionary strings, ref List<string> configErrors, PwDatabase db)
        {
            if (strings == null)
                strings = entry.Strings;
            EntryConfig conf = null;
            string json = strings.ReadSafe("KPRPC JSON");
            if (string.IsNullOrEmpty(json))
                conf = new EntryConfig(db.GetKPRPCConfig().DefaultMatchAccuracy);
            else
            {
                try
                {
                    conf = (EntryConfig)Jayrock.Json.Conversion.JsonConvert.Import(typeof(EntryConfig), json);
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
                        MessageBox.Show("There are configuration errors in this entry. To fix the entry and prevent this warning message appearing, please edit the value of the 'KPRPC JSON' advanced string. Please ask for help on https://forum.kee.pm if you're not sure how to fix this. The URL of the entry is: " + url + " and the full configuration data is: " + json, "Warning: Configuration errors", MessageBoxButtons.OK, MessageBoxIcon.Warning);
                    }
                    return null;
                }
            }
            return conf;
        }

        public static EntryConfig GetKPRPCConfig(this PwEntry entry, ProtectedStringDictionary strings, PwDatabase db)
        {
            List<string> dummy = null;
            return entry.GetKPRPCConfig(strings, ref dummy, db);
        }

        public static EntryConfig GetKPRPCConfig(this PwEntry entry, PwDatabase db)
        {
            List<string> dummy = null;
            return entry.GetKPRPCConfig(null, ref dummy, db);
        }

        public static void SetKPRPCConfig(this PwEntry entry, EntryConfig newConfig)
        {
            entry.Strings.Set("KPRPC JSON", new ProtectedString(
                true, Jayrock.Json.Conversion.JsonConvert.ExportToString(newConfig)));
        }
        
        public static MatchAccuracyMethod GetMatchAccuracyMethod(this PwEntry entry, URLSummary urlsum, PwDatabase db)
        {
            var conf = entry.GetKPRPCConfig(db);
            var dbConf = db.GetKPRPCConfig();
            MatchAccuracyMethod overridenMethod;
            if (dbConf.MatchedURLAccuracyOverrides.TryGetValue(urlsum.Domain, out overridenMethod))
                return overridenMethod;
            else
                return conf.GetMatchAccuracyMethod();
        }

        public static DatabaseConfig GetKPRPCConfig(this PwDatabase db)
        {
            if (!db.CustomData.Exists("KeePassRPC.Config"))
            {
                //TODO: Set custom data and migrate the old config custom data to this
                // version (but don't save the DB - we can do this again and again until
                // user decides to save a change for another reason)
                return new DatabaseConfig();
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
    }
}

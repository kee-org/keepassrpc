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
        public static EntryConfig GetKPRPCConfig(this PwEntry entry, ProtectedStringDictionary strings, ref List<string> configErrors)
        {
            if (strings == null)
                strings = entry.Strings;
            EntryConfig conf = null;
            string json = strings.ReadSafe("KPRPC JSON");
            if (string.IsNullOrEmpty(json))
                conf = new EntryConfig();
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
                        MessageBox.Show("There are configuration errors in this entry. To fix the entry and prevent this warning message appearing, please edit the value of the 'KPRPC JSON' advanced string. Please ask for help on http://keefox.org/help/forum if you're not sure how to fix this. The URL of the entry is: " + url + " and the full configuration data is: " + json, "Warning: Configuration errors", MessageBoxButtons.OK, MessageBoxIcon.Warning);
                    }
                    return null;
                }
            }
            return conf;
        }

        public static EntryConfig GetKPRPCConfig(this PwEntry entry, ProtectedStringDictionary strings)
        {
            List<string> dummy = null;
            return entry.GetKPRPCConfig(strings, ref dummy);
        }

        public static EntryConfig GetKPRPCConfig(this PwEntry entry)
        {
            List<string> dummy = null;
            return entry.GetKPRPCConfig(null, ref dummy);
        }

        public static void SetKPRPCConfig(this PwEntry entry, EntryConfig newConfig)
        {
            entry.Strings.Set("KPRPC JSON", new ProtectedString(
                true, Jayrock.Json.Conversion.JsonConvert.ExportToString(newConfig)));
        }
    }
}

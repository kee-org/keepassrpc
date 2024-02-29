using KeePassRPC.Models.Shared;

namespace KeePassRPC.Models.DataExchange
{
    public class Entry2 : LightEntry
    {
        public string HTTPRealm;
        public FormField[] FormFieldList;

        // How accurately do the URLs in this entry match the URL we are looking for?
        // Higher = better match.
        // We don't consider protocol
        public int MatchAccuracy;

        public bool AlwaysAutoFill;
        public bool NeverAutoFill;
        public bool AlwaysAutoSubmit;
        public bool NeverAutoSubmit;
        public int Priority; // "Kee priority" = 0 (no longer used)

        public Group Parent;
        public Database Db;

        public Entry2() { }

        public Entry2(
            string[] urls,
            string hTTPRealm,
            string title,
            FormField[] formFieldList,
            string uniqueID,
            bool alwaysAutoFill,
            bool neverAutoFill,
            bool alwaysAutoSubmit,
            bool neverAutoSubmit,
            int priority,
            Group parent,
            string iconImageData,
            Database db,
            int matchAccuracy)
        {
            URLs = urls;
            HTTPRealm = hTTPRealm;
            Title = title;
            FormFieldList = formFieldList;
            UniqueID = uniqueID;
            AlwaysAutoFill = alwaysAutoFill;
            NeverAutoFill = neverAutoFill;
            AlwaysAutoSubmit = alwaysAutoSubmit;
            NeverAutoSubmit = neverAutoSubmit;
            Priority = priority;
            Parent = parent;
            IconImageData = iconImageData;
            Db = db;
            MatchAccuracy = matchAccuracy;
        }
    }
}
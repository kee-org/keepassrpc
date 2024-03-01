using KeePassRPC.Models.Shared;

namespace KeePassRPC.Models.DataExchange.V2
{
    public class Entry2 : LightEntry2
    {
        public string Realm;
        public ResolvedField[] Fields;
        public EntryAutomationBehaviour? Behaviour;

        // How accurately do the URLs in this entry match the URL we are looking for?
        // Higher = better match.
        // We don't consider protocol
        public int MatchAccuracy;

        public Group2 Parent;
        public Database2 Db;

        public EntryMatcherConfig[] MatcherConfigs;

        public Entry2() { }

        public Entry2(
            string[] urls,
            string realm,
            string title,
            ResolvedField[] fields,
            EntryAutomationBehaviour? behaviour,
            string uuid,
            Group2 parent,
            Icon icon,
            Database2 db,
            int matchAccuracy,
            EntryMatcherConfig[] matcherConfigs)
        {
            Urls = urls;
            Realm = realm;
            Title = title;
            Fields = fields;
            Uuid = uuid;
            Parent = parent;
            Icon = icon;
            Db = db;
            MatchAccuracy = matchAccuracy;
            Behaviour = behaviour;
            MatcherConfigs = matcherConfigs;
        }
    }
}
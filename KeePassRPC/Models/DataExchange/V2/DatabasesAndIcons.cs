namespace KeePassRPC.Models.DataExchange.V2
{
    public class DatabasesAndIcons
    {
        public Database2[] Databases;
        public IconCollection[] Icons;
    }

    public class DatabaseAndIcons
    {
        public Database2 Database;
        public IconCollection Icons;
    }

    public class IconCollection
    {
        // Sanity check since we can't use generic dictionaries with JSON-RPC - arrays should
        // be in order anyway but just in case this is a tiny overhead
        public string DatabaseFilename;
        
        public string DatabaseIcon;
        public IconData[] CustomIcons;
        public IconData[] StandardIcons;
    }

    public class IconData
    {
        public string Id; // int for standard or uuid for custom
        public string Icon;
    }
}
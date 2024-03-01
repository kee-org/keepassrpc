namespace KeePassRPC.Models.DataExchange.V2
{
    public class Group2
    {
        public string Title;
        public string UniqueID;
        public Icon Icon;
        public string Path;

        public Group2[] ChildGroups;
        public Entry2[] ChildEntries;
        public LightEntry2[] ChildLightEntries;

        public Group2() { }

        public Group2(string title,
            string uniqueId,
            Icon icon,
            string path)
        {
            Title = title;
            UniqueID = uniqueId;
            Icon = icon;
            Path = path;
        }
    }
}
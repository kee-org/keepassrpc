namespace KeePassRPC.Models.DataExchange
{
    public class Group
    {
        public string Title;
        public string UniqueID;
        public string IconImageData;
        public string Path;

        public Group[] ChildGroups;
        public Entry[] ChildEntries;
        public LightEntry[] ChildLightEntries;

        public Group() { }

        public Group(string title,
            string uniqueID,
            string iconImageData,
            string path)
        {
            Title = title;
            UniqueID = uniqueID;
            IconImageData = iconImageData;
            Path = path;
        }
    }
}
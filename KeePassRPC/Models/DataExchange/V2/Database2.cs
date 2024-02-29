namespace KeePassRPC.Models.DataExchange.V2
{
    public class Database2
    {
        public string Name;
        public string FileName;
        public Group2 Root;
        public bool Active;
        public Icon Icon;

        public Database2() { }

        public Database2(string name,
            string fileName,
            Group2 root,
            bool active,
            Icon icon)
        {
            Name = name;
            Root = root;
            FileName = fileName;
            Active = active;
            Icon = icon;
        }
    }
}
namespace KeePassRPC.Models.DataExchange
{
    public class Database2
    {
        public string Name;
        public string FileName;
        public Group Root;
        public bool Active;
        public string IconImageData;

        public Database2() { }

        public Database2(string name,
            string fileName,
            Group root,
            bool active,
            string iconImageData)
        {
            Name = name;
            Root = root;
            FileName = fileName;
            Active = active;
            IconImageData = iconImageData;
        }
    }
}
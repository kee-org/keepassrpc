namespace KeePassRPC.Models.DataExchange
{
    public class Database
    {
        public string Name;
        public string FileName;
        public Group Root;
        public bool Active;
        public string IconImageData;

        public Database() { }

        public Database(string name,
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
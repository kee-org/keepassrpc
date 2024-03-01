namespace KeePassRPC.Models.DataExchange
{
    public class LightEntry
    {
        public string[] URLs;
        public string Title;
        public string UniqueID;
        public string UsernameValue;
        public string UsernameName;
        public string IconImageData;

        public LightEntry() { }

        public LightEntry(
            string[] urls,
            string title,
            string uniqueId,
            string iconImageData,
            string usernameName,
            string usernameValue)
        {
            URLs = urls;
            Title = title;
            UniqueID = uniqueId;
            IconImageData = iconImageData;
            UsernameName = usernameName;
            UsernameValue = usernameValue;
        }
    }
}
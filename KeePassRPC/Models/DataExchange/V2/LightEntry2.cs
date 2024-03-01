namespace KeePassRPC.Models.DataExchange.V2
{
    public class LightEntry2
    {
        public string[] Urls;
        public string Title;
        public string UniqueID;
        public string UsernameValue;
        public string UsernameName;
        public Icon Icon;
        public string[] AuthenticationMethods;

        public LightEntry2() { }

        public LightEntry2(
            string[] urls,
            string title,
            string uniqueId,
            Icon icon,
            string usernameName,
            string usernameValue,
            string[] authenticationMethods)
        {
            Urls = urls;
            Title = title;
            UniqueID = uniqueId;
            Icon = icon;
            UsernameName = usernameName;
            UsernameValue = usernameValue;
            AuthenticationMethods = authenticationMethods;
        }
    }
}
using System;

namespace KeePassRPC
{
    public class KeyContainerClass
    {
        public string Key;
        public DateTime AuthExpires;
        public string Username;
        public string ClientName;

        public KeyContainerClass() { }

        public KeyContainerClass(string key, DateTime expiry, string username, string clientName)
        {
            Key = key;
            AuthExpires = expiry;
            Username = username;
            ClientName = clientName;
        }
    }
}

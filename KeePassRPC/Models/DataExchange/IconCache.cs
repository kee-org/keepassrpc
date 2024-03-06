using System.Collections.Generic;

namespace KeePassRPC.Models.DataExchange
{
    public abstract class IconCache<T>
    {
        private static readonly object _iconCacheLock = new object();

        private static readonly Dictionary<T, string> _icons = new Dictionary<T, string>();
        // public static Dictionary<PwUuid, string> Icons { get { } set { } }
        public static void AddIcon(T iconId, string base64Representation)
        {
            lock (_iconCacheLock)
            {
                if (!_icons.ContainsKey(iconId))
                    _icons.Add(iconId, base64Representation);
            }
        }

        public static string GetIconEncoding(T iconId)
        {
            lock (_iconCacheLock)
            {
                string base64Representation;
                if (!_icons.TryGetValue(iconId, out base64Representation))
                    return null;
                return base64Representation;
            }
        }



    }
}
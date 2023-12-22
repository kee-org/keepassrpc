using System.Collections.Generic;

namespace KeePassRPC.Models.DataExchange
{
    public class IconCache<T>
    {
        private static object iconCacheLock = new object();
        public static Dictionary<T, string> _icons = new Dictionary<T, string>();
        // public static Dictionary<PwUuid, string> Icons { get { } set { } }
        public static void AddIcon(T iconId, string base64representation)
        {
            lock (iconCacheLock)
            {
                if (!_icons.ContainsKey(iconId))
                    _icons.Add(iconId, base64representation);
            }
        }

        public static string GetIconEncoding(T iconId)
        {
            string base64representation = null;
            lock (iconCacheLock)
            {
                if (!_icons.TryGetValue(iconId, out base64representation))
                    return null;
                return base64representation;
            }
        }



    }
}
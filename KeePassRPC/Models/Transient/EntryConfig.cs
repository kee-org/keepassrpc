using KeePassRPC.Models.Persistent;

namespace KeePassRPC.Models.Transient
{
    public class EntryConfig
    {
        public EntryConfig(EntryConfigv1 v1, EntryConfigv2 v2)
        {
            V1 = v1;
            V2 = v2;
        }

        public EntryConfigv1 V1;
        public EntryConfigv2 V2;
    }
}
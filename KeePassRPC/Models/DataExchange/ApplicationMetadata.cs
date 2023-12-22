namespace KeePassRPC.Models.DataExchange
{
    public class ApplicationMetadata
    {
        public string KeePassVersion;
        public bool IsMono;
        public string NETCLR;
        public string NETversion;
        public string MonoVersion;

        public ApplicationMetadata() { }
        public ApplicationMetadata(string keePassVersion, bool isMono, string nETCLR, string nETversion, string monoVersion)
        {
            KeePassVersion = keePassVersion;
            IsMono = isMono;
            NETCLR = nETCLR;
            NETversion = nETversion;
            MonoVersion = monoVersion;
        }
    }
}
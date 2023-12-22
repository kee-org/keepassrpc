namespace KeePassRPC.Models.DataExchange
{
    public class KPRPCMessage
    {
        public string protocol;
        public JSONRPCContainer jsonrpc;
        public SRPParams srp;
        public KeyParams key;
        public int version;
        public string clientDisplayName;
        public string clientDisplayDescription;
        public string clientTypeId;
        public Error error;
        public string[] features;
    }
}
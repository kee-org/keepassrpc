using Jayrock.Json.Conversion;

namespace KeePassRPC.Models.DataExchange
{
    public class SRPParams
    {
        [JsonMemberName("I")]
        public string I;
        [JsonMemberName("A")]
        public string A;
        [JsonMemberName("S")]
        public string S;
        [JsonMemberName("B")]
        public string B;
        [JsonMemberName("M")]
        public string M;
        [JsonMemberName("M2")]
        public string M2;
        [JsonMemberName("s")]
        public string s;
        public string stage;
        public int securityLevel;
    }
}
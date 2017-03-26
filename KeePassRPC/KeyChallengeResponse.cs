using System;
using KeePassRPC.DataExchangeModel;

namespace KeePassRPC
{

    class KeyChallengeResponse
    {

        public string cc;
        public string cr;
        public string sc;
        public string sr;
        static int ProtocolVersion;

        public KeyChallengeResponse (int protocolVersion)
        {
            ProtocolVersion = protocolVersion;
        }

        public string KeyChallengeResponse1(string userName, int securityLevel)
        {
            BigInteger scTemp = new BigInteger();
            scTemp.genRandomBits(256, new Random((int)DateTime.Now.Ticks));
            sc = scTemp.ToString().ToLower();


            KPRPCMessage data2client = new KPRPCMessage();
            data2client.protocol = "setup";
            data2client.key = new KeyParams();
            data2client.key.sc = sc;
            data2client.key.securityLevel = securityLevel;
            data2client.version = ProtocolVersion;


            string response = Jayrock.Json.Conversion.JsonConvert.ExportToString(data2client);
            return response;
        }

        public string KeyChallengeResponse2(string cc, string cr, KeyContainerClass kc, int securityLevel, out bool authorised)
        {
            string response = null;
            this.cc = cc;
            this.cr = KeePassLib.Utility.MemUtil.ByteArrayToHexString(Utils.Hash("1" + kc.Key + this.sc + this.cc)).ToLower();
            if (cr != this.cr)
            {
                authorised = false;
                KPRPCMessage data2client = new KPRPCMessage();
                data2client.protocol = "setup";
                data2client.version = ProtocolVersion;
                data2client.error = new Error(ErrorCode.AUTH_FAILED, new string[] { "Keys do not match" });
                response = Jayrock.Json.Conversion.JsonConvert.ExportToString(data2client);
            }
            else
            {
                this.sr = KeePassLib.Utility.MemUtil.ByteArrayToHexString(Utils.Hash("0" + kc.Key + this.sc + this.cc)).ToLower();
                authorised = true;

                KPRPCMessage data2client = new KPRPCMessage();
                data2client.protocol = "setup";
                data2client.key = new KeyParams();
                data2client.key.sr = this.sr;
                data2client.key.securityLevel = securityLevel;
                data2client.version = ProtocolVersion;
                response = Jayrock.Json.Conversion.JsonConvert.ExportToString(data2client);
            }
            return response;
        }

    }
}

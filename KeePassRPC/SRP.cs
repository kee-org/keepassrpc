using System;
using KeePassRPC.DataExchangeModel;

namespace KeePassRPC
{
    class SRP
    {
        private BigInteger _N;
        private string _Nstr;
        private BigInteger _g;
        private string _gHex;
        private BigInteger _k;
        private string _K;
        private BigInteger _x;
        private BigInteger _v;
        private string _s;
        private string _I;
        private BigInteger _B;
        private string _Bstr;
        private BigInteger _b;

        public BigInteger N { get { return _N; } }
        public string Nstr { get { return _Nstr; } }
        public BigInteger g { get { return _g; } }
        public string gHex { get { return _gHex; } }
        public BigInteger k { get { return _k; } }
        public string K { get { return _K; } }
        public BigInteger x { get { return _x; } }
        public BigInteger v { get { return _v; } }
        public string s { get { return _s; } }
        public string I { get { return _I; } }
        public BigInteger B { get { return _B; } }
        public string Bstr { get { return _Bstr; } }
        public BigInteger b { get { return _b; } }
        private BigInteger S;
        public string M, M2;

        private bool _Authenticated;
        public bool Authenticated { get { return _Authenticated; } }

        // If someone wants to use the session key for encrypting traffic, they can
        // access the key with this property.
        public string Key
        {
            get
            {
                if (_K == null)
                {
                    if (Authenticated)
                    {
                        _K = KeePassLib.Utility.MemUtil.ByteArrayToHexString(Utils.Hash(S.ToString(16))).ToLower();
                        return _K;
                    }
                    else
                        throw new System.Security.Authentication.AuthenticationException("User has not been authenticated.");
                }
                else
                    return _K;
            }
        }

        public SRP()
        {
            _Nstr = "d4c7f8a2b32c11b8fba9581ec4ba4f1b04215642ef7355e37c0fc0443ef756ea2c6b8eeb755a1c723027663caa265ef785b8ff6a9b35227a52d86633dbdfca43";
            _N = new BigInteger(_Nstr, 16);
            _g = new BigInteger(2);
            _k = new BigInteger("b7867f1299da8cc24ab93e08986ebc4d6a478ad0", 16);
        }

        internal void CalculatePasswordHash(string password)
        {
            BigInteger sTemp = new BigInteger();
            sTemp.genRandomBits(256, new Random((int)DateTime.Now.Ticks));
            _s = sTemp.ToString();
            _x = new BigInteger(Utils.Hash(_s + password));
            _v = g.modPow(_x, _N);
        }

        internal void Setup()
        {
            _b = new BigInteger();
            _b.genRandomBits(256, new Random((int)DateTime.Now.Ticks));

            _B = (_k * _v) + (_g.modPow(_b, _N));
            while (_B % _N == 0)
            {
                _b.genRandomBits(256, new Random((int)DateTime.Now.Ticks));
                _B = (_k * _v) + (_g.modPow(_b, _N));
            }
            _Bstr = _B.ToString(16);
        }

        // Send salt to the client and store the parameters they sent to us
        internal Error Handshake(string I, string Astr)
        {
            this._I = I;
            return Calculations(Astr, v);
        }

        // Calculate S, M, and M2
        // This is the server side of the SRP specification
        private Error Calculations(string Astr, BigInteger v)
        {
            BigInteger A = new BigInteger(Astr, 16);

            // u = H(A,B)
            BigInteger u = new BigInteger(Utils.Hash(Astr + this.Bstr));
            if (u == 0)
                return new Error(ErrorCode.AUTH_INVALID_PARAM);

            //S = (Av^u) ^ b
            BigInteger Avu = A * (v.modPow(u, N));
            this.S = Avu.modPow(b, N);

            // Calculate the auth hash we will expect from the client (M) and the one we will send back in the next step (M2)
            // M = H(A, B, S)
            //M2 = H(A, M, S)
            string Mstr = A.ToString(16) + this.B.ToString(16) + this.S.ToString(16);
            this.M = KeePassLib.Utility.MemUtil.ByteArrayToHexString(Utils.Hash(Mstr));
            this.M2 = KeePassLib.Utility.MemUtil.ByteArrayToHexString(Utils.Hash(A.ToString(16) + this.M.ToLower() + this.S.ToString(16)));
            return new Error(ErrorCode.SUCCESS);
        }

        // Receive M from the client and verify it
        internal void Authenticate(string Mclient)
        {
            if (Mclient.ToLower() == M.ToLower())
            {
                _Authenticated = true;
            }
        }


    }
}

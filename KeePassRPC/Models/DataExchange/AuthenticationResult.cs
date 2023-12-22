namespace KeePassRPC.Models.DataExchange
{
    public class AuthenticationResult
    {
        // private int _result;
        public int Result;// { get { return _result; } }
        //private string _name;
        public string Name;// { get { return _name; } }

        public AuthenticationResult() { }
        public AuthenticationResult(int res, string name)
        {
            Name = name;
            Result = res;
            //_name = name;
            //_result = res;
        }
    }
}
namespace KeePassRPC.Models.DataExchange
{
    public class Error
    {
        public ErrorCode code;
        public string[] messageParams;

        public Error() { }
        public Error(ErrorCode code, string[] messageParams) { this.code = code; this.messageParams = messageParams; }
        public Error(ErrorCode code) { this.code = code; }
    }
}
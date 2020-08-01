namespace KeePassRPC
{
    public struct WebSocketServerConfig
    {
        public int WebSocketPort;
        public bool BindOnlyToLoopback;
        public string[] PermittedOrigins;
    }
}
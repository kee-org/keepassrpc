namespace KeePassRPC.JsonRpc
{
    internal interface IJsonRpcRequestLifetimeNotice
    {
        void OnStart(ClientMetadata cm);
        void OnEnd();
    }
}
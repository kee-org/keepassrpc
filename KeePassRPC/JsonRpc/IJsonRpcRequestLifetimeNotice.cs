namespace KeePassRPC.JsonRpc
{
    interface IJsonRpcRequestLifetimeNotice
    {
        void OnStart(ClientMetadata cm);
        void OnEnd();
    }
}
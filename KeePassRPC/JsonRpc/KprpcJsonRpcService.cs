using Jayrock.JsonRpc;

namespace KeePassRPC.JsonRpc
{
    public class KprpcJsonRpcService : JsonRpcService,
        IJsonRpcRequestLifetimeNotice,
        IJsonRpcClientMetadataProperty
    {
        private ClientMetadata _clientMetadata;

        public ClientMetadata ClientMetadata
        {
            get { return _clientMetadata; }
        }

        void IJsonRpcRequestLifetimeNotice.OnStart(ClientMetadata cm)
        {
            _clientMetadata = cm;
        }

        void IJsonRpcRequestLifetimeNotice.OnEnd()
        {
            _clientMetadata = null;
        }
    }
}
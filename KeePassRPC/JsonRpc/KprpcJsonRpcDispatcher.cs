using System.Collections;
using Jayrock.JsonRpc;
using Jayrock.Services;

namespace KeePassRPC.JsonRpc
{
    internal sealed class KprpcJsonRpcDispatcher : JsonRpcDispatcher
    {
        public ClientMetadata ClientMetadata;

        public KprpcJsonRpcDispatcher(IService service) : base(service)
        {
        }
        
        public override IDictionary Invoke(IDictionary request, bool authorised)
        {
            var notice = Service as IJsonRpcRequestLifetimeNotice;
            if (notice != null) notice.OnStart(ClientMetadata);
            try
            {
                var response = base.Invoke(request, authorised);
                return response;
            }
            finally
            {
                if (notice != null) notice.OnEnd();
            }
        }
    }
}
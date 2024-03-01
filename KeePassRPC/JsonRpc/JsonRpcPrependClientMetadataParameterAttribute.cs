using System;
using System.Collections;
using System.Linq;
using System.Reflection;
using Jayrock;
using Jayrock.JsonRpc;
using Jayrock.Services;

namespace KeePassRPC.JsonRpc
{
    public class ClientMetadata
    {
        public string[] Features;
    }

    interface IJsonRpcRequestLifetimeNotice
    {
        void OnStart(ClientMetadata cm);
        void OnEnd(ClientMetadata cm);
    }

    sealed class KprpcJsonRpcDispatcher : Jayrock.JsonRpc.JsonRpcDispatcher
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
                if (notice != null) notice.OnEnd(ClientMetadata);
            }
        }
    }

    interface IJsonRpcClientMetadataProperty
    {
        ClientMetadata ClientMetadata { get; }
    }

    public class KprpcJsonRpcService : Jayrock.JsonRpc.JsonRpcService,
        IJsonRpcRequestLifetimeNotice,
        IJsonRpcClientMetadataProperty
    {
        ClientMetadata _clientMetadata;

        ClientMetadata IJsonRpcClientMetadataProperty.ClientMetadata
        {
            get { return _clientMetadata; }
        }

        void IJsonRpcRequestLifetimeNotice.OnStart(ClientMetadata cm)
        {
            _clientMetadata = cm;
        }

        void IJsonRpcRequestLifetimeNotice.OnEnd(ClientMetadata cm)
        {
            _clientMetadata = null;
        }
    }

    [AttributeUsage(AttributeTargets.Method)]
    sealed class JsonRpcPrependClientMetadataParameterAttribute : Attribute,
        IMethodModifier,
        IAttributeAttachment
    {
        MethodInfo _method;

        void IAttributeAttachment.SetAttachment(ICustomAttributeProvider obj)
        {
            _method = (MethodInfo)obj;
        }

        void IMethodModifier.Modify(MethodBuilder builder)
        {
            builder.Handler = new MethodImpl(_method.GetParameters(), builder.Handler);
        }

        sealed class MethodImpl : IMethodImpl
        {
            readonly IMethodImpl _impl;
            readonly ParameterInfo[] _parameters;

            public MethodImpl(ParameterInfo[] parameters, IMethodImpl impl)
            {
                _impl = impl;
                _parameters = parameters;
            }

            object[] PrependClientMetadata(IService service, object[] args)
            {
                // The expected parameters of our method, including ClientMetaData
                var parameters = _parameters;

                IJsonRpcClientMetadataProperty rp;
                ClientMetadata cm;
                if ((rp = service as IJsonRpcClientMetadataProperty) != null
                    && (cm = rp.ClientMetadata) != null)
                {
                    var cmArr = new object[] { cm };
                    return cmArr.Concat(args).ToArray();
                }

                return (new object[] { null }).Concat(args).ToArray();
            }

            //Invoke(IService service, string[] names, object[] args)
            public object Invoke(IService service, object[] args)
            {
                return _impl.Invoke(service, PrependClientMetadata(service, args));
            }

            public IAsyncResult BeginInvoke(IService service, object[] args, AsyncCallback callback, object asyncState)
            {
                return _impl.BeginInvoke(service, PrependClientMetadata(service, args), callback, asyncState);
            }

            public object EndInvoke(IService service, IAsyncResult asyncResult)
            {
                return _impl.EndInvoke(service, asyncResult);
            }

            public bool IsAsynchronous
            {
                get { return _impl.IsAsynchronous; }
            }
        }
    }
}
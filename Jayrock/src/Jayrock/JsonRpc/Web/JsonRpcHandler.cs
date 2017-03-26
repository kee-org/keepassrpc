#region License, Terms and Conditions
//
// Jayrock - JSON and JSON-RPC for Microsoft .NET Framework and Mono
// Written by Atif Aziz (www.raboof.com)
// Copyright (c) 2005 Atif Aziz. All rights reserved.
//
// This library is free software; you can redistribute it and/or modify it under
// the terms of the GNU Lesser General Public License as published by the Free
// Software Foundation; either version 3 of the License, or (at your option)
// any later version.
//
// This library is distributed in the hope that it will be useful, but WITHOUT
// ANY WARRANTY; without even the implied warranty of MERCHANTABILITY or FITNESS
// FOR A PARTICULAR PURPOSE. See the GNU Lesser General Public License for more
// details.
//
// You should have received a copy of the GNU Lesser General Public License
// along with this library; if not, write to the Free Software Foundation, Inc.,
// 59 Temple Place, Suite 330, Boston, MA 02111-1307 USA 
//
#endregion

namespace Jayrock.JsonRpc.Web
{
    #region Imports

    using System;
    using System.Collections;
    using System.Configuration;
    using System.Security.Principal;
    using System.Web;
    using System.Web.Caching;
    using System.Web.SessionState;

    #endregion

    public class JsonRpcHandler : JsonRpcService, IHttpHandler
    {
        private HttpContext _context;
        private IDictionary _featureByName;
        private bool _featureByNameInitialized;

        public virtual void ProcessRequest(HttpContext context)
        {
            _context = context;

            try
            {
                ProcessRequest();
            }
            finally
            {
                _context = null;
            }
        }
        
        public virtual void ProcessRequest()
        {
            object feature = InferFeature();

            if (feature == null)
                throw new JsonRpcException("There is no service feature available for this type of request.");

            IHttpHandler handler = feature as IHttpHandler;
        
            if (handler == null)
                throw new JsonRpcException(string.Format("The {0} feature does not support HTTP.", feature.GetType().FullName));

            handler.ProcessRequest(Context);
        }

        protected virtual object InferFeature()
        {
            HttpRequest request = Context.Request;
            string verb = request.RequestType;

            if (CaselessString.Equals(verb, "GET") ||
                CaselessString.Equals(verb, "HEAD"))
            {
                //
                // If there is path tail then it indicates a GET-safe RPC.
                //
                
                if (request.PathInfo.Length > 1)
                    return GetFeatureByName("getrpc");
                
                //
                // Otherwise, get the feature name from anonymous query 
                // string parameter.
                //

                return GetFeatureByName(Mask.EmptyString(request.QueryString[null], "help"));
            }
            else if (CaselessString.Equals(verb, "POST")) 
            {
                //
                // POST means RPC.
                //
                
                return GetFeatureByName("rpc");
            }

            return null;
        }

        protected IDictionary Features
        {
            get
            {
                if (!_featureByNameInitialized)
                {
                    _featureByNameInitialized = true;
                    _featureByName = GetFeatures();
                }

                return _featureByName;
            }
        }

        protected virtual IDictionary GetFeatures()
        {
            IDictionary config = (IDictionary) ConfigurationSettings.GetConfig("jayrock/jsonrpc/features");
            
            if (config == null)
            {
                //
                // Check an alternate path for backward compatibility.
                //
                
                config = (IDictionary) ConfigurationSettings.GetConfig("jayrock/json.rpc/features");
            }
            
            if (config == null)
                config = DefaultFeatures;
            
            return config;
        }

        protected virtual object GetFeatureByName(string name)
        {
            IDictionary features = Features;
            
            if (features == null || !features.Contains(name))
                throw new JsonRpcException(string.Format("There is no feature registered for '{0}' type of requests.", name));

            string featureTypeSpec = Mask.NullString((string) features[name]);
            
            if (featureTypeSpec.Length == 0)
                throw new JsonRpcException("Missing feature type specification.");

            Type featureType = TypeResolution.GetType(featureTypeSpec);
            return Activator.CreateInstance(featureType, new object[] { this });
        }

        bool IHttpHandler.IsReusable
        {
            get { return false; }
        }

        public HttpContext Context
        {
            get { return _context; }
        }

        public HttpApplication ApplicationInstance
        {
            get { return Context.ApplicationInstance; }
        }

        public HttpApplicationState Application
        {
            get { return Context.Application; }
        }

        public HttpServerUtility Server
        {
            get { return Context.Server; }
        }

        public HttpSessionState Session
        {
            get { return Context.Session; }
        }

        public HttpRequest Request
        {
            get { return Context.Request; }
        }

        public HttpResponse Response
        {
            get { return Context.Response; }
        }
        
        public IPrincipal User
        {
            get { return Context.User; }
        }

        private static IDictionary DefaultFeatures
        {
            get 
            {
                string key = typeof(JsonRpcService).FullName;
                IDictionary config = (IDictionary) HttpRuntime.Cache.Get(key);
                
                if (config == null)
                {
                    config = new Hashtable(6);
                    
                    config.Add("rpc", typeof(JsonRpcExecutive).AssemblyQualifiedName); 
                    config.Add("proxy", typeof(JsonRpcProxyGenerator).AssemblyQualifiedName); 
                    config.Add("help", typeof(JsonRpcHelp).AssemblyQualifiedName); 
                    config.Add("test", typeof(JsonRpcTester).AssemblyQualifiedName); 
                    config.Add("getrpc", typeof(JsonRpcGetProtocol).AssemblyQualifiedName); 
                    config.Add("pyproxy", typeof(JsonRpcPythonProxyGenerator).AssemblyQualifiedName); 
                    
                    HttpRuntime.Cache.Add(key, config, null, Cache.NoAbsoluteExpiration, Cache.NoSlidingExpiration, CacheItemPriority.Default, null);
                }
                
                return config;
            }
        }
    }
}

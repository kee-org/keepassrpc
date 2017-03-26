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
    using System.ComponentModel;
    using System.Security.Principal;
    using System.Web;
    using System.Web.SessionState;
    using Jayrock.Services;

    #endregion

    public abstract class JsonRpcServiceFeature : IHttpHandler
    {
        private HttpContext _context;
        private readonly IService _service;

        protected JsonRpcServiceFeature(IService service)
        {
            if (service == null)
                throw new ArgumentNullException("service");
            
            _service = service;
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

        public virtual IService Service
        {
            get { return _service; }
        }

        public virtual void ProcessRequest(HttpContext context)
        {
            _context = context;
            ProcessRequest();
        }

        protected abstract void ProcessRequest();

        //
        // NOTE! IsReusable is discouraged from being overridden by a subclass
        // because this implementation assumes that the context will only be
        // established once per request.
        //

        public bool IsReusable
        {
            get { return false; }
        }
    }
}
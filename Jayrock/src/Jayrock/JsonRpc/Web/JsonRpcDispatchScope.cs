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
    using System.Diagnostics;
    using System.Web;
    using Jayrock.Json.Conversion;

    #endregion

    internal sealed class JsonRpcDispatchScope : IDisposable
    {
        private bool _disposed;
        private ExportContext _expctx;
        private ImportContext _impctx;

        public JsonRpcDispatchScope(JsonRpcDispatcher dispatcher, HttpContext context) 
        {
            Debug.Assert(dispatcher != null);
            Debug.Assert(context != null);

            //
            // Setup for local execution if client request is from the same
            // machine. The dispatcher uses this to determine whether to 
            // emit a detailed stack trace or not in the event of an error.
            //

            if (HttpRequestSecurity.IsLocal(context.Request))
                dispatcher.LocalExecution = true;

            //
            // Initialize the import and export contexts, which are pooled
            // per-application instance.
            //

            IDictionary appVars = AppVars.Get(context.ApplicationInstance);

            ExportContext expctx = (ExportContext) appVars[typeof(ExportContext)];

            if (expctx == null)
            {
                expctx = JsonConvert.CreateExportContext();
                appVars.Add(typeof(ExportContext), expctx);
            }
            
            dispatcher.JsonExporter = new JsonExportHandler(expctx.Export);

            ImportContext impctx = (ImportContext) appVars[typeof(ImportContext)];

            if (impctx == null)
            {
                impctx = JsonConvert.CreateImportContext();
                appVars.Add(typeof(ImportContext), impctx);
            }

            dispatcher.JsonImporter = new JsonImportHandler(impctx.Import);

            _impctx = impctx;
            _expctx = expctx;
        }

        public void Dispose()
        {
            if (_disposed)
                return;

            ExportContext expctx = _expctx;
            ImportContext impctx = _impctx;
            _disposed = true;

            //
            // Clear the import and export contexts so that they may be 
            // reused for another request.
            //

            expctx.Items.Clear();
            impctx.Items.Clear();
        }
    }
}
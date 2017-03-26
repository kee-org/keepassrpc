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
    using System.Collections.Specialized;
    using System.IO;
    using System.Text.RegularExpressions;
    using System.Web;
    using Jayrock.Json;
    using Jayrock.Json.Conversion;
    using Jayrock.Services;

    #endregion

    public sealed class JsonRpcGetProtocol : JsonRpcServiceFeature
        // TODO: Add IHttpAsyncHandler as soon as JsonRpcWorker supports 
        //       async processing.
    {
        private static readonly Regex _jsonpex;

        static JsonRpcGetProtocol()
        {
            _jsonpex = new Regex(@"^ 
                     [a-z_] [a-z0-9_]+ ( \[ [0-9]+ \] )?
                ( \. [a-z_] [a-z0-9_]+ ( \[ [0-9]+ \] )? )* $",
                RegexOptions.IgnoreCase
                | RegexOptions.Singleline
                | RegexOptions.ExplicitCapture
                | RegexOptions.IgnorePatternWhitespace
                | RegexOptions.CultureInvariant
                | RegexOptions.Compiled);
            
        }
        
        public JsonRpcGetProtocol(IService service) : 
            base(service) {}

        protected override void ProcessRequest()
        {
            string httpMethod = Request.RequestType;

            if (!CaselessString.Equals(httpMethod, "GET") &&
                !CaselessString.Equals(httpMethod, "HEAD"))
            {
                throw new JsonRpcException(string.Format("HTTP {0} is not supported for RPC execution. Use HTTP GET or HEAD only.", httpMethod));
            }

            string callback = Mask.NullString(Request.QueryString["jsonp"]);
            bool padded = callback.Length > 0;
            
            //
            // The response type depends on whether JSONP (JSON with 
            // Padding) is in effect or not.
            //

            Response.ContentType = padded ? "text/javascript" : "application/json";
            
            //
            // Validate that the JSONP callback method conforms to the 
            // allowed syntax. If not, issue a client-side exception
            // that will certainly help to bring problem to light, even if
            // a little too aggressively.
            //
            
            if (padded)
            {
                if (!_jsonpex.IsMatch(callback))
                {
                    Response.Write("throw new Error('Invalid JSONP callback parameter value.');");
                    Response.End();
                }
            }
            
            //
            // Convert the query string into a call object.
            //

            StringWriter sw = new StringWriter();
            JsonWriter writer = JsonText.CreateWriter(sw);
            
            writer.WriteStartObject();
            
            writer.WriteMember("id");
            writer.WriteNumber(-1);
            
            writer.WriteMember("method");
            
            string methodName = Mask.NullString(Request.PathInfo);
            
            if (methodName.Length == 0)
            {
                writer.WriteNull();
            }
            else
            {
                //
                // If the method name contains periods then we replace it
                // with dashes to mean the one and same thing. This is
                // done to provide dashes as an alternative to some periods
                // since some web servers may block requests (for security
                // reasons) if a path component of the URL contains more
                // than one period.
                //
                
                writer.WriteString(methodName.Substring(1).Replace('-', '.'));
            }
            
            writer.WriteMember("params");
            NameValueCollection query = new NameValueCollection(Request.QueryString);
            query.Remove(string.Empty);
            JsonConvert.Export(Request.QueryString, writer);
            
            writer.WriteEndObject();
            
            //
            // Delegate rest of the work to JsonRpcDispatcher.
            //

            JsonRpcDispatcher dispatcher = JsonRpcDispatcherFactory.CreateDispatcher(Service);
            using (new JsonRpcDispatchScope(dispatcher, Context))
            {
                dispatcher.RequireIdempotency = true;

                if (padded)
                {
                    //
                    // For JSONP, see details here:
                    // http://bob.pythonmac.org/archives/2005/12/05/remote-json-jsonp/
                    //

                    Response.Write(callback);
                    Response.Write('(');
                }

                dispatcher.Process(new StringReader(sw.ToString()), Response.Output);

                if (padded)
                    Response.Write(')');
            }
        }
    }
}
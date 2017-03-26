#region License, Terms and Conditions
//
// Jayrock - A JSON-RPC implementation for the Microsoft .NET Framework
// Written by Atif Aziz (www.raboof.com)
// Copyright (c) Atif Aziz. All rights reserved.
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

namespace Jayrock.JsonRpc
{
    #region Imports

    using System;
    using System.Collections;
    using System.Diagnostics;
    using System.IO;
    using System.Net;
    using System.Text;
    using System.Web.Services.Protocols;
    using Jayrock.Json;
    using Jayrock.Json.Conversion;

    #endregion

    public class JsonRpcClient : HttpWebClientProtocol
    {
        private int _id;
        private static readonly object[] _zeroArgs = new object[0];

        public object Invoke(string method)
        {
            return Invoke(AnyType.Value, method);
        }

        public object Invoke(Type returnType, string method)
        {
            return Invoke(returnType, method, (object) null);
        }

        public object Invoke(string method, object[] args)
        {
            return Invoke(AnyType.Value, method, args);
        }

        public object Invoke(Type returnType, string method, object[] args)
        {
            return Invoke(returnType, method, (object) args);
        }

        public object InvokeVargs(string method, params object[] args)
        {
            return Invoke(method, args);
        }

        public object InvokeVargs(Type returnType, string method, params object[] args)
        {
            return Invoke(returnType, method, args);
        }

        public object Invoke(string method, IDictionary args)
        {
            return Invoke(AnyType.Value, method, args);
        }

        public object Invoke(Type returnType, string method, IDictionary args)
        {
            return Invoke(returnType, method, (object) args);
        }

        public virtual object Invoke(Type returnType, string method, object args)
        {
            if (method == null) 
                throw new ArgumentNullException("method");
            if (method.Length == 0)
                throw new ArgumentException(null, "method");
            if (returnType == null) 
                throw new ArgumentNullException("returnType");

            WebRequest request = GetWebRequest(new Uri(Url));
            request.Method = "POST";
            
            using (Stream stream = request.GetRequestStream())
            using (StreamWriter writer = new StreamWriter(stream, Encoding.UTF8))
            {
                JsonObject call = new JsonObject();
                call["id"] = ++_id;
                call["method"] = method;
                call["params"] = args != null ? args : _zeroArgs;
                JsonConvert.Export(call, writer);
            }
            
            using (WebResponse response = GetWebResponse(request))
            using (Stream stream = response.GetResponseStream())
            using (StreamReader reader = new StreamReader(stream, Encoding.UTF8))
                return OnResponse(JsonText.CreateReader(reader), returnType);
        }

        public object OnResponse(JsonReader reader, Type returnType)
        {
            Debug.Assert(reader != null);
            Debug.Assert(returnType != null);

            bool resultSpecified = false;
            object result = null;

            //
            // JSON-RPC 2.0 specification/protocol, states that either error 
            // or result must be present but not both. JSON-RPC 1.0 is less 
            // strict and states that one or the other must be null. There 
            // is an ambiguity however with 1.0 when both result and error 
            // are null. Here, it is treated like a successful null result.
            //

            NamedJsonBuffer[] members = JsonBuffer.From(reader).GetMembersArray();
            foreach (NamedJsonBuffer member in members)
            {
                if (string.CompareOrdinal(member.Name, "error") == 0)
                {
                    object errorObject = JsonConvert.Import(member.Buffer.CreateReader());
                    if (errorObject != null)
                        OnError(errorObject);
                }
                else if (string.CompareOrdinal(member.Name, "result") == 0)
                {
                    resultSpecified = true;
                    result = returnType != typeof(JsonBuffer) 
                           ? JsonConvert.Import(returnType, member.Buffer.CreateReader()) 
                           : member.Buffer;
                }
            }

            if (!resultSpecified) // never gets here on error
                throw new Exception("Invalid JSON-RPC response. It contains neither a result nor an error.");
            
            return result;
        }

        protected virtual void OnError(object errorObject) 
        {
            IDictionary error = errorObject as IDictionary;
                        
            if (error != null)
                throw new Exception(error["message"] as string);
                        
            throw new Exception(errorObject as string);
        }
    }
}

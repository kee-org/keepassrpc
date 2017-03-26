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
    using System.Diagnostics;
    using System.Web;

    #endregion

    internal sealed class JsonRpcWebTraceAdapter : TraceListener
    {
        public override void Write(string message, string category)
        {
            if (IsJsonRpcCategory(category))
                WebTraceWrite(category, message);
        }

        public override void WriteLine(string message, string category)
        {
            if (IsJsonRpcCategory(category))
                WebTraceWrite(category, message + Environment.NewLine);
        }

        private static bool IsJsonRpcCategory(string category)
        {
            return category != null && 
                category.Length >= JsonRpcTrace.BaseCategory.Length &&
                category.Substring(0, JsonRpcTrace.BaseCategory.Length) == JsonRpcTrace.BaseCategory;
        }

        public void WebTraceWrite(string category, string message)
        {
            HttpContext current = HttpContext.Current;
            
            if (current == null)
                return;

            TraceContext trace = current.Trace;
               
            if (trace == null || !trace.IsEnabled)
                return;

            if (JsonRpcTrace.ErrorCategory.Equals(category) ||
                JsonRpcTrace.WarningCategory.Equals(category))
            {
                trace.Warn(category, message);
            }
            else
            {
                trace.Write(category, message);
            }
        }

        //
        // NOTE! Tracing messages without a category are dropped.
        //

        public override void Write(string message)
        {
        }

        public override void WriteLine(string message)
        {
        }
    }
}

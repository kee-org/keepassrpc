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

namespace Jayrock.JsonRpc
{
    #region Imports

    using System;
    using System.Diagnostics;

    #endregion

    public sealed class JsonRpcTrace
    {
        public static readonly string BaseCategory = "JSON-RPC";
        public static readonly string ErrorCategory = "JSON-RPC (Error)";
        public static readonly string WarningCategory = "JSON-RPC (Warning)";
        
        private static readonly TraceSwitch _switch = new TraceSwitch("JSON-RPC", "JSON-RPC tracing");

        private JsonRpcTrace()
        {
            throw new NotSupportedException();
        }

        public static TraceSwitch Switch
        {
            get { return _switch; }
        }

        //
        // The following boolean properties are just short-hand for getting the
        // same information over the Switch property. They are provided to make
        // the code at the call site short.
        //

        internal static bool TraceInfo { get { return Switch.TraceInfo; } }
        internal static bool TraceVerboseInfo { get { return TraceInfo && Switch.TraceVerbose; } }
        internal static bool TraceWarning { get { return Switch.TraceWarning; } }
        internal static bool TraceError { get { return Switch.TraceError; } }

        [ Conditional("TRACE") ]
        public static void Info(string message)
        {
            Trace.WriteLine(message, BaseCategory);
        }

        [ Conditional("TRACE") ]
        public static void Info(string message, params object[] args)
        {
            Info(string.Format(message, args));
        }

        [ Conditional("TRACE") ]
        public static void Warning(string message)
        {
            Trace.WriteLine(message, WarningCategory);
        }

        [ Conditional("TRACE") ]
        public static void Warning(string message, params object[] args)
        {
            Warning(string.Format(message, args));
        }

        [ Conditional("TRACE") ]
        public static void Error(Exception e)
        {
            Trace.WriteLine(e.ToString(), ErrorCategory);
        }
    }
}

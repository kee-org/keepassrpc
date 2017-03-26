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

namespace JsonRpcClientDemo
{
    #region Imports

    using System;
    using System.Collections;
    using System.Net;
    using Jayrock.Json;
    using Jayrock.JsonRpc;

    #endregion

    internal sealed class Program
    {
        [ STAThread ]
        static void Main()
        {
            //
            // Demonstration of client calls to a JSON-RPC service.
            //
            
            JsonRpcClient client = new JsonRpcClient();
            client.Url = "http://www.raboof.com/projects/jayrock/demo.ashx";
            Console.WriteLine(client.Invoke("system.about"));
            Console.WriteLine(client.Invoke("system.version"));
            Console.WriteLine(string.Join(Environment.NewLine, (string[]) (new ArrayList((ICollection) client.Invoke("system.listMethods"))).ToArray(typeof(string))));
            Console.WriteLine(client.Invoke("now"));
            Console.WriteLine(((DateTime)client.Invoke(typeof(DateTime), "now")).ToString("r"));
            Console.WriteLine(client.InvokeVargs("sum", 123, 456));
            Console.WriteLine(client.Invoke("sum", new JsonObject { { "a", 123 }, { "b", 456 } }));
            Console.WriteLine(client.InvokeVargs("total", new[] { 1, 2, 3, 4, 5, 6, 7, 8, 9 }));
            client.CookieContainer = new CookieContainer();
            Console.WriteLine(client.Invoke("counter"));
            Console.WriteLine(client.Invoke("counter"));
        }
    }
}
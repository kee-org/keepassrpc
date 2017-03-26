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
    using Jayrock.Json;
    using NUnit.Framework;

    #endregion

    [ TestFixture ]
    public class TestJsonRpcError
    {
        [ Test ]
        public void InitializationFromException()
        {
            ApplicationException exception = new ApplicationException();
            JsonObject error = JsonRpcError.FromException(ThrowAndCatch(exception));
            Assert.IsNotNull(error);
            Assert.AreEqual(3, error.Count);
            Assert.AreEqual(exception.Message, error["message"]);
            Assert.AreEqual("JSONRPCError", error["name"]);
            JsonArray errors = (JsonArray) error["errors"];
            Assert.AreEqual(1, errors.Count);
            error = (JsonObject) errors.Shift();
            Assert.AreEqual(exception.Message, error["message"]);
            Assert.AreEqual("ApplicationException", error["name"]);
        }

        [ Test ]
        public void StackTraceInclusion()
        {
            ApplicationException exception = new ApplicationException();
            JsonObject error = JsonRpcError.FromException(ThrowAndCatch(exception), true);
            string trace = (string) error["stackTrace"];
            Assert.IsNotNull(trace);
            Assert.IsNotEmpty(trace);
        }

        [ Test ]
        public void InnerExceptions()
        {
            Exception inner = new FormatException();
            ApplicationException outer = new ApplicationException(null, inner);
            JsonObject error = JsonRpcError.FromException(ThrowAndCatch(outer));
            JsonArray errors = (JsonArray) error["errors"];
            Assert.AreEqual(2, errors.Count);
            error = (JsonObject) errors.Shift();
            Assert.AreEqual(outer.Message, error["message"]);
            Assert.AreEqual("ApplicationException", error["name"]);
            error = (JsonObject) errors.Shift();
            Assert.AreEqual(inner.Message, error["message"]);
            Assert.AreEqual("FormatException", error["name"]);
        }

        private static Exception ThrowAndCatch(Exception exceptionToThrow)
        {
            try
            {
                throw exceptionToThrow;
            }
            catch (Exception exception)
            {
                return exception;
            }
        }
    }
}
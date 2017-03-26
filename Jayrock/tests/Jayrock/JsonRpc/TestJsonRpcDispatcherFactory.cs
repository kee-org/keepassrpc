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
    using Jayrock.Services;
    using NUnit.Framework;

    #endregion

    [ TestFixture ]
    public class TestJsonRpcDispatcherFactory
    {
        [ Test ]
        public void DefaultImplementationCreatesJsonRpcDispatcher()
        {
            Assert.IsInstanceOfType(typeof(JsonRpcDispatcher), JsonRpcDispatcherFactory.Default(new TestService()));
        }

        [ Test ]
        [ ExpectedException(typeof(ArgumentNullException)) ]
        public void CannotSetCurrentToNull()
        {
            JsonRpcDispatcherFactory.Current = null;
        }

        [ Test ]
        public void SetCustomHandler()
        {
            JsonRpcDispatcherFactory.Handler handler = new JsonRpcDispatcherFactory.Handler(CreateJsonRpcDispatcher);
            JsonRpcDispatcherFactory.Current = handler;
            Assert.AreSame(handler, JsonRpcDispatcherFactory.Current);
        }

        private JsonRpcDispatcher CreateJsonRpcDispatcher(IService service)
        {
            throw new NotImplementedException();
        }

        private sealed class TestService : IService
        {
            public ServiceClass GetClass()
            {
                throw new NotImplementedException();
            }
        }
    }
}
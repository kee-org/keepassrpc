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

    using System.Reflection;
    using Jayrock.Services;
    using NUnit.Framework;

    #endregion

    [ TestFixture ]
    public class TestJsonRpcMethodAttribute
    {
        [Test]
        public void FirstParameterOfStaticMethodDoesNotAppearInTheReflectedModel()
        {
            ServiceClassBuilder scb = new ServiceClassBuilder();
            MethodInfo method = typeof(ServiceWithStaticMethod).GetMethod("StaticMethod");
            JsonRpcMethodAttribute attribute = new JsonRpcMethodAttribute();
            IAttributeAttachment attachment = attribute;
            attachment.SetAttachment(method);
            IMethodReflector reflector = attribute;
            MethodBuilder mb = scb.DefineMethod();
            reflector.Build(mb);
            Assert.AreEqual(0, mb.Parameters.Count);
        }

        private sealed class ServiceWithStaticMethod
        {
            public static void StaticMethod(IService service) {}
        }
    }
}
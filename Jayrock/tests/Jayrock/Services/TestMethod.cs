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

namespace Jayrock.Services
{
    #region Imports

    using System;
    using System.Security;
    using System.Security.Permissions;
    using Jayrock.Json;
    using Jayrock.JsonRpc;
    using NUnit.Framework;

    #endregion

    [ TestFixture ]
    public class TestMethod
    {
        [ Test ]
        public void FailedMethodYieldsInvocationException()
        {
            try
            {
                TestService service = new TestService();
                service.GetClass().GetMethodByName("BadMethod").Invoke(service, null, null);
                Assert.Fail("Expecting an exception.");
            }
            catch (TargetMethodException e)
            {
                Assert.IsTrue(e.InnerException.GetType() == typeof(ApplicationException), "Unexpected inner exception ({0}).", e.InnerException.GetType().FullName);
            }
        }

        [ Test ]
        public void VariableArguments()
        {
            TestService service = new TestService();
            Method method = service.GetClass().FindMethodByName("VarMethod");
            object[] args = new object[] { 1, 2, 3, 4 };
            object[] invokeArgs = method.TransposeVariableArguments(args);
            Assert.AreEqual(1, invokeArgs.Length);
            Assert.AreEqual(args, invokeArgs[0]);
        }

        [ Test ]
        public void FixedAndVariableArguments()
        {
            TestService service = new TestService();
            Method method = service.GetClass().FindMethodByName("FixedVarMethod");
            object[] args = new object[] { 1, 2, 3, 4 };
            args = method.TransposeVariableArguments(args);
            Assert.AreEqual(3, args.Length);
            Assert.AreEqual(1, args[0]);
            Assert.AreEqual(2, args[1]);
            Assert.AreEqual(new object[] { 3, 4 }, args[2]);
        }

        [ Test ]
        public void RetransposingYieldsTheSame()
        {
            TestService service = new TestService();
            Method method = service.GetClass().FindMethodByName("FixedVarMethod");
            object[] args = new object[] { 1, 2, 3, 4 };
            args = method.TransposeVariableArguments(args);
            args = method.TransposeVariableArguments(args);
            Assert.AreEqual(3, args.Length);
            Assert.AreEqual(1, args[0]);
            Assert.AreEqual(2, args[1]);
            Assert.AreEqual(new object[] { 3, 4 }, args[2]);
        }

        [ Test ]
        public void Bug8131()
        {
            //
            // Bug #8131: Varargs transposition when last arg is collection
            // http://developer.berlios.de/bugs/?func=detailbug&bug_id=8131&group_id=4638
            //
            
            TestService service = new TestService();
            Method method = service.GetClass().FindMethodByName("VarMethod");
            object[] args = new object[] { 1, 2, new int[] { 3, 4 } };
            args = method.TransposeVariableArguments(args);
            Assert.AreEqual(1, args.Length);
            object[] varargs = (object[]) args[0];
            Assert.AreEqual(3, varargs.Length);
            Assert.AreEqual(1, varargs[0]);
            Assert.AreEqual(2, varargs[1]);
            Assert.AreEqual(new int[] { 3, 4 }, varargs[2]);
        }
        
        [ Test ]
        public void Bug8148()
        {
            //
            // Bug #8148: Varargs transposition drops args
            // http://developer.berlios.de/bugs/?func=detailbug&bug_id=8148&group_id=4638
            //
            
            TestService service = new TestService();
            Method method = service.GetClass().FindMethodByName("FixedVarMethod");
            object[] args = new object[] { 1, 2, 
                new int[] { 3, 4 }, 
                new JsonObject(new string[] { "five", "six" }, new object[] { 5, 6 }) };
            args = method.TransposeVariableArguments(args);
            Assert.AreEqual(3, args.Length);
            Assert.AreEqual(1, args[0]);
            Assert.AreEqual(2, args[1]);
            object[] varargs = (object[]) args[2];
            Assert.AreEqual(2, varargs.Length);
            Assert.AreEqual(new int[] { 3, 4 }, varargs[0]);
            JsonObject o = (JsonObject) varargs[1];
            Assert.AreEqual(5, o["five"]);
            Assert.AreEqual(6, o["six"]);
        }

        [Test, ExpectedException(typeof(SecurityException))]
        #if MONO
        [Ignore("See http://code.google.com/p/jayrock/issues/detail?id=23")]
        #endif
        public void Issue12()
        {
            // Issue #12: PrincipalPermissionAttribute causes InvalidCastException when applied to service methods 
            // http://code.google.com/p/jayrock/issues/detail?id=12

            SecuredService service = new SecuredService();
            Method method = service.GetClass().GetMethodByName("SecuredMethod");
            try
            {
                method.Invoke(service, null, null);
            }
            catch (TargetMethodException e)
            {
                Assert.IsNotNull(e.InnerException);
                throw e.InnerException;
            }
        }

        private sealed class TestService : JsonRpcService
        {
            [ JsonRpcMethod ]
            public void BadMethod()
            {
                throw new ApplicationException();
            }

            [ JsonRpcMethod ]
            public void VarMethod(params object[] args)
            {
            }

            [ JsonRpcMethod ]
            public void FixedVarMethod(int a, int b, params object[] args)
            {
            }
        }

        private sealed class SecuredService : JsonRpcService
        {
            [JsonRpcMethod, PrincipalPermission(SecurityAction.Demand)]
            public void SecuredMethod() { }
        }
    }
}


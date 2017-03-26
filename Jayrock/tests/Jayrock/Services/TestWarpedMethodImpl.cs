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
    using System.ComponentModel;
    using System.Threading;
    using NUnit.Framework;

    #endregion

    [ TestFixture ]
    public class TestWarpedMethodImpl
    {
        [ Test ]
        public void Invocation()
        {
            TestMethodImpl baseMethod = new TestMethodImpl();
            
            WarpedMethodImpl warpedMethod = new WarpedMethodImpl(baseMethod, typeof(Thing),
                TypeDescriptor.GetProperties(typeof(Thing)), 
                TypeDescriptor.GetProperties(typeof(ResultThing))[0]);
            
            DummyService service = new DummyService();
            object[] args = new object[] { 42, "foobar" };
            baseMethod.InvokeResult = new ResultThing(123);
            object result = warpedMethod.Invoke(service, args);

            Assert.AreSame(service, baseMethod.InvokeService);
            Assert.IsNotNull(baseMethod.InvokeArgs);
            Assert.AreEqual(1, baseMethod.InvokeArgs.Length);
            Assert.IsNotNull(baseMethod.InvokeArgs[0]);
            Thing thing = baseMethod.InvokeArgs[0] as Thing;
            Assert.IsInstanceOfType(typeof(Thing), thing);
            Assert.AreEqual(args[0], thing.Int);
            Assert.AreEqual(args[1], thing.Str);
            Assert.AreEqual(baseMethod.InvokeResult.Result, result);
        }

        [ Test ]
        public void InvocationWithNoResult()
        {
            TestMethodImpl baseMethod = new TestMethodImpl();
            WarpedMethodImpl warpedMethod = new WarpedMethodImpl(baseMethod, typeof(Thing),
                TypeDescriptor.GetProperties(typeof(Thing)), null);
            Assert.IsNull(warpedMethod.Invoke(new DummyService(), new object[0]));
        }

        [ Test ]
        public void AsyncInvocation()
        {
            TestMethodImpl baseMethod = new TestMethodImpl();

            WarpedMethodImpl warpedMethod = new WarpedMethodImpl(baseMethod, typeof(Thing),
                TypeDescriptor.GetProperties(typeof(Thing)),
                TypeDescriptor.GetProperties(typeof(ResultThing))[0]);

            DummyService service = new DummyService();
            object[] args = new object[] { 42, "foobar" };
            baseMethod.InvokeResult = new ResultThing(123);
            IAsyncResult ar = warpedMethod.BeginInvoke(service, args, null, null);
            object result = warpedMethod.EndInvoke(service, ar);

            Assert.AreSame(ar, baseMethod.EndInvokeAsyncResult);
            Assert.AreSame(service, baseMethod.InvokeService);
            Assert.IsNotNull(baseMethod.InvokeArgs);
            Assert.AreEqual(1, baseMethod.InvokeArgs.Length);
            Assert.IsNotNull(baseMethod.InvokeArgs[0]);
            Thing thing = baseMethod.InvokeArgs[0] as Thing;
            Assert.IsInstanceOfType(typeof(Thing), thing);
            Assert.AreEqual(args[0], thing.Int);
            Assert.AreEqual(args[1], thing.Str);
            Assert.AreEqual(baseMethod.InvokeResult.Result, result);
        }

        [ Test, ExpectedException(typeof(ArgumentNullException)) ]
        public void CannotInitializeWithNullBaseImpl()
        {
            new WarpedMethodImpl(null, typeof(Thing),
                TypeDescriptor.GetProperties(typeof(Thing)), null);
        }

        [ Test, ExpectedException(typeof(ArgumentNullException)) ]
        public void CannotInitializeWithNullArgsType()
        {
            new WarpedMethodImpl(new TestMethodImpl(), null,
                TypeDescriptor.GetProperties(typeof(Thing)), null);
        }

        [ Test, ExpectedException(typeof(ArgumentNullException)) ]
        public void CannotInitializeWithNullArgs()
        {
            new WarpedMethodImpl(new TestMethodImpl(), typeof(Thing), null, null);
        }

        [ Test ]
        public void AsynchronousQueryIsDelegated()
        {
            TestMethodImpl impl = new TestMethodImpl();
            Assert.IsFalse(impl.IsAsynchronousCalled);
            Assert.IsFalse(new WarpedMethodImpl(impl, typeof(Thing), new PropertyDescriptorCollection(null), null).IsAsynchronous);
            Assert.IsTrue(impl.IsAsynchronousCalled);
        }

        private sealed class Thing
        {
            private int _int;
            private string _str;
            
            public int Int
            {
                get { return _int; }
                set { _int = value; }
            }

            public string Str
            {
                get { return _str; }
                set { _str = value; }
            }
        }

        private sealed class ResultThing
        {
            private readonly int _result;
            
            public ResultThing(int result)
            {
                _result = result;
            }

            public int Result
            {
                get { return _result; }
            }
        }

        public class DummyService : IService
        {
            public ServiceClass GetClass()
            {
                throw new NotImplementedException();
            }
        }

        private sealed class TestMethodImpl : IMethodImpl
        {
            public IService InvokeService;
            public object[] InvokeArgs;
            public ResultThing InvokeResult;
            public IAsyncResult EndInvokeAsyncResult;
            public bool IsAsynchronousCalled;

            public object Invoke(IService service, object[] args)
            {
                InvokeService = service;
                InvokeArgs = args;
                return InvokeResult;
            }

            public bool IsAsynchronous
            {
                get
                {
                    IsAsynchronousCalled = true;
                    return false;
                }
            }

            public IAsyncResult BeginInvoke(IService service, object[] args, AsyncCallback callback, object asyncState)
            {
                Invoke(service, args);
                return new DummyAsyncResult();
            }

            public object EndInvoke(IService service, IAsyncResult asyncResult)
            {
                EndInvokeAsyncResult = asyncResult;
                return InvokeResult;
            }

            private sealed class DummyAsyncResult : IAsyncResult
            {
                public bool IsCompleted { get { throw new NotImplementedException(); } }
                public WaitHandle AsyncWaitHandle { get { throw new NotImplementedException(); } }
                public object AsyncState { get { throw new NotImplementedException(); } }
                public bool CompletedSynchronously { get { throw new NotImplementedException(); } }
            }
        }
    }
}
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
    using System.Reflection;
    using System.Threading;
    using NUnit.Framework;

    #endregion

    [ TestFixture ]
    public class TestTypeMethodImpl
    {
        [ Test, ExpectedException(typeof(ArgumentNullException)) ]
        public void CannotInitializeWithNullMethod()
        {
            new TypeMethodImpl(null);
        }

        [ Test ]
        public void MethodPropertyReflectsInitializedMethod()
        {
            MethodInfo method = GetMethod("Sub");
            TypeMethodImpl impl = new TypeMethodImpl(method);
            Assert.IsNotNull(impl.Method);
            Assert.AreSame(method, impl.Method);
        }

        [ Test ]
        public void InvokeCallsMethod()
        {
            MockService service = new MockService();
            TypeMethodImpl impl = GetImpl("Sub");
            impl.Invoke(service, null);
            Assert.AreSame(impl.Method, service.LastCalledMethod);
        }

        [ Test ]
        public void InvokeReturnsResult()
        {
            MockService service = new MockService();
            service.NextReturn = new object();
            Assert.AreSame(service.NextReturn, GetImpl("Function").Invoke(service, null));
        }
        
        [ Test ]
        public void BeginInvokeWithoutCallback()
        {
            MockService service = new MockService();
            TypeMethodImpl impl = GetImpl("Sub");
            object state = new object();
            IAsyncResult ar = impl.BeginInvoke(service, null, null, state);
            Assert.IsNotNull(ar);
            Assert.IsTrue(ar.CompletedSynchronously);
            Assert.IsTrue(ar.IsCompleted);
            Assert.AreSame(state, ar.AsyncState);
            Assert.IsNotNull(ar.AsyncWaitHandle);
            Assert.AreSame(impl.Method, service.LastCalledMethod);
        }

        [ Test ]
        public void BeginInvokeWithCallback()
        {
            MockService service = new MockService();
            TypeMethodImpl impl = GetImpl("Sub");
            bool[] called = new bool[1];
            impl.BeginInvoke(service, null, new AsyncCallback(OnInvoked), called);
            Assert.IsTrue(called[0]);
        }

        [ Test, ExpectedException(typeof(ArgumentNullException)) ]
        public void CannotInvokeWithNullService()
        {
            GetImpl("Sub").Invoke(null, null);
        }

        [ Test, ExpectedException(typeof(ArgumentNullException)) ]
        public void CannotBeginInvokeWithNullService()
        {
            GetImpl("Sub").BeginInvoke(null, null, null, null);
        }

        [ Test, ExpectedException(typeof(InvocationException)) ]
        public void InvokeThrowsInvocationExceptionWhenBadParamCount()
        {
            GetImpl("Sub").Invoke(new MockService(), new object[] { 1, 2, 3 });
        }

        [ Test, ExpectedException(typeof(InvocationException)) ]
        public void InvokeThrowsInvocationExceptionWhenParamTypeMismatch()
        {
            GetImpl("TwoArgSub").Invoke(new MockService(), new object[] { "1", "2" });
        }

        [ Test ]
        public void ExceptionInMethodPropagatedAsTargetMethodException()
        {
            MockService service = new MockService();
            service.NextException = new ApplicationException();
            TypeMethodImpl impl = GetImpl("Erroneous");
            try
            {
                impl.Invoke(service, null);
                Assert.Fail("Expected " + typeof(TargetMethodException));
            }
            catch (TargetMethodException e)
            {
                Assert.IsNotNull(e.InnerException);
                Assert.AreSame(service.NextException, e.InnerException);
            }
        }

        [ Test ]
        public void EndInvokePropagatesThrownException()
        {
            MockService service = new MockService();
            service.NextException = new ApplicationException();
            TypeMethodImpl impl = GetImpl("Erroneous");
            IAsyncResult ar = impl.BeginInvoke(service, null, null, null);
            try
            {
                impl.EndInvoke(service, ar);
                Assert.Fail("Expected " + typeof(TargetMethodException));
            }
            catch (TargetMethodException e)
            {
                Assert.IsNotNull(e.InnerException);
                Assert.AreSame(service.NextException, e.InnerException);
            }
        }

        [ Test, ExpectedException(typeof(ArgumentNullException)) ]
        public void CannotCallEndInvokeWithNullService()
        {
            TypeMethodImpl impl = GetImpl("Sub");
            impl.EndInvoke(null, impl.BeginInvoke(new MockService(), null, null, null));
        }

        [ Test, ExpectedException(typeof(ArgumentNullException)) ]
        public void CannotCallEndInvokeWithNullAsyncResult()
        {
            GetImpl("Sub").EndInvoke(new MockService(), null);
        }

        [ Test ]
        public void ImplementationIsNotAsynchronous()
        {
            Assert.IsFalse(GetImpl("Sub").IsAsynchronous);
        }
        
        [ Test, ExpectedException(typeof(ArgumentException)) ]
        public void CannotCallEndInvokeWithBadAsyncResultType()
        {
            GetImpl("Sub").EndInvoke(new MockService(), new DummyAsyncResult());
        }

        [ Test ]
        public void InvokingStaticMethodPrependsServiceObjectToArgs()
        {
            MockService service = new MockService();
            const int num = 42;
            const string str = "hello";
            object result = GetImpl("Static").Invoke(service, new object[] { num, str });
            Assert.AreEqual(new object[] { service, num, str }, result);
        }

        private sealed class DummyAsyncResult : IAsyncResult
        {
            public bool IsCompleted { get { throw new NotImplementedException(); } }
            public WaitHandle AsyncWaitHandle { get { throw new NotImplementedException(); } }
            public object AsyncState { get { throw new NotImplementedException(); } }
            public bool CompletedSynchronously { get { throw new NotImplementedException(); } }
        }

        private static void OnInvoked(IAsyncResult ar)
        {
            Assert.IsNotNull(ar);
            ((bool[]) ar.AsyncState)[0] = true;
        }

        private static TypeMethodImpl GetImpl(string name)
        {
            return new TypeMethodImpl(typeof(MockService).GetMethod(name));
        }

        private static MethodInfo GetMethod(string name)
        {
            return typeof(MockService).GetMethod(name);
        }

        private sealed class MockService : IService
        {
            public MethodBase LastCalledMethod;
            public object NextReturn;
            public Exception NextException;
            
            public void Sub()
            {
                LastCalledMethod = MethodBase.GetCurrentMethod();
            }
            
            public void TwoArgSub(int a, int b)
            {
                LastCalledMethod = MethodBase.GetCurrentMethod();
            }

            public object Function()
            {
                LastCalledMethod = MethodBase.GetCurrentMethod();
                return NextReturn;
            }
            
            public void Erroneous()
            {
                throw NextException;
            }

            public static object[] Static(IService service, int num, string str)
            {
                return new object[] { service, num, str };
            }
            
            public ServiceClass GetClass()
            {
                throw new NotImplementedException();
            }
        }
    }
}

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
    using Jayrock.Services;
    using NUnit.Framework;

    #endregion

    [ TestFixture ]
    public class TestJsonRpcServiceReflector
    {
        [ Test, ExpectedException(typeof(ArgumentNullException)) ]
        public void NullType()
        {
            JsonRpcServices.GetClassFromType(null);
        }

        [ Test ]
        public void ServiceNameIsTypeName()
        {
            ServiceClass clazz = JsonRpcServices.GetClassFromType(typeof(EmptyService));
            Assert.AreEqual("EmptyService", clazz.Name);
        }

        [ Test ]
        public void UntaggedMethodsNotExported()
        {
            ServiceClass clazz = JsonRpcServices.GetClassFromType(typeof(EmptyService));
            Assert.AreEqual(0, clazz.GetMethods().Length);
        }

        [ Test ]
        public void TaggedMethodsExported()
        {
            ServiceClass clazz = JsonRpcServices.GetClassFromType(typeof(TestService));
            Assert.AreEqual(4, clazz.GetMethods().Length);
        }

        [ Test ]
        public void CustomServiceName()
        {
            ServiceClass clazz = JsonRpcServices.GetClassFromType(typeof(TestService));
            Assert.AreEqual("MyService", clazz.Name);
        }
        
        [ Test, ExpectedException(typeof(ArgumentNullException)) ]
        public void NullService()
        {
            JsonRpcServices.GetClassFromType(null);
        }

        [ Test ]
        public void DefaultNameIsMethodName()
        {
            ServiceClass clazz = JsonRpcServices.GetClassFromType(typeof(TestService));
            Assert.AreEqual("Foo", clazz.FindMethodByName("Foo").Name);
        }

        [ Test ]
        public void AffliatedWithService()
        {
            ServiceClass clazz = JsonRpcServices.GetClassFromType(typeof(TestService));
            foreach (Method method in clazz.GetMethods())
                Assert.AreSame(clazz, method.ServiceClass);
        }

        [ Test ]
        public void CustomNameViaAttribute()
        {
            ServiceClass clazz = JsonRpcServices.GetClassFromType(typeof(TestService));
            Method method = clazz.FindMethodByName("Foo");
            Assert.AreEqual("Foo", method.Name);
            Assert.AreEqual("Foo", method.InternalName);
        }

        [ Test ]
        public void AttributeFromMethod()
        {
            ServiceClass clazz = JsonRpcServices.GetClassFromType(typeof(TestService));
            Method method = clazz.FindMethodByName("Baz");
            Assert.AreEqual("Baz", method.Name);
            Assert.AreEqual("Bar", method.InternalName);
        }

        [ Test ]
        public void ResultTypeIsMethodReturnType()
        {
            ServiceClass clazz = JsonRpcServices.GetClassFromType(typeof(TestService));
            Assert.AreEqual(typeof(void), clazz.GetMethodByName("Foo").ResultType);
            Assert.AreEqual(typeof(void), clazz.GetMethodByName("Baz").ResultType);
            Assert.AreEqual(typeof(int), clazz.GetMethodByName("Sum").ResultType);
            Assert.AreEqual(typeof(string), clazz.GetMethodByName("Format").ResultType);
        }

        [ Test ]
        public void Parameters()
        {
            ServiceClass clazz = JsonRpcServices.GetClassFromType(typeof(TestService));
            Parameter[] parameters = clazz.GetMethodByName("Sum").GetParameters();
            Assert.AreEqual(2, parameters.Length);

            Parameter parameter; 
            
            parameter = parameters[0];
            Assert.AreEqual("a", parameter.Name);
            Assert.AreEqual(typeof(int), parameter.ParameterType);
            Assert.AreEqual(0, parameter.Position);
            
            parameter = parameters[1];
            Assert.AreEqual("b", parameters[1].Name);
            Assert.AreEqual(typeof(int), parameter.ParameterType);
            Assert.AreEqual(1, parameter.Position);
        }

        [ Test ]
        public void ParamArray()
        {
            ServiceClass clazz = JsonRpcServices.GetClassFromType(typeof(TestService));

            Assert.IsFalse(clazz.GetMethodByName("Foo").HasParamArray);
            Assert.IsFalse(clazz.GetMethodByName("Baz").HasParamArray);

            Method method;
            Parameter[] parameters;

            method = clazz.GetMethodByName("Sum");
            parameters = method.GetParameters();
            Assert.IsFalse(method.HasParamArray);
            Assert.IsFalse(parameters[0].IsParamArray);
            Assert.IsFalse(parameters[1].IsParamArray);

            method = clazz.GetMethodByName("Format");
            parameters = method.GetParameters();
            Assert.IsTrue(method.HasParamArray);
            Assert.IsFalse(parameters[0].IsParamArray);
            Assert.IsTrue(parameters[1].IsParamArray);
        }
        
        [ Test ]
        public void Invocation()
        {
            ServiceClass clazz = JsonRpcServices.GetClassFromType(typeof(TestService));
            TestService service = new TestService();
            object result = clazz.GetMethodByName("Sum").Invoke(service, null, new object[] { 2, 3 });
            Assert.AreEqual(5, result);
        }
    
        [ Test ]
        public void MethodDescriptions()
        {
            ServiceClass clazz = JsonRpcServices.GetClassFromType(typeof(TestService));
            Assert.AreEqual(0, clazz.GetMethodByName("Foo").Description.Length);
            Assert.AreEqual(0, clazz.GetMethodByName("Baz").Description.Length);
            Assert.AreEqual(0, clazz.GetMethodByName("Sum").Description.Length);
            Assert.AreEqual("Formats a string.", clazz.GetMethodByName("Format").Description);
        }
        
        [ Test ]
        public void ServiceDescription()
        {
            Assert.AreEqual("A test service.", JsonRpcServices.GetClassFromType(typeof(TestService)).Description);
        }
        
        [ Test ]
        public void CustomAttributes()
        {
            ArrayList expectedValues = new ArrayList(new int[] { 12, 34, 56 });
            MyAttribute[] attributes = (MyAttribute[]) JsonRpcServices.GetClassFromType(typeof(TestService)).GetMethodByName("Foo").GetCustomAttributes(typeof(MyAttribute));
            Assert.AreEqual(3, attributes.Length);
            foreach (MyAttribute attribute in attributes)
                expectedValues.Remove(attribute.TestValue);
            Assert.AreEqual(0, expectedValues.Count);
        }

        [ Test ]
        public void CustomAttributesAreCopied()
        {
            Method method = JsonRpcServices.GetClassFromType(typeof(TestService)).GetMethodByName("Foo");
            Assert.AreNotSame(method.GetCustomAttributes()[0], method.GetCustomAttributes()[0]);
        }

        [ Test ]
        public void FindFirstCustomAttribute()
        {
            ArrayList expectedValues = new ArrayList(new int[] { 12, 34, 56 });
            Method method = JsonRpcServices.GetClassFromType(typeof(TestService)).GetMethodByName("Foo");
            MyAttribute attribute = (MyAttribute) method.FindFirstCustomAttribute(typeof(MyAttribute));
            expectedValues.Remove(attribute.TestValue);
            Assert.AreEqual(2, expectedValues.Count);
        }

        [ Test ]
        public void FindFirstCustomAttributeYieldsCopy()
        {
            Method method = JsonRpcServices.GetClassFromType(typeof(TestService)).GetMethodByName("Foo");
            Assert.AreNotSame(method.FindFirstCustomAttribute(typeof(MyAttribute)), method.FindFirstCustomAttribute(typeof(MyAttribute)));
        }

        [ Test ]
        public void MethodIdempotency()
        {
            ServiceClass clazz = JsonRpcServices.GetClassFromType(typeof(IdempotencyTestService));
            Assert.IsFalse(clazz.GetMethodByName("NonIdempotentMethod").Idempotent);
            Assert.IsTrue(clazz.GetMethodByName("IdempotentMethod").Idempotent);
        }
        
        [ Test ]
        public void MethodLookupIsCaseFree()
        {
            ServiceClass clazz = JsonRpcServices.GetClassFromType(typeof(TestService));
            Method foo = clazz.FindMethodByName("Foo");
            Assert.AreEqual("Foo", foo.Name);
            Assert.AreSame(foo, clazz.FindMethodByName("foo"));
            Assert.AreSame(foo, clazz.FindMethodByName("FOO"));
            Assert.AreSame(foo, clazz.FindMethodByName("Foo"));
        }

        [ Test ]
        public void MethodWithWarpedParametersAndResult()
        {
            ServiceClass clazz = JsonRpcServices.GetClassFromType(typeof(ServiceWithMethodsUsingWarpedParameters));
            Method foo = clazz.FindMethodByName("Foo");

            Assert.AreEqual(typeof(DateTime), foo.ResultType);

            Parameter[] parameters = foo.GetParameters();
            Assert.AreEqual(2, parameters.Length);

            Parameter parameter;

            parameter = parameters[0];
            Assert.AreEqual("stringArg", parameter.Name);
            Assert.AreEqual(0, parameter.Position);
            Assert.AreEqual(typeof(string), parameter.ParameterType);

            parameter = parameters[1];
            Assert.AreEqual("intArg", parameter.Name);
            Assert.AreEqual(1, parameter.Position);
            Assert.AreEqual(typeof(int), parameter.ParameterType);
        }

        [ Test ]
        public void MethodWithWarpedParametersButVoidResult()
        {
            ServiceClass clazz = JsonRpcServices.GetClassFromType(typeof(ServiceWithMethodsUsingWarpedParameters));
            Method foo = clazz.FindMethodByName("FooNoResult");
            
            Assert.AreEqual(typeof(void), foo.ResultType);
        }

        private sealed class EmptyService
        {
        }

        [ JsonRpcService("MyService") ]
        [ JsonRpcHelp("A test service.") ]
        private sealed class TestService : IService
        {
            [ JsonRpcMethod ]
            [ MyAttribute(12), MyAttribute(56), MyAttribute(34) ]
            public void Foo() { throw new NotImplementedException(); }

            [ JsonRpcMethod("Baz") ]
            public void Bar() { throw new NotImplementedException(); }

            [ JsonRpcMethod ]
            public int Sum(int a, int b)
            {
                return a + b;
            }

            [ JsonRpcMethod, JsonRpcHelp("Formats a string.") ]
            public string Format(string format, params object[] args)
            {
                throw new NotImplementedException();
            }

            public ServiceClass GetClass()
            {
                throw new NotImplementedException();
            }
        }

        private sealed class IdempotencyTestService
        {
            [ JsonRpcMethod ]
            public void NonIdempotentMethod() {}

            [ JsonRpcMethod(Idempotent = true) ]
            public void IdempotentMethod() {}
        }

        [ AttributeUsage(AttributeTargets.All, AllowMultiple = true) ]
        private class MyAttribute : Attribute
        {
            private readonly int _testValue;

            public MyAttribute(int testValue)
            {
                _testValue = testValue;
            }

            public int TestValue
            {
                get { return _testValue; }
            }
        }

        private sealed class ServiceWithMethodsUsingWarpedParameters : IService
        {
            public FooArgs Args;
            public FooResult Result = null;

            [ JsonRpcMethod(WarpedParameters = true) ]
            public FooResult Foo(FooArgs args) 
            {
                this.Args = args;
                return Result;
            }

            [ JsonRpcMethod(WarpedParameters = true) ]
            public void FooNoResult(FooArgs args)
            {
                this.Args = args;
            }

            public sealed class FooArgs
            {
                //
                // NOTE: The default assignments on the following fields is
                // to maily shut off the following warning from the compiler:
                //
                // warning CS0649: Field '...' is never assigned to, and will always have its default value
                //
                // This warning is harmless. Since the C# 1.x compiler does
                // not support #pragma warning disable, we have to resort
                // to a more brute force method.
                //

                public string stringArg = null;
                public int intArg = 0;
            }

            public sealed class FooResult
            {
                public DateTime Result;

                public FooResult(DateTime result)
                {
                    this.Result = result;
                }
            }

            public ServiceClass GetClass()
            {
                throw new NotImplementedException();
            }
        }
    }
}

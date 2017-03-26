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
    using NUnit.Framework;

    #endregion

    [ TestFixture ]
    public class TestMethodBuilder
    {
        private MethodBuilder _builder;
        private ServiceClassBuilder _classBuilder;

        [ SetUp ]
        public void Init()
        {
            _classBuilder = new ServiceClassBuilder();
            _builder = _classBuilder.DefineMethod();
        }

        [ Test ] 
        public void Defaults()
        {
            Assert.IsNotNull(_builder.Name);
            Assert.IsNotNull(_builder.InternalName);
            Assert.AreSame(typeof(void), _builder.ResultType);
            Assert.IsNull(_builder.Handler);
            Assert.IsNotNull(_builder.Description);
            Assert.AreEqual(0, _builder.Description.Length);
            Assert.IsNotNull(_builder.ServiceClass);
            Assert.IsFalse(_builder.Idempotent);
            Assert.IsFalse(_builder.HasParameters);
            Assert.IsNotNull(_builder.CustomAttributes);
        }
        
        [ Test ]
        public void GetSetName()
        {
            const string name = "MyMethod";
            _builder.Name = name;
            Assert.AreEqual(name, _builder.Name);
        }
        
        [ Test ]
        public void GetSetInternalName()
        {
            const string name = "MyMethod";
            _builder.InternalName = name;
            Assert.AreEqual(name, _builder.InternalName);
        }
        
        [ Test ]
        public void GetSetReturnType()
        {
            Type type = typeof(int);
            _builder.ResultType = type;
            Assert.AreSame(type, _builder.ResultType);
        }

        [ Test, ExpectedException(typeof(ArgumentNullException)) ]
        public void ReturnTypeCannotBeNull()
        {
            _builder.ResultType = null;
        }

        [ Test ]
        public void HasParameters()
        {
            Assert.IsFalse(_builder.HasParameters);
            _builder.DefineParameter();
            Assert.IsTrue(_builder.HasParameters);
        }

        [ Test ]
        public void DefineTwoParameters()
        {
            Assert.IsNotNull(_builder.DefineParameter());
            Assert.IsNotNull(_builder.DefineParameter());
            ParameterBuilder[] parameters = (ParameterBuilder[]) CollectionHelper.ToArray(_builder.Parameters, typeof(ParameterBuilder));
            Assert.IsNotNull(parameters);
            Assert.AreEqual(2, parameters.Length);
        }
        
        [ Test ]
        public void GetSetHandler()
        {
            IMethodImpl handler = new DummyMethod();
            _builder.Handler = handler;
            Assert.AreSame(handler, _builder.Handler);
        }

        [ Test ]
        public void ParametersAreAutoPositioned()
        {
            Assert.AreEqual(0, _builder.DefineParameter().Position);
            Assert.AreEqual(1, _builder.DefineParameter().Position);
            Assert.AreEqual(2, _builder.DefineParameter().Position);
        }

        [Test,ExpectedException(typeof(ArgumentNullException))]
        public void CannotSetCustomAttributesToNull()
        {
            _builder.CustomAttributes = null;
        }

        [Test]
        public void CustomAttributes()
        {
            TestCustomAttributeProvider attributes = new TestCustomAttributeProvider();
            _builder.CustomAttributes = attributes;
            Assert.AreSame(attributes, _builder.CustomAttributes);
        }

        [ Test ]
        public void GetSetDescription()
        {
            const string name = "Test description of a method.";
            _builder.Description = name;
            Assert.AreEqual(name, _builder.Description);
        }

        [ Test ]
        public void ServiceClassReference()
        {
            Assert.AreSame(_classBuilder, _builder.ServiceClass);
        }

        [ Test ]
        public void GetSetIdempotent()
        {
            _builder.Idempotent = true;
            Assert.IsTrue(_builder.Idempotent);
        }
        
        [ Test ]
        public void ParametersCollectionIsReadOnly()
        {
            Assert.IsTrue(_builder.Parameters.IsReadOnly);
        }

        private sealed class DummyMethod : IMethodImpl
        {
            public object Invoke(IService service, object[] args) { throw new NotImplementedException(); }
            public bool IsAsynchronous { get { throw new NotImplementedException(); } }
            public IAsyncResult BeginInvoke(IService service, object[] args, AsyncCallback callback, object asyncState) { throw new NotImplementedException(); }
            public object EndInvoke(IService service, IAsyncResult asyncResult) { throw new NotImplementedException(); }
        }

        private sealed class TestCustomAttributeProvider : ICustomAttributeProvider
        {
            public object[] GetCustomAttributes(Type attributeType, bool inherit)
            {
                throw new NotImplementedException();
            }

            public object[] GetCustomAttributes(bool inherit)
            {
                throw new NotImplementedException();
            }

            public bool IsDefined(Type attributeType, bool inherit)
            {
                throw new NotImplementedException();
            }
        }
    }
}


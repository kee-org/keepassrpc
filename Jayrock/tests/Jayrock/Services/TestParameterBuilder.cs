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
    using NUnit.Framework;

    #endregion

    [ TestFixture ]
    public class TestParameterBuilder
    {
        private MethodBuilder _methodBuilder;
        private ParameterBuilder _builder;

        [ SetUp ]
        public void Init()
        {
            _methodBuilder = (new ServiceClassBuilder()).DefineMethod();
            _builder = _methodBuilder.DefineParameter();
        }

        [ Test ] 
        public void Defaults()
        {
            Assert.IsNotNull(_builder.Name);
            Assert.AreEqual(0, _builder.Position);
            Assert.AreSame(typeof(object), _builder.ParameterType);
            Assert.IsFalse(_builder.IsParamArray);
            Assert.IsNotNull(_builder.Method);
        }

        [ Test ]
        public void GetSetName()
        {
            const string name = "MyParameter";
            _builder.Name = name;
            Assert.AreEqual(name, _builder.Name);
        }
        
        [ Test ]
        public void GetSetReturnType()
        {
            Type type = typeof(int);
            _builder.ParameterType = type;
            Assert.AreSame(type, _builder.ParameterType);
        }

        [ Test ]
        public void GetPosition()
        {
            Assert.AreEqual(0, _builder.Position);
            Assert.AreEqual(1, _methodBuilder.DefineParameter().Position);
        }

        [ Test, ExpectedException(typeof(ArgumentNullException)) ]
        public void ReturnTypeCannotBeNull()
        {
            _builder.ParameterType = null;
        }

        [ Test ]
        public void GetSetIsParamArray()
        {
            _builder.IsParamArray = true;
            Assert.AreEqual(true, _builder.IsParamArray);
        }
        
        [ Test ]
        public void MethodReference()
        {
            Assert.AreSame(_methodBuilder, _builder.Method);
        }
    }
}


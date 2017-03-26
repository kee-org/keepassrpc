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

    using NUnit.Framework;

    #endregion

    [ TestFixture ]
    public class TestServiceClassBuilder
    {
        private ServiceClassBuilder _builder;

        [ SetUp ]
        public void Init()
        {
            _builder = new ServiceClassBuilder();
        }

        [ Test ] 
        public void Defaults()
        {
            Assert.IsNotNull(_builder.Name);
            Assert.IsFalse(_builder.HasMethods);
            Assert.IsNotNull(_builder.Methods);
            Assert.AreEqual(0, _builder.Methods.Count);
            Assert.IsNotNull(_builder.Description);
            Assert.AreEqual(0, _builder.Description.Length);
        }

        [ Test ]
        public void GetSetName()
        {
            const string name = "MyService";
            _builder.Name = name;
            Assert.AreEqual(name, _builder.Name);
        }
        
        [ Test ]
        public void DefineTwoMethods()
        {
            Assert.IsFalse(_builder.HasMethods);
            Assert.IsNotNull(_builder.DefineMethod());
            Assert.IsTrue(_builder.HasMethods);
            Assert.IsNotNull(_builder.DefineMethod());
            Assert.IsTrue(_builder.HasMethods);
            MethodBuilder[] methods = (MethodBuilder[]) CollectionHelper.ToArray(_builder.Methods, typeof(MethodBuilder));
            Assert.IsNotNull(methods);
            Assert.AreEqual(2, methods.Length);
        }

        [ Test ]
        public void GetSetDescription()
        {
            const string name = "Test description of a service.";
            _builder.Description = name;
            Assert.AreEqual(name, _builder.Description);
        }

        [ Test ]
        public void MethodsCollectionIsReadOnly()
        {
            Assert.IsTrue(_builder.Methods.IsReadOnly);
        }
    }
}

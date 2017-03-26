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

namespace Jayrock.Tests
{
    #region Imports

    using System;
    using System.Globalization;
    using System.Reflection;
    using Jayrock.Json;
    using Jayrock.Services;
    using NUnit.Framework;

    #endregion

    public abstract class TestUtilityBase
    {
        private Type _targetType;

        protected TestUtilityBase() :
            this(null) {}

        protected TestUtilityBase(Type targetType)
        {
            _targetType = targetType;
        }

        public Type TargetType
        {
            get
            {
                return _targetType != null 
                     ? _targetType 
                     : (_targetType = InferTargetType());
            }
        }

        private Type InferTargetType()
        {
            Type thisType = GetType();
            
            string name = thisType.Name;
            if (!CultureInfo.InvariantCulture.CompareInfo.IsPrefix(name, "Test", CompareOptions.Ordinal))
                throw new Exception(string.Format("{0} is not an appropriate target for this base class.", name));
            
            string suffix = name.Substring(4);
            if (suffix.Length == 0)
                throw new Exception(string.Format("{0} is not an appropriate target for this base class.", name));
            
            string targetTypeName = thisType.Namespace + "." + suffix;
            
            Assembly[] assemblies = new Assembly[] { typeof(JsonText).Assembly, typeof(IService).Assembly };
            foreach (Assembly assembly in assemblies)
            {
                Type targetType = assembly.GetType(targetTypeName, false, false);
                if (targetType != null)
                    return targetType;
            }

            throw new Exception(string.Format("{0} not found.", targetTypeName));
        }

        [ Test ]
        public void CannotBeCreated()
        {
            try
            {
                Activator.CreateInstance(TargetType, true);
                Assert.Fail();
            }
            catch (TargetInvocationException e)
            {
                Assert.IsTrue(e.InnerException is NotSupportedException);
            }
        }
    }
}

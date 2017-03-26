#region License, Terms and Conditions
//
// Jayrock - JSON and JSON-RPC for Microsoft .NET Framework and Mono
// Written by Atif Aziz (www.raboof.com)
// Copyright (c) 2005 Atif Aziz. All rights reserved.
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
    using System.Collections;
    using System.Diagnostics;

    #endregion

    [ Serializable ]
    public sealed class ServiceClass
    {
        private readonly string _serviceName;
        private readonly string _description;
        private readonly Method[] _methods;
        private readonly string[] _methodNames;             // FIXME: [ NonSerialized ]
        private readonly Method[] _sortedMethods;    // FIXME: [ NonSerialized ]
        
        internal ServiceClass(ServiceClassBuilder classBuilder)
        {
            Debug.Assert(classBuilder != null);
            
            _serviceName = classBuilder.Name;
            _description = classBuilder.Description;

            //
            // Set up methods and their names.
            //

            ICollection methodBuilders = classBuilder.Methods;
            _methods = new Method[methodBuilders.Count];
            _methodNames = new string[methodBuilders.Count];
            int methodIndex = 0;

            foreach (MethodBuilder methodBuilder in methodBuilders)
            {
                Method method = new Method(methodBuilder, this);

                //
                // Check for duplicates.
                //

                if (Array.IndexOf(_methodNames, method.Name) >= 0)
                    throw new DuplicateMethodException(string.Format("The method '{0}' cannot be exported as '{1}' because this name has already been used by another method on the '{2}' service.", method.Name, method.InternalName, _serviceName));

                //
                // Add the method to the class and index it by its name.
                //

                _methods[methodIndex] = method;
                _methodNames[methodIndex++] = method.Name;
            }

            //
            // Keep a sorted list of parameters and their names so we can
            // do fast look ups using binary search.
            //
            
            _sortedMethods = (Method[]) _methods.Clone();
            InvariantStringArray.Sort(_methodNames, _sortedMethods);
        }

        public string Name
        {
            get { return _serviceName; }
        }

        public string Description
        {
            get { return _description; }
        }

        public Method[] GetMethods()
        {
            //
            // IMPORTANT! Never return the private array instance since the
            // caller could modify its state and compromise the integrity as
            // well as the assumptions made in this implementation.
            //

            return (Method[]) _methods.Clone();
        }

        public Method FindMethodByName(string name)
        {
            //
            // First make a quick, case-sensitive look-up.
            //
            
            int i = InvariantStringArray.BinarySearch(_methodNames, name);
            
            if (i >= 0)
                return _sortedMethods[i];
            
            //
            // Failing, use a slower case-insensitive look-up.
            // TODO: Consider speeding up FindMethodByName for case-insensitive look-ups.
            //

            foreach (Method method in _methods)
            {
                if (CaselessString.Equals(method.Name, name))
                    return method;
            }
            
            return null;
        }

        public Method GetMethodByName(string name)
        {
            Method method = FindMethodByName(name);

            if (method == null)
                throw new MethodNotFoundException();

            return method;
        }
    }
}


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
    using System.Globalization;
    using System.Reflection;

    #endregion

    [ Serializable ]
    public sealed class Method : ICustomAttributeProvider
    {
        private readonly ServiceClass _class;
        private readonly string _name;
        private readonly string _internalName;
        private readonly Type _resultType;
        private readonly Parameter[] _parameters;
        private readonly string[] _parameterNames;              // FIXME: [ NonSerialized ]
        private readonly Parameter[] _sortedParameters;  // FIXME: [ NonSerialized ]
        private readonly IMethodImpl _handler;
        private readonly string _description;
        private readonly bool _idempotent;
        private readonly ICustomAttributeProvider _attributes;

        internal Method(MethodBuilder methodBuilder, ServiceClass clazz)
        {
            Debug.Assert(methodBuilder != null);
            Debug.Assert(clazz != null);
            
            _name = methodBuilder.Name;
            _internalName = Mask.EmptyString(methodBuilder.InternalName, methodBuilder.Name);
            _resultType = methodBuilder.ResultType;
            _description = methodBuilder.Description;
            _handler = methodBuilder.Handler;
            _idempotent = methodBuilder.Idempotent;
            _attributes = methodBuilder.CustomAttributes;
            _class = clazz;
            
            //
            // Set up parameters and their names.
            //
            
            ICollection parameterBuilders = methodBuilder.Parameters;
            _parameters = new Parameter[parameterBuilders.Count];
            _parameterNames = new string[parameterBuilders.Count];

            foreach (ParameterBuilder parameterBuilder in parameterBuilders)
            {
                Parameter parameter = new Parameter(parameterBuilder, this);
                int position = parameter.Position;
                _parameters[position] = parameter;
                _parameterNames[position] = parameter.Name;
            }
            
            //
            // Keep a sorted list of parameters and their names so we can
            // do fast look ups using binary search.
            //
            
            _sortedParameters = (Parameter[]) _parameters.Clone();
            InvariantStringArray.Sort(_parameterNames, _sortedParameters);
        }

        public string Name
        {
            get { return _name; }
        }

        public string InternalName
        {
            get { return _internalName; }
        }
        
        public Parameter[] GetParameters()
        {
            //
            // IMPORTANT! Never return the private array instance since the
            // caller could modify its state and compromise the integrity as
            // well as the assumptions made in this implementation.
            //

            return (Parameter[]) _parameters.Clone();
        }

        public Type ResultType
        {
            get { return _resultType; }
        }
        
        public string Description
        {
            get { return _description; }
        }

        public bool Idempotent
        {
            get { return _idempotent; }
        }

        public ServiceClass ServiceClass
        {
            get { return _class; }
        }

        public object[] GetCustomAttributes(Type attributeType)
        {
            return GetCustomAttributes(attributeType, true);
        }

        public object[] GetCustomAttributes(Type attributeType, bool inherit)
        {
            return _attributes.GetCustomAttributes(attributeType, inherit);
        }

        public object[] GetCustomAttributes()
        {
            return GetCustomAttributes(true);
        }

        public object[] GetCustomAttributes(bool inherit)
        {
            return _attributes.GetCustomAttributes(inherit);
        }

        public bool IsDefined(Type attributeType, bool inherit)
        {
            return _attributes.IsDefined(attributeType, inherit);
        }

        public object FindFirstCustomAttribute(Type type)
        {
            if (type == null)
                throw new ArgumentNullException("type");

            object[] attributes = _attributes.GetCustomAttributes(type, true);
            return attributes != null && attributes.Length > 0 ? attributes[0] : null;
        }

        public object Invoke(IService service, string[] names, object[] args)
        {
            if (names != null)
                args = MapArguments(names, args);
            
            return _handler.Invoke(service, TransposeVariableArguments(args));
        }

        /// <remarks>
        /// The default implementation calls Invoke synchronously and returns
        /// an IAsyncResult that also indicates that the operation completed
        /// synchronously. If a callback was supplied, it will be called 
        /// before BeginInvoke returns. Also, if Invoke throws an exception, 
        /// it is delayed until EndInvoke is called to retrieve the results.
        /// </remarks>

        public IAsyncResult BeginInvoke(IService service, string[] names, object[] args, AsyncCallback callback, object asyncState)
        {
            return _handler.BeginInvoke(service, args, callback, asyncState);
        }

        public object EndInvoke(IService service, IAsyncResult asyncResult)
        {
            return _handler.EndInvoke(service, asyncResult);
        }

        /// <summary>
        /// Determines if the method accepts variable number of arguments or
        /// not. A method is designated as accepting variable arguments by
        /// annotating the last parameter of the method with the JsonRpcParams
        /// attribute.
        /// </summary>

        public bool HasParamArray
        {
            get
            {
                return _parameters.Length > 0 && 
                    _parameters[_parameters.Length - 1].IsParamArray;
            }
        }

        private object[] MapArguments(string[] names, object[] args)
        {
            Debug.Assert(names != null);
            Debug.Assert(args != null);
            Debug.Assert(names.Length == args.Length);
            
            object[] mapped = new object[_parameters.Length];

            for (int i = 0; i < names.Length; i++)
            {
                string name = names[i];
                
                if (name == null || name.Length == 0)
                    continue;
                
                object arg = args[i];
                
                if (arg == null)
                    continue;
                
                int position = -1;
                
                if (name.Length <= 2)
                {
                    char ch1;
                    char ch2;

                    if (name.Length == 2)
                    {
                        ch1 = name[0];
                        ch2 = name[1];
                    }
                    else
                    {
                        ch1 = '0';
                        ch2 = name[0];
                    }

                    if (ch1 >= '0' && ch1 <= '9' &&
                        ch2 >= '0' && ch2 <= '9')
                    {
                        position = int.Parse(name, NumberStyles.Integer, CultureInfo.InvariantCulture);
                    
                        if (position < _parameters.Length)
                            mapped[position] = arg;
                    }
                }
                
                if (position < 0)
                {
                    int order = InvariantStringArray.BinarySearch(_parameterNames, name, /* ignoreCase */ true);
                    if (order >= 0)
                        position = _sortedParameters[order].Position;
                }
                
                if (position >= 0)
                    mapped[position] = arg;
            }

            return mapped;
        }

        /// <summary>
        /// Takes an array of arguments that are designated for a method and
        /// transposes them if the target method supports variable arguments (in
        /// other words, the last parameter is annotated with the JsonRpcParams
        /// attribute). If the method does not support variable arguments then
        /// the input array is returned verbatim. 
        /// </summary>

        // TODO: Allow args to be null to represent empty arguments.
        // TODO: Allow parameter conversions

        public object[] TransposeVariableArguments(object[] args)
        {
            //
            // If the method does not have take variable arguments then just
            // return the arguments array verbatim.
            //

            if (!HasParamArray)
                return args;

            int parameterCount = _parameters.Length;

            object[] varArgs = null;
            
            //
            // The variable argument may already be setup correctly as an
            // array. If so then the formal and actual parameter count will
            // match here.
            //
            
            if (args.Length == parameterCount)
            {
                object lastArg = args[args.Length - 1];

                if (lastArg != null)
                {
                    //
                    // Is the last argument already set up as an object 
                    // array ready to be received as the variable arguments?
                    //
                    
                    varArgs = lastArg as object[];
                    
                    if (varArgs == null)
                    {
                        //
                        // Is the last argument an array of some sort? If so 
                        // then we convert it into an array of objects since 
                        // that is what we support right now for variable 
                        // arguments.
                        //
                        // TODO: Allow variable arguments to be more specific type, such as array of integers.
                        // TODO: Don't make a copy if one doesn't have to be made. 
                        // For example if the types are compatible on the receiving end.
                        //
                        
                        Array lastArrayArg = lastArg as Array;
                        
                        if (lastArrayArg != null && lastArrayArg.GetType().GetArrayRank() == 1)
                        {
                            varArgs = new object[lastArrayArg.Length];
                            Array.Copy(lastArrayArg, varArgs, varArgs.Length);
                        }
                    }
                }
            }

            //
            // Copy out the extra arguments into a new array that represents
            // the variable parts.
            //

            if (varArgs == null)
            {
                varArgs = new object[(args.Length - parameterCount) + 1];
                Array.Copy(args, parameterCount - 1, varArgs, 0, varArgs.Length);
            }

            //
            // Setup a new array of arguments that has a copy of the fixed
            // arguments followed by the variable arguments array setup above.
            //

            object[] transposedArgs = new object[parameterCount];
            Array.Copy(args, transposedArgs, parameterCount - 1);
            transposedArgs[transposedArgs.Length - 1] = varArgs;
            return transposedArgs;
        }
    }
}

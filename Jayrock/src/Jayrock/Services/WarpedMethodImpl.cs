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
    using System.ComponentModel;
    using Services;

    #endregion

    public sealed class WarpedMethodImpl : IMethodImpl
    {
        private readonly IMethodImpl _handler;
        private readonly Type _argsType;
        private readonly PropertyDescriptorCollection _args;
        private readonly PropertyDescriptor _result;

        public WarpedMethodImpl(IMethodImpl baseImpl, Type argsType, PropertyDescriptorCollection args, PropertyDescriptor result)
        {
            if (baseImpl == null) 
                throw new ArgumentNullException("baseImpl");
            if (argsType == null) 
                throw new ArgumentNullException("argsType");
            if (args == null) 
                throw new ArgumentNullException("args");

            _handler = baseImpl;
            _argsType = argsType;
            _args = args;
            _result = result;
        }

        public object Invoke(IService service, object[] args)
        {
            object result = _handler.Invoke(service, WarpedArgsFromArgsArray(args));

            if (result == null || _result == null)
                return null;

            return _result.GetValue(result);
        }

        public bool IsAsynchronous
        {
            get { return _handler.IsAsynchronous; }
        }

        public IAsyncResult BeginInvoke(IService service, object[] args, AsyncCallback callback, object asyncState)
        {
            return _handler.BeginInvoke(service, WarpedArgsFromArgsArray(args), callback, asyncState);
        }

        public object EndInvoke(IService service, IAsyncResult asyncResult)
        {
            return _result.GetValue(_handler.EndInvoke(service, asyncResult));
        }

        private object[] WarpedArgsFromArgsArray(object[] args)
        {
            object argsObj = Activator.CreateInstance(_argsType);

            for (int i = 0; i < args.Length; i++)
                _args[i].SetValue(argsObj, args[i]);

            return new object[] { argsObj };
        }
    }
}
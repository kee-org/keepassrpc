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
    using System.Diagnostics;
    using System.Threading;

    #endregion

    internal sealed class SynchronousAsyncResult : IAsyncResult
    {
        private ManualResetEvent _waitHandle;
        private readonly object _asyncState;
        private readonly object _result;
        private readonly Exception _exception;
        private bool _ended;

        public static SynchronousAsyncResult Success(object asyncState, object result)
        {
            return new SynchronousAsyncResult(asyncState, result, null);
        }

        public static SynchronousAsyncResult Failure(object asyncState, Exception e)
        {
            Debug.Assert(e != null);

            return new SynchronousAsyncResult(asyncState, null, e);
        }

        private SynchronousAsyncResult(object asyncState, object result, Exception e)
        {
            _asyncState = asyncState;
            _result = result;
            _exception = e;
        }

        public bool IsCompleted 
        {
            get { return true; }
        }

        public WaitHandle AsyncWaitHandle 
        {
            get
            {
                //
                // Create the async handle on-demand, assuming the caller
                // insists on having it even though CompletedSynchronously and
                // IsCompleted should make this redundant.
                //

                if (_waitHandle == null)
                    _waitHandle = new ManualResetEvent(true);
    
                return _waitHandle;
            }
        }

        public object AsyncState 
        {
            get { return _asyncState; }
        }

        public bool CompletedSynchronously 
        {
            get { return true; }
        }

        public object End(string methodName)
        {
            if (_ended)
                throw new InvalidOperationException(string.Format("End {0} can only be called once for each asynchronous operation.", methodName));

            _ended = true;

            if (_exception != null)
                throw _exception;

            return _result;
        }
    }
}


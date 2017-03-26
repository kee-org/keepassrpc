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

namespace Jayrock
{
    #region Imports

    using System;
    using System.Collections;

    #endregion

    internal sealed class ReverseComparer : IComparer
    {
        private readonly IComparer _comparer;

        public ReverseComparer(IComparer comparer)
        {
            if (comparer == null) throw new ArgumentNullException("comparer");
            _comparer = comparer;
        }

        public int Compare(object x, object y)
        {
            return -1 * _comparer.Compare(x, y);
        }
    }

    internal delegate IComparable ComparableSelector(object a);

    internal sealed class DelegatingComparer : IComparer
    {
        private readonly ComparableSelector _selector;

        public DelegatingComparer(ComparableSelector selector)
        {
            if (selector == null) throw new ArgumentNullException("selector");
            _selector = selector;
        }

        public int Compare(object x, object y)
        {
            return _selector(x).CompareTo(_selector(y));
        }
    }
}
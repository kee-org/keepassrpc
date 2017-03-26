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

    /// <summary> 
    /// Helper methods for dictionaries. This type supports the
    /// Jayrock infrastructure and is not intended to be used directly from
    /// your code. 
    /// </summary>
    
    public sealed class DictionaryHelper
    {
        private static readonly DictionaryEntry[] _zeroEntries = new DictionaryEntry[0];
        
        public static DictionaryEntry[] GetEntries(IDictionary dictionary)
        {
            if (dictionary == null || dictionary.Count == 0)
                return _zeroEntries;
            
            //
            // IMPORTANT!
            //
            // Dictionary entries are enumerated here manually using 
            // IDictionaryEnumerator rather than relying on foreach. Using 
            // IDictionaryEnumerator is faster and more robust. It is faster 
            // because unboxing is avoided by going over 
            // IDictionaryEnumerator.Entry rather than 
            // IDictionaryEnumerator.Current that is used by foreach. It is 
            // more robust because many people may get the implementation of 
            // IDictionary.GetEnumerator wrong, especially if they are 
            // implementing IDictionary<K, V> in .NET Framework 2.0. If the
            // implementations simply return the enumerator from the wrapped
            // dictionary then Current will return KeyValuePair<K, V> instead
            // of DictionaryEntry and cause a casting exception.
            //

            DictionaryEntry[] entries = new DictionaryEntry[dictionary.Count];
            IDictionaryEnumerator e = dictionary.GetEnumerator();
            
            try
            {       
                int index = 0;
                
                while (e.MoveNext())
                    entries[index++] = e.Entry;
                
                return entries;
            }
            finally
            {
                IDisposable disposable = e as IDisposable;
                
                if (disposable != null)
                    disposable.Dispose();
            }
        }
        
        private DictionaryHelper()
        {
            throw new NotSupportedException();
        }
    }
}
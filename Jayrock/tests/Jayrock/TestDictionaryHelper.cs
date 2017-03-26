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

namespace Jayrock
{
    #region Imports

    using System;
    using System.Collections;
    using NUnit.Framework;

    #endregion

    [ TestFixture ]
    public class TestDictionaryHelper
    {
        [ Test ]
        public void NullDictionaryYieldsZeroEntries()
        {
            DictionaryEntry[] entries = DictionaryHelper.GetEntries(null);
            Assert.IsNotNull(entries);
            Assert.AreEqual(0, entries.Length);
        }

        [ Test ]
        public void EmptyDictionaryYieldsZeroEntries()
        {
            DictionaryEntry[] entries = DictionaryHelper.GetEntries(new Hashtable());
            Assert.AreEqual(0, entries.Length);
        }

        [ Test ]
        public void NonEmptyDictionary()
        {
            Hashtable map = new Hashtable();
            
            map.Add(1, "one");
            map.Add(2, "two");
            map.Add(3, "three");
            
            ArrayList entryList = new ArrayList(DictionaryHelper.GetEntries(map));
            Assert.AreEqual(3, entryList.Count);
            
            DictionaryEntry first = Shift(entryList);
            Assert.AreEqual(first.Value, map[first.Key]);
            map.Remove(first.Key);
            
            DictionaryEntry second = Shift(entryList);
            Assert.AreEqual(second.Value, map[second.Key]);
            map.Remove(second.Key);

            DictionaryEntry third = Shift(entryList);
            Assert.AreEqual(third.Value, map[third.Key]);
            map.Remove(third.Key);
            
            Assert.AreEqual(0, map.Count);
            Assert.AreEqual(0, entryList.Count);
        }

        [ Test ]
        public void DisposableDictionaryEnumerator()
        {
            MockDictionary dict = new MockDictionary();
            dict.Add(1, "one");
            Assert.IsFalse(dict.EnumeratorDisposed);
            DictionaryHelper.GetEntries(dict);
            Assert.IsTrue(dict.EnumeratorDisposed);
        }
        
        private DictionaryEntry Shift(IList entryList)
        {
            DictionaryEntry entry = (DictionaryEntry) entryList[0];
            entryList.RemoveAt(0);
            return entry;
        }
        
        private sealed class MockDictionary : DictionaryBase, IDictionary
        {
            public bool EnumeratorDisposed;
            
            public void Add(int key, string value)
            {
                Dictionary.Add(key, value);
            }
            
            IDictionaryEnumerator IDictionary.GetEnumerator()
            {
                return new DictionaryEnumerator(this, InnerHashtable.GetEnumerator());
            }
           
            private sealed class DictionaryEnumerator : IDictionaryEnumerator, IDisposable
            {
                private readonly MockDictionary _dictionary;
                private readonly IDictionaryEnumerator _inner;

                public DictionaryEnumerator(MockDictionary dictionary, IDictionaryEnumerator inner)
                {
                    _dictionary = dictionary;
                    _inner = inner;
                }

                public object Key
                {
                    get { return _inner.Key; }
                }

                public object Value
                {
                    get { return _inner.Value; }
                }

                public DictionaryEntry Entry
                {
                    get { return _inner.Entry; }
                }

                public bool MoveNext()
                {
                    return _inner.MoveNext();
                }

                public void Reset()
                {
                    _inner.Reset();
                }

                public object Current
                {
                    get { return _inner.Current; }
                }

                public void Dispose()
                {
                    _dictionary.EnumeratorDisposed = true;
                }
            }
        }
    }
}
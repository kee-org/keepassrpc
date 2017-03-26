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

namespace Jayrock.Collections
{
    #region Imports

    using System;
    using System.Collections;
    using System.IO;
    using System.Reflection;
    using System.Runtime.Serialization;
    using System.Runtime.Serialization.Formatters.Binary;
    using NUnit.Framework;

    #endregion

    [ TestFixture ]
    public class TestKeyedCollection
    {
        [ Test ]
        public void AddValue()
        {
            NamedValueCollection values = new NamedValueCollection();
            NamedValue value = new NamedValue("Foo", new object());
            values.Add(value);
            Assert.AreEqual(1, values.Count);
            Assert.AreSame(value, values[value.Name]);
        }

        [ Test, ExpectedException(typeof(ArgumentException)) ]
        public void CannotAddWithNullKey()
        {
            NamedValueCollection values = new NamedValueCollection();
            values.Add(new NamedValue(null, new object()));
        }

        [ Test ]
        public void ValueContainment()
        {
            NamedValueCollection values = new NamedValueCollection();
            NamedValue value = new NamedValue("Foo", new object());
            Assert.IsFalse(values.Contains(value.Name));
            values.Add(value);
            Assert.IsTrue(values.Contains(value.Name));
        }

        [ Test, ExpectedException(typeof(ArgumentNullException)) ]
        public void CannotUseNullKeyForContainmentTest()
        {
            NamedValueCollection values = new NamedValueCollection();
            values.Contains(null);
        }
        
        [ Test ]
        public void RemoveValue()
        {
            NamedValueCollection values = new NamedValueCollection();
            NamedValue value = new NamedValue("Foo", new object());
            values.Add(value);
            Assert.AreEqual(1, values.Count);
            Assert.IsTrue(values.Remove(value.Name));
            Assert.IsFalse(values.Contains(value.Name));
            Assert.AreEqual(0, values.Count);
        }

        [ Test, ExpectedException(typeof(ArgumentNullException)) ]
        public void CannotRemoveNullKey()
        {
            NamedValueCollection values = new NamedValueCollection();
            values.Remove(null);
        }

        [ Test ]
        public void RemoveByIndex()
        {
            NamedValueCollection values = new NamedValueCollection();
            NamedValue value = new NamedValue("Foo", new object());
            values.Add(value);
            Assert.IsTrue(values.Contains(value.Name));
            Assert.AreEqual(1, values.Count);
            values.RemoveAt(0);
            Assert.IsFalse(values.Contains(value.Name));
            Assert.AreEqual(0, values.Count);
        }

        [ Test ]
        public void ResetValueByIndex()
        {
            NamedValueCollection values = new NamedValueCollection();
            NamedValue value = new NamedValue("Foo", new object());
            values.Add(value);
            Assert.IsTrue(values.Contains(value.Name));
            Assert.AreEqual(1, values.Count);
            NamedValue newValue = new NamedValue("Bar", new object());
            ((IList) values)[0] = newValue;
            Assert.AreEqual(1, values.Count);
            Assert.IsFalse(values.Contains(value.Name));
            Assert.IsTrue(values.Contains(newValue.Name));
        }

        [ Test ]
        public void Clear()
        {
            NamedValueCollection values = new NamedValueCollection();
            NamedValue value = new NamedValue("Foo", new object());
            values.Add(value);
            Assert.AreEqual(1, values.Count);
            Assert.IsTrue(values.Contains(value.Name));
            values.Clear();
            Assert.AreEqual(0, values.Count);
            Assert.IsFalse(values.Contains(value.Name));
        }

        [ Test ]
        public void GetKeys()
        {
            NamedValueCollection values = new NamedValueCollection();
            string[] names = new string[] { "one", "two", "three" };
            foreach (string name in names)
                values.Add(new NamedValue(name, new object()));
            Assert.AreEqual(names.Length, values.Count);
            string[] keys = values.GetKeys();
            Assert.AreEqual(names, keys);
        }

        [ Test ]
        public void Enumeration()
        {
            NamedValueCollection values = new NamedValueCollection();
            NamedValue value1 = new NamedValue("one", new object()); values.Add(value1);
            NamedValue value2 = new NamedValue("two", new object()); values.Add(value2);
            NamedValue value3 = new NamedValue("three", new object()); values.Add(value3);
            Assert.AreEqual(3, values.Count);
            IEnumerator e = values.GetEnumerator();
            Assert.IsTrue(e.MoveNext());
            Assert.AreSame(value1, e.Current);
            Assert.IsTrue(e.MoveNext());
            Assert.AreSame(value2, e.Current);
            Assert.IsTrue(e.MoveNext());
            Assert.AreSame(value3, e.Current);
            Assert.IsFalse(e.MoveNext());
        }

        [ Test ]
        public void RemoveNonExistingKey()
        {
            NamedValueCollection values = new NamedValueCollection();
            Assert.IsFalse(values.Remove("something"));
        }

        [ Test, ExpectedException(typeof(ArgumentNullException)) ]
        public void CannotSendNullArrayToBaseListKeysByIndex()
        {
            (new NamedValueCollection()).ListKeysTo(null);
        }

        [ Test, ExpectedException(typeof(ArgumentException)) ]
        public void CannotSendMultiDimensionalArrayToBaseListKeysByIndex()
        {
            (new NamedValueCollection()).ListKeysTo(new string[0, 0]);
        }

        [ Test, ExpectedException(typeof(ArgumentNullException)) ]
        public void CannotIndexByNullKey()
        {
            /* object unused = */ new NamedValueCollection()[null] /* [1] */ .ToString();

            //
            // [1] The ToString call is unnecessary but it is added here
            //     instead of taking the indexed value into an unused 
            //     variable to avoid the CS0219 warning issue from Mono's 
            //     C# compiler. See: 
            //     http://bugzilla.novell.com/show_bug.cgi?id=316137
        }

        [ Test ]
        public void AddsNoSerializableState()
        {
            MemberInfo[] members = FormatterServices.GetSerializableMembers(typeof(KeyedCollection));
            foreach (MemberInfo member in members)
            Assert.AreNotEqual(typeof(KeyedCollection), member.DeclaringType);
        }

        [ Test ]
        public void MappingIsRestoredUponDeserialization()
        {
            NamedValueCollection collection = new NamedValueCollection();
            Assert.IsFalse(collection.OnDeserializationCallbackCalled);
            collection.Add(new NamedValue("foo", "bar"));
            BinaryFormatter formatter = new BinaryFormatter();
            MemoryStream ms = new MemoryStream();
            formatter.Serialize(ms, collection);
            ms.Position = 0; // rewind
            collection = (NamedValueCollection) formatter.Deserialize(ms);
            Assert.IsTrue(collection.OnDeserializationCallbackCalled);
            NamedValue entry = collection["foo"];
            Assert.IsNotNull(entry);
            Assert.AreEqual("foo", entry.Name); 
            Assert.AreEqual("bar", entry.Value);
        }

        [ Serializable ]
        private sealed class NamedValue
        {
            public string Name;
            public object Value;

            public NamedValue(string name, object value)
            {
                Name = name;
                Value = value;
            }
        }
        
        [ Serializable ]
        private sealed class NamedValueCollection : KeyedCollection
        {
            public bool OnDeserializationCallbackCalled;

            public NamedValue this[string key]
            {
                get { return (NamedValue) GetByKey(key); }
            }

            public void Add(NamedValue value)
            {
                base.Add(value);
            }
            
            public bool Contains(string key)
            {
                return base.Contains(key);
            }

            public bool Remove(string key)
            {
                return base.Remove(key);
            }

            public string[] GetKeys()
            {
                string[] keys = new string[Count];
                ListKeysByIndex(keys);
                return keys;
            }

            public void ListKeysTo(Array keys)
            {
                ListKeysByIndex(keys);
            }

            protected override object KeyFromValue(object value)
            {
                return ((NamedValue) value).Name;
            }

            protected override void OnDeserializationCallback(object sender)
            {
                OnDeserializationCallbackCalled = true;
                base.OnDeserializationCallback(sender);
            }
        }
    }
}

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

namespace Jayrock.Json.Conversion
{
    #region Imports

    using System;
    using System.Collections;
    using System.Collections.Specialized;
    using System.Threading;
    using Jayrock.Json.Conversion;
    using Jayrock.Json.Conversion.Converters;
    using NUnit.Framework;

    #endregion

    [ TestFixture ]
    public class TestImportContext
    {
        [ Test ]
        public void StockImporters()
        {
            AssertInStock(typeof(ByteImporter), typeof(byte));
            AssertInStock(typeof(Int16Importer), typeof(short));
            AssertInStock(typeof(Int32Importer), typeof(int));
            AssertInStock(typeof(Int64Importer), typeof(long));
            AssertInStock(typeof(SingleImporter), typeof(float));
            AssertInStock(typeof(DoubleImporter), typeof(double));
            AssertInStock(typeof(DecimalImporter), typeof(decimal));
            AssertInStock(typeof(DateTimeImporter), typeof(DateTime));
            AssertInStock(typeof(StringImporter), typeof(string));
            AssertInStock(typeof(BooleanImporter), typeof(bool));
            AssertInStock(typeof(AnyImporter), typeof(object));
            AssertInStock(typeof(ArrayImporter), typeof(object[]));
            AssertInStock(typeof(ByteArrayImporter), typeof(byte[]));
            AssertInStock(typeof(EnumImporter), typeof(System.Globalization.UnicodeCategory));
            AssertInStock(typeof(ImportAwareImporter), typeof(JsonObject));
            AssertInStock(typeof(ImportAwareImporter), typeof(IDictionary));
            AssertInStock(typeof(ImportAwareImporter), typeof(JsonArray));
            AssertInStock(typeof(ImportAwareImporter), typeof(IList));
            AssertInStock(typeof(ImportAwareImporter), typeof(ImportableThing));
            AssertInStock(typeof(GuidImporter), typeof(Guid));
            AssertInStock(typeof(NameValueCollectionImporter), typeof(NameValueCollection));
            AssertInStock(typeof(ComponentImporter), typeof(ValueThing));
            AssertInStock(typeof(UriImporter), typeof(Uri));
            AssertInStock(typeof(JsonBufferImporter), typeof(JsonBuffer));

            #if !NET_1_0 && !NET_1_1 

            AssertInStock(typeof(NullableImporter), typeof(int?));
            AssertInStock(typeof(DictionaryImporter<string, string>), typeof(System.Collections.Generic.IDictionary<string, string>));
            AssertInStock(typeof(DictionaryImporter<int, object>), typeof(System.Collections.Generic.IDictionary<int, object>));
            AssertInStock(typeof(DictionaryImporter<Guid, string>), typeof(SubDictionaryThing));

            // TODO Use AssertInStock once CollectionImporter is public
            Assert.IsNotNull(new ImportContext().FindImporter(typeof(System.Collections.Generic.IList<string>)));
            Assert.IsNotNull(new ImportContext().FindImporter(typeof(System.Collections.Generic.ICollection<string>)));
            Assert.IsNotNull(new ImportContext().FindImporter(typeof(System.Collections.Generic.IEnumerable<string>)));
            Assert.IsNotNull(new ImportContext().FindImporter(typeof(IEnumerable)));

            #endif // !NET_1_0 && !NET_1_1

            #if !NET_1_0 && !NET_1_1 && !NET_2_0

            // TODO Use AssertInStock once CollectionImporter is public
            Assert.IsNotNull(new ImportContext().FindImporter(typeof(System.Collections.Generic.ISet<string>)));
            
            AssertInStock(typeof(BigIntegerImporter), typeof(System.Numerics.BigInteger));
            AssertInStock(typeof(ExpandoObjectImporter), typeof(System.Dynamic.ExpandoObject));

            AssertInStock(typeof(TupleImporter), typeof(Tuple<int>));
            AssertInStock(typeof(TupleImporter), typeof(Tuple<int, int>));
            AssertInStock(typeof(TupleImporter), typeof(Tuple<int, int, int>));
            AssertInStock(typeof(TupleImporter), typeof(Tuple<int, int, int, int>));
            AssertInStock(typeof(TupleImporter), typeof(Tuple<int, int, int, int, int>));
            AssertInStock(typeof(TupleImporter), typeof(Tuple<int, int, int, int, int, int>));
            AssertInStock(typeof(TupleImporter), typeof(Tuple<int, int, int, int, int, int, int>));
            AssertInStock(typeof(TupleImporter), typeof(Tuple<int, int, int, int, int, int, int, int>));

            #endif // !NET_1_0 && !NET_1_1 && !NET_2_0
        }

        [ Test ]
        public void HasItems()
        {
            Assert.IsNotNull((new ImportContext()).Items);
        }

        [ Test ]
        public void Registration()
        {
            ImportContext context = new ImportContext();
            ThingImporter importer = new ThingImporter();
            context.Register(importer);
            Assert.AreSame(importer, context.FindImporter(typeof(Thing)));
        }

        [ Test ]
        public void RegistrationIsPerContext()
        {
            ImportContext context = new ImportContext();
            ThingImporter exporter = new ThingImporter();
            context.Register(exporter);
            context = new ImportContext();
            Assert.AreNotSame(exporter, context.FindImporter(typeof(Thing)));
        }

        private static void AssertInStock(Type expected, Type type)
        {
            ImportContext context = new ImportContext();
            IImporter importer = context.FindImporter(type);
            Assert.IsNotNull(importer, "No importer found for {0}", type.FullName);
            Assert.IsInstanceOfType(expected, importer, type.FullName);
        }
        
        private sealed class ImportableThing : IJsonImportable
        {
            public void Import(ImportContext context, JsonReader reader)
            {
                throw new NotImplementedException();
            }
        }

        private sealed class Thing {}

        private sealed class ThingImporter : IImporter
        {
            public Type OutputType
            {
                get { return typeof(Thing); }
            }

            public object Import(ImportContext context, JsonReader reader)
            {
                throw new NotImplementedException();
            }
        }
    
        public struct ValueThing
        {
            public int Field1;
            public int Field2;
        }

        #if !NET_1_0 && !NET_1_1 

        internal class DictionaryThing : System.Collections.Generic.IDictionary<Guid, string>
        {
            #region Implementation of IEnumerable

            public System.Collections.Generic.IEnumerator<System.Collections.Generic.KeyValuePair<Guid, string>> GetEnumerator()
            {
                throw new NotImplementedException();
            }

            IEnumerator IEnumerable.GetEnumerator()
            {
                return GetEnumerator();
            }

            #endregion

            #region Implementation of ICollection<KeyValuePair<Guid,string>>

            public void Add(System.Collections.Generic.KeyValuePair<Guid, string> item)
            {
                throw new NotImplementedException();
            }

            public void Clear()
            {
                throw new NotImplementedException();
            }

            public bool Contains(System.Collections.Generic.KeyValuePair<Guid, string> item)
            {
                throw new NotImplementedException();
            }

            public void CopyTo(System.Collections.Generic.KeyValuePair<Guid, string>[] array, int arrayIndex)
            {
                throw new NotImplementedException();
            }

            public bool Remove(System.Collections.Generic.KeyValuePair<Guid, string> item)
            {
                throw new NotImplementedException();
            }

            public int Count
            {
                get { throw new NotImplementedException(); }
            }

            public bool IsReadOnly
            {
                get { throw new NotImplementedException(); }
            }

            #endregion

            #region Implementation of IDictionary<Guid,string>

            public bool ContainsKey(Guid key)
            {
                throw new NotImplementedException();
            }

            public void Add(Guid key, string value)
            {
                throw new NotImplementedException();
            }

            public bool Remove(Guid key)
            {
                throw new NotImplementedException();
            }

            public bool TryGetValue(Guid key, out string value)
            {
                throw new NotImplementedException();
            }

            public string this[Guid key]
            {
                get { throw new NotImplementedException(); }
                set { throw new NotImplementedException(); }
            }

            public System.Collections.Generic.ICollection<Guid> Keys
            {
                get { throw new NotImplementedException(); }
            }

            public System.Collections.Generic.ICollection<string> Values
            {
                get { throw new NotImplementedException(); }
            }

            #endregion
        }

        private sealed class SubDictionaryThing : DictionaryThing
        {
        }

        #endif // !NET_1_0 && !NET_1_1
    }
}

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

namespace Jayrock.Json.Conversion
{
    #region Imports

    using System;
    using System.Collections;
    using System.Configuration;
    using System.Diagnostics;
    using Jayrock.Json.Conversion.Converters;
    using Jayrock.Reflection;
    #if !NET_1_0 && !NET_1_1
    using System.Collections.Generic;
    #endif

    #endregion

    [ Serializable ]
    public class ImportContext
    {
        private ImporterCollection _importers;
        private IDictionary _items;

        private static ImporterCollection _stockImporters;
        
        public virtual object Import(JsonReader reader)
        {
            return Import(AnyType.Value, reader);
        }

        public virtual object Import(Type type, JsonReader reader)
        {
            if (type == null)
                throw new ArgumentNullException("type");
            
            if (reader == null)
                throw new ArgumentNullException("reader");

            IImporter importer = FindImporter(type);

            if (importer == null)
                throw new JsonException(string.Format("Don't know how to import {0} from JSON.", type.FullName));

            reader.MoveToContent();
            return importer.Import(this, reader);
        }

#if !NET_1_0 && !NET_1_1

        public virtual T Import<T>(JsonReader reader)
        {
            return (T) Import(typeof(T), reader);
        }

#endif

        public virtual void Register(IImporter importer)
        {
            if (importer == null)
                throw new ArgumentNullException("importer");
            
            Importers.Put(importer);
        }

        public virtual IImporter FindImporter(Type type) 
        {
            if (type == null)
                throw new ArgumentNullException("type");

            IImporter importer = Importers[type];
            
            if (importer != null)
                return importer;
            
            importer = StockImporters[type];

            if (importer == null)
               importer = FindCompatibleImporter(type);

            if (importer != null)
            {
                Register(importer);
                return importer;
            }

            return null;
        }

        public IDictionary Items
        {
            get
            {
                if (_items == null)
                    _items = new Hashtable();
                
                return _items;
            }
        }

        private static IImporter FindCompatibleImporter(Type type) 
        {
            Debug.Assert(type != null);

            if (typeof(IJsonImportable).IsAssignableFrom(type))
                return new ImportAwareImporter(type);

            if (type.IsArray && type.GetArrayRank() == 1)
                return new ArrayImporter(type);

            if (type.IsEnum)
                return new EnumImporter(type);

            #if !NET_1_0 && !NET_1_1 

            if (Reflector.IsConstructionOfNullable(type))
                return new NullableImporter(type);

            bool isGenericList = Reflector.IsConstructionOfGenericTypeDefinition(type, typeof(IList<>));
            bool isGenericCollection = !isGenericList && Reflector.IsConstructionOfGenericTypeDefinition(type, typeof(ICollection<>));
            bool isSequence = !isGenericCollection && (type == typeof(IEnumerable) || Reflector.IsConstructionOfGenericTypeDefinition(type, typeof(IEnumerable<>)));
            
            if (isGenericList || isGenericCollection || isSequence)
            {
                Type itemType = type.IsGenericType 
                              ? type.GetGenericArguments()[0] 
                              : typeof(object);
                Type importerType = typeof(CollectionImporter<,>).MakeGenericType(new Type[] { type, itemType });
                return (IImporter) Activator.CreateInstance(importerType, new object[] { isSequence });
            }

            if (Reflector.IsConstructionOfGenericTypeDefinition(type, typeof(IDictionary<,>)))
                return (IImporter) Activator.CreateInstance(typeof(DictionaryImporter<,>).MakeGenericType(type.GetGenericArguments()));

            Type genericDictionaryType = Reflector.FindConstructionOfGenericInterfaceDefinition(type, typeof(IDictionary<,>));
            if (genericDictionaryType != null)
            {
                Type[] args2 = genericDictionaryType.GetGenericArguments();
                Debug.Assert(args2.Length == 2);
                Type[] args3 = new Type[3];
                args3[0] = type;        // [ TDictionary, ... , ...    ]
                args2.CopyTo(args3, 1); // [ TDictionary, TKey, TValue ]
                return (IImporter)Activator.CreateInstance(typeof(DictionaryImporter<,,>).MakeGenericType(args3));
            }

            #endif // !NET_1_0 && !NET_1_1 
            
            #if !NET_1_0 && !NET_1_1 && !NET_2_0

            if (Reflector.IsConstructionOfGenericTypeDefinition(type, typeof(ISet<>)))
            {
                Type[] typeArguments = type.GetGenericArguments();
                Type hashSetType = typeof(HashSet<>).MakeGenericType(typeArguments);
                return (IImporter)Activator.CreateInstance(typeof(CollectionImporter<,,>).MakeGenericType(new Type[] { hashSetType, type, typeArguments[0] }));
            }

            if (Reflector.IsTupleFamily(type))
                return new TupleImporter(type);

            #endif
            
            if ((type.IsPublic || type.IsNestedPublic) && 
                !type.IsPrimitive && 
                (type.IsValueType || type.GetConstructors().Length > 0))
            {
                return new ComponentImporter(type, new ObjectConstructor(type));
            }

            CustomTypeDescriptor anonymousClass = CustomTypeDescriptor.TryCreateForAnonymousClass(type);
            if (anonymousClass != null)
                return new ComponentImporter(type, anonymousClass, new ObjectConstructor(type));

            return null;
        }

        private ImporterCollection Importers
        {
            get
            {
                if (_importers == null)
                    _importers = new ImporterCollection();
                
                return _importers;
            }
        }

        private static ImporterCollection StockImporters
        {
            get
            {
                if (_stockImporters == null)
                {
                    ImporterCollection importers = new ImporterCollection();

                    importers.Add(new ByteImporter());
                    importers.Add(new Int16Importer());
                    importers.Add(new Int32Importer());
                    importers.Add(new Int64Importer());
                    importers.Add(new SingleImporter());
                    importers.Add(new DoubleImporter());
                    importers.Add(new DecimalImporter());
                    importers.Add(new StringImporter());
                    importers.Add(new BooleanImporter());
                    importers.Add(new DateTimeImporter());
                    importers.Add(new GuidImporter());
                    importers.Add(new UriImporter());
                    importers.Add(new ByteArrayImporter());
                    importers.Add(new AnyImporter());
                    importers.Add(new DictionaryImporter());
                    importers.Add(new ListImporter());
                    importers.Add(new NameValueCollectionImporter());
                    importers.Add(new JsonBufferImporter());

                    #if !NET_1_0 && !NET_1_1 && !NET_2_0

                    importers.Add(new BigIntegerImporter());
                    importers.Add(new ExpandoObjectImporter());
                    
                    #endif // !NET_1_0 && !NET_1_1 && !NET_2_0

                    IList typeList = (IList) ConfigurationSettings.GetConfig("jayrock/json.conversion.importers");

                    if (typeList != null && typeList.Count > 0)
                    {
                        foreach (Type type in typeList)
                            importers.Put((IImporter) Activator.CreateInstance(type));
                    }

                    _stockImporters = importers;
                }
                
                return _stockImporters;
            }
        }
    }
}

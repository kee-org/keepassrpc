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

#if !NET_1_0 && !NET_1_1 && !NET_2_0 

namespace Jayrock.Json.Conversion.Converters
{
    #region Imports

    using System;
    using System.Diagnostics;
    using System.Linq;
    using System.Linq.Expressions;
    using System.Reflection;
    using Jayrock.Reflection;

    #endregion

    public class TupleImporter : ImporterBase
    {
        private readonly Func<ImportContext, JsonReader, object> _importer;
        private readonly bool _single;
        private static readonly MethodInfo _importMethod = ((MethodCallExpression)((Expression<Func<ImportContext, object>>)(context => context.Import(null, null))).Body).Method;

        public TupleImporter(Type outputType) :
            base(outputType)
        {
            if (!Reflector.IsTupleFamily(outputType))
                throw new ArgumentException(null, "outputType");
            _importer = CompileItemsImporter(outputType);
            _single = outputType.GetGenericArguments().Length == 1;
        }

        protected override object ImportFromBoolean(ImportContext context, JsonReader reader)
        {
            return _single 
                 ? _importer(context, reader) 
                 : base.ImportFromBoolean(context, reader);
        }

        protected override object ImportFromNumber(ImportContext context, JsonReader reader)
        {
            return _single 
                 ? _importer(context, reader) 
                 : base.ImportFromNumber(context, reader);
        }

        protected override object ImportFromString(ImportContext context, JsonReader reader)
        {
            return _single 
                 ? _importer(context, reader) 
                 : base.ImportFromString(context, reader);
        }

        protected override object ImportFromArray(ImportContext context, JsonReader reader)
        {
            if (_single)
                return _importer(context, reader);

            reader.Read();
            var result = _importer(context, reader);
            reader.ReadToken(JsonTokenClass.EndArray);
            return result;
        }

        protected override object ImportFromObject(ImportContext context, JsonReader reader)
        {
            return _single 
                 ? _importer(context, reader) 
                 : base.ImportFromObject(context, reader);
        }

        private static Func<ImportContext, JsonReader, object> CompileItemsImporter(Type tupleType)
        {
            Debug.Assert(tupleType != null);

            var context = Expression.Parameter(typeof(ImportContext), "context");
            var reader = Expression.Parameter(typeof(JsonReader), "reader");

            var argTypes = tupleType.GetGenericArguments();
            var createMethod = typeof(Tuple).GetMethods(BindingFlags.Public | BindingFlags.Static)
                                            .Single(method => method.IsGenericMethodDefinition
                                                              && "Create".Equals(method.Name, StringComparison.Ordinal)
                                                              && argTypes.Length == method.GetGenericArguments().Length)
                                            .MakeGenericMethod(argTypes);

            //
            // Suppose tupleType is Tuple<int, string, DateTime>, emit a 
            // call expression like this:
            //
            //  Tuple.Create((int)      context.Import(typeof(int), reader),
            //               (string)   context.Import(typeof(string), reader),
            //               (DateTime) context.Import(typeof(DateTime), reader));
            //
            //

            var args = from argType in argTypes
                       select Expression.Convert(Expression.Call(context, _importMethod, Expression.Constant(argType), reader), argType);
            var body = Expression.Call(createMethod, args);
            var lambda = Expression.Lambda<Func<ImportContext, JsonReader, object>>(body, context, reader);
            return lambda.Compile();
        }
    }
}

#endif // !NET_1_0 && !NET_1_1 && !NET_2_0

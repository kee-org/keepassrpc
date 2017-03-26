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
    using System.Collections.Generic;
    using System.Diagnostics;
    using System.Linq;
    using System.Linq.Expressions;
    using System.Reflection;
    using Jayrock.Reflection;

    #endregion

    public class TupleExporter : ExporterBase
    {
        private readonly Action<ExportContext, object, JsonWriter> _exporter;
        private static readonly MethodInfo _exportMethod = ((MethodCallExpression) ((Expression<Action<ExportContext>>) (context => context.Export(null, null))).Body).Method;

        public TupleExporter(Type inputType)
            : base(inputType)
        {
            if (!Reflector.IsTupleFamily(inputType))
                throw new ArgumentException(null, "inputType");

            _exporter = CompileExporter(inputType);
        }

        protected override void ExportValue(ExportContext context, object value, JsonWriter writer)
        {
            if (context == null) throw new ArgumentNullException("context");
            if (writer == null) throw new ArgumentNullException("writer");

            writer.WriteStartArray();
            _exporter(context, value, writer);
            writer.WriteEndArray();
        }

        private static Action<ExportContext, object, JsonWriter> CompileExporter(Type tupleType)
        {
            Debug.Assert(tupleType != null);

            var context = Expression.Parameter(typeof(ExportContext), "context");
            var obj     = Expression.Parameter(typeof(object), "obj");
            var writer  = Expression.Parameter(typeof(JsonWriter), "writer");
            
            var tuple   = Expression.Variable(tupleType, "tuple");
            var body    = Expression.Block
                          (
                              new[] { tuple },
                              new[] { Expression.Assign(tuple, Expression.Convert(obj, tupleType)) } /* ...
                              ... */ .Concat(CreateItemExportCallExpressions(context, tuple, writer))
                          );

            var lambda  = Expression.Lambda<Action<ExportContext, object, JsonWriter>>(body, context, obj, writer);
            return lambda.Compile();
        }

        private static IEnumerable<Expression> CreateItemExportCallExpressions(ParameterExpression context, ParameterExpression tuple, ParameterExpression writer)
        {
            Debug.Assert(context != null);
            Debug.Assert(tuple != null);
            Debug.Assert(writer != null);

            //
            // Suppose type of tuple is Tuple<int, string, DateTime>, return
            // call expressions like this:
            //
            //  context.Export((object) tuple.Item1, writer);
            //  context.Export((object) tuple.Item2, writer);
            //  context.Export((object) tuple.Item3, writer);
            //

            var properties = tuple.Type.GetProperties();
            return from property in properties
                   select Expression.Call
                   (
                       context, _exportMethod, 
                           Expression.Convert(Expression.MakeMemberAccess(tuple, property), typeof(object)), 
                           writer
                   );
        }
    }
}

#endif // !NET_1_0 && !NET_1_1 && !NET_2_0

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

#if !NET_1_0 && !NET_1_1

namespace Jayrock.Json.Conversion.Converters
{
    #region Imports

    using System;
    using System.Diagnostics;
    using Jayrock.Reflection;

    #endregion

    public class NullableImporter : ImporterBase
    {
        private readonly Type _underlyingType;

        public NullableImporter(Type outputType) : 
            base(outputType)
        {
            if (!Reflector.IsConstructionOfNullable(outputType))
                throw new ArgumentException(null, "outputType");
            _underlyingType = 
            #if !MONO
                Nullable.GetUnderlyingType(outputType);            
            #else
                outputType.GetGenericArguments()[0];
            #endif
        }

        private object ImportUnderlyingType(ImportContext context, JsonReader reader)
        {
            Debug.Assert(context != null);
            Debug.Assert(reader != null);
            return context.Import(_underlyingType, reader);
        }

        protected override object ImportFromBoolean(ImportContext context, JsonReader reader)
        {
            return ImportUnderlyingType(context, reader);
        }

        protected override object ImportFromNumber(ImportContext context, JsonReader reader)
        {
            return ImportUnderlyingType(context, reader);
        }

        protected override object ImportFromString(ImportContext context, JsonReader reader)
        {
            return ImportUnderlyingType(context, reader);
        }

        protected override object ImportFromArray(ImportContext context, JsonReader reader)
        {
            return ImportUnderlyingType(context, reader);
        }

        protected override object ImportFromObject(ImportContext context, JsonReader reader)
        {
            return ImportUnderlyingType(context, reader);
        }
    }
}

#endif // !NET_1_0 && !NET_1_1

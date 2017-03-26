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

namespace Jayrock.Json
{
    #region Imports

    using System;
    using System.Runtime.Serialization;
    using Jayrock.Json.Conversion;

    #endregion

    /// <summary>
    /// Represent the one and only representation of the "null" value in JSON.
    /// </summary>

    [ Serializable ]
    public sealed class JsonNull : IObjectReference
    {
        public const string Text = "null";
        public static readonly JsonNull Value = new JsonNull();

        private JsonNull() {}

        public override string ToString()
        {
            return JsonNull.Text;
        }

        public static bool LogicallyEquals(object o)
        {
            //
            // Equals a null reference?
            //

            if (o == null)
                return true;

            //
            // Equals self, of course?
            //

            if (o.Equals(JsonNull.Value))
                return true;

            //
            // Equals the logical null value used in database applications?
            //

            if (Convert.IsDBNull(o))
                return true;
            
            //
            // Instance is not one of the known logical null values.
            //

            return false;
        }
                
        object IObjectReference.GetRealObject(StreamingContext context)
        {
            return JsonNull.Value;
        }
    }
}

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
    /// <summary>
    /// Defines methods to export a single JSON Object member.
    /// </summary>
    
    public interface IObjectMemberExporter
    {
        /// <summary>
        /// Gets a value from a source object and exports it as a JSON 
        /// Object member (name plus value).
        /// </summary>
        /// <remarks>
        /// The implementation is responsible for writing the member
        /// name and value. The implementation is free to omit writing
        /// anything to produce a terser output. For example, if the
        /// corresponding field or property of the source object is
        /// null then the implementation could just do nothing instead
        /// of emitting the member name with a value of JSON Null.
        /// </remarks>
        
        void Export(ExportContext context, JsonWriter writer, object source);
    }
}
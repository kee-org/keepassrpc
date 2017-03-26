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
    /// Defines methods to import a single JSON Object member value.
    /// </summary>

    public interface IObjectMemberImporter
    {
        /// <summary>
        /// Imports a single incoming JSON Object member and sends its value
        /// to the target object (usually to a corresponding field or 
        /// property).
        /// </summary>
        /// <remarks>
        /// The implementation is only responsible for reading the member
        /// value and not the name. The caller should have already
        /// read the JSON Object member name. The implementation is, 
        /// however, responsible for projecting the member value on to
        /// the target object. Whether it actually does that or not,
        /// it must under all circumstances advance the reader to the
        /// token following the value (which should normally be another
        /// member name or end of the object).
        /// </remarks>
        
        void Import(ImportContext context, JsonReader reader, object target);
    }
}
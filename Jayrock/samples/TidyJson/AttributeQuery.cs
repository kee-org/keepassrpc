#region License, Terms and Conditions
//
// The MIT License
// Copyright (c) 2006, Atif Aziz. All rights reserved.
//
// Permission is hereby granted, free of charge, to any person obtaining a 
// copy of this software and associated documentation files 
// (the "Software"), to deal in the Software without restriction, 
// including without limitation the rights to use, copy, modify, merge, 
// publish, distribute, sublicense, and/or sell copies of the Software, 
// and to permit persons to whom the Software is furnished to do so, subject 
// to the following conditions:
//
// The above copyright notice and this permission notice shall be included 
// in all copies or substantial portions of the Software.
//
// THE SOFTWARE IS PROVIDED "AS IS", WITHOUT WARRANTY OF ANY KIND, EXPRESS 
// OR IMPLIED, INCLUDING BUT NOT LIMITED TO THE WARRANTIES OF 
// MERCHANTABILITY, FITNESS FOR A PARTICULAR PURPOSE AND NONINFRINGEMENT. 
// IN NO EVENT SHALL THE AUTHORS OR COPYRIGHT HOLDERS BE LIABLE FOR ANY 
// CLAIM, DAMAGES OR OTHER LIABILITY, WHETHER IN AN ACTION OF CONTRACT, 
// TORT OR OTHERWISE, ARISING FROM, OUT OF OR IN CONNECTION WITH THE 
// SOFTWARE OR THE USE OR OTHER DEALINGS IN THE SOFTWARE.
//
//
// Author(s):
//  Atif Aziz (http://www.raboof.com)
//
#endregion

namespace TidyJson
{
    #region Imports

    using System;
    using System.Reflection;

    #endregion

    internal static class AttributeQuery<T>
        where T : Attribute
    {
        public static T Get<P>(P provider)
            where P : ICustomAttributeProvider
        {
            return Get(provider, false);
        }
        
        public static T Get<P>(P provider, bool inherit)
            where P : ICustomAttributeProvider
        {
            T attribute = Find(provider, inherit);

            if (attribute == null)
                throw new ObjectNotFoundException(string.Format("The attribute {0} was not found.", typeof(T).FullName));

            return attribute;
        }

        public static T Find<P>(P provider)
            where P : ICustomAttributeProvider
        {
            return Find(provider, false);
        }

        public static T Find<P>(P provider, bool inherit)
            where P : ICustomAttributeProvider
        {
            T[] attributes = FindAll(provider, inherit);
            return attributes.Length > 0 ? attributes[0] : null;
        }

        public static T FindAll<P>(P provider)
            where P : ICustomAttributeProvider
        {
            return Find(provider, false);
        }

        public static T[] FindAll<P>(P provider, bool inherit)
            where P : ICustomAttributeProvider
        {
            return (T[]) provider.GetCustomAttributes(typeof(T), inherit);
        }
    }
}

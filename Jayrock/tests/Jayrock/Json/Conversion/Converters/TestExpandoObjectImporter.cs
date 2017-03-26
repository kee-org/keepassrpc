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

#if !NET_1_0 && !NET_1_1 && !NET_2_0

namespace Jayrock.Json.Conversion.Converters
{
    #region Imports

    using System;
    using System.Collections.Generic;
    using System.Data;
    using System.Dynamic;
    using System.Text;
    using NUnit.Framework;

    #endregion

    [ TestFixture ]
    public class TestExpandoObjectImporter
    {
        [ Test ]
        public void Superclass()
        {
            Assert.IsInstanceOfType(typeof(ImporterBase), new ExpandoObjectImporter());
        }

        [ Test ]
        public void InputTypeIsExpandoObject()
        {
            Assert.AreSame(typeof(ExpandoObject), (new ExpandoObjectImporter()).OutputType);
        }

        [Test]
        public void Export()
        {
            var context = JsonConvert.CreateImportContext();
            var importer = new ExpandoObjectImporter();
            var expando = importer.Import(context, JsonText.CreateReader(@"
            {
                num: 123, flag: true, str: foobar, nil: null,
                obj: { x: 456, y: 789 },
                arr: [ 123, { x: 456, y: 789 } ],
            }"));
            Assert.IsNotNull(expando);
            Assert.IsInstanceOfType(typeof(ExpandoObject), expando);
            dynamic obj = expando;
            Assert.AreEqual(123, ((JsonNumber) obj.num).ToInt32());
            Assert.IsTrue((bool) obj.flag);
            Assert.AreEqual("foobar", (string) obj.str);
            Assert.IsInstanceOfType(typeof(ExpandoObject), (object) obj.obj);
            Assert.AreEqual(456, ((JsonNumber) obj.obj.x).ToInt32());
            Assert.AreEqual(789, ((JsonNumber) obj.obj.y).ToInt32());
            Assert.IsTrue(typeof(IList<object>).IsAssignableFrom(((object)obj.arr).GetType()));
            var arr = (IList<object>) obj.arr;
            Assert.AreEqual(123, ((JsonNumber) arr[0]).ToInt32());
            Assert.IsInstanceOfType(typeof(ExpandoObject), arr[1]);
            Assert.AreEqual(456, ((JsonNumber) (((dynamic) (arr[1])).x)).ToInt32());
            Assert.AreEqual(789, ((JsonNumber) (((dynamic) (arr[1])).y)).ToInt32());
        }
    }
}

#endif // !NET_1_0 && !NET_1_1 && !NET_2_0

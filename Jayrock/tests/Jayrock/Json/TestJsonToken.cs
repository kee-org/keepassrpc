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

namespace Jayrock.Json
{
    #region Imports

    using System;
    using NUnit.Framework;

    #endregion

    [ TestFixture ]
    public class TestJsonToken
    {
        [ Test ]
        public void NullToken()
        {
            Assert.AreEqual(JsonTokenClass.Null, JsonToken.Null().Class);
        }
        
        [ Test ]
        public void StringToken()
        {
            JsonToken token = JsonToken.String("hello");
            Assert.AreEqual(JsonTokenClass.String, token.Class);
            Assert.AreEqual("hello", token.Text);
        }

        [ Test ]
        public void StringTokenNeverNull()
        {
            Assert.IsNotNull(JsonToken.String(null).Text);
        }

        [ Test ]
        public void NumberToken()
        {
            JsonToken token = JsonToken.Number("123");
            Assert.AreEqual(JsonTokenClass.Number, token.Class);
            Assert.AreEqual("123", token.Text);
        }

        [ Test, ExpectedException(typeof(ArgumentNullException)) ]
        public void NumberTokenTextCannotBeNull()
        {
            JsonToken.Number(null);
        }

        [ Test, ExpectedException(typeof(ArgumentException)) ]
        public void NumberTokenTextCannotBeZeroLength()
        {
            JsonToken.Number("");
        }

        [ Test ]
        public void BooleanToken()
        {
            JsonToken token = JsonToken.Boolean(true);
            Assert.AreEqual(JsonTokenClass.Boolean, token.Class);
            Assert.AreEqual("true", token.Text);

            token = JsonToken.Boolean(false);
            Assert.AreEqual(JsonTokenClass.Boolean, token.Class);
            Assert.AreEqual("false", token.Text);
        }

        [ Test ]
        public void ObjectTokens()
        {
            Assert.AreEqual(JsonTokenClass.Object, JsonToken.Object().Class);
            Assert.AreEqual(JsonTokenClass.EndObject, JsonToken.EndObject().Class);
        }

        [ Test ]
        public void MemberToken()
        {
            JsonToken token = JsonToken.Member("test");
            Assert.AreEqual(JsonTokenClass.Member, token.Class);
            Assert.AreEqual("test", token.Text);
        }

        [ Test ]
        public void ArrayTokens()
        {
            Assert.AreEqual(JsonTokenClass.Array, JsonToken.Array().Class);
            Assert.AreEqual(JsonTokenClass.EndArray, JsonToken.EndArray().Class);
        }
        
        [ Test ]
        public void BOFEOF()
        {
            Assert.AreEqual(JsonTokenClass.BOF, JsonToken.BOF().Class);
            Assert.AreEqual(JsonTokenClass.EOF, JsonToken.EOF().Class);
        }
        
        [ Test ]
        public void JsonTokenString()
        {
            Assert.AreEqual("String:hello", JsonToken.String("hello").ToString());
            Assert.AreEqual("Number:123", JsonToken.Number("123").ToString());
            Assert.AreEqual("Boolean:true", JsonToken.Boolean(true).ToString());
            Assert.AreEqual("Boolean:false", JsonToken.Boolean(false).ToString());
            Assert.AreEqual("Null:null", JsonToken.Null().ToString());
            Assert.AreEqual("Array", JsonToken.Array().ToString());
            Assert.AreEqual("EndArray", JsonToken.EndArray().ToString());
            Assert.AreEqual("Object", JsonToken.Object().ToString());
            Assert.AreEqual("EndObject", JsonToken.EndObject().ToString());
            Assert.AreEqual("Member:test", JsonToken.Member("test").ToString());
            Assert.AreEqual("BOF", JsonToken.BOF().ToString());
            Assert.AreEqual("EOF", JsonToken.EOF().ToString());
        }

        [ Test ]
        public void EqualityWhenSameClassAndText()
        {
            Assert.IsTrue(JsonToken.String("hello").Equals(JsonToken.String("hello")));
        }

        [Test]
        public void InEqualityWhenSameClassDifferentText()
        {
            Assert.IsFalse(JsonToken.String("hello").Equals(JsonToken.String("world")));
        }

        [Test]
        public void InEqualityWhenDifferentClassSameText()
        {
            Assert.IsFalse(JsonToken.String("123").Equals(JsonToken.Number("123")));
        }

        [Test]
        public void EqualityWithTextlessClass()
        {
            Assert.AreEqual(JsonToken.BOF(), JsonToken.BOF());
        }

        [Test]
        public void InEqualityWithNull()
        {
            Assert.IsFalse(JsonToken.Null().Equals(null));
        }

        [Test]
        public void InEqualityWithAnotherType()
        {
            Assert.IsFalse(JsonToken.Null().Equals(123));
        }

        [Test]
        public void HashCodeNonZero()
        {
            Assert.AreNotEqual(0, JsonToken.EOF().GetHashCode());
            Assert.AreNotEqual(0, JsonToken.String("string").GetHashCode());
        }
    }
}
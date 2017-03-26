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
    using System.Collections;
    using System.Globalization;
    using NUnit.Framework;

    #endregion

    [ TestFixture ]
    public class TestJsonNumber
    {
        [ Test ]
        public void DefaultConstructionEqualsZero()
        {
            Assert.AreEqual("0", (new JsonNumber()).ToString());
        }

        [ Test ]
        public void One()
        {
            Assert.AreEqual("1", Number("1").ToString());
        }

        [ Test ]
        public void Float()
        {
            Assert.AreEqual("1.2345", Number("1.2345").ToString());
        }

        [ Test ]
        public void NullMeansZero()
        {
            Assert.AreEqual("0", Number(null).ToString());
        }

        [ Test, ExpectedException(typeof(ArgumentException)) ]
        public void CannotInitWithBadNumber()
        {
            Number("one");
        }
        
        [ Test ]
        public void ToInt32()
        {
            Assert.AreEqual(123456789, Number("123456789").ToInt32());
        }
        
        [ Test ]
        public void ToInt16()
        {
            Assert.AreEqual(12345, Number("12345").ToInt16());
        }

        [ Test ]
        public void ToByte()
        {
            Assert.AreEqual(123, Number("123").ToByte());
        }

        [ Test ]
        public void ToChar()
        {
            Assert.AreEqual('\x2126', Number("8486").ToChar());
        }

        [ Test ]
        public void ToBoolean()
        {
            Assert.AreEqual(false, Number(null).ToBoolean());
            Assert.AreEqual(false, Number("0").ToBoolean());
            Assert.AreEqual(true, Number("1").ToBoolean());
            Assert.AreEqual(true, Number("-123").ToBoolean());
            Assert.AreEqual(true, Number("123").ToBoolean());
        }

        [ Test ]
        public void ToInt64()
        {
            Assert.AreEqual(123456789012345, Number("123456789012345").ToInt64());
        }

        [ Test ]
        public void ToSingle()
        {
            Assert.AreEqual(1.5f, Number("1.5").ToSingle());
        }

        [ Test ]
        public void ToDouble()
        {
            Assert.AreEqual(1.5, Number("1.5").ToDouble());
        }

        [ Test ]
        public void ToDecimal()
        {
            Assert.AreEqual(1.5m, Number("1.5").ToDecimal());
        }

        [ Test ]
        public void ToDateTime()
        {
            Assert.AreEqual(new DateTime(2006, 7, 17, 10, 56, 56), Number("1153133816").ToDateTime().ToUniversalTime());
        }

        #if NET_4_0

        [ Test ]
        public void ToBigInteger()
        {
            JsonNumber number = Number("784637716923335095224261902710254454442933591094742482943");
            Assert.AreEqual(System.Numerics.BigInteger.Pow(long.MaxValue, 3), number.ToBigInteger());
        }

        [Test, ExpectedException(typeof(FormatException)) ]
        public void CannotToBigIntegerOnFloat()
        {
            Number("1.5").ToBigInteger();
        }

        [ Test ]
        public void ConvertToBigInteger()
        {
            JsonNumber number = Number("784637716923335095224261902710254454442933591094742482943");
            Assert.AreEqual(System.Numerics.BigInteger.Pow(long.MaxValue, 3), Convert.ChangeType(number, typeof(System.Numerics.BigInteger)));
        }

        [Test, ExpectedException(typeof(FormatException)) ]
        public void CannotConvertToBigIntegerOnFloat()
        {
            Convert.ChangeType(Number("1.5"), typeof(System.Numerics.BigInteger));
        }
        
        #endif
        
        [ Test ]
        public void LogicalEquality()
        {
            Assert.IsFalse(Number(null).LogicallyEquals(null), "null");
            Assert.IsTrue(Number("123").LogicallyEquals(123), "integer");
            Assert.IsTrue(Number("123.5").LogicallyEquals(123.5m), "decimal");
        }
        
        [ Test ]
        public void TypeCodeIsObject()
        {
            IConvertible n = new JsonNumber();
            Assert.AreEqual(TypeCode.Object, n.GetTypeCode());
        }
        
        [ Test ]
        public void ConversionToDecimalUsingExponentialNotation()
        {
            //
            // Exercises bug #13407
            // http://developer.berlios.de/bugs/?func=detailbug&bug_id=13407&group_id=4638
            //

            Assert.AreEqual(7.25e-5m, Number("7.25e-005").ToDecimal());
        }

        [ Test ]
        public void NullEquality()
        {
            Assert.IsFalse(new JsonNumber("1234").Equals(null));
        }

        [ Test ]
        public void TypeEquality()
        {
            Assert.IsFalse(new JsonNumber("1234").Equals("1234"));
        }

        [Test]
        public void Equality()
        {
            Assert.IsTrue(new JsonNumber("1234").Equals(new JsonNumber("1234")));
        }

        [Test]
        public void ValidGrammar()
        {
            TestValidity
            (
                true,
                "0",
                "123",
                "4567",
                "123.0",
                "123.456",
                "0.0",
                "0.0123",
                "-123.456",
                "123.456e789",
                "123.456e+789",
                "123.456e-789",
                "123.456E789",
                "123.456E+789",
                "123.456E-789",
                // http://code.google.com/p/jayrock/issues/detail?id=17
                "1.79769313486232e+308",
                "23456789012E66",
                "23456789012e66",
                "23456789012E+66",
                "23456789012e+66",
                "23456789012E-66",
                "23456789012e-66"
            );
        }

        [Test]
        public void InvalidGrammar()
        {
            TestValidity
            (
                false,
                "",
                "0.",
                ".0",
                ".",
                "0123",
                "+123.456",
                "+123.456e789",
                "+123.456e+789",
                "+123.456e-789",
                "+123.456E789",
                "+123.456E+789",
                "+123.456E-789",
                "123.456a+789",
                "123.456a123",
                "123.456e",
                "123.456e+",
                "123.456e-",
                "  +123.456e+789",
                "+123.456e+789  ",
                "Infinity",
                "-Infinity"
            );
        }

        [Test]
        public void ValidGrammarAllowingLeadingWhite()
        {
            TestValidity
            (
                true, NumberStyles.AllowLeadingWhite, 
                "123.456e+789", 
                "\r\n\t 123.456e+789"
            );
        }

        [Test]
        public void InvalidGrammarAllowingLeadingWhite()
        {
            TestValidity(false, NumberStyles.AllowLeadingWhite, "+123.456e+789 \t\r\n");
        }

        [Test]
        public void ValidGrammarAllowingTrailingWhite()
        {
            TestValidity
            (
                true, NumberStyles.AllowTrailingWhite, 
                "123.456e+789", 
                "123.456e+789 \t\r\n"
            );
        }

        [Test]
        public void InvalidGrammarAllowingTrainingWhite()
        {
            TestValidity(false, NumberStyles.AllowTrailingWhite, "\r\n\t +123.456e+789");
        }

        [Test]
        public void ValidGrammarAllowingWhite()
        {
            TestValidity
            (
                true, 
                NumberStyles.AllowLeadingWhite | NumberStyles.AllowTrailingWhite, 
                "-123.456e+789", 
                "\r\n\t -123.456e+789",
                "-123.456e+789 \t\r\n"
            );
        }

        [Test, ExpectedException(typeof(ArgumentNullException))]
        public void CannotValidateGrammarOnNullInput()
        {
            JsonNumber.IsValid(null);
        }

        [Test]
        public void CannotValidateGrammarWithNonWhiteNumberStyles()
        {
            ArrayList list = new ArrayList(Enum.GetValues(typeof(NumberStyles)));
            list.Remove(NumberStyles.None);
            list.Remove(NumberStyles.AllowLeadingWhite);
            list.Remove(NumberStyles.AllowTrailingWhite);
            foreach (NumberStyles styles in list)
            {
                try
                {
                    JsonNumber.IsValid("123", styles);
                    Assert.Fail(styles.ToString());
                }
                catch (ArgumentException)
                {
                    continue;
                }
                catch (Exception e)
                {
                    Assert.Fail("Failed for {0}. {1}", styles, e.Message);
                }
            }
        }

        [Test]
        public void NumberTooLargeForDouble()
        {
            // http://code.google.com/p/jayrock/issues/detail?id=17
            Number("1.79769313486232e+308");
        }

        [Test,ExpectedException(typeof(ArgumentException))]
        public void CannotHaveWhiteSpace()
        {
            Number(" 123.456 ");
        }

        [Test,ExpectedException(typeof(ArgumentException))]
        public void CannotHaveWhiteSpaceAroundNumberTooLargeForDouble()
        {
            Number(" 1.79769313486232e+308 ");
        }

        private static void TestValidity(bool expected, params string[] inputs)
        {
            TestValidity(expected, NumberStyles.None, inputs);
        }

        private static void TestValidity(bool expected, NumberStyles styles, params string[] inputs)
        {
            foreach (string input in inputs)
                Assert.AreEqual(expected, JsonNumber.IsValid(input, styles), input + " (" + styles + ")");
        }

        private static JsonNumber Number(string s)
        {
            return new JsonNumber(s);
        }
    }
}
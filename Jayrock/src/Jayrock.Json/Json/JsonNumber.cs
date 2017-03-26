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
    using System.Diagnostics;
    using System.Globalization;
    using System.Text.RegularExpressions;
    using Jayrock.Json.Conversion;

    #endregion
    
    /// <summary> 
    /// Represents a JSON Number.  This class models a number as a string 
    /// and only converts to a native numerical representation when needed 
    /// and therefore told.  
    /// </summary>
    /// <remarks>
    /// This class cannot be used to compare two numbers or perform
    /// mathematical operations like addition and substraction without 
    /// first converting to an actual native numerical data type.
    /// Use <see cref="LogicallyEquals"/> to test for equality.
    /// </remarks>

    [ Serializable ]
    public struct JsonNumber : IConvertible
    {
        private readonly string _value;

        public JsonNumber(string value)
        {
            if (value != null && !IsValid(value)) 
                throw new ArgumentException("value");

            _value = value;
        }

        private string Value
        {
            get { return Mask.EmptyString(_value, "0"); }
        }
        
        public override int GetHashCode()
        {
            return Value.GetHashCode();
        }

        public override bool Equals(object obj)
        {
            return obj != null && (obj is JsonNumber) && Equals((JsonNumber) obj);
        }

        public bool Equals(JsonNumber other)
        {
            return Value.Equals(other.Value);
        }

        public override string ToString()
        {
            return Value;
        }
        
        public bool LogicallyEquals(object o)
        {
            if (o == null) 
                return false;
            
            return Convert.ChangeType(this, o.GetType(), CultureInfo.InvariantCulture).Equals(o);
        }

        //
        // IMPORTANT! The following ToXXX methods will throw 
        // OverflowException in case the JsonNumber instance contains a value
        // that is too big or small to be represented as the request type.
        //

        public bool ToBoolean()
        {
            return ToInt64() != 0;
        }

        public char ToChar()
        {
            return (char) Convert.ToUInt16(Value, CultureInfo.InvariantCulture);
        }

        public byte ToByte()
        {
            return Convert.ToByte(Value, CultureInfo.InvariantCulture);
        }

        public short ToInt16()
        {
            return Convert.ToInt16(Value, CultureInfo.InvariantCulture);
        }

        public int ToInt32()
        {
            return Convert.ToInt32(Value, CultureInfo.InvariantCulture);
        }

        public long ToInt64()
        {
            return Convert.ToInt64(Value, CultureInfo.InvariantCulture);
        }

        public float ToSingle()
        {
            return Convert.ToSingle(Value, CultureInfo.InvariantCulture);
        }

        public double ToDouble()
        {
            return Convert.ToDouble(Value, CultureInfo.InvariantCulture);
        }

        public decimal ToDecimal()
        {
            return decimal.Parse(Value, NumberStyles.Float, CultureInfo.InvariantCulture);
        }

        #if !NET_1_0 && !NET_1_1 && !NET_2_0

        public System.Numerics.BigInteger ToBigInteger()
        {
            return ToBigInteger(CultureInfo.InvariantCulture);
        }

        private System.Numerics.BigInteger ToBigInteger(IFormatProvider provider)
        {
            return System.Numerics.BigInteger.Parse(Value, NumberStyles.Integer, provider);
        }

        public static explicit operator System.Numerics.BigInteger(JsonNumber number)
        {
            return number.ToBigInteger();
        }

        #endif // !NET_1_0 && !NET_1_1 && !NET_2_0

        public DateTime ToDateTime()
        {
            return UnixTime.ToDateTime(ToInt64());
        }

        #region IConvertible implementation

        TypeCode IConvertible.GetTypeCode()
        {
            return TypeCode.Object;
        }

        bool IConvertible.ToBoolean(IFormatProvider provider)
        {
            return ToBoolean();
        }

        char IConvertible.ToChar(IFormatProvider provider)
        {
            return ToChar();
        }

        sbyte IConvertible.ToSByte(IFormatProvider provider)
        {
            return Convert.ToSByte(ToInt32());
        }

        byte IConvertible.ToByte(IFormatProvider provider)
        {
            return ToByte();
        }

        short IConvertible.ToInt16(IFormatProvider provider)
        {
            return ToInt16();
        }

        ushort IConvertible.ToUInt16(IFormatProvider provider)
        {
            return Convert.ToUInt16(Value, provider);
        }

        int IConvertible.ToInt32(IFormatProvider provider)
        {
            return ToInt32();
        }

        uint IConvertible.ToUInt32(IFormatProvider provider)
        {
            return Convert.ToUInt32(Value, provider);
        }

        long IConvertible.ToInt64(IFormatProvider provider)
        {
            return ToInt64();
        }

        ulong IConvertible.ToUInt64(IFormatProvider provider)
        {
            return Convert.ToUInt64(Value, provider);
        }

        float IConvertible.ToSingle(IFormatProvider provider)
        {
            return ToSingle();
        }

        double IConvertible.ToDouble(IFormatProvider provider)
        {
            return ToDouble();
        }

        decimal IConvertible.ToDecimal(IFormatProvider provider)
        {
            return ToDecimal();
        }

        DateTime IConvertible.ToDateTime(IFormatProvider provider)
        {
            return ToDateTime();
        }

        string IConvertible.ToString(IFormatProvider provider)
        {
            return ToString();
        }

        object IConvertible.ToType(Type conversionType, IFormatProvider provider)
        {
            #if !NET_1_0 && !NET_1_1 && !NET_2_0
            
            if (conversionType == typeof(System.Numerics.BigInteger))
                return ToBigInteger(provider);
            
            #endif // !NET_1_0 && !NET_1_1 && !NET_2_0

            return Convert.ChangeType(this, conversionType, provider);
        }

        #endregion

        //
        // Explicit conversion operators.
        //
        
        public static explicit operator byte(JsonNumber number)
        {
            return number.ToByte();
        }

        public static explicit operator short(JsonNumber number)
        {
            return number.ToInt16();
        }

        public static explicit operator int(JsonNumber number)
        {
            return number.ToInt32();
        }
        
        public static explicit operator long(JsonNumber number)
        {
            return number.ToInt64();
        }
        
        public static explicit operator float(JsonNumber number)
        {
            return number.ToSingle();
        }
        
        public static explicit operator double(JsonNumber number)
        {
            return number.ToDouble();
        }
        
        public static explicit operator decimal(JsonNumber number)
        {
            return number.ToDecimal();
        }
        
        public static explicit operator DateTime(JsonNumber number)
        {
            return number.ToDateTime();
        }

        /// <summary>
        /// Determines if given text is a valid number per JSON grammar
        /// described in RFC 4627.
        /// </summary>

        public static bool IsValid(string text)
        {
            return IsValid(text, NumberStyles.None);
        }

        /// <summary>
        /// Determines if given text is a valid number per JSON grammar
        /// described in RFC 4627. An additional parameter can be used
        /// to specify whether leading and/or trailing white space should
        /// be allowed or not.
        /// </summary>
        /// <remarks>
        /// If whitespace is allowed then any whitespace as per Unicode
        /// is permitted, which is a wider set than what, for example,
        /// <see cref="NumberStyles.AllowTrailingWhite"/> and
        /// <see cref="NumberStyles.AllowLeadingWhite"/> list in their
        /// documentation.
        /// </remarks>

        public static bool IsValid(string text, NumberStyles styles)
        {
            if (text == null) throw new ArgumentNullException("text");

            if (styles >= 0 && (int) styles < _grammars.Length)
                return _grammars[(int) styles].IsMatch(text);

            throw new ArgumentException(null, "styles");
        }

        // WARNING! There be dragons!
        // We assume that MS will never change the values assigned to 
        // the members of NumberStyles, at least not the following ones.

        private static readonly Regex[] _grammars = new Regex[]
        {
            Regex(false, false), // 0 = None
            Regex(true,  false), // 1 = AllowLeadingWhite
            Regex(false, true),  // 2 = AllowTrailingWhite
            Regex(true,  true),  // 3 = AllowLeadingWhite | AllowTrailingWhite
        };

        private static Regex Regex(bool lws, bool rws)
        {
            return new Regex(
                "^" 
                + (lws ? @"\s*" : null)
                /*
                        number = [ minus ] int [ frac ] [ exp ]
                        decimal-point = %x2E       ; .
                        digit1-9 = %x31-39         ; 1-9
                        e = %x65 / %x45            ; e E
                        exp = e [ minus / plus ] 1*DIGIT
                        frac = decimal-point 1*DIGIT
                        int = zero / ( digit1-9 *DIGIT )
                        minus = %x2D               ; -
                        plus = %x2B                ; +
                        zero = %x30                ; 0
                */
                + @"      -?                # [ minus ]
                                            # int
                        (  0                #   zero
                           | [1-9][0-9]* )  #   / ( digit1-9 *DIGIT )
                                            # [ frac ]
                        ( \.                #   decimal-point 
                          [0-9]+ )?         #   1*DIGIT
                                            # [ exp ]
                        ( [eE]              #   e
                          [+-]?             #   [ minus / plus ]
                          [0-9]+ )?         #   1*DIGIT
                  " // NOTE! DO NOT move the closing quote 
                    // Moving it to the line above change the pattern!
                + (rws ? @"\s*" : null)
                + "$", 
                RegexOptions.IgnorePatternWhitespace
                | RegexOptions.ExplicitCapture
                | RegexOptions.Compiled);
        }
    }
}
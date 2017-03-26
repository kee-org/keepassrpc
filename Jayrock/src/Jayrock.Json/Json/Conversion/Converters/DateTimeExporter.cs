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

namespace Jayrock.Json.Conversion.Converters
{
    #region Imports

    using System;
    using System.Data;
    using System.Diagnostics;
    using System.Globalization;

    #endregion

    /// <remarks>
    /// See <a href="http://www.w3.org/TR/NOTE-datetime">W3C note on date 
    /// and time formats</a>.
    /// </remarks>

    public class DateTimeExporter : ExporterBase
    {
        public DateTimeExporter() : 
            base(typeof(DateTime)) {}

        protected override void ExportValue(ExportContext context, object value, JsonWriter writer)
        {
            if (context == null) throw new ArgumentNullException("context");
            if (value == null) throw new ArgumentNullException("value");
            if (writer == null) throw new ArgumentNullException("writer");

            ExportTime((DateTime) value, writer);
        }

        protected virtual void ExportTime(DateTime time, JsonWriter writer)
        {
            if (writer == null) throw new ArgumentNullException("writer");

            writer.WriteString(FormatDateTime(time));
        }

        static char[] FormatDateTime(DateTime when)
        {
            // http://stackoverflow.com/questions/1176276/how-do-i-improve-the-performance-of-code-using-datetime-tostring/1176350#1176350

            char[] chars = new char["yyyy-MM-ddTHH:mm:ss.fffffffzzzzzz".Length];

            // Separators

            chars[4]  = chars[7]  = '-';
            chars[10] = 'T';
            chars[13] = chars[16] = chars[30] = ':';
            chars[19] = '.';

            // Date
            
            Digits4(chars, when.Year,  0);
            Digits2(chars, when.Month, 5);
            Digits2(chars, when.Day,   8);

            // Time

            Digits2(chars, when.Hour,   11);
            Digits2(chars, when.Minute, 14);
            Digits2(chars, when.Second, 17);
            Digits7(chars, (int) (when.Ticks % 10000000L), 20);

            // UTC offset

            TimeSpan offset = TimeZone.CurrentTimeZone.GetUtcOffset(when);
            chars[27] = offset.Ticks >= 0 ? '+' : '-';
            Digits2(chars, Math.Abs(offset.Hours),   28);
            Digits2(chars, offset.Minutes, 31);
            
            return chars;
        }

        static void Digits2(char[] buffer, int value, int offset)
        {
            buffer[offset++] = (char) ('0' + (value / 10));
            buffer[offset]   = (char) ('0' + (value % 10));
        }
        
        static void Digits3(char[] buffer, int value, int offset)
        {
            buffer[offset++] = (char) ('0' + (value / 100));
            Digits2(buffer, value % 100, offset);
        }
        
        static void Digits4(char[] buffer, int value, int offset)
        {
            buffer[offset++] = (char) ('0' + (value / 1000));
            Digits3(buffer, value % 1000, offset);
        }

        static void Digits7(char[] buffer, int value, int offset)
        {
            Digits4(buffer, value / 1000, offset);
            Digits3(buffer, value % 1000, offset + 4);
        }
    }
}
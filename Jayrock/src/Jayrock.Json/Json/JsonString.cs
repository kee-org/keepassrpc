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
    using System.IO;
    using System.Text;

    #endregion

    public sealed class JsonString
    {
        /// <summary>
        /// Produces a string in double quotes with backslash sequences in all
        /// the right places.
        /// </summary>
        /// <returns>A correctly formatted string for insertion in a JSON
        /// message.
        /// </returns>
        /// <remarks>
        /// Public Domain 2002 JSON.org, ported to C# by Are Bjolseth
        /// (teleplan.no) and nearly re-written by Atif Aziz (www.raboof.com)
        /// </remarks>
    
        public static string Enquote(string str)
        {
            if (str == null || str.Length == 0)
                return "\"\"";

            return Enquote(str, null).ToString();
        }
        
        public static StringBuilder Enquote(string str, StringBuilder sb)
        {
            return EnquoteStringOrChars(str, null, 0, Mask.NullString(str).Length, sb);
        }

        public static string Enquote(char[] chars, int offset, int length)
        {
            return Enquote(chars, offset, length, null).ToString();
        }

        public static StringBuilder Enquote(char[] chars, int offset, int length, StringBuilder sb)
        {
            if (chars == null) throw new ArgumentNullException("chars");
            return EnquoteStringOrChars(null, chars, offset, length, sb);
        }

        private static StringBuilder EnquoteStringOrChars(string str, char[] chars, int offset, int length, StringBuilder sb)
        {
            if (chars != null)
            {
                if (offset < 0) throw new ArgumentOutOfRangeException("offset", offset, null);
                if (length < 0) throw new ArgumentOutOfRangeException("length", offset, null);
            }

            if (sb == null)
                sb = new StringBuilder(length + 4);
            
            sb.Append('"');
            
            char last;
            char ch = '\0';

            int end = offset + length;
            if (chars != null && end > chars.Length)
                throw new ArgumentOutOfRangeException();

            for (int index = offset; index < end; index++)
            {
                last = ch;
                ch = chars != null ? chars[index] : str[index];
                Enquote(sb, last, ch);
            }

            return sb.Append('"');
        }

        private static void Enquote(StringBuilder sb, char last, char ch)
        {
            switch (ch)
            {
                case '\\':
                case '"':
                {
                    sb.Append('\\');
                    sb.Append(ch);
                    break;
                }
                        
                case '/':
                {
                    if (last == '<')
                        sb.Append('\\');
                    sb.Append(ch);
                    break;
                }
                    
                case '\b': sb.Append("\\b"); break;
                case '\t': sb.Append("\\t"); break;
                case '\n': sb.Append("\\n"); break;
                case '\f': sb.Append("\\f"); break;
                case '\r': sb.Append("\\r"); break;
                    
                default:
                {
                    if (ch < ' ')
                    {
                        sb.Append("\\u");
                        sb.Append(((int) ch).ToString("x4", CultureInfo.InvariantCulture));
                    }
                    else
                    {
                        sb.Append(ch);
                    }
                    
                    break;
                }
            }
        }

        /// <summary>
        /// Return the characters up to the next close quote character.
        /// Backslash processing is done. The formal JSON format does not
        /// allow strings in single quotes, but an implementation is allowed to
        /// accept them.
        /// </summary>
        /// <param name="quote">The quoting character, either " or '</param>
        /// <returns>A String.</returns>
        
        // TODO: Consider rendering Dequote public
        
        internal static string Dequote(BufferedCharReader input, char quote)
        {
            return Dequote(input, quote, null).ToString();
        }

        internal static StringBuilder Dequote(BufferedCharReader input, char quote, StringBuilder output)
        {
            Debug.Assert(input != null);

            if (output == null)
                output = new StringBuilder();
            
            char[] hexDigits = null;
            
            while (true)
            {
                char ch = input.Next();

                if ((ch == BufferedCharReader.EOF) || (ch == '\n') || (ch == '\r')) 
                    throw new FormatException("Unterminated string.");

                if (ch == '\\')
                {
                    ch = input.Next();

                    switch (ch)
                    {
                        case 'b': output.Append('\b'); break; // Backspace
                        case 't': output.Append('\t'); break; // Horizontal tab
                        case 'n': output.Append('\n'); break; // Newline
                        case 'f': output.Append('\f'); break; // Form feed
                        case 'r': output.Append('\r'); break; // Carriage return 
                            
                        case 'u':
                        {
                            if (hexDigits == null)
                                hexDigits = new char[4];
                            
                            output.Append(ParseHex(input, hexDigits)); 
                            break;
                        }
                            
                        default:
                            output.Append(ch);
                            break;
                    }
                }
                else
                {
                    if (ch == quote)
                        return output;

                    output.Append(ch);
                }
            }
        }
        
        /// <summary>
        /// Eats the next four characters, assuming hex digits, and converts
        /// into the represented character value.
        /// </summary>
        /// <returns>The parsed character.</returns>

        private static char ParseHex(BufferedCharReader input, char[] hexDigits) 
        {
            Debug.Assert(input != null);
            Debug.Assert(hexDigits != null);
            Debug.Assert(hexDigits.Length == 4);
            
            hexDigits[0] = input.Next();
            hexDigits[1] = input.Next();
            hexDigits[2] = input.Next();
            hexDigits[3] = input.Next();
            
            return (char) ushort.Parse(new string(hexDigits), NumberStyles.HexNumber);
        }

        private JsonString()
        {
            throw new NotSupportedException();
        }
    }
}

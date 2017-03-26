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
    using System.IO;

    #endregion

    /// <summary>
    /// This type supports the Jayrock JSON infrastructure and is not 
    /// intended to be used directly from your code. 
    /// Beware! There be dragons!
    /// </summary>    
    /// <remarks>
    /// This type may become inaccessible in the future. It is public
    /// for the purpose of unit testing.
    /// </remarks>

    // TODO: Make internal and use InternalsVisibleToAttribute when code base has been upgraded to .NET 2.0.

    public sealed class BufferedCharReader
    {
        private readonly TextReader _reader;
        private readonly int _bufferSize;
        private char[] _buffer;
        private int _index;
        private int _end;
        private bool _backed;
        private char _backup;
        private int _charCount;
        private int _lineNumber;
        private int _linePosition;
        private int _lastLinePosition;
        private bool _sawLineFeed = true;

        public const char EOF = (char) 0;

        public BufferedCharReader(TextReader reader) :
            this(reader, 0) {}
        
        public BufferedCharReader(TextReader reader, int bufferSize)
        {
            Debug.Assert(reader != null);
            
            _reader = reader;
            _bufferSize = Math.Max(256, bufferSize);
        }

        public int CharCount { get { return _charCount; } }
        public int LineNumber { get { return _lineNumber; } }
        public int LinePosition { get { return _linePosition; } }

        /// <summary>
        /// Back up one character. This provides a sort of lookahead capability,
        /// so that one can test for a digit or letter before attempting to,
        /// for example, parse the next number or identifier.
        /// </summary>
        /// <remarks>
        /// This implementation currently does not support backing up more
        /// than a single character (the last read).
        /// </remarks>
        
        public void Back()
        {
            Debug.Assert(!_backed);

            if (_charCount == 0) 
                return;

            _backed = true;
            
            _charCount--;
            _linePosition--;
            
            if (_linePosition == 0)
            {
                _lineNumber--;
                _linePosition = _lastLinePosition;
                _sawLineFeed = true;
            }
        }

        /// <summary>
        /// Determine if the source string still contains characters that Next()
        /// can consume.
        /// </summary>
        /// <returns>true if not yet at the end of the source.</returns>
        
        public bool More()
        {
            if (!_backed && _index == _end)
            {
                if (_buffer == null)
                    _buffer = new char[_bufferSize];
                
                _index = 0;
                _end = _reader.Read(_buffer, 0, _buffer.Length);
                
                if (_end == 0)
                    return false;
            }

            return true;
        }

        /// <summary>
        /// Get the next character in the source string.
        /// </summary>
        /// <returns>The next character, or 0 if past the end of the source string.</returns>
        
        public char Next()
        {
            char ch;

            if (_backed)
            {
                _backed = false;
                ch = _backup;
            }
            else
            {
                if (!More())
                    return EOF;

                ch = _buffer[_index++];
                _backup = ch;
            }

            return UpdateCounters(ch);
        }

        private char UpdateCounters(char ch) 
        {
            _charCount++;

            if (_sawLineFeed)
            {
                _lineNumber++;
                _lastLinePosition = _linePosition;
                _linePosition = 1;
                _sawLineFeed = false;
            }
            else
            {
                _linePosition++;
            }

            _sawLineFeed = ch == '\n';
            return ch;
        }
    }
}

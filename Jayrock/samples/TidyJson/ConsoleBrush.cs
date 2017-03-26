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

    #endregion

    [ Serializable ]
    internal struct ConsoleBrush
    {
        private readonly ConsoleColor foreground;
        private readonly ConsoleColor background;

        public ConsoleBrush(ConsoleColor color) :
            this(color, color) {}

        public ConsoleBrush(ConsoleColor foreground, ConsoleColor background)
        {
            this.foreground = foreground;
            this.background = background;
        }

        public ConsoleColor Foreground
        {
            get { return foreground; }
        }

        public ConsoleColor Background
        {
            get { return background; }
        }

        public ConsoleBrush ResetForeground(ConsoleColor value)
        {
            return new ConsoleBrush(value, Background);
        }

        public ConsoleBrush ResetBackground(ConsoleColor value)
        {
            return new ConsoleBrush(Foreground, value);
        }

        public void Apply()
        {
            Console.BackgroundColor = Background;
            Console.ForegroundColor = Foreground;
        }

        public static ConsoleBrush Current
        {
            get { return new ConsoleBrush(Console.ForegroundColor, Console.BackgroundColor); }
        }

        public override string ToString()
        {
            return Foreground + " on " + Background;
        }
    }
}
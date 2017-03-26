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
    public class TestJsonReader
    {
        private bool _disposed;
        
        [ SetUp ]
        public void Init()
        {
            _disposed = false;
        }
        
        [ Test ]
        public void ClosingRaisesDisposed()
        {
            JsonReader reader = new StubJsonReader();   
            reader.Disposed += new EventHandler(Reader_Disposed);
            Assert.IsFalse(_disposed);
            reader.Close();
            Assert.IsTrue(_disposed);
        }

        [ Test ]
        public void CloseWithoutDisposedHandlerHarmless()
        {
            JsonReader reader = new StubJsonReader();   
            reader.Close();
        }

        private void Reader_Disposed(object sender, EventArgs e)
        {
            _disposed = true;
        }

        private sealed class StubJsonReader : JsonReader 
        {
            public override bool Read()
            {
                throw new NotImplementedException();
            }

            public override JsonToken Token
            {
                get { throw new NotImplementedException(); }
            }

            public override int Depth
            {
                get { throw new NotImplementedException(); }
            }

            public override int MaxDepth
            {
                get { throw new NotImplementedException(); }
                set { throw new NotImplementedException(); }
            }
        }
    }
}
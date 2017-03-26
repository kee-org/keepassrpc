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
    using System.IO;
    using NUnit.Framework;

    #endregion

    [ TestFixture ]
    public class TestJsonRecorder
    {
        [ Test ]
        public void Blank()
        {
            JsonRecorder recorder = new JsonRecorder();
            JsonReader reader = recorder.CreatePlayer();
            Assert.AreEqual(JsonTokenClass.BOF, reader.TokenClass);
            Assert.IsFalse(reader.Read());
            Assert.IsTrue(reader.EOF);
        }

        [ Test ]
        public void String()
        {
            JsonRecorder recorder = new JsonRecorder();
            recorder.WriteString("Hello World");
            JsonTextWriter writer = new JsonTextWriter();
            recorder.Playback(writer);
            Assert.AreEqual("[\"Hello World\"]", writer.ToString());
        }
        
        [ Test ]
        public void Complex()
        {
            const string input = @"{'menu': {
                'header': 'SVG Viewer',
                'items': [
                    {'id': 'Open'},
                    {'id': 'OpenNew', 'label': 'Open New'},
                    null,
                    {'id': 'ZoomIn', 'label': 'Zoom In'},
                    {'id': 'ZoomOut', 'label': 'Zoom Out'},
                    {'id': 'OriginalView', 'label': 'Original View'},
                    null,
                    {'id': 'Quality'},
                    {'id': 'Pause'},
                    {'id': 'Mute'},
                    null,
                    {'id': 'Find', 'label': 'Find...'},
                    {'id': 'FindAgain', 'label': 'Find Again'},
                    {'id': 'Copy'},
                    {'id': 'CopyAgain', 'label': 'Copy Again'},
                    {'id': 'CopySVG', 'label': 'Copy SVG'},
                    {'id': 'ViewSVG', 'label': 'View SVG'},
                    {'id': 'ViewSource', 'label': 'View Source'},
                    {'id': 'SaveAs', 'label': 'Save As'},
                    null,
                    {'id': 'Help'},
                    {'id': 'About', 'label': 'About Adobe CVG Viewer...'}
                ]
            }}";

            JsonTextReader reader = new JsonTextReader(new StringReader(input));
            JsonTextWriter writer = new JsonTextWriter();
            JsonRecorder.Record(reader).Playback(writer);
            Assert.AreEqual("{\"menu\":{\"header\":\"SVG Viewer\",\"items\":[{\"id\":\"Open\"},{\"id\":\"OpenNew\",\"label\":\"Open New\"},null,{\"id\":\"ZoomIn\",\"label\":\"Zoom In\"},{\"id\":\"ZoomOut\",\"label\":\"Zoom Out\"},{\"id\":\"OriginalView\",\"label\":\"Original View\"},null,{\"id\":\"Quality\"},{\"id\":\"Pause\"},{\"id\":\"Mute\"},null,{\"id\":\"Find\",\"label\":\"Find...\"},{\"id\":\"FindAgain\",\"label\":\"Find Again\"},{\"id\":\"Copy\"},{\"id\":\"CopyAgain\",\"label\":\"Copy Again\"},{\"id\":\"CopySVG\",\"label\":\"Copy SVG\"},{\"id\":\"ViewSVG\",\"label\":\"View SVG\"},{\"id\":\"ViewSource\",\"label\":\"View Source\"},{\"id\":\"SaveAs\",\"label\":\"Save As\"},null,{\"id\":\"Help\"},{\"id\":\"About\",\"label\":\"About Adobe CVG Viewer...\"}]}}", writer.ToString());
        }

        [ Test, ExpectedException(typeof(InvalidOperationException)) ]
        public void CannotReadPartialRecording()
        {
            JsonRecorder recorder = new JsonRecorder();
            recorder.WriteStartArray();
            recorder.CreatePlayer();
        }
    }
}

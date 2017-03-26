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

namespace Jayrock.Json.Conversion
{
    #region Imports

    using System;
    using NUnit.Framework;

    #endregion

    [ TestFixture ]
    public class TestJsonConvert
    {
        private bool _createImportContextCalled;
        private bool _createExportContextCalled;

        [ SetUp ]
        public void Init()
        {
            _createImportContextCalled = false;
            _createExportContextCalled = false;
        }

        [Test]
        public void DefaultExportContextFactory()
        {
            Assert.IsNotNull(JsonConvert.DefaultExportContextFactory);
        }

        [ Test ]
        public void DefaultImportContextFactory()
        {
            Assert.IsNotNull(JsonConvert.DefaultImportContextFactory);
        }

        [ Test ]
        public void SetCurrentExportContextFactory()
        {
            try
            {
                ExportContextFactoryHandler factory = new ExportContextFactoryHandler(CreateExportContext);
                JsonConvert.CurrentExportContextFactory = factory;
                Assert.AreSame(factory, JsonConvert.CurrentExportContextFactory);
                JsonConvert.CreateExportContext();
                Assert.IsTrue(_createExportContextCalled);
            }
            finally
            {
                JsonConvert.CurrentExportContextFactory = JsonConvert.DefaultExportContextFactory;
            }
        }

        [ Test ]
        [ ExpectedException(typeof(ArgumentNullException)) ]
        public void CannotSetCurrentExportContextFactoryToNull()
        {
            JsonConvert.CurrentExportContextFactory = null;
        }

        [ Test ]
        public void SetCurrentImportContextFactory()
        {
            try
            {
                ImportContextFactoryHandler factory = new ImportContextFactoryHandler(CreateImportContext);
                JsonConvert.CurrentImportContextFactory = factory;
                Assert.AreSame(factory, JsonConvert.CurrentImportContextFactory);
                JsonConvert.CreateImportContext();
                Assert.IsTrue(_createImportContextCalled);
            }
            finally
            {
                JsonConvert.CurrentImportContextFactory = JsonConvert.DefaultImportContextFactory;                
            }
        }

        [ Test ]
        [ ExpectedException(typeof(ArgumentNullException)) ]
        public void CannotSetCurrentImportContextFactoryToNull()
        {
            JsonConvert.CurrentImportContextFactory = null;
        }

        private ExportContext CreateExportContext()
        {
            _createExportContextCalled = true;
            return null;
        }

        private ImportContext CreateImportContext()
        {
            _createImportContextCalled = true;
            return null;
        }
    }
}
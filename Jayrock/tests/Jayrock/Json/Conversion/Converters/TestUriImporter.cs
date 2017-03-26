namespace Jayrock.Json.Conversion.Converters
{
    #region Imports

    using System;
    using System.IO;
    using NUnit.Framework;

    #endregion

    [ TestFixture ]
    public class TestUriImporter
    {
        [ Test ]
        public void ImportString()
        {
            AssertImport(new Uri("http://jayrock.berlios.de/"), "\"http://jayrock.berlios.de/\"");
        }

        [ Test ]
        public void ImportNull()
        {
            AssertImport(null, "null");
        }

        [ Test, ExpectedException(typeof(JsonException)) ]
        public void CannotImportNumber()
        {
            AssertImport(null, "123");
        }

        [ Test, ExpectedException(typeof(JsonException)) ]
        public void CannotImportTrue()
        {
            AssertImport(null, "true");
        }

        [ Test, ExpectedException(typeof(JsonException)) ]
        public void CannotImportFalse()
        {
            AssertImport(null, "false");
        }

        [ Test, ExpectedException(typeof(JsonException)) ]
        public void CannotImportArray()
        {
            AssertImport(null, "[]");
        }
        
        [ Test, ExpectedException(typeof(JsonException)) ]
        public void CannotImportObject()
        {
            AssertImport(null, "{}");
        }
        
        private static void AssertImport(Uri expected, string input)
        {
            JsonTextReader reader = new JsonTextReader(new StringReader(input));
            UriImporter importer = new UriImporter();
            object o = importer.Import(new ImportContext(), reader);
            Assert.IsTrue(reader.EOF, "Reader must be at EOF.");
            if (expected != null)
                Assert.IsInstanceOfType(typeof(Uri), o);
            Assert.AreEqual(expected, o);
        }
    }
}
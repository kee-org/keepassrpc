namespace JsonImportDemo
{
    #region Imports

    using System;
    using System.Collections;
    using Jayrock.Json;
    using Jayrock.Json.Conversion;

    #endregion

    /// <summary>
    /// This project demonstrates using JsonConvert to unpack JSON text into 
    /// a given set of application and general types.
    /// </summary>

    internal static class Program
    {
        private static void Main()
        {
            string text = @"
                {
                    ""header"": ""SVG Viewer"",
                    ""items"": [
                        {""id"": ""Open""},
                        {""id"": ""OpenNew"", ""label"": ""Open New""},
                        null,
                        {""id"": ""ZoomIn"", ""label"": ""Zoom In""},
                        {""id"": ""ZoomOut"", ""label"": ""Zoom Out""},
                        {""id"": ""OriginalView"", ""label"": ""Original View""},
                        null,
                        {""id"": ""Quality""},
                        {""id"": ""Pause""},
                        {""id"": ""Mute""},
                        null,
                        {""id"": ""Find"", ""label"": ""Find...""},
                        {""id"": ""FindAgain"", ""label"": ""Find Again""},
                        {""id"": ""Copy""},
                        {""id"": ""CopyAgain"", ""label"": ""Copy Again""},
                        {""id"": ""CopySVG"", ""label"": ""Copy SVG""},
                        {""id"": ""ViewSVG"", ""label"": ""View SVG""},
                        {""id"": ""ViewSource"", ""label"": ""View Source""},
                        {""id"": ""SaveAs"", ""label"": ""Save As""},
                        null,
                        {""id"": ""Help""},
                        {""id"": ""About"", ""label"": ""About Adobe CVG Viewer...""}
                    ]
                }";

            //
            // The above menu definition in JSON text is imported using
            // two approaches in the demonstrations. The first method 
            // unpacks the data into application-supplied types. The 
            // second, unpacks into general types supplied by Jayrock.
            //

            ImportByTypeDemo(text);
            Console.WriteLine();
            AutoImportDemo(text);
        }

        private static void ImportByTypeDemo(string text) 
        {
            Menu menu = (Menu) JsonConvert.Import(typeof(Menu), text);

            string separator = new string('-', 40);

            Console.WriteLine(menu.Header);
            Console.WriteLine(separator);
            
            foreach (MenuItem item in menu.Items)
            {
                Console.Write('\t');

                if (item != null)
                    Console.WriteLine("{0} ({1})", item.Label ?? item.Id, item.Id);
                else
                    Console.WriteLine(separator);
            }
        }

        //
        // Disable warning CS0649:
        // Field '...' is never assigned to, and will always have its default value null
        //

        #pragma warning disable 649

        public class Menu
        {
            public string Header;
            public MenuItem[] Items;
        }

        public class MenuItem
        {
            public string Id;
            public string Label;
        }

        private static void AutoImportDemo(string text)
        {
            //
            // Without any type specification, Import will unpack
            // a JSON object as JsonObject, array as JsonArray,
            // and string as System.String, number as JsonNumber,
            // Boolean as System.Boolean and null simply as a 
            // local null reference.
            //

            JsonObject menu = (JsonObject) JsonConvert.Import(text);

            string separator = new string('-', 40);

            Console.WriteLine(menu["header"]);
            Console.WriteLine(separator);

            JsonArray items = (JsonArray) menu["items"];

            foreach (JsonObject item in items)
            {
                Console.Write('\t');

                if (item != null)
                {
                    string id = (string) item["id"];
                    string label = (string) item["label"];
                    Console.WriteLine("{0} ({1})", label ?? id, id);
                }
                else
                {
                    Console.WriteLine(separator);
                }
            }
        }
    }
}

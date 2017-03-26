namespace JsonConversionsDemo
{
    #region Imports

    using System;
    using System.Collections.Generic;
    using System.ComponentModel;
    using System.Diagnostics;
    using System.IO;
    using System.Net;
    using Jayrock.Json;
    using Jayrock.Json.Conversion;

    #endregion

    public class Point
    {
        public int X, Y;
    }

    public class Shape
    {
        public string Name;
        public List<Point> Points;
    }

    internal static class Program
    {
        private static void Run()
        {
            ImportContext impctx = new ImportContext();

            //
            // Import a strongly-typed collection of integers...
            //

            impctx.Register(new ListImporter<int>());

            List<int> numbers = (List<int>) impctx.Import(typeof(List<int>), 
                JsonText.CreateReader("[ 1, 2, 3 ]"));
            numbers.ForEach(Console.WriteLine);
            Console.WriteLine();

            //
            // Import a Shape object containing a strongly-typed collection of 
            // Point objects.
            //

            impctx.Register(new ListImporter<Point>());

            Shape shape = (Shape) impctx.Import(typeof(Shape), JsonText.CreateReader(@"{ 
                    name: 'square', 
                    points: [
                        { x: 10, y: 10 },
                        { x: 20, y: 10 }, 
                        { x: 20, y: 20 },
                        { x: 10, y: 20 }
                    ]
                }"));
            JsonConvert.Export(shape, CreatePrettyWriter(Console.Out));
            Console.WriteLine();

            //
            // Import CookieCollection using duck-typing. In other words,
            // as long as CookieCollection walks and quacks like a
            // collection of Cookie elements then it's good enough for
            // DuckCollectionImporter. DuckCollectionImporter can infer
            // that CookieCollection contains Cookie elements.
            //

            impctx.Register(new DuckCollectionImporter(typeof(CookieCollection)));

            const string cookiesText = @"[
                    { name: 'one',   value: 1, expires: '2099-01-02' },
                    { name: 'two',   value: 2, expires: '2088-03-04' },
                    { name: 'three', value: 3, expires: '2077-05-06' }
                ]";
            
            CookieCollection cookies = (CookieCollection) impctx.Import(typeof(CookieCollection), JsonText.CreateReader(cookiesText));
            JsonConvert.Export(cookies, CreatePrettyWriter(Console.Out));
            Console.WriteLine();

            //
            // Now repeat, but replace with a new CookieCollection importer
            // that is identical in behavior but based on generics. Here,
            // the difference is that DuckCollectionImporter<,> does not
            // have to guess the element type as it is provided as a type
            // argument.
            //

            impctx.Register(new DuckCollectionImporter<CookieCollection, Cookie>());
            cookies = (CookieCollection)impctx.Import(typeof(CookieCollection), JsonText.CreateReader(cookiesText));
            JsonConvert.Export(cookies, CreatePrettyWriter(Console.Out));
            Console.WriteLine();

            //
            // Those Cookie objects have a lot of properties. Say our JSON
            // text only needs a subset. Here, we register an exporter that 
            // provides a custom view of the type. We only expose the name, 
            // value and expiration time. What's more, we rename the 
            // Expires property so that it appears as "expiration" in JSON.
            //

            ExportContext expctx = new ExportContext();

            JsonType.
                BuildFor(typeof(Cookie)).
                AddProperty("Name").
                AddProperty("Value").
                AddProperty("Expires").As("expiration").
                Register(expctx);

            expctx.Export(cookies, CreatePrettyWriter(Console.Out));
            Console.WriteLine();
        }

        private static void Main()
        {
            try
            {
                Run();
            }
            catch (Exception e)
            {
                Console.Error.WriteLine(e.GetBaseException().Message);
                Trace.WriteLine(e.ToString());
            }
        }

        private static JsonWriter CreatePrettyWriter(TextWriter writer)
        {
            JsonTextWriter jsonw = new JsonTextWriter(writer);
            jsonw.PrettyPrint = true;
            return jsonw;
        }
    }
}

namespace YahooJsonNewsReader
{
    #region Imports

    using System;
    using System.IO;
    using System.Net;
    using Jayrock.Json;

    #endregion

    /// <summary>
    /// This program demonstrates using JsonTextReader to parse JSON text in 
    /// a forward-only, stream-oriented fashion.
    /// </summary>

    internal static class Program
    {
        private static void Main(string[] args)
        {
            const int keywordsLimit = 70;
            
            Console.WriteLine("Yahoo (JSON) News Reader");
            Console.WriteLine();
            Console.WriteLine("When prompted, enter keywords (e.g. Ray Charles) to search");
            Console.WriteLine("news for. Program ends if an empty keyword list is supplied.");

            WebClient client = new WebClient();
            string urlFormat = args.Length > 0 ? args[0].Trim() : string.Empty;

            if (string.IsNullOrEmpty(urlFormat))
            {
                urlFormat = "http://search.yahooapis.com/NewsSearchService/V1/newsSearch?" +
                    "appid=YahooDemo&query={0}&results=10&language=en&output=json";
            }

            while (true)
            {
                Console.WriteLine();
                Console.Write("Keywords: ");

                string keywords = Console.ReadLine().Trim();

                if (keywords.Length == 0)
                    return;

                if (keywords.Length > keywordsLimit)
                {
                    Console.WriteLine("Keywords exceed {0:N0} chars! Try again.", keywordsLimit);
                    continue;
                }

                Console.WriteLine();
                Console.Write("Downloading news...");

                string escapedKeywords = Uri.EscapeUriString(keywords);
                Uri url = new Uri(string.Format(urlFormat, escapedKeywords));
                string news = client.DownloadString(url);

                Console.WriteLine("Done!");
                Console.WriteLine();

                //
                // Read through the entire JSON response and display the
                // string value of object member named "Title". The
                // shape of the news data looks something like this:
                //
                //  { 
                //      "ResultSet" : {
                //          "totalResultsAvailable" : ...,
                //          "totalResultsReturned" : ...,
                //          "firstResultPosition": ...,
                //          "Result" : [ 
                //             {
                //                  "Title" : "..."
                //                 ...
                //              },
                //              ...
                //          ]
                //      }
                //  }
                //

                using (JsonTextReader reader = new JsonTextReader(new StringReader(news)))
                {
                    while (reader.Read())
                    {
                        if (reader.Depth == 4 &&
                            reader.TokenClass == JsonTokenClass.Member && 
                            reader.Text == "Title")
                        {
                            reader.Read(/* member */);
                            Console.WriteLine(reader.ReadString());
                        }
                    }
                }
            }
        }
    }
}

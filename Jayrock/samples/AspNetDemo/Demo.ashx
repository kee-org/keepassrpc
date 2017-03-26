<%@ WebHandler Class="JayrockWeb.DemoService" Language="C#" %>

namespace JayrockWeb
{
    #region Imports

    using System;
    using System.Data;
    using System.Collections;
    using System.Collections.Specialized;
    using System.Web;
    using System.Web.SessionState;
    using System.Web.UI;
    using System.Web.UI.WebControls;
    using Jayrock.JsonRpc;
    using Jayrock.JsonRpc.Web;

    #endregion

    [ JsonRpcHelp("This is a JSON-RPC service that demonstrates the basic features of the Jayrock library.") ]    
    public class DemoService : JsonRpcHandler, IRequiresSessionState 
    {
        [ JsonRpcMethod("echo", Idempotent = true)]
        [ JsonRpcHelp("Echoes back the text sent as input.") ]
        public string Echo(string text)
        {
            return text;
        }

        [ JsonRpcMethod("echoObject", Idempotent = true)]
        [ JsonRpcHelp("Echoes back the object sent as input.") ]
        public object EchoOject(object o)
        {
            return o;
        }

        [ JsonRpcMethod("echoArgs", Idempotent = true)]
        [ JsonRpcHelp("Echoes back the arguments sent as input. This method demonstrates variable number of arguments.") ]
        public object EchoArgs(params object[] args)
        {
            return args;
        }

        [ JsonRpcMethod("echoAsStrings", Idempotent = true)]
        [ JsonRpcHelp("Echoes back the arguments as an array of strings. This method demonstrates working with variable number of arguments.") ]
        public object EchoAsStrings(params object[] args)
        {
            string[] strings = new string[args.Length];

            for (int i = 0; i < args.Length; i++)
            {
                if (args[i] != null)
                    strings[i] = args[i].ToString();
            }
                
            return strings;
        }

        [ JsonRpcMethod("echoGuid", Idempotent = true)]
        [ JsonRpcHelp("Echoes back the given GUID. This method demonstrates working with an argument typed as System.Guid.") ]
        public Guid EchoGuid(Guid id)
        {
            return id;
        }

        [ JsonRpcMethod("add", Idempotent = true)]
        [ JsonRpcHelp("Return the sum of two integers.") ]
        public int Add(int a, int b)
        {
            return a + b;
        }
        
        [ JsonRpcMethod("getStringArray", Idempotent = true)]
        [ JsonRpcHelp("Returns an array of city names. Demonstrates returning a strongly-typed array.") ]
        public string[] GetCities()
        {
            return new string[] { "London", "Zurich", "Paris", "New York" };
        }
        
        [ JsonRpcMethod("now", Idempotent = true)]
        [ JsonRpcHelp("Returns the local time on the server. Demonstrates how DateTime is returned simply as a string using the ISO 8601 format.") ]
        public DateTime Now()
        {
            return DateTime.Now;
        }

        [ JsonRpcMethod("newGuid", Idempotent = true)]
        [ JsonRpcHelp("Generates and returns a GUID as a string.") ]
        public Guid NewGuid()
        {
            return Guid.NewGuid();
        }

        [ JsonRpcMethod("cookies", Idempotent = true)]
        [ JsonRpcHelp("Returns the cookie names seen by the server.") ]
        public HttpCookieCollection Cookies()
        {
            return Request.Cookies;
        }
        
        [ JsonRpcMethod("serverVariables", Idempotent = true)]
        [ JsonRpcHelp("Returns the server variables collection at the server. Demonstrates returning NameValueCollection.") ]
        public NameValueCollection ServerVariables()
        {
            return Request.ServerVariables;
        }

        [ JsonRpcMethod("getAuthor", Idempotent = true)]
        [ JsonRpcHelp("Returns information about the author. Demonstrates how a Hashtable from the server is automatically converted into an object on the client-side.") ]
        public IDictionary GetAuthor()
        {
            Hashtable author = new Hashtable();
            author["FirstName"] = "Atif";
            author["LastName"] = "Aziz";
            return author;
        }

        [ JsonRpcMethod("getCouple", Idempotent = true) ]
        [ JsonRpcHelp("Returns a server-typed object representing a couple. Demonstrates to returning server-typed objects.")]
        public Marriage GetCouple()
        {
            return new Marriage(
                new Person("Mickey", "Mouse"),
                new Person("Minnie", "Mouse"));
        }

        [ JsonRpcMethod("swapNames", Idempotent = true)]
        [ JsonRpcHelp("Swaps first and last name of person. Demonstrates receiving and returning a server-typed object.")]
        public Person SwapPersonNames(Person p)
        {
            return p == null ? new Person() : new Person(p.LastName, p.FirstName);
        }

        [JsonRpcMethod("getDataSet", Idempotent = true)]
        [ JsonRpcHelp("Returns the Northwind employees as a DataSet.") ]
        public DataSet GetEmployeeSet()
        {
            DataSet ds = new DataSet();
            ds.ReadXml(Server.MapPath("NorthwindData.xml"));
            return ds;
        }
        
        [ JsonRpcMethod("getDataTable", Idempotent = true)]
        [ JsonRpcHelp("Returns the Northwind employees as a DataTable.") ]
        public DataTable GetEmployeeTable()
        {
            return GetEmployeeSet().Tables[0];
        }

        [ JsonRpcMethod("getRowArray", Idempotent = true)]
        [ JsonRpcHelp("Returns the Northwind employees as an array of DataRow objects.") ]
        public DataRow[] GetEmployeeRowArray()
        {
            return GetEmployeeSet().Tables[0].Select();
        }

        [ JsonRpcMethod("getRowCollection", Idempotent = true)]
        [ JsonRpcHelp("Returns the Northwind employees as a DataRowCollection.") ]
        public DataRowCollection GetEmployeeRows()
        {
            return GetEmployeeSet().Tables[0].Rows;
        }

        [ JsonRpcMethod("getDataView", Idempotent = true)]
        [ JsonRpcHelp("Returns the Northwind employees as a DataView object.") ]
        public DataView GetEmployeeView()
        {
            return GetEmployeeSet().Tables[0].DefaultView;
        }

        [ JsonRpcMethod("getFirstDataRow", Idempotent = true)]
        [ JsonRpcHelp("Returns the first Northwind employee as a DataRow object.") ]
        public DataRow GetFirstEmployeeRow()
        {
            return GetEmployeeSet().Tables[0].Rows[0];
        }

        [ JsonRpcMethod("getFirstDataRowView", Idempotent = true)]
        [ JsonRpcHelp("Returns the first Northwind employee as a DataRowView object.") ]
        public DataRowView GetFirstEmployeeRowView()
        {
            return GetEmployeeSet().Tables[0].DefaultView[0];
        }

        #if NET_2_0

        //
        // The following method has to is conditionally enabled because
        // it makes use of System.Data.DataTableReader that was only 
        // introduced with .NET Framework 2.0. To include this method
        // in the compiled service, define the NET_2_0 symbol by 
        // changing the directive above to read:
        //
        // <%@ WebHandler ... CompilerOptions="/d:NET_2_0" %>
        //

        [ JsonRpcMethod("streamEmployees", Idempotent = true) ]
        [ JsonRpcHelp("Returns the Northwind employees as an IDataReader instance and which gets streamed out as an array of record objects.") ]
        public IDataReader StreamEmyploees()
        {
            return GetEmployeeSet().CreateDataReader();
        }

        #endif

        [ JsonRpcMethod("getDropDown", Idempotent = true)]
        [ JsonRpcHelp("Returns a data-bound DropDownList to the client as HTML.") ]
        public Control EmployeeDropDown()
        {
            DropDownList ddl = new DropDownList();
            DataSet ds = GetEmployeeSet();
            ds.Tables[0].Columns.Add("FullName", typeof(string), "FirstName + ' ' + LastName");
            ddl.DataSource = ds;
            ddl.DataMember = "Employees";
            ddl.DataTextField = "FullName";
            ddl.DataValueField = "EmployeeID";
            ddl.DataBind();
            return ddl;
        }
        
        [ JsonRpcMethod("getDataGrid", Idempotent = true)]
        [ JsonRpcHelp("Returns a data-bound DataGrid to the client as HTML.") ]
        public Control EmployeeDataGrid()
        {
            DataGrid grid = new DataGrid();
            grid.DataSource = GetEmployeeSet();
            grid.DataBind();
            return grid;
        }

        [JsonRpcMethod("total", Idempotent = true)]
        [JsonRpcHelp("Returns the total of all integers sent in an array.")]
        public double Total(double[] values)
        {
            double total = 0;

            if (values != null)
            {
                foreach (double value in values)
                    total += value;
            }

            return total;
        }
        
        [ JsonRpcMethod("sleep", Idempotent = true) ]
        [ JsonRpcHelp("Blocks the request for the specified number of milliseconds (maximum 7 seconds).") ]
        public void Sleep(int milliseconds)
        {
            System.Threading.Thread.Sleep(Math.Min(7000, milliseconds));
        }
        
        [ JsonRpcMethod("throwError", Idempotent = true)]
        [ JsonRpcHelp("Throws an error if you try to call this method.") ]
        public void ThrowError()
        {
            throw new ApplicationException();
        }

        [ JsonRpcMethod("format", Idempotent = true)]
        [ JsonRpcHelp("Formats placeholders in a format specification with supplied replacements. This method demonstrates fixed and variable arguments.") ]
        public string Format(string format, params object[] args)
        {
            return string.Format(format, args);
        }
        
        [ JsonRpcMethod("counter", Idempotent = true)]
        [ JsonRpcHelp("Increments a counter and returns its new value. Demonstrates use of session state.") ]
        public int SessionCounter()
        {
            int counter = 0;
            object counterObject = Session["Counter"];
            if (counterObject != null)
                counter = (int) counterObject;
            Session["Counter"] = ++counter;
            return counter;
        }

        [ JsonRpcMethod("encode", Idempotent = true)]
        [ JsonRpcHelp("Returns the bytes of a string in a given encoding that are transmitted as a Base64 string.") ]
        public byte[] EncodeBytes(string s, string encoding)
        {
            return System.Text.Encoding.GetEncoding(encoding).GetBytes(s);
        }

        [JsonRpcMethod("decode", Idempotent = true)]
        [JsonRpcHelp("Returns the string from encoded bytes (transmitted as a Base64 string).")]
        public string DecodeString(byte[] bytes, string encoding)
        {
            return System.Text.Encoding.GetEncoding(encoding).GetString(bytes);
        }

        [ JsonRpcMethod("stypeof", Idempotent = true) ]
        [ JsonRpcHelp("Returns the CLR type that a given value converted to on the server.") ]
        public string GetObjectType(object o)
        {
            return o != null ? o.GetType().FullName : null;
        }

        [ JsonRpcMethod("echoTime", Idempotent = true) ]
        [ JsonRpcHelp("Echoes back the date/time sent as parameter.") ]
        public DateTime EchoTime(DateTime time)
        {
            return time;
        }

        [ JsonRpcMethod("wadd", Idempotent = true, WarpedParameters = true) ]
        [ JsonRpcHelp("Adds two float arguments and returns their result. This method demostrates use of warped parameters.") ]
        public WarpedAddOutput WarpedAdd(WarpedAddInput args)
        {
            return new WarpedAddOutput(args.X + args.Y);
        }
        
        //
        // NOTE: To send and receive typed objects, use public types only 
        // that have a default constructor. Only public read/write fields
        // and properties are convert to and from JSON.
        //

        public class Marriage
        {
            public Person Husband;
            public Person Wife;

            public Marriage() { }

            public Marriage(Person husband, Person wife)
            {
                this.Husband = husband;
                this.Wife = wife;
            }
        }

        public class Person
        {
            public string FirstName;
            public string LastName;

            public Person() { }

            public Person(string fn, string ln)
            {
                this.FirstName = fn;
                this.LastName = ln;
            }
        }

        public class WarpedAddInput
        {
            public double X;
            public double Y;
        }

        public class WarpedAddOutput
        {
            public double Result;

            public WarpedAddOutput(double result)
            {
                this.Result = result;
            }
        }
    }
}

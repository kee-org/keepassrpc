#region License, Terms and Conditions
//
// Jayrock - JSON and JSON-RPC for Microsoft .NET Framework and Mono
// Written by Atif Aziz (www.raboof.com)
// Copyright (c) 2005 Atif Aziz. All rights reserved.
//
// This library is free software; you can redistribute it and/or modify it under
// the terms of the GNU Lesser General Public License as published by the Free
// Software Foundation; either version 2.1 of the License, or (at your option)
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

namespace Jayrock.Json.Conversion.Converters
{
    #region Imports

    using System;
    using System.Diagnostics;
    using System.IO;
    using System.Reflection;
    using System.Web;
    using System.Web.UI;

    #endregion

    public sealed class ControlExporter : ExporterBase
    {
        public ControlExporter() : 
            this(typeof(Control)) {}

        public ControlExporter(Type inputType) : 
            base(inputType) {}

        protected override void ExportValue(ExportContext context, object value, JsonWriter writer)
        {
            Debug.Assert(context != null);
            Debug.Assert(value != null);
            Debug.Assert(writer != null);

            ExportControl((Control) value, writer);
        }

        private static void ExportControl(Control control, JsonWriter writer)
        {
            Debug.Assert(control != null);
            Debug.Assert(writer != null);

            StringWriter stringWriter = new StringWriter();
            HtmlTextWriter htmlWriter = GetHtmlWriter(stringWriter);
            control.RenderControl(htmlWriter);
            writer.WriteString(stringWriter.ToString());
        }

        private static HtmlTextWriter GetHtmlWriter(TextWriter innerWriter)
        {
            Debug.Assert(innerWriter != null);

            //
            // If there is an HTTP context lying around then use the HTML writer
            // from there. Otherwise create the base implementation.
            //

            HttpContext context = HttpContext.Current;

            if (context != null)
            {
                Type htmlWriterType = context.Request.Browser.TagWriter;
                return (HtmlTextWriter) Activator.CreateInstance(htmlWriterType, new object[] { innerWriter });
            }
            else
            {
                return new HtmlTextWriter(innerWriter);
            }
        }
    }

}

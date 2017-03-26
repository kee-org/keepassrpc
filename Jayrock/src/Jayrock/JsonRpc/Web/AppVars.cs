#region License, Terms and Conditions
//
// Jayrock - JSON and JSON-RPC for Microsoft .NET Framework and Mono
// Written by Atif Aziz (www.raboof.com)
// Copyright (c) 2005 Atif Aziz. All rights reserved.
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

namespace Jayrock.JsonRpc.Web
{
    #region Imports

    using System;
    using System.Collections;
    using System.Diagnostics;
    using System.Web;

    #endregion

    public sealed class AppVars
    {
        private static Hashtable _varsByApp;
        private static readonly object _lock = new object();

        public static IDictionary FromCurrentContext()
        {
            return FromContext(HttpContext.Current);
        }

        public static IDictionary FromContext(HttpContext context)
        {
            if (context == null) 
                throw new ArgumentNullException("context");

            return Get(context.ApplicationInstance);
        }

        public static IDictionary Get(HttpApplication application)
        {
            if (application == null)
                throw new ArgumentNullException("application");

            lock (_lock)
            {
                //
                // Allocate map of object per application on demand.
                //

                if (_varsByApp == null)
                    _varsByApp = new Hashtable();

                //
                // Get the list of modules fot the application. If this is
                // the first registration for the supplied application object
                // then setup a new and empty list.
                //

                IDictionary vars = (IDictionary) _varsByApp[application];

                if (vars == null)
                {
                    vars = new Hashtable(4);
                    _varsByApp.Add(application, vars);
                    application.Disposed += new EventHandler(OnApplicationDisposed);
                }

                return vars;
            }
        }

        private static void OnApplicationDisposed(object sender, EventArgs args)
        {
            HttpApplication application = (HttpApplication) sender;

            application.Disposed -= new EventHandler(OnApplicationDisposed);

            lock (_lock)
            {
                if (_varsByApp == null)
                    return;

                _varsByApp.Remove(application);

                if (_varsByApp.Count == 0)
                    _varsByApp = null;
            }
        }
    }
}

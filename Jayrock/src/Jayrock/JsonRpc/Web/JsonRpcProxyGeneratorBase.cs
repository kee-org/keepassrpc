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
    using System.IO;
    using System.Security;
    using System.Web;
    using Jayrock.Services;

    #endregion

    public abstract class JsonRpcProxyGeneratorBase : JsonRpcServiceFeature
    {
        private DateTime _lastModifiedTime;
        private bool _lastModifiedTimeInitialized;

        public JsonRpcProxyGeneratorBase(IService service) : 
            base(service) {}

        protected override void ProcessRequest()
        {
            if (!Modified())
            {
                Response.StatusCode = 304;
                return;
            }

            if (HasLastModifiedTime)
            {
                DateTime lastModifiedTime = this.LastModifiedTime;

                //
                // Fix for issue #12072:
                // http://developer.berlios.de/bugs/?func=detailbug&bug_id=12072&group_id=4638
                //
                // If the server time or time zone is incorrect then 
                // the last modified time of the source could be into 
                // the future (e.g. if it was copied to the server from 
                // a machine in another timezone). 
                // HttpCachePolicy.SetLastModified throws an exception 
                // for a time value that is into the future so pin the 
                // last modified time to the current time if it is 
                // indeed in the future.
                
                DateTime now = DateTime.Now;
                
                if (lastModifiedTime > now)
                    lastModifiedTime = now;

                Response.Cache.SetCacheability(HttpCacheability.Public);
                Response.Cache.SetLastModified(lastModifiedTime);
            }

            Response.ContentType = ContentType;

            string clientFileName = Mask.NullString(ClientFileName);
            
            if (clientFileName.Length > 0)
            {
                Response.AppendHeader("Content-Disposition", 
                                      "attachment; filename=" + ClientFileName);
            }

            WriteProxy(new IndentedTextWriter(Response.Output));
        }

        protected abstract void WriteProxy(IndentedTextWriter writer);
        
        protected virtual string ContentType
        {
            get { return "text/plain"; }
        }

        protected abstract string ClientFileName { get; }
        
        private bool Modified()
        {
            if (!HasLastModifiedTime)
                return true;

            string modifiedSinceHeader = Mask.NullString(Request.Headers["If-Modified-Since"]);

            //
            // Apparently, Netscape added a non-standard extension to the
            // If-Modified-Since header in HTTP/1.0 where extra parameters
            // can be sent using a semi-colon as the delimiter. One such 
            // parameter is the original content length, which was meant 
            // to improve the accuracy of If-Modified-Since in case a 
            // document is updated twice in the same second. Here's an
            // example: 
            //
            // If-Modified-Since: Thu, 11 May 2006 07:59:51 GMT; length=3419
            //
            // HTTP/1.1 solved the same problem in a better way via the ETag 
            // header and If-None-Match. However, it looks like that some
            // proxies still use this technique, so the following checks for
            // a semi-colon in the header value and clips it to everything
            // before it.
            //
            // This is a fix for bug #7462:
            // http://developer.berlios.de/bugs/?func=detailbug&bug_id=7462&group_id=4638
            //
        
            int paramsIndex = modifiedSinceHeader.IndexOf(';');
            if (paramsIndex >= 0)
                modifiedSinceHeader = modifiedSinceHeader.Substring(0, paramsIndex);

            if (modifiedSinceHeader.Length == 0)
                return true;

            DateTime modifiedSinceTime;
            
            try
            {
                modifiedSinceTime = InternetDate.Parse(modifiedSinceHeader);
            }
            catch (FormatException)
            {
                //
                // Accorinding to the HTTP specification, if the passed 
                // If-Modified-Since date is invalid, the response is 
                // exactly the same as for a normal GET. See section
                // 14.25 of RFC 2616 for more information:
                // http://www.w3.org/Protocols/rfc2616/rfc2616-sec14.html#sec14.25
                //

                return true;
            }

            DateTime time = LastModifiedTime;
            time = new DateTime(time.Year, time.Month, time.Day, time.Hour, time.Minute, time.Second);

            if (time > modifiedSinceTime)
                return true;

            return false;
        }

        private bool HasLastModifiedTime
        {
            get { return LastModifiedTime > DateTime.MinValue; }
        }

        private DateTime LastModifiedTime
        {
            get
            {
                if (!_lastModifiedTimeInitialized)
                {
                    _lastModifiedTimeInitialized = true;

                    //
                    // The last modified time is determined by taking the
                    // last modified time of the physical file (for example,
                    // a DLL) representing the type's assembly.
                    //

                    try
                    {
                        Uri codeBase = new Uri(Service.GetType().Assembly.CodeBase);

                        if (codeBase != null && codeBase.IsFile)
                        {
                            string path = codeBase.LocalPath;

                            if (File.Exists(path))
                            {
                                try
                                {
                                    _lastModifiedTime = File.GetLastWriteTime(path);
                                }
                                catch (UnauthorizedAccessException) { /* ignored */ }
                                catch (IOException) { /* ignored */ }
                            }
                        }
                    }
                    catch (SecurityException)
                    {
                        //
                        // This clause ignores security exceptions that may
                        // be caused by an application that is partially
                        // trusted and therefore would not be allowed to
                        // disover the service assembly code base as well
                        // as the physical file's modification time.
                        //
                    }
                }

                return _lastModifiedTime;
            }
        }
    }
}
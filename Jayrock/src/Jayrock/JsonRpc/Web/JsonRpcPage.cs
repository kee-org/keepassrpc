//#region License, Terms and Conditions
////
//// Jayrock - JSON and JSON-RPC for Microsoft .NET Framework and Mono
//// Written by Atif Aziz (www.raboof.com)
//// Copyright (c) 2005 Atif Aziz. All rights reserved.
////
//// This library is free software; you can redistribute it and/or modify it under
//// the terms of the GNU Lesser General Public License as published by the Free
//// Software Foundation; either version 3 of the License, or (at your option)
//// any later version.
////
//// This library is distributed in the hope that it will be useful, but WITHOUT
//// ANY WARRANTY; without even the implied warranty of MERCHANTABILITY or FITNESS
//// FOR A PARTICULAR PURPOSE. See the GNU Lesser General Public License for more
//// details.
////
//// You should have received a copy of the GNU Lesser General Public License
//// along with this library; if not, write to the Free Software Foundation, Inc.,
//// 59 Temple Place, Suite 330, Boston, MA 02111-1307 USA 
////
//#endregion

//namespace Jayrock.JsonRpc.Web
//{
//    #region Imports

//    using System;
//    using System.Collections;
//    using System.Globalization;
//    using System.Web.UI;
//    using System.Web.UI.HtmlControls;
//    using Jayrock.Services;

//    #endregion

//    internal abstract class JsonRpcPage : Page
//    {
//        private readonly IService _service;
//        private ServiceClass _serviceClass;
//        private Method[] _methods;
//        private bool _serviceClassInitialized;
//        private HtmlGenericControl _head;
//        private HtmlGenericControl _body;

//        protected JsonRpcPage(IService service)
//        {
//            if (service == null)
//                throw new ArgumentNullException("service");
            
//            _service = service;
//        }

//        public IService Service
//        {
//            get { return _service; }
//        }

//        protected ServiceClass ServiceClass
//        {
//            get
//            {
//                if (!_serviceClassInitialized)
//                {
//                    _serviceClassInitialized = true;
//                    _serviceClass = Service.GetClass();
//                }

//                return _serviceClass;
//            }
//        }

//        protected Method[] SortedMethods
//        {
//            get
//            {
//                if (_methods == null)
//                {
//                    Method[] methods = ServiceClass.GetMethods();
//                    Array.Sort(methods, new MethodNameComparer());
//                    _methods = methods;
//                }

//                return _methods;
//            }
//        }

//        protected override void OnInit(EventArgs e)
//        {
//            LiteralControl docType = new LiteralControl("<!DOCTYPE HTML PUBLIC \"-//W3C//DTD HTML 4.01 Transitional//EN\" \"http://www.w3.org/TR/html4/loose.dtd\">");
//            Controls.Add(docType);

//            HtmlGenericControl html = new HtmlGenericControl("html");
//            Controls.Add(html);

//            _head = new HtmlGenericControl("head");
//            html.Controls.Add(_head);

//            _body = new HtmlGenericControl("body");
//            html.Controls.Add(_body);

//            HtmlGenericControl title = new HtmlGenericControl("title");
//            title.InnerText = PageTitle;
//            _head.Controls.Add(title);

//            base.OnInit(e);
//        }
        
//        protected override void OnLoad(EventArgs e)
//        {
//            AddStyleSheet();
//            AddHeader();
//            AddContent();
//            AddFooter();

//            base.OnLoad(e);
//        }

//        protected virtual void AddStyleSheet()
//        {
//        }
        
//        protected virtual void AddHeader()
//        {
//        }

//        protected virtual void AddContent()
//        {
//        }
        
//        protected virtual void AddFooter()
//        {
//        }

//        protected virtual Control Head
//        {
//            get { return _head; }
//        }

//        protected virtual Control Body
//        {
//            get { return _body; }
//        }

//        protected virtual string PageTitle
//        {
//            get { return ServiceClass.Name; }
//        }

//        private sealed class MethodNameComparer : IComparer
//        {
//            public int Compare(object x, object y)
//            {
//                Method methodX = (Method) x;
//                Method methodY = (Method) y;
//                return string.Compare(methodX.Name, methodY.Name, false, CultureInfo.InvariantCulture);
//            }
//        }
//    }
//}

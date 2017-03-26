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

namespace Jayrock.JsonRpc
{
    #region Imports

    using System;
    using System.Collections;
    using System.ComponentModel.Design;
    using System.Diagnostics;
    using System.IO;
    using Jayrock.Json;
    using Jayrock.Json.Conversion;
    using Jayrock.Services;

    #endregion

    public class JsonRpcDispatcher
    {
        private readonly IService _service;
        private string _serviceName;
        private bool _localExecution;
        private bool _requireIdempotency;
        private JsonExportHandler _exporter;
        private JsonImportHandler _importer;

        public JsonRpcDispatcher(IService service)
        {
            if (service == null)
                throw new ArgumentNullException("service");

            _service = service;
        }

        public bool LocalExecution
        {
            get { return _localExecution; }
            set { _localExecution = value; }
        }

        public bool RequireIdempotency
        {
            get { return _requireIdempotency; }
            set { _requireIdempotency = value; }
        }

        public IService Service
        {
            get { return _service; }
        }

        private string ServiceName
        {
            get
            {
                if (_serviceName == null)
                    _serviceName = JsonRpcServices.GetServiceName(Service);

                return _serviceName;
            }
        }

        public JsonImportHandler JsonImporter
        {
            get
            {
                if (_importer == null)
                {
                    ImportContext context = JsonConvert.CreateImportContext();
                    _importer = new JsonImportHandler(context.Import);
                }

                return _importer;
            }

            set
            {
                if (value == null)
                    throw new ArgumentNullException("value");

                _importer = value;
            }
        }

        public JsonExportHandler JsonExporter
        {
            get
            {
                if (_exporter == null)
                {
                    ExportContext context = JsonConvert.CreateExportContext();
                    _exporter = new JsonExportHandler(context.Export);
                }

                return _exporter;
            }

            set
            {
                if (value == null)
                    throw new ArgumentNullException("value");

                _exporter = value;
            }
        }

        public string Process(string request, bool authorised)
        {
            StringWriter writer = new StringWriter();
            Process(new StringReader(request), writer, authorised);
            return writer.ToString();
        }

        public void Process(TextReader request, TextWriter response, bool authorised)
        {
            if (request == null)
                throw new ArgumentNullException("request");

            if (response == null)
                throw new ArgumentNullException("response");

            Process(JsonText.CreateReader(request), JsonText.CreateWriter(response), authorised);
        }

        public virtual void Process(JsonReader request, JsonWriter response, bool authorised)
        {
            if (request == null)
                throw new ArgumentNullException("request");

            if (response == null)
                throw new ArgumentNullException("response");

            IDictionary requestObject;

            try
            {
                requestObject = ParseRequest(request);
            }
            catch (BadRequestException e)
            {
                requestObject = e.Request as IDictionary;

                WriteResponse(
                    CreateResponse(requestObject, /* result */ null,
                        OnError(e.InnerException, requestObject)),
                    response);

                return;
            }

            IDictionary responseObject = Invoke(requestObject, authorised);
            WriteResponse(responseObject, response);
        }

        /* TODO: Add async processing.

            IAsyncResult BeginProcess(JsonReader input, JsonWriter output, AsyncCallback callback, object asyncState);
            void BeginProcess(IAsyncResult asyncResult); */

        public virtual IDictionary Invoke(IDictionary request, bool authorised)
        {
            if (request == null)
                throw new ArgumentNullException();

            //
            // Get the ID of the request.
            //

            object id = request["id"];

            //
            // If the ID is not there or was not set then this is a notification
            // request from the client that does not expect any response. Right
            // now, we don't support this.
            //

            bool isNotification = JsonNull.LogicallyEquals(id);
            
            if (isNotification)
                throw new NotSupportedException("Notification are not yet supported.");

            if (JsonRpcTrace.TraceInfo)
                JsonRpcTrace.Info("Received request with the ID {0}.", id.ToString());

            //
            // Get the method name and arguments.
            //
    
            string methodName = Mask.NullString((string) request["method"]);

            if (methodName.Length == 0)
                throw new JsonRpcException("No method name supplied for this request.");

            //TODO2: change RE authorisation options deny setclientname - just remove it entirely?!?
            if (!authorised && (methodName != "Authenticate" && methodName != "SetClientName"))
                throw new JsonRpcException("Not authorised.");

            if (JsonRpcTrace.Switch.TraceInfo)
                JsonRpcTrace.Info("Invoking method {1} on service {0}.", ServiceName, methodName);

            //
            // Invoke the method on the service and handle errors.
            //
    
            object error = null;
            object result = null;

            try
            {
                IService service = Service;
                Method method = service.GetClass().GetMethodByName(methodName);
                
                if (RequireIdempotency && !method.Idempotent)
                    throw new JsonRpcException(string.Format("Method {1} on service {0} is not allowed for idempotent type of requests.", ServiceName, methodName));
                        
                object[] args;
                string[] names = null;

                object argsObject = request["params"];
                IDictionary argByName = argsObject as IDictionary;
                
                if (argByName != null)
                {
                    names = new string[argByName.Count];
                    argByName.Keys.CopyTo(names, 0);
                    args = new object[argByName.Count];
                    argByName.Values.CopyTo(args, 0);
                }
                else
                {
                    args = CollectionHelper.ToArray((ICollection) argsObject);
                }
                
                result = method.Invoke(service, names, args);
            }
            catch (MethodNotFoundException e)
            {
                error = OnError(e, request);
            }
            catch (InvocationException e)
            {
                error = OnError(e, request);
            }
            catch (TargetMethodException e)
            {
                error = OnError(e.InnerException, request);
            }
            catch (Exception e)
            {
                if (JsonRpcTrace.Switch.TraceError)
                    JsonRpcTrace.Error(e);

                throw;
            }

            //
            // Setup and return the response object.
            //

            return CreateResponse(request, result, error);
        }

        protected virtual IDictionary CreateResponse(IDictionary request, object result, object error)
        {
            if (result != null && request == null)
                throw new ArgumentNullException("request");

            if (result != null && error != null)
                throw new ArgumentException("One of result or error parameters must be a null reference.");

            JsonObject response = new JsonObject();
            
            if (request != null)
                response["id"] = request["id"];

            if (error != null)
                response["error"] = error;
            else
                response["result"] = result;

            return response;
        }

        protected virtual IDictionary ParseRequest(JsonReader input)
        {
            if (input == null)
                throw new ArgumentNullException("input");

            JsonReader reader = input; // alias for clarity
            JsonImportHandler importer = JsonImporter;
    
            JsonObject request = new JsonObject();
            Method method = null;
            JsonReader paramsReader = null;
            object args = null;
            
            try
            {
                reader.ReadToken(JsonTokenClass.Object);
        
                while (reader.TokenClass != JsonTokenClass.EndObject)
                {
                    string memberName = reader.ReadMember();
            
                    switch (memberName)
                    {
                        case "id" :
                        {
                            request["id"] = importer(AnyType.Value, reader);
                            break;
                        }
                
                        case "method" :
                        {
                            string methodName = reader.ReadString();
                            request["method"] = methodName;
                            method = Service.GetClass().GetMethodByName(methodName);
                    
                            if (paramsReader != null)
                            {
                                //
                                // If the parameters were already read in and
                                // buffer, then deserialize them now that we know
                                // the method we're dealing with.
                                //
                        
                                args = ReadParameters(method, paramsReader, importer);
                                paramsReader = null;
                            }
                    
                            break;
                        }
                
                        case "params" :
                        {
                            //
                            // Is the method already known? If so, then we can
                            // deserialize the parameters right away. Otherwise
                            // we record them until hopefully the method is
                            // encountered.
                            //
                    
                            if (method != null)
                            {
                                args = ReadParameters(method, reader, importer);
                            }
                            else
                            {
                                JsonRecorder recorder = new JsonRecorder();
                                recorder.WriteFromReader(reader);
                                paramsReader = recorder.CreatePlayer();
                            }

                            break;
                        }
                    
                        default:
                        {
                            reader.Skip();
                            break;
                        }
                    }
                }
        
                reader.Read();

                if (args != null)
                    request["params"] = args;
        
                return request;
            }
            catch (JsonException e)
            {
                throw new BadRequestException(e.Message, e, request);
            }
            catch (MethodNotFoundException e)
            {
                throw new BadRequestException(e.Message, e, request);
            }
        }

        protected virtual void WriteResponse(IDictionary response, JsonWriter output)
        {
            if (response == null)
                throw new ArgumentNullException("response");

            if (output == null)
                throw new ArgumentNullException("output");
            
            JsonExporter(response, output);
        }

        protected virtual object OnError(Exception e, IDictionary request)
        {
            if (JsonRpcTrace.Switch.TraceError)
                JsonRpcTrace.Error(e);

            return JsonRpcError.FromException(e, LocalExecution);
        }

        private static object ReadParameters(Method method, JsonReader reader, JsonImportHandler importer)
        {
            Debug.Assert(method != null);
            Debug.Assert(reader != null);
            Debug.Assert(importer != null);
            
            reader.MoveToContent();
            
            Parameter[] parameters = method.GetParameters();
                            
            if (reader.TokenClass == JsonTokenClass.Array)
            {
                reader.Read();
                ArrayList argList = new ArrayList(parameters.Length);
                                
                for (int i = 0; i < parameters.Length && reader.TokenClass != JsonTokenClass.EndArray; i++)
                    argList.Add(importer(parameters[i].ParameterType, reader));

                reader.StepOut();
                return argList.ToArray();
            }
            else if (reader.TokenClass == JsonTokenClass.Object)
            {
                reader.Read();
                JsonObject argByName = new JsonObject();
                                
                while (reader.TokenClass != JsonTokenClass.EndObject)
                {
                    // TODO: Imporve this lookup.
                    // FIXME: Does not work when argument is positional.
                                    
                    Type parameterType = AnyType.Value;
                    string name = reader.ReadMember();

                    foreach (Parameter parameter in parameters)
                    {
                        if (CaselessString.Equals(parameter.Name, name))
                        {
                            parameterType = parameter.ParameterType;
                            break;
                        }
                    }
                                    
                    argByName.Put(name, importer(parameterType, reader));
                }
                                
                reader.Read();
                return argByName;
            }
            else
            {
                return importer(AnyType.Value, reader);
            }
        }
    }
}

<%@ WebHandler Language="C#" Class="JayrockWeb.TemplateService" %>

namespace JayrockWeb
{
    #region Imports
    
    using System;
    using System.Web;
    using Jayrock.Json;
    using Jayrock.JsonRpc;
    using Jayrock.JsonRpc.Web;
    
    #endregion
    
    /// <summary>
    /// A template to be used as the starting point for your own service.
    /// </summary>

    public class TemplateService : JsonRpcHandler
    {
        /*
        [ JsonRpcMethod("greetings") ] 
        public string Greetings()
        {
            return "Welcome!";
        }
        */
    }
}

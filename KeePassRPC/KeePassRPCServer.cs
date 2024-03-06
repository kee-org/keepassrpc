using System;
using System.Linq;
using Fleck2;
using Fleck2.Interfaces;

namespace KeePassRPC
{
    public class KeePassRPCServer
    {
        private static KeePassRPCService Service;
        private KeePassRPCExt KeePassRPCPlugin;
        private WebSocketServer _webSocketServer;
        private static WebSocketServerConfig WebsocketConfig;

        private void FleckLogger(LogLevel ll, string s, Exception e)
        {
            if (KeePassRPCPlugin.logger != null)
                try
                {
                    KeePassRPCPlugin.logger.WriteLine("Fleck says: " + s + (e != null ? (". Exception: " + e) : ""));
                }
                catch (Exception)
                {
                    // Don't care
                }
        }

        private void StartWebsockServer(WebSocketServerConfig config)
        {
            FleckLog.Level = LogLevel.Debug;
            FleckLog.LogAction = new Fleck2Extensions.Action<LogLevel, string, Exception>(FleckLogger);
            // Fleck library changed behaviour with recent .NET versions so we have to supply the port in the location string
            _webSocketServer = new WebSocketServer("ws://localhost:" + config.WebSocketPort, config.BindOnlyToLoopback);
            Action<IWebSocketConnection> applyConfiguration = new Action<IWebSocketConnection>(InitSocket);
            _webSocketServer.Start(applyConfiguration);
        }

        private void InitSocket(IWebSocketConnection socket)
        {
            socket.OnOpen = delegate ()
            {
                // Immediately reject connections with unexpected origins
                if (!ValidateOrigin(socket.ConnectionInfo.Origin)) {
                    if (KeePassRPCPlugin.logger != null) {
                        try
                        {
                            KeePassRPCPlugin.logger.WriteLine(socket.ConnectionInfo.Origin + " is not permitted to access KeePassRPC.");
                        }
                        catch (Exception)
                        {
                            // Don't care
                        }
                    }
                } else {
                    KeePassRPCPlugin.AddRPCClientConnection(socket);
                }
            };
            socket.OnClose = delegate ()
            {
                KeePassRPCPlugin.RemoveRPCClientConnection(socket);
            };
            socket.OnMessage = delegate (string message)
            {
                KeePassRPCPlugin.MessageRPCClientConnection(socket, message, Service);
            };
        }

        private bool ValidateOrigin(string origin)
        {
            if (WebsocketConfig.PermittedOrigins.Any(o => origin.StartsWith(o))) {
                return true;
            }
            return false;
        }

        /// <summary>
        /// Starts the web socket listener
        /// </summary>
        /// <param name="service">The KeePassRPCService the server should interact with.</param>
        public KeePassRPCServer(KeePassRPCService service, KeePassRPCExt keePassRPCPlugin, WebSocketServerConfig websocketConfig)
        {
            if (keePassRPCPlugin.logger != null) keePassRPCPlugin.logger.WriteLine("Starting KPRPCServer");
            Service = service;
            KeePassRPCPlugin = keePassRPCPlugin;
            WebsocketConfig = websocketConfig;
            StartWebsockServer(websocketConfig);
            if (keePassRPCPlugin.logger != null) keePassRPCPlugin.logger.WriteLine("Started KPRPCServer");
        }
    }
}

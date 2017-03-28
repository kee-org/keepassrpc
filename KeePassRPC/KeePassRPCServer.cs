using System;
using Fleck2;
using Fleck2.Interfaces;

namespace KeePassRPC
{
    public class KeePassRPCServer
    {
        private static KeePassRPCService Service;
        KeePassRPCExt KeePassRPCPlugin;
        private WebSocketServer _webSocketServer;

        void FleckLogger(LogLevel ll, string s, Exception e)
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

        void StartWebsockServer(int port, bool bindOnlyToLoopback)
        {
            FleckLog.Level = LogLevel.Debug;
            FleckLog.LogAction = new Fleck2Extensions.Action<LogLevel, string, Exception>(FleckLogger);
            // Fleck library changed behaviour with recent .NET versions so we have to supply the port in the location string
            _webSocketServer = new WebSocketServer("ws://localhost:" + port, bindOnlyToLoopback);
            Action<IWebSocketConnection> config = new Action<IWebSocketConnection>(InitSocket);
            _webSocketServer.Start(config);
        }

        void InitSocket(IWebSocketConnection socket)
        {
            socket.OnOpen = delegate ()
            {
                KeePassRPCPlugin.AddRPCClientConnection(socket);
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

        /// <summary>
        /// Starts the web socket listener
        /// </summary>
        /// <param name="service">The KeePassRPCService the server should interact with.</param>
        public KeePassRPCServer(KeePassRPCService service, KeePassRPCExt keePassRPCPlugin, int webSocketPort, bool bindOnlyToLoopback)
        {
            if (keePassRPCPlugin.logger != null) keePassRPCPlugin.logger.WriteLine("Starting KPRPCServer");
            Service = service;
            KeePassRPCPlugin = keePassRPCPlugin;
            StartWebsockServer(webSocketPort, bindOnlyToLoopback);
            if (keePassRPCPlugin.logger != null) keePassRPCPlugin.logger.WriteLine("Started KPRPCServer");
        }
    }
}

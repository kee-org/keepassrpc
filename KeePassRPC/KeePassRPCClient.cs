/*
  KeePassRPC - Uses JSON-RPC to provide RPC facilities to KeePass.
  Example usage includes the KeeFox firefox extension.
  
  Copyright 2010 Chris Tomlinson <keefox@christomlinson.name>

  This program is free software; you can redistribute it and/or modify
  it under the terms of the GNU General Public License as published by
  the Free Software Foundation; either version 2 of the License, or
  (at your option) any later version.

  This program is distributed in the hope that it will be useful,
  but WITHOUT ANY WARRANTY; without even the implied warranty of
  MERCHANTABILITY or FITNESS FOR A PARTICULAR PURPOSE.  See the
  GNU General Public License for more details.

  You should have received a copy of the GNU General Public License
  along with this program; if not, write to the Free Software
  Foundation, Inc., 51 Franklin St, Fifth Floor, Boston, MA  02110-1301  USA
*/

using System;
using System.Collections.Generic;
using System.Text;
using System.IO;
using Fleck2.Interfaces;
using KeePassRPC.DataExchangeModel;
using Jayrock.JsonRpc;
using System.Security.Cryptography;
using System.Xml.Serialization;
using System.Threading;

namespace KeePassRPC
{
    /// <summary>
    /// Represents a client that has connected to this RPC server.
    /// </summary>
    public class KeePassRPCClientConnection
    {
        // wanted to use uint really but that seems to break Jayrock JSON-RPC - presumably becuase there is no such concept in JavaScript
        static private int _protocolVersion = 0;
        static int ProtocolVersion { get {
            if (_protocolVersion == 0)
            {
                _protocolVersion = BitConverter.ToInt32(new byte[] {
                    (byte)KeePassRPCExt.PluginVersion.Build,
                    (byte)KeePassRPCExt.PluginVersion.Minor,
                    (byte)KeePassRPCExt.PluginVersion.Major,0},0);
            }
            return _protocolVersion;
        } }

        /// <summary>
        /// The ID of the next signal we'll send to the client
        /// </summary>
        private int _currentCallBackId = 0;
        private bool _authorised;
        private IWebSocketConnection _webSocketConnection = null;
        private SRP _srp;
        private KeyChallengeResponse _kcp;
        private int securityLevel;
        private int securityLevelClientMinimum;
        private string userName;

        // Read-only username is accessible to anyone but only once the connection has been confirmed
        public string UserName { get { if (Authorised) return userName; else return ""; } }

        private KeyChallengeResponse Kcp
        {
            get { return _kcp; }
            set { _kcp = value; }
        }

        private KeePassRPC.Forms.AuthForm _authForm;
        KeePassRPCExt KPRPC = null;
        
        /// <summary>
        /// The underlying web socket connection that links us to this client.
        /// </summary>
        public IWebSocketConnection WebSocketConnection
        {
            get { return _webSocketConnection; }
            private set { _webSocketConnection = value; }
        }
        
        /// <summary>
        /// Whether this client has successfully authenticated to the
        /// server and been authorised to communicate with KeePass
        /// </summary>
        /// //TODO1.6: verify this is always set and unset at correct times (non-trivial due to required compatibility with old and new KPRPC protocols)
        public bool Authorised
        {
            get { return _authorised; }
            set { _authorised = value; }
        }

        private long KeyExpirySeconds
        {
            get
            {
                // read from config file
                return KPRPC._host.CustomConfig.GetLong("KeePassRPC.AuthorisationExpiryTime", 31536000);
            }
        }

        /// <summary>
        /// The secret key used to encrypt messages
        /// </summary>
        private KeyContainerClass KeyContainer
        {
            get {
                if (_keyContainer == null)
                {
                    // if we're already authorised to communicate but do not have the key yet, we know it's waiting for us in the recently authenticated SRP object
                    if (Authorised)
                    {
                        _keyContainer = new KeyContainerClass(_srp.Key, DateTime.UtcNow.AddSeconds(KeyExpirySeconds), userName, clientName);
                    }
                        // otherwise we know that the key is going to be stored according to spec (if not we'll return a null key to trigger a fresh SRP auth process)
                    else
                    {
                        byte[] serialisedKeyContainer = null;

                        // check security level and find key in appropriate place
                        if (securityLevel == 1)
                        {
                            // read from config file
                            string serialisedKeyContainerString = KPRPC._host.CustomConfig.GetString("KeePassRPC.Key." + userName, "");
                            if (string.IsNullOrEmpty(serialisedKeyContainerString))
                                return null;
                            serialisedKeyContainer = Convert.FromBase64String(serialisedKeyContainerString);
                        }
                        else if (securityLevel == 2)
                        {
                            // read from encrypted config file
                            string secret = KPRPC._host.CustomConfig.GetString("KeePassRPC.Key." + userName, "");
                            if (string.IsNullOrEmpty(secret))
                                return null;
                            try
                            {
                                byte[] keyBytes = System.Security.Cryptography.ProtectedData.Unprotect(
                                Convert.FromBase64String(secret),
                                new byte[] { 172, 218, 37, 36, 15 },
                                DataProtectionScope.CurrentUser);
                                serialisedKeyContainer = keyBytes;
                            }
                            catch (Exception)
                            {
                                // This can happen if user changes from medium security to low security
                                // and maybe other operating system / .NET failures
                                return null;
                            }
                        }
                        else
                            return null;

                        if (serialisedKeyContainer == null)
                            return null;
                        else
                        {
                            try
                            {
                                XmlSerializer mySerializer = new XmlSerializer(typeof(KeyContainerClass));
                                _keyContainer = (KeyContainerClass)mySerializer.Deserialize(new MemoryStream(serialisedKeyContainer));
                            }
                            catch (Exception)
                            {
                                return null;
                            }
                        }
                    }
                }
                return _keyContainer;
            }
            set
            {
                _keyContainer = value;

                KeyContainerClass kc = new KeyContainerClass(_srp.Key, DateTime.UtcNow.AddSeconds(KeyExpirySeconds), userName, clientName);

                XmlSerializer mySerializer = new
                XmlSerializer(typeof(KeyContainerClass));
                MemoryStream myWriter = new MemoryStream();
                mySerializer.Serialize(myWriter, kc);
                byte[] serialisedKeyContainer = myWriter.ToArray();

                // We probably want to store the key somewhere that will persist beyond an application restart
                if (securityLevel == 1)
                {
                    // Store unencrypted in config file
                    KPRPC._host.CustomConfig.SetString("KeePassRPC.Key." + userName, Convert.ToBase64String(serialisedKeyContainer));
                    KPRPC._host.MainWindow.Invoke((System.Windows.Forms.MethodInvoker)delegate { KPRPC._host.MainWindow.SaveConfig(); });
                }
                else if (securityLevel == 2)
                {
                    try
                    {
                        // Encrypt the data using DataProtectionScope.CurrentUser. The result can be decrypted 
                        //  only by the same current user. 

                        byte[] secret = System.Security.Cryptography.ProtectedData.Protect(
                            serialisedKeyContainer,
                            new byte[] { 172, 218, 37, 36, 15 },
                            DataProtectionScope.CurrentUser);

                        KPRPC._host.CustomConfig.SetString("KeePassRPC.Key." + userName, Convert.ToBase64String(secret));
                        KPRPC._host.MainWindow.Invoke((System.Windows.Forms.MethodInvoker)delegate { KPRPC._host.MainWindow.SaveConfig(); });
                    }
                    catch (CryptographicException e)
                    {
                        if (KPRPC.logger != null) KPRPC.logger.WriteLine("Could not store KeePassRPC's secret key so you will have to re-authenticate clients such as KeeFox. The following exception caused this problem: " + e);
                    }
                }
                // else we don't persist the key anywhere - no security implications
                // of this fallback behaviour but it will be annoying for the user
            }
        }

        private KeyContainerClass _keyContainer;
        private string clientName;
        
        public KeePassRPCClientConnection(IWebSocketConnection connection, bool isAuthorised, KeePassRPCExt kprpc)
        {
            WebSocketConnection = connection;
            Authorised = isAuthorised;

            //TODO2: Can we lazy load these since some sessions will require only one of these authentication mechanisms?
            _srp = new SRP();
            Kcp = new KeyChallengeResponse(ProtocolVersion);

            // Load from config, default to medium security if user has not yet requested anything different
            securityLevel = (int)kprpc._host.CustomConfig.GetLong("KeePassRPC.SecurityLevel", 2);
            securityLevelClientMinimum = (int)kprpc._host.CustomConfig.GetLong("KeePassRPC.SecurityLevelClientMinimum", 2);
            KPRPC = kprpc;
        }

        /// <summary>
        /// Sends the specified signal to the client.
        /// </summary>
        /// <param name="signal">The signal.</param>
        public void Signal(KeePassRPC.DataExchangeModel.Signal signal, string methodName)
        {
            try
            {
                Jayrock.Json.JsonObject call = new Jayrock.Json.JsonObject();
                call["id"] = ++_currentCallBackId;
                call["method"] = methodName;
                call["params"] = new int[] { (int)signal };

                StringBuilder sb = new StringBuilder();
                Jayrock.Json.Conversion.JsonConvert.Export(call, sb);
                KPRPCMessage data2client = new KPRPCMessage();
                data2client.protocol = "jsonrpc";
                data2client.version = ProtocolVersion;
                data2client.jsonrpc = Encrypt(sb.ToString());

                // If there was a problem encrypting our message, just abort - the
                // client won't be able to do anything useful with an error message
                if (data2client.jsonrpc == null)
                {
                    if (KPRPC.logger != null) KPRPC.logger.WriteLine("Encryption error when trying to send signal: " + signal);
                    return;
                }

                // Signalling through the websocket needs to be processed on a different thread becuase handling the incoming messages results in a lock on the list of known connections (which also happens before this Signal function is called) so we want to process this as quickly as possible and avoid deadlocks.
                
                // Respond to each message on a different thread
                ThreadStart work = delegate
                {
                    WebSocketConnection.Send(Jayrock.Json.Conversion.JsonConvert.ExportToString(data2client));
                };
                Thread messageHandler = new Thread(work);
                messageHandler.Name = "signalDispatcher";
                messageHandler.Start();
            }
            catch (System.IO.IOException)
            {
                // Sometimes a connection is unexpectedly closed e.g. by Firefox
                // or (more likely) dodgy security "protection". From one year's
                // worth of bug reports (35) 100% of unexpected application
                // exceptions were IOExceptions.
                //
                // We will now ignore this type of exception and allow KeeFox to
                // re-establish the link to KeePass as part of its regular polling loop.
                //
                // The requested KPRPC signal will never be recieved by KeeFox
                // but this should be OK in practice becuase KeeFox will 
                // re-establish the relevant state information as soon as it reconnects.
                //
                // BUT: the exception to this rule is when KeeFox fails to receive the
                // "shutdown" signal - it then gets itself in an inconsistent state
                // and has no opportunity to recover until KeePass is running again.
                return;
            }
            catch (Exception ex)
            {
                System.Windows.Forms.MessageBox.Show("ERROR! Please click on this box, press CTRL-C on your keyboard and paste into a new post on the KeeFox forum (http://keefox.org/help/forum). Doing this will help other people to use KeeFox without any unexpected error messages like this. Please briefly describe what you were doing when the problem occurred, which version of KeeFox, KeePass and Firefox you use and what other security software you run on your machine. Thanks! Technical detail follows: " + ex.ToString());
            }
        }

        public void ReceiveMessage(string message, KeePassRPCService service)
        {
            // Inspect incoming message
            KPRPCMessage kprpcm;

            try
            {
                kprpcm = (KPRPCMessage)Jayrock.Json.Conversion.JsonConvert.Import(typeof(KPRPCMessage), message);
            }
            catch (Exception )
            {
                kprpcm = null;
            }

            if (kprpcm == null)
            {
                KPRPCMessage data2client = new KPRPCMessage();
                data2client.protocol = "error";
                data2client.srp = new SRPParams();
                data2client.version = ProtocolVersion;

                data2client.error = new Error(ErrorCode.INVALID_MESSAGE, new string[] { "Contents can't be interpreted as an SRPEncapsulatedMessage" });
                this.Authorised = false;

                string response = Jayrock.Json.Conversion.JsonConvert.ExportToString(data2client);
                this.WebSocketConnection.Send(response);
                return;
            }

            if (kprpcm.version != ProtocolVersion)
            {
                KPRPCMessage data2client = new KPRPCMessage();
                data2client.protocol = "error";
                data2client.srp = new SRPParams();
                data2client.version = ProtocolVersion;

                data2client.error = new Error(kprpcm.version > ProtocolVersion ? ErrorCode.VERSION_CLIENT_TOO_HIGH : ErrorCode.VERSION_CLIENT_TOO_LOW, new string[] { ProtocolVersion.ToString() });
                this.Authorised = false;

                string response = Jayrock.Json.Conversion.JsonConvert.ExportToString(data2client);
                this.WebSocketConnection.Send(response);
                return;
            }

            //1: Is it an SRP message?
            switch (kprpcm.protocol)
            {
                case "setup": KPRPCReceiveSetup(kprpcm); break;
                case "jsonrpc": KPRPCReceiveJSONRPC(kprpcm.jsonrpc, service); break;
                default: KPRPCMessage data2client = new KPRPCMessage();
                    data2client.protocol = "error";
                    data2client.srp = new SRPParams();
                    data2client.version = ProtocolVersion;

                    data2client.error = new Error(ErrorCode.UNRECOGNISED_PROTOCOL, new string[] { "Use setup or jsonrpc" });
                    this.Authorised = false;

                    string response = Jayrock.Json.Conversion.JsonConvert.ExportToString(data2client);
                    this.WebSocketConnection.Send(response); 
                    return;
            }

        }

  	    void KPRPCReceiveSetup (KPRPCMessage kprpcm) {

            if (this.Authorised)
            {
                KPRPCMessage data2client = new KPRPCMessage();
                data2client.protocol = "setup";
                data2client.srp = new SRPParams();
                data2client.version = ProtocolVersion;

                data2client.error = new Error(ErrorCode.AUTH_RESTART, new string[] { "Already authorised" });
                this.Authorised = false;

                string response = Jayrock.Json.Conversion.JsonConvert.ExportToString(data2client);
                this.WebSocketConnection.Send(response);

                return;
            }



            if (kprpcm.srp != null)
            {
                KPRPCMessage data2client = new KPRPCMessage();
                data2client.protocol = "setup";
                data2client.version = ProtocolVersion;

                int clientSecurityLevel = kprpcm.srp.securityLevel;

                if (clientSecurityLevel < securityLevelClientMinimum)
                {
                    data2client.error = new Error(ErrorCode.AUTH_CLIENT_SECURITY_LEVEL_TOO_LOW, new string[] { securityLevelClientMinimum.ToString() });
                    /* TODO1.3: need to disconnect/delete/reset this connection once we've decided we are not interested in letting the client connect. Maybe 
                     * tie in to finding a way to abort if user clicks a "cancel" button on the auth form.
                     */
                    this.WebSocketConnection.Send(Jayrock.Json.Conversion.JsonConvert.ExportToString(data2client));
                }
                else
                {
                    switch (kprpcm.srp.stage)
                    {
                        case "identifyToServer": this.WebSocketConnection.Send(SRPIdentifyToServer(kprpcm)); break;
                        case "proofToServer": this.WebSocketConnection.Send(SRPProofToServer(kprpcm)); break;
                        default: return;
                    }
                }
            }
            else
            {
                KPRPCMessage data2client = new KPRPCMessage();
                data2client.protocol = "setup";
                data2client.version = ProtocolVersion;

                int clientSecurityLevel = kprpcm.key.securityLevel;

                if (clientSecurityLevel < securityLevelClientMinimum)
                {
                    data2client.error = new Error(ErrorCode.AUTH_CLIENT_SECURITY_LEVEL_TOO_LOW, new string[] { securityLevelClientMinimum.ToString() });
                    /* TODO1.3: need to disconnect/delete/reset this connection once we've decided we are not interested in letting the client connect. Maybe 
                     * tie in to finding a way to abort if user clicks a "cancel" button on the auth form.
                     */
                    this.WebSocketConnection.Send(Jayrock.Json.Conversion.JsonConvert.ExportToString(data2client));
                }
                else
                {
                    if (!string.IsNullOrEmpty(kprpcm.key.username))
                    {
                        // confirm username
                        this.userName = kprpcm.key.username;
                        KeyContainerClass kc = this.KeyContainer;

                        if (kc == null)
                        {
                            this.userName = null;
                            data2client.error = new Error(ErrorCode.AUTH_FAILED, new string[] { "Stored key not found - Caused by changed Firefox profile or KeePass instance; changed OS user credentials; or KeePass config file may be corrupt" });
                            /* TODO1.3: need to disconnect/delete/reset this connection once we've decided we are not interested in letting the client connect. Maybe 
                             * tie in to finding a way to abort if user clicks a "cancel" button on the auth form.
                             */
                            this.WebSocketConnection.Send(Jayrock.Json.Conversion.JsonConvert.ExportToString(data2client));
                            return;
                        } 
                        if (kc.Username != this.userName)
                        {
                            this.userName = null;
                            data2client.error = new Error(ErrorCode.AUTH_FAILED, new string[] { "Username mismatch - KeePass config file is probably corrupt" });
                            /* TODO1.3: need to disconnect/delete/reset this connection once we've decided we are not interested in letting the client connect. Maybe 
                             * tie in to finding a way to abort if user clicks a "cancel" button on the auth form.
                             */
                            this.WebSocketConnection.Send(Jayrock.Json.Conversion.JsonConvert.ExportToString(data2client));
                            return;
                        }
                        if (kc.AuthExpires < DateTime.UtcNow)
                        {
                            this.userName = null;
                            data2client.error = new Error(ErrorCode.AUTH_EXPIRED);
                            /* TODO1.3: need to disconnect/delete/reset this connection once we've decided we are not interested in letting the client connect. Maybe 
                             * tie in to finding a way to abort if user clicks a "cancel" button on the auth form.
                             */
                            this.WebSocketConnection.Send(Jayrock.Json.Conversion.JsonConvert.ExportToString(data2client));
                            return;
                        }

                        this.WebSocketConnection.Send(Kcp.KeyChallengeResponse1(this.userName, securityLevel));
                    }
                    else if (!string.IsNullOrEmpty(kprpcm.key.cc) && !string.IsNullOrEmpty(kprpcm.key.cr))
                    {
                        bool authorised = false;
                        this.WebSocketConnection.Send(Kcp.KeyChallengeResponse2(kprpcm.key.cc, kprpcm.key.cr, KeyContainer, securityLevel, out authorised));
                        Authorised = authorised;
                        if (authorised)
                        {
                            // We assume the user has manually verified the client name as part of the initial SRP setup so it's fairly safe to use it to determine the type of client connection to which we want to promote our null connection
                            KPRPC.PromoteNullRPCClient(this, KeyContainer.ClientName);
                        }
                    }
                }
            }

  	    }

  	    string SRPIdentifyToServer (KPRPCMessage srpem)
        {
            SRPParams srp = srpem.srp;
            Error error;
            KPRPCMessage data2client = new KPRPCMessage();
            data2client.protocol = "setup";
            data2client.srp = new SRPParams();
            data2client.srp.stage = "identifyToClient";
            data2client.version = ProtocolVersion;

            // Generate a new random password
            // SRP isn't very susceptible to brute force attacks but we get 32 bits worth of randomness just in case
            byte[] password = Utils.GetRandomBytes(4);
            string plainTextPassword = Utils.GetTypeablePassword(password);

            // caclulate the hash of our randomly generated password
            _srp.CalculatePasswordHash(plainTextPassword);


            if (string.IsNullOrEmpty(srp.I))
            {
                data2client.error = new Error(ErrorCode.AUTH_MISSING_PARAM, new string[] { "I" });
            }
            else if (string.IsNullOrEmpty(srp.A))
            {
                data2client.error = new Error(ErrorCode.AUTH_MISSING_PARAM, new string[] { "A" });
            }
            else
            {

                // Init relevant SRP protocol variables
                _srp.Setup();

                // Begin the SRP handshake
                error = _srp.Handshake(srp.I, srp.A);

                if (error.code > 0)
                    data2client.error = error;
                else
                {
                    // store the username and client name for future reference
                    userName = _srp.I;
                    clientName = srpem.clientDisplayName;

                    data2client.srp.s = _srp.s;
                    data2client.srp.B = _srp.Bstr;

                    data2client.srp.securityLevel = securityLevel;

                    //pass the params through to the main kprpcext thread via begininvoke - that function will then create and show the form as a modal dialog
                    string secLevel = "low";
                    if (srp.securityLevel == 2)
                        secLevel = "medium";
                    else if (srp.securityLevel == 3)
                        secLevel = "high";
                    KPRPC.InvokeMainThread (new ShowAuthDialogDelegate(ShowAuthDialog), secLevel, srpem.clientDisplayName, srpem.clientDisplayDescription, plainTextPassword);
                }
            }
	    	    
            return Jayrock.Json.Conversion.JsonConvert.ExportToString(data2client);
  	    }

        private delegate void ShowAuthDialogDelegate(string securityLevel, string name, string description, string password);

        private delegate void HideAuthDialogDelegate();


        void ShowAuthDialog(string securityLevel, string name, string description, string password)
        {
            if (_authForm != null)
                _authForm.Hide();
            _authForm = new KeePassRPC.Forms.AuthForm(this, securityLevel, name, description, password);
            _authForm.Show();
        }

        void HideAuthDialog()
        {
            if (_authForm != null)
                _authForm.Hide();
        }

        public void ShuttingDown()
        {
            // Hide the auth dialog as long as we're not trying to shut down the main thread at the same time
            // (and as long as this isn't a v<1.2 connection)
            if (KPRPC != null && !KPRPC.terminating)
                KPRPC.InvokeMainThread(new HideAuthDialogDelegate(HideAuthDialog));
        }

        string SRPProofToServer(KPRPCMessage srpem)
        {
            SRPParams srp = srpem.srp;

            KPRPCMessage data2client = new KPRPCMessage();
            data2client.protocol = "setup";
            data2client.srp = new SRPParams();
            data2client.srp.stage = "proofToClient";
            data2client.version = ProtocolVersion;

            if (string.IsNullOrEmpty(srp.M))
            {
                data2client.error = new Error(ErrorCode.AUTH_MISSING_PARAM, new string[] { "M" });
            }
            else
            {
                _srp.Authenticate(srp.M);

                if (!_srp.Authenticated)
                    data2client.error = new Error(ErrorCode.AUTH_FAILED, new string[] { "Keys do not match" });
                else
                {
                    data2client.srp.M2 = _srp.M2;
                    data2client.srp.securityLevel = securityLevel;
                    KeyContainer = new KeyContainerClass(_srp.Key,DateTime.UtcNow.AddSeconds(KeyExpirySeconds),userName,clientName);
                    Authorised = true;
                    // We assume the user has checked the client name as part of the initial SRP setup so it's fairly safe to use it to determine the type of client connection to which we want to promote our null connection
                    KPRPC.PromoteNullRPCClient(this, clientName);
                    KPRPC.InvokeMainThread(new HideAuthDialogDelegate(HideAuthDialog));

                    // If we've never shown the user the welcome screen and have never
                    // known a KeeFox add-on from the previous KPRPC protocol, show it now
                    bool welcomeDisplayed = KPRPC._host.CustomConfig.GetBool("KeePassRPC.KeeFoxWelcomeDisplayed",false);
                    if (!welcomeDisplayed
                        && string.IsNullOrEmpty(KPRPC._host.CustomConfig.GetString("KeePassRPC.knownClients.KeeFox Firefox add-on")))
                        KPRPC.InvokeMainThread(new KeePassRPCExt.WelcomeKeeFoxUserDelegate(KPRPC.WelcomeKeeFoxUser));
                    if (!welcomeDisplayed)
                        KPRPC._host.CustomConfig.SetBool("KeePassRPC.KeeFoxWelcomeDisplayed",true);
                }
            }

            return Jayrock.Json.Conversion.JsonConvert.ExportToString(data2client);
  	    }

        void KPRPCReceiveJSONRPC(JSONRPCContainer jsonrpcEncrypted, KeePassRPCService service)
        {
            string jsonrpc = Decrypt(jsonrpcEncrypted);
            StringBuilder sb = new StringBuilder();

            JsonRpcDispatcher dispatcher = JsonRpcDispatcherFactory.CreateDispatcher(service);

            dispatcher.Process(new StringReader(jsonrpc),
                new StringWriter(sb), Authorised);
            string output = sb.ToString();

            KPRPCMessage data2client = new KPRPCMessage();
            data2client.protocol = "jsonrpc";
            data2client.version = ProtocolVersion;
            data2client.jsonrpc = Encrypt(output);

            // If there was a problem encrypting our message, respond to the
            // client with a non-encrypted error message
            if (data2client.jsonrpc == null)
            {
                data2client = new KPRPCMessage();
                data2client.protocol = "error";
                data2client.version = ProtocolVersion;
                data2client.error = new Error(ErrorCode.AUTH_RESTART, new string[] { "Encryption error" });
                this.Authorised = false;
                if (KPRPC.logger != null) KPRPC.logger.WriteLine("Encryption error when trying to reply to client message");
            }
            _webSocketConnection.Send(Jayrock.Json.Conversion.JsonConvert.ExportToString(data2client));
            
        }

        public JSONRPCContainer Encrypt(string plaintext)
        {
            if (string.IsNullOrEmpty(plaintext))
                return null;

            KeyContainerClass kc = this.KeyContainer;
            SHA1 sha = new SHA1CryptoServiceProvider();

            byte[] plaintextBytes = Encoding.UTF8.GetBytes(plaintext);

            // Encrypt the client's message
            RijndaelManaged myRijndael = new RijndaelManaged();
            myRijndael.GenerateIV();
            myRijndael.Key = KeePassLib.Utility.MemUtil.HexStringToByteArray(kc.Key);
            ICryptoTransform encryptor = myRijndael.CreateEncryptor();
            MemoryStream msEncrypt = new MemoryStream(100);
            CryptoStream cryptoStream = new CryptoStream(msEncrypt, encryptor, CryptoStreamMode.Write);

            try
            {
                cryptoStream.Write(plaintextBytes, 0, plaintextBytes.Length);
            }
            catch (ArgumentException)
            {
                //The sum of the count and offset parameters is longer than the length of the buffer.
                return null;
            }
            catch (NotSupportedException)
            {
                // Underlying stream does not support writing (not sure how this could happen)
                return null;
            }

            try
            {
                cryptoStream.FlushFinalBlock();
            }
            catch (NotSupportedException)
            {
                // 	The current stream is not writable. -or- The final block has already been transformed. 
                return null;
            }
            catch (CryptographicException)
            {
                // The key is corrupt which can cause invalid padding to the stream. 
                return null;
            }

            byte[] encrypted = msEncrypt.ToArray();
            
            // Get the raw bytes that are used to calculate the HMAC

            byte[] HmacKey = sha.ComputeHash(myRijndael.Key);
            byte[] ourHmacSourceBytes = new byte[HmacKey.Length + encrypted.Length + myRijndael.IV.Length];

            // These calls can throw a variety of different exceptions but
            // I can't see why they would so we will not try to differentiate the cause of them
            try
            {
                //TODO2: HMAC calculations might be stengthened against attacks on SHA 
                // and/or gain improved performance through use of algorithms like AES-CMAC or HKDF

                Array.Copy(HmacKey, ourHmacSourceBytes, HmacKey.Length);
                Array.Copy(encrypted, 0, ourHmacSourceBytes, HmacKey.Length, encrypted.Length);
                Array.Copy(myRijndael.IV, 0, ourHmacSourceBytes, HmacKey.Length + encrypted.Length, myRijndael.IV.Length);

                // Calculate the HMAC
                byte[] ourHmac = sha.ComputeHash(ourHmacSourceBytes);

                // Package the data ready for transmission
                JSONRPCContainer cont = new JSONRPCContainer();
                cont.iv = Convert.ToBase64String(myRijndael.IV);
                cont.message = Convert.ToBase64String(encrypted);
                cont.hmac = Convert.ToBase64String(ourHmac);

                return cont;
            }
            catch (ArgumentNullException)
            {
                return null;
            }
            catch (RankException)
            {
                return null;
            }
            catch (ArrayTypeMismatchException)
            {
                return null;
            }
            catch (ArgumentOutOfRangeException)
            {
                return null;
            }
            catch (ArgumentException)
            {
                return null;
            }
            catch (ObjectDisposedException)
            {
                return null;
            }
        }

        public string Decrypt(JSONRPCContainer jsonrpcEncrypted)
        {
            if (string.IsNullOrEmpty(jsonrpcEncrypted.message)
                || string.IsNullOrEmpty(jsonrpcEncrypted.iv)
                || string.IsNullOrEmpty(jsonrpcEncrypted.hmac))
                return null;

            KeyContainerClass kc = this.KeyContainer;
            SHA1 sha = new SHA1CryptoServiceProvider();

            byte[] rawKeyBytes;
            byte[] keyBytes;
            byte[] messageBytes;
            byte[] IVBytes;

            // Get the raw bytes that are used to calculate the HMAC
            try
            {
                rawKeyBytes = KeePassLib.Utility.MemUtil.HexStringToByteArray(kc.Key);
                keyBytes = sha.ComputeHash(rawKeyBytes);
                messageBytes = Convert.FromBase64String(jsonrpcEncrypted.message);
                IVBytes = Convert.FromBase64String(jsonrpcEncrypted.iv);
            }
            catch (FormatException)
            {
                // Should only happen if there is a fault with the client end
                // of the protocol or if an attacker tries to inject invalid data
                return null;
            }
            catch (ArgumentNullException)
            {
                // kc.Key must = null
                return null;
            }

            // These calls can throw a variety of different exceptions but
            // I can't see why they would so we will not try to differentiate the cause of them
            try
            {
                byte[] ourHmacSourceBytes = new byte[keyBytes.Length + messageBytes.Length + IVBytes.Length];
                Array.Copy(keyBytes, ourHmacSourceBytes, keyBytes.Length);
                Array.Copy(messageBytes, 0, ourHmacSourceBytes, keyBytes.Length, messageBytes.Length);
                Array.Copy(IVBytes, 0, ourHmacSourceBytes, keyBytes.Length + messageBytes.Length, IVBytes.Length);

                // Calculate the HMAC
                byte[] ourHmac = sha.ComputeHash(ourHmacSourceBytes);

                // Check our HMAC against the one supplied by the client
                if (Convert.ToBase64String(ourHmac) != jsonrpcEncrypted.hmac)
                {
                    //TODO2: If we ever want/need to include some DOS protection we
                    // could use this error condition to throttle requests from badly behaved clients
                    if (KPRPC.logger != null) KPRPC.logger.WriteLine("HMAC did not match");
                    return null;
                }
            }
            catch (ArgumentNullException)
            {
                return null;
            }
            catch (RankException)
            {
                return null;
            }
            catch (ArrayTypeMismatchException)
            {
                return null;
            }
            catch (ArgumentOutOfRangeException)
            {
                return null;
            }
            catch (ArgumentException)
            {
                return null;
            }
            catch (ObjectDisposedException)
            {
                return null;
            }

            // Decrypt the client's message
            RijndaelManaged myRijndael = new RijndaelManaged();
            ICryptoTransform decryptor = myRijndael.CreateDecryptor(rawKeyBytes, IVBytes);
            MemoryStream msDecrypt = new MemoryStream();
            CryptoStream cryptoStream = new CryptoStream(msDecrypt, decryptor, CryptoStreamMode.Write);

            try
            {
                cryptoStream.Write(messageBytes, 0, messageBytes.Length);
            }
            catch (ArgumentException)
            {
                //The sum of the count and offset parameters is longer than the length of the buffer.
                return null;
            }
            catch (NotSupportedException)
            {
                // Underlying stream does not support writing (not sure how this could happen)
                return null;
            }

            try
            {
                cryptoStream.FlushFinalBlock();
            }
            catch (NotSupportedException)
            {
                // 	The current stream is not writable. -or- The final block has already been transformed. 
                return null;
            }
            catch (CryptographicException)
            {
                // The key is corrupt which can cause invalid padding to the stream. 
                return null;
            }

            byte[] decrypted = msDecrypt.ToArray();
            string result = Encoding.UTF8.GetString(decrypted);
            return result;
        }

    }

    /// <summary>
    /// Tracks requests from RPC clients while they are being authorised
    /// </summary>
    public class PendingRPCClient
    {
        public string ClientId;
        public string Hash;
        public List<string> KnownClientList;

        public PendingRPCClient(string clientId, string hash, List<string> knownClientList)
        {
            ClientId = clientId;
            Hash = hash;
            KnownClientList = knownClientList;
        }
    }

}

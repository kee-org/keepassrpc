/*
  KeePassRPC - Uses JSON-RPC to provide RPC facilities to KeePass.
  Example usage includes the KeeFox firefox extension.
  
  Copyright 2010-2015 Chris Tomlinson <keefox@christomlinson.name>

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

using System.Collections.Generic;
using KeePassRPC.Forms;
using System.Windows.Forms;
using System.Drawing;
using KeePassLib;
using KeePassLib.Collections;
using KeePass.UI;
using KeePass.Forms;

namespace KeePassRPC
{
    /// <summary>
    /// Base class for all RPCClient managers.
    /// </summary>
    public abstract class KeePassRPCClientManager
    {
        private string _name;
        private string _callbackMethodName;
        public string Name { get { return _name; } private set { _name = value; } }
        public string CallbackMethodName { get { return _callbackMethodName; } private set { _callbackMethodName = value; } }
        private List<KeePassRPCClientConnection> _RPCClientConnections = new List<KeePassRPCClientConnection>(1);
        private static object _lockRPCClients = new object();

        public KeePassRPCClientManager(string name, string callbackName)
        {
            Name = name;
            CallbackMethodName = callbackName;
        }

        private KeePassRPCClientManager()
        {
        }

        /// <summary>
        /// Signals all clients.
        /// </summary>
        /// <param name="signal">The signal.</param>
        public virtual void SignalAll(KeePassRPC.DataExchangeModel.Signal signal)
        {
            foreach (KeePassRPCClientConnection client in _RPCClientConnections)
                client.Signal(signal, CallbackMethodName);
        }

        /// <summary>
        /// Adds an RPC client.
        /// </summary>
        /// <param name="client">The client.</param>
        public void AddRPCClientConnection(KeePassRPCClientConnection client)
        {
            lock (_lockRPCClients)
            {
                _RPCClientConnections.Add(client);
            }
        }

        /// <summary>
        /// Removes an RPC client.
        /// </summary>
        /// <param name="client">The client.</param>
        public void RemoveRPCClientConnection(KeePassRPCClientConnection client)
        {
            lock (_lockRPCClients)
            {
                client.ShuttingDown();
                _RPCClientConnections.Remove(client);
            }
        }

        /// <summary>
        /// Gets the current RPC clients. ACTUAL client list may change immediately after this array is returned.
        /// </summary>
        /// <value>The current RPC clients.</value>
        public KeePassRPCClientConnection[] CurrentRPCClientConnections
        {
            get
            {
                lock (_lockRPCClients)
                {
                    KeePassRPCClientConnection[] clients = new KeePassRPCClientConnection[_RPCClientConnections.Count];
                    _RPCClientConnections.CopyTo(clients);
                    return clients;
                }
            }
        }

        /// <summary>
        /// Terminates this server.
        /// </summary>
        public void Terminate()
        {
            lock (_lockRPCClients)
            {
                SignalAll(KeePassRPC.DataExchangeModel.Signal.EXITING);
                _RPCClientConnections.Clear();
            }
        }

        public virtual void AttachToEntryDialog(KeePassRPCExt plugin, PwEntry entry, TabControl mainTabControl, PwEntryForm form, CustomListViewEx advancedListView, ProtectedStringDictionary strings)
        {
            return;
        }

        public virtual void AttachToGroupDialog(KeePassRPCExt plugin, PwGroup group, TabControl mainTabControl)
        {
            return;
        }


    }

    public class NullRPCClientManager : KeePassRPCClientManager
    {
        public NullRPCClientManager()
            : base("Null", "NULL")
        {

        }

        /// <summary>
        /// We don't know how to signal null clients so we skip it
        /// </summary>
        /// <param name="signal">The signal.</param>
        public override void SignalAll(KeePassRPC.DataExchangeModel.Signal signal)
        {
            return;
        }

    }

    public class KeeFoxRPCClientManager : KeePassRPCClientManager
    {
        public KeeFoxRPCClientManager()
            : base("KeeFox", "KPRPCListener")
        {

        }

        public override void AttachToEntryDialog(KeePassRPCExt plugin, PwEntry entry, TabControl mainTabControl, PwEntryForm form, CustomListViewEx advancedListView, ProtectedStringDictionary strings)
        {
            KeeFoxEntryUserControl entryControl = new KeeFoxEntryUserControl(plugin, entry, advancedListView, form, strings);
            TabPage keefoxTabPage = new TabPage("KeeFox");
            entryControl.Dock = DockStyle.Fill;
            keefoxTabPage.Controls.Add(entryControl);
            if (mainTabControl.ImageList == null)
                mainTabControl.ImageList = new ImageList();
            int imageIndex = mainTabControl.ImageList.Images.Add(global::KeePassRPC.Properties.Resources.KeeFox16, Color.Transparent);
            keefoxTabPage.ImageIndex = imageIndex;
            mainTabControl.TabPages.Add(keefoxTabPage);
        }

        public override void AttachToGroupDialog(KeePassRPCExt plugin, PwGroup group, TabControl mainTabControl)
        {
            KeeFoxGroupUserControl groupControl = new KeeFoxGroupUserControl(plugin, group);
            TabPage keefoxTabPage = new TabPage("KeeFox");
            groupControl.Dock = DockStyle.Fill;
            keefoxTabPage.Controls.Add(groupControl);
            if (mainTabControl.ImageList == null)
                mainTabControl.ImageList = new ImageList();
            int imageIndex = mainTabControl.ImageList.Images.Add(global::KeePassRPC.Properties.Resources.KeeFox16, Color.Transparent);
            keefoxTabPage.ImageIndex = imageIndex;
            mainTabControl.TabPages.Add(keefoxTabPage);
        }
    }
}
using System.Collections.Generic;
using KeePassRPC.Forms;
using System.Windows.Forms;
using System.Drawing;
using KeePassLib;
using KeePassLib.Collections;
using KeePass.UI;
using KeePass.Forms;
using System.Reflection;
using KeePassRPC.Models.DataExchange;

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
        public virtual void SignalAll(Signal signal)
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
                SignalAll(Signal.EXITING);
                _RPCClientConnections.Clear();
            }
        }

        public virtual void AttachToEntryDialog(KeePassRPCExt plugin, PwEntry entry, TabControl mainTabControl, PwEntryForm form, CustomListViewEx advancedListView, ProtectedStringDictionary strings, StringDictionaryEx customData)
        {
            return;
        }

        public virtual void AttachToGroupDialog(KeePassRPCExt plugin, PwGroup group, TabControl mainTabControl)
        {
            return;
        }


    }

    public class GeneralRPCClientManager : KeePassRPCClientManager
    {
        public GeneralRPCClientManager()
            : base("General", "KPRPCListener")
        {

        }

        public override void AttachToEntryDialog(KeePassRPCExt plugin, PwEntry entry, TabControl mainTabControl, PwEntryForm form, CustomListViewEx advancedListView, ProtectedStringDictionary strings, StringDictionaryEx customData)
        {
            UserControl entryControl;

            string mvString = KeePass.Util.MultipleValues.MultipleValuesEx.CueString;
            string json1 = strings.ReadSafe("KPRPC JSON");
            string json2 = customData.Get("KPRPC JSON");
            if ((!string.IsNullOrEmpty(json1) && mvString == json1) || (!string.IsNullOrEmpty(json2) && mvString == json2))
            {
                entryControl = new KeeMultiEntryUserControl();
            } else
            {
                entryControl = new KeeEntryUserControl(plugin, entry, advancedListView, form, strings, customData);
            }

            TabPage keeTabPage = new TabPage("Kee");
            entryControl.Dock = DockStyle.Fill;
            keeTabPage.Controls.Add(entryControl);
            if (mainTabControl.ImageList == null)
                mainTabControl.ImageList = new ImageList();
            int imageIndex = mainTabControl.ImageList.Images.Add(global::KeePassRPC.Properties.Resources.KPRPC64, Color.Transparent);
            keeTabPage.ImageIndex = imageIndex;
            mainTabControl.TabPages.Add(keeTabPage);
        }

        public override void AttachToGroupDialog(KeePassRPCExt plugin, PwGroup group, TabControl mainTabControl)
        {
            KeeGroupUserControl groupControl = new KeeGroupUserControl(plugin, group);
            TabPage keeTabPage = new TabPage("Kee");
            groupControl.Dock = DockStyle.Fill;
            keeTabPage.Controls.Add(groupControl);
            if (mainTabControl.ImageList == null)
                mainTabControl.ImageList = new ImageList();
            int imageIndex = mainTabControl.ImageList.Images.Add(global::KeePassRPC.Properties.Resources.KPRPC64, Color.Transparent);
            keeTabPage.ImageIndex = imageIndex;
            mainTabControl.TabPages.Add(keeTabPage);
        }

    }

    public class KeeFoxRPCClientManager : KeePassRPCClientManager
    {
        public KeeFoxRPCClientManager()
            : base("KeeFox", "KPRPCListener")
        {

        }

        public override void AttachToEntryDialog(KeePassRPCExt plugin, PwEntry entry, TabControl mainTabControl, PwEntryForm form, CustomListViewEx advancedListView, ProtectedStringDictionary strings, StringDictionaryEx customData)
        {
            KeeEntryUserControl entryControl = new KeeEntryUserControl(plugin, entry, advancedListView, form, strings, customData);
            TabPage keefoxTabPage = new TabPage("KeeFox");
            entryControl.Dock = DockStyle.Fill;
            keefoxTabPage.Controls.Add(entryControl);
            if (mainTabControl.ImageList == null)
                mainTabControl.ImageList = new ImageList();
            int imageIndex = mainTabControl.ImageList.Images.Add(global::KeePassRPC.Properties.Resources.KPRPC64, Color.Transparent);
            keefoxTabPage.ImageIndex = imageIndex;
            mainTabControl.TabPages.Add(keefoxTabPage);
        }

        public override void AttachToGroupDialog(KeePassRPCExt plugin, PwGroup group, TabControl mainTabControl)
        {
            KeeGroupUserControl groupControl = new KeeGroupUserControl(plugin, group);
            TabPage keefoxTabPage = new TabPage("KeeFox");
            groupControl.Dock = DockStyle.Fill;
            keefoxTabPage.Controls.Add(groupControl);
            if (mainTabControl.ImageList == null)
                mainTabControl.ImageList = new ImageList();
            int imageIndex = mainTabControl.ImageList.Images.Add(global::KeePassRPC.Properties.Resources.KPRPC64, Color.Transparent);
            keefoxTabPage.ImageIndex = imageIndex;
            mainTabControl.TabPages.Add(keefoxTabPage);
        }
    }
}
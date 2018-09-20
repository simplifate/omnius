using System;
using System.Collections.Generic;
using System.Collections.Specialized;
using System.Linq;
using System.Reactive.Concurrency;
using System.Reactive.Linq;
using System.Text;
using System.Threading.Tasks;
using System.Web.Mvc;
using FSS.Omnius.Modules.CORE;
using FSS.Omnius.Modules.Entitron.Entity;
using FSS.Omnius.Modules.Entitron.Entity.Nexus;
using FSS.Omnius.Modules.Entitron.Entity.Persona;
using FSS.Omnius.Modules.Entitron.Entity.Tapestry;
using FSS.Omnius.Modules.Watchtower;
using ReactiveSockets;


namespace FSS.Omnius.Modules.Nexus.Service
{
    public class TCPSocketListenerService
    {
        static Dictionary<string, ReactiveListener> listeners = new Dictionary<string, ReactiveListener>(); 

        public static void AddListener(TCPSocketListener model)
        {
            if(listeners.ContainsKey(model.Name)) {
                listeners[model.Name].Dispose();
                listeners.Remove(model.Name);
            }
           
            ReactiveListener server = new ReactiveListener(model.Port);
            server.Connections.Subscribe(socket => new Observer(socket, model.Name, model.BufferSize));
                
            server.Start();

            listeners.Add(model.Name, server);
        }

        public static void RemoveListener(string name)
        {
            if(listeners.ContainsKey(name)) {
                listeners[name].Dispose();
                listeners.Remove(name);
            }
        }
        
        public static void onNewMessage(byte[] body, string listenerName)
        {
            var core = COREobject.i;
            DBEntities context = core.Context;
            TCPSocketListener listener = context.TCPListeners.Where(l => l.Name == listenerName).FirstOrDefault();
            if(listener != null) {
                Block block = GetBlockWithWF(context, listener.ApplicationId, listener.BlockName.RemoveDiacritics());

                if (block != null) {
                    core.Application = listener.Application;

                    try {
                        PersonaAppRole role = context.AppRoles.FirstOrDefault(r => r.Name == "System" && r.ApplicationId == listener.ApplicationId);
                        core.User = context.Users.FirstOrDefault(u => u.Users_Roles.Any(r => r.RoleName == role.Name && r.ApplicationId == role.ApplicationId));
                    }
                    catch (Exception e) {
                        OmniusInfo.Log($"Chyba při zpracování socketu: {listenerName} ({e})", OmniusLogSource.Nexus, null, null);
                    }

                    OmniusInfo.Log($"Začátek zpracování socketu: {listener.Name} / Blok {listener.BlockName} / Button {listener.WorkflowName}", OmniusLogSource.Nexus, listener.Application, core.User);
                        
                    FormCollection fc = new FormCollection();
                    Dictionary<string, object> vars = new Dictionary<string, object>();
                    vars.Add("__SocketRequestBody__", body);

                    var runResult = new Modules.Tapestry.Tapestry(core).run(block, listener.WorkflowName, -1, fc, 0, null, vars);

                    OmniusInfo.Log($"Konec zpraconání mailu: {listener.Name} / Blok {listener.BlockName} / Button {listener.WorkflowName}", OmniusLogSource.Hermes, listener.Application, core.User);
                }
            }
        }

        public static void Refresh()
        {
            using (DBEntities e = new DBEntities()) {

                if (listeners.Count > 0) {
                    foreach (KeyValuePair<string, ReactiveListener> listener in listeners) {
                        listener.Value.Dispose();
                    }
                    listeners.Clear();
                }

                foreach (TCPSocketListener listener in e.TCPListeners) {
                    AddListener(listener);
                }
            }
        }
        
        private static Block GetBlockWithWF(DBEntities context, int appId, string blockName)
        {
            return context.Blocks.FirstOrDefault(b => b.WorkFlow.ApplicationId == appId && b.Name == blockName);
        }
    }

    class Observer : IObserver<ReactiveSocket>
    {
        private ReactiveSocket socket;
        private string listenerName;
        private int bufferSize;

        public Observer(ReactiveSocket socket, string listenerName, int bufferSize)
        {
            this.socket = socket;
            this.listenerName = listenerName;
            this.bufferSize = bufferSize;

            var protocol = new ByteChannel(socket, bufferSize);

            // Here we hook the "echo" prototocol
            protocol.Receiver.Subscribe(
                b => {
                    try {
                        TCPSocketListenerService.onNewMessage(b, listenerName);
                    }
                    catch( Exception e) {
                        OmniusInfo.Log($"Chyba při zpracování socketu: {listenerName} ({e})", OmniusLogSource.Nexus, null, null);
                    }
                },
                e => {
                    OmniusInfo.Log($"Chyba při zpracování socketu: {listenerName} ({e})", OmniusLogSource.Nexus, null, null);
                }
            );
        }

        public void OnData(byte[] data) {
            TCPSocketListenerService.onNewMessage(data, listenerName);
        }
        public void OnNext(ReactiveSocket socket) {}
        public void OnError(Exception e) { }
        public void OnCompleted() { }
    }

    class ByteChannel
    {
        public IObservable<byte[]> Receiver { get; private set; }
        private IReactiveSocket socket;
        
        public ByteChannel(IReactiveSocket socket, int bufferSize)
        {
            this.socket = socket;
            Receiver = from body in socket.Receiver.Buffer(bufferSize) select body.Take(bufferSize).ToArray();
        }
    }
}

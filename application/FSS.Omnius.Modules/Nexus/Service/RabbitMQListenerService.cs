using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using System.Timers;
using System.Web.Mvc;
using FSS.Omnius.Modules.Entitron.Entity;
using FSS.Omnius.Modules.Entitron.Entity.Nexus;
using FSS.Omnius.Modules.Entitron.Entity.Persona;
using FSS.Omnius.Modules.Entitron.Entity.Tapestry;
using FSS.Omnius.Modules.Watchtower;
using RabbitMQ.Client;
using RabbitMQ.Client.Events;

namespace FSS.Omnius.Modules.Nexus.Service
{
    class RabbitMQListener
    {
        public EventingBasicConsumer Consumer { get; set; }
        public IModel Channel { get; set; }
        public IConnection Connection { get; set; }
        public IConnectionFactory Factory { get; set; }
    }

    public class RabbitMQListenerService
    {
        private static Dictionary<string, RabbitMQListener> listeners = new Dictionary<string, RabbitMQListener>();
        private static List<Entitron.Entity.Nexus.RabbitMQ> failedConnetionList = new List<Entitron.Entity.Nexus.RabbitMQ>();
        private static bool reconnectIntervalSet = false;
        private static Timer reconnectTimer;

        public RabbitMQListenerService()
        {
            var context = DBEntities.instance;


            var factory = new ConnectionFactory() { HostName = "localhost" };
            using (var connection = factory.CreateConnection())
            using (var channel = connection.CreateModel()) {
         

                var consumer = new EventingBasicConsumer(channel);
                consumer.Received += (model, ea) => {
                    var body = ea.Body;
                    var message = Encoding.UTF8.GetString(body);
                    Console.WriteLine(" [x] Received {0}", message);
                };
            }
        }

        public static void onNewMessage(object model, BasicDeliverEventArgs args)
        {
            var body = args.Body;
            var message = Encoding.UTF8.GetString(body);

            using (DBEntities db = new DBEntities()) {
                Entitron.Entity.Nexus.RabbitMQ listener = db.RabbitMQs.Where(q => q.Name == args.ConsumerTag).FirstOrDefault();
                if (listener != null) {
                    Block block = GetBlockWithWF(db, listener.ApplicationId.Value, listener.BlockName.RemoveDiacritics());

                    if (block != null) {
                        var core = new CORE.CORE();
                        Entitron.Entitron.Create(listener.Application);

                        try {
                            PersonaAppRole role = db.AppRoles.FirstOrDefault(r => r.Name == "System" && r.ApplicationId == listener.ApplicationId);
                            core.User = db.Users.FirstOrDefault(u => u.Users_Roles.Any(r => r.RoleName == role.Name && r.ApplicationId == role.ApplicationId));

                            OmniusInfo.Log($"Začátek zpracování RabbitMQ: {listener.Name} / Blok {listener.BlockName} / Button {listener.WorkflowName}", OmniusLogSource.Nexus, listener.Application, core.User);

                            FormCollection fc = new FormCollection();
                            Dictionary<string, object> vars = new Dictionary<string, object>();
                            vars.Add("__RabbitMQMessage__", message);

                            var runResult = core.Tapestry.run(core.User, block, listener.WorkflowName, -1, fc, 0, null, vars);

                            OmniusInfo.Log($"Konec zpracování RabbitMQ: {listener.Name} / Blok {listener.BlockName} / Button {listener.WorkflowName}", OmniusLogSource.Hermes, listener.Application, core.User);

                            if (runResult.Item1.Errors.Count == 0) {
                                listeners[args.ConsumerTag].Channel.BasicAck(args.DeliveryTag, false);
                            }
                        }
                        catch (Exception e) {
                            OmniusInfo.Log($"Chyba při zpracování RabbitMQ: {args.ConsumerTag} ({e})", OmniusLogSource.Nexus, null, null);
                        }
                    }
                }
            }
        }

        #region Listeners management

        public static void AddListener(Entitron.Entity.Nexus.RabbitMQ model)
        {
            if (listeners.ContainsKey(model.Name)) {
                listeners[model.Name].Channel.Close();
                listeners[model.Name].Channel.Dispose();
                listeners[model.Name].Connection.Close();
                listeners[model.Name].Connection.Dispose();
                listeners.Remove(model.Name);
            }

            RabbitMQListener listener = new RabbitMQListener() {
                Factory = new ConnectionFactory() { HostName = model.HostName, Port = model.Port }
            };

            if(!string.IsNullOrEmpty(model.Password) && !string.IsNullOrEmpty(model.UserName)) {
                listener.Factory.UserName = model.UserName;
                listener.Factory.Password = model.Password;
            }

            try {
                listener.Connection = listener.Factory.CreateConnection();
                listener.Channel = listener.Connection.CreateModel();
                listener.Consumer = new EventingBasicConsumer(listener.Channel);
                listener.Consumer.Received += onNewMessage;

                listener.Channel.BasicConsume(model.QueueName, false, model.Name, listener.Consumer);
                listener.Channel.BasicQos(0, 1, false);

                listeners.Add(model.Name, listener);

                if(failedConnetionList.Contains(model)) {
                    failedConnetionList.Remove(model);
                }
            }
            catch(Exception) {
                if (!failedConnetionList.Contains(model)) {
                    failedConnetionList.Add(model);
                }
            }

            if(failedConnetionList.Count > 0 && !reconnectIntervalSet) {
                SetInterval();
            }
        }

        private static void TryReconnect(Object source, ElapsedEventArgs args)
        {
            List<Entitron.Entity.Nexus.RabbitMQ> tempList = new List<Entitron.Entity.Nexus.RabbitMQ>();
            tempList.AddRange(failedConnetionList);

            foreach(var model in tempList) {
                AddListener(model);
            }

            if(failedConnetionList.Count == 0) {
                reconnectIntervalSet = false;
                reconnectTimer.Enabled = false;
            }
        }

        private static void SetInterval()
        {
            reconnectTimer = new Timer(600000);

            reconnectTimer.Elapsed += TryReconnect;
            reconnectTimer.AutoReset = true;
            reconnectTimer.Enabled = true;
            reconnectIntervalSet = true;
        }

        public static void RemoveListener(string name)
        {
            if (listeners.ContainsKey(name)) {
                listeners[name].Channel.Close();
                listeners[name].Channel.Dispose();
                listeners[name].Connection.Close();
                listeners[name].Connection.Dispose();
                listeners.Remove(name);
            }
        }

        public static void Refresh()
        {
            using (DBEntities e = new DBEntities()) 
            {
                if (listeners.Count > 0) {
                    foreach (KeyValuePair<string, RabbitMQListener> listener in listeners) {
                        var l = listener.Value;
                        l.Channel.Close();
                        l.Channel.Dispose();
                        l.Connection.Close();
                        l.Connection.Dispose();
                    }
                    listeners.Clear();
                }

                foreach (Entitron.Entity.Nexus.RabbitMQ listener in e.RabbitMQs.Where(q => q.Type == ChannelType.RECEIVE)) {
                    AddListener(listener);
                }
            }
        }

        #endregion

        #region helpers

        private static Block GetBlockWithWF(DBEntities context, int appId, string blockName)
        {
            return context.Blocks
                .Include("SourceTo_ActionRules")
                .FirstOrDefault(b => b.WorkFlow.ApplicationId == appId && b.Name.ToLower() == blockName.ToLower());
        }

        #endregion
    }
}

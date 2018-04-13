using FSS.Omnius.Modules.CORE;
using FSS.Omnius.Modules.Nexus.Service;
using FSS.Omnius.Modules.Tapestry.Actions.Nexus;
using N = FSS.Omnius.Modules.Entitron.Entity.Nexus;
using System;
using System.Collections.Generic;
using FSS.Omnius.Modules.Entitron.Entity;
using System.Linq;
using Newtonsoft.Json.Linq;

namespace FSS.Omnius.Modules.Tapestry.Actions.Entitron
{
    [NexusRepository]
    public class SendToRabbitMQAction : Action
    {
        private static Dictionary<string, CachedResult> cache = new Dictionary<string, CachedResult>();

        public override int Id => 3008;

        public override string[] InputVar => new string[] { "Message" };

        public override string Name => "Send to RabbitMQ";

        public override string[] OutputVar => new string[] { "Result", "Error" };

        public override int? ReverseActionId => null;

        public override void InnerRun(Dictionary<string, object> vars, Dictionary<string, object> outputVars, Dictionary<string, object> InvertedInputVars, Message message)
        {
            try {
                CORE.CORE core = (CORE.CORE)vars["__CORE__"];
                var context = DBEntities.appInstance(core.Application);

                string rabbitMQName = (string)vars["rabbitMQ"];
                object messageObject = vars["Message"];

                if(string.IsNullOrEmpty(rabbitMQName)) {
                    throw new Exception($"{Name}: Please attach RabbitMQ integration.");
                }

                if(messageObject == null) {
                    throw new Exception($"{Name}: Message must not be null.");
                }

                N.RabbitMQ mq = context.RabbitMQs.SingleOrDefault(q => q.Name == rabbitMQName);
                if(mq == null) {
                    throw new Exception($"{Name}: Requested RabbitMQ {rabbitMQName} was not found");
                }

                RabbitMQService service = new RabbitMQService(mq);
                
                string body;
                if (messageObject is JToken) {
                    body = messageObject.ToString();
                }
                else {
                    body = Convert.ToString(messageObject);
                }

                service.Send(body);
                service.Close();
                
                outputVars["Result"] = true;
                outputVars["Error"] = "";
            }
            catch(Exception e) {
                outputVars["Result"] = false;
                outputVars["Error"] = e.Message;
            }
        }
    }
}

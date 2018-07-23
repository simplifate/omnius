using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using FSS.Omnius.Modules.CORE;
using FSS.Omnius.Modules.Hermes;

namespace FSS.Omnius.Modules.Tapestry.Actions.Hermes
{
    public class RunMailerAction : Action
    {
        public override int Id => 2010;

        public override string[] InputVar => new string[] { "?serverName" };

        public override string[] OutputVar => new string[] { };

        public override int? ReverseActionId => null;

        public override string Name => "Run mailer";

        public override void InnerRun(Dictionary<string, object> vars, Dictionary<string, object> outputVars, Dictionary<string, object> InvertedInputVars, Message message)
        {
            string serverName = vars.ContainsKey("serverName")
                ? (string)vars["serverName"]
                : "Test";

            Mailer mailer = new Mailer(serverName);
            mailer.RunSender();
        }
    }
}

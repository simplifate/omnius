using FSS.Omnius.Modules.CORE;
using FSS.Omnius.Modules.Hermes;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace FSS.Omnius.Modules.Tapestry.Actions.Mozaic
{
    [MozaicRepository]
    class SendMailAction : Action
    {
        public override int Id
        {
            get {
                return 2005;
            }
        }
        public override int? ReverseActionId
        {
            get {
                return null;
            }
        }
        public override string[] InputVar
        {
            get {
                return new string[] { "Recipients", "Subject", "Template", "?CC", "?BCC", "?Data" };
            }
        }
        public override string[] OutputVar
        {
            get {
                return new string[] { "Result", "ErrorMessage" };
            }
        }
        public override string Name
        {
            get {
                return "Send mail";
            }
        }

        public override void InnerRun(Dictionary<string, object> vars, Dictionary<string, object> outputVars, Dictionary<string, object> InvertedInputVars, Message message)
        {
            var dataDictionary = new Dictionary<string, object>();
            if (vars.ContainsKey("Data"))
                dataDictionary = (Dictionary<string, object>)vars["Data"];

            Mailer mail = new Mailer("", (string)vars["Template"], dataDictionary);
            mail.To((Dictionary<string, string>)vars["Recipients"]);
            mail.SendMail();
        }
    }
}

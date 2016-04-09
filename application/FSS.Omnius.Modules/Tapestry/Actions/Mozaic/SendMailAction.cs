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
                return new string[] { "Recipients", "CC", "BCC", "Subject", "Template" };
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
            Mailer mail = new Mailer(null, (string)vars["Template"], new object());
            mail.Subject((string)vars["Subject"]);
            mail.To((Dictionary<string, string>)vars["Recipients"]);
            mail.SendMail();
        }
    }
}

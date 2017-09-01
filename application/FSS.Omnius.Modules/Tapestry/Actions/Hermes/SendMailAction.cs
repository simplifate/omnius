using FSS.Omnius.Modules.CORE;
using FSS.Omnius.Modules.Entitron.Entity;
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
                return new string[] { "?Recipients", "Subject", "Template", "?CC", "?BCC", "?Data" };
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
            var context = DBEntities.instance;
            var dataDictionary = new Dictionary<string, object>();
            if (vars.ContainsKey("Data"))
                dataDictionary = (Dictionary<string, object>)vars["Data"];
            string templateName = (string)vars["Template"];

            Mailer czechMailer = new Mailer("", templateName, dataDictionary, 1);
            Mailer englishMailer = new Mailer("", templateName, dataDictionary, 2);

            if (vars.ContainsKey("Recipients"))
            {
                var recipientsInputDict = (Dictionary<string, string>)vars["Recipients"];

                var recipientsCzechOutput = new Dictionary<string, string>();
                var recipientsEnglishOutput = new Dictionary<string, string>();

                foreach (var addressPair in recipientsInputDict)
                {
                    var user = context.Users.Where(u => u.Email == addressPair.Key).FirstOrDefault();
                    if (user != null && user.LocaleId == 2)
                        recipientsEnglishOutput.Add(addressPair.Key, addressPair.Value);
                    else
                        recipientsCzechOutput.Add(addressPair.Key, addressPair.Value);
                }

                czechMailer.BCC(recipientsCzechOutput);
                englishMailer.BCC(recipientsEnglishOutput);
            }
            if (vars.ContainsKey("BCC")) {
                var bccInputDict = (Dictionary<string, string>)vars["BCC"];

                var bccCzechOutput = new Dictionary<string, string>();
                var bccEnglishOutput = new Dictionary<string, string>();

                foreach (var addressPair in bccInputDict)
                {
                    var user = context.Users.Where(u => u.Email == addressPair.Key).FirstOrDefault();
                    if (user != null && user.LocaleId == 2)
                        bccEnglishOutput.Add(addressPair.Key, addressPair.Value);
                    else
                        bccCzechOutput.Add(addressPair.Key, addressPair.Value);
                }

                czechMailer.BCC(bccCzechOutput);
                englishMailer.BCC(bccEnglishOutput);
            }

            czechMailer.SendBySender();
            englishMailer.SendBySender();
        }
    }
}

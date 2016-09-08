using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using FSS.Omnius.Modules.CORE;
using FSS.Omnius.Modules.Entitron;
using FSS.Omnius.Modules.Hermes;

namespace FSS.Omnius.Modules.Tapestry.Actions.other
{
    [OtherRepository]
    class SendMailForEachAction : Action
    {
        public override int Id
        {
            get
            {
                return 193;
            }
        }
        public override int? ReverseActionId
        {
            get
            {
                return null;
            }
        }
        public override string[] InputVar
        {
            get
            {
                return new string[] { "Template", "?ListOfData", "?Dictionary" };
            }
        }
        public override string[] OutputVar
        {
            get
            {
                return new string[] { "Result", "ErrorMessage" };
            }
        }
        public override string Name
        {
            get
            {
                return "Send mail for each";
            }
        }

        public override void InnerRun(Dictionary<string, object> vars, Dictionary<string, object> outputVars, Dictionary<string, object> InvertedInputVars, Message message)
        {
            var listOfDBItems = new List<DBItem>();
            listOfDBItems = (List<DBItem>)vars["ListOfData"];

            var urlDictionary = new Dictionary<string, object>();
            if (vars.ContainsKey("Dictionary"))
                urlDictionary = (Dictionary<string, object>)vars["Dictionary"];

            foreach (var dbItem in listOfDBItems)
            {
                Dictionary<string, object> newDictionary = new Dictionary<string, object>();    // data
                Dictionary<string, string> recipients = new Dictionary<string, string>();

                foreach (var columnName in dbItem.getColumnNames())
                {
                    newDictionary.Add(columnName, dbItem[columnName].ToString());
                }

                if (vars.ContainsKey("Dictionary"))
                {
                    foreach (var item in urlDictionary)
                    {
                        newDictionary.Add(item.Key, item.Value);
                    }           
                }
               
                recipients.Add(newDictionary["__email"].ToString(), newDictionary["__email"].ToString());

                Mailer mail = new Mailer("", (string)vars["Template"], newDictionary);
                mail.To(recipients);
                mail.SendMail();
            }         
        }
    }
}

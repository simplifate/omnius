using FSS.Omnius.Modules.CORE;
using FSS.Omnius.Modules.Entitron.DB;
using FSS.Omnius.Modules.Hermes;
using System;
using System.Collections.Generic;
using System.Linq;

namespace FSS.Omnius.Modules.Tapestry2.Actions
{
    public class Hermes : ActionManager
    {
        [Action(193, "Send mail for each")]
        public static void SendMailForEach(COREobject core, string Template, List<DBItem> ListOfData, Dictionary<string, object> Dictionary = null, bool SendAsBCC = false)
        {
            var context = core.Context;

            var urlDictionary = Dictionary ?? new Dictionary<string, object>();

            foreach (var dbItem in ListOfData)
            {
                Dictionary<string, object> newDictionary = new Dictionary<string, object>();    // data
                Dictionary<string, string> recipients = new Dictionary<string, string>();

                foreach (var columnName in dbItem.getColumnNames())
                    newDictionary.Add(columnName, dbItem[columnName].ToString());
                
                foreach (var item in urlDictionary)
                    newDictionary.Add(item.Key, item.Value);

                string emailAddress = newDictionary["__email"].ToString();
                recipients.Add(emailAddress, emailAddress);

                var user = context.Users.Where(u => u.Email == emailAddress).FirstOrDefault();
                Locale selectedLocale = (user != null && user.Locale == Locale.en)
                    ? Locale.en
                    : Locale.cs;

                Mailer mail = new Mailer("", Template, newDictionary, selectedLocale);

                if (SendAsBCC)
                    mail.BCC(recipients);
                else
                    mail.To(recipients);

                mail.SendMail();
            }
        }

        [Action(2005, "Send mail")]
        public static void SendMail(COREobject core, string Template, string Subject = "", Dictionary<string, string> Recipients = null, Dictionary<string, string> CC = null, Dictionary<string, string> BCC = null, Dictionary<string, object> Data = null)
        {
            var context = core.Context;
            var dataDictionary = Data ?? new Dictionary<string, object>();

            Mailer czechMailer = new Mailer("", Template, dataDictionary, Locale.cs);
            Mailer englishMailer = new Mailer("", Template, dataDictionary, Locale.en);

            var recipientsCzechOutput = new Dictionary<string, string>();
            var recipientsEnglishOutput = new Dictionary<string, string>();
            var bccCzechOutput = new Dictionary<string, string>();
            var bccEnglishOutput = new Dictionary<string, string>();

            if (Recipients != null)
            {
                foreach (var addressPair in Recipients)
                {
                    var user = context.Users.Where(u => u.Email == addressPair.Key).FirstOrDefault();
                    if (user != null && user.Locale == Locale.en)
                        recipientsEnglishOutput.Add(addressPair.Key, addressPair.Value);
                    else
                        recipientsCzechOutput.Add(addressPair.Key, addressPair.Value);
                }

                czechMailer.To(recipientsCzechOutput);
                englishMailer.To(recipientsEnglishOutput);
            }
            if (BCC != null)
            {
                foreach (var addressPair in BCC)
                {
                    var user = context.Users.Where(u => u.Email == addressPair.Key).FirstOrDefault();
                    if (user != null && user.Locale == Locale.en)
                        bccEnglishOutput.Add(addressPair.Key, addressPair.Value);
                    else
                        bccCzechOutput.Add(addressPair.Key, addressPair.Value);
                }

                czechMailer.BCC(bccCzechOutput);
                englishMailer.BCC(bccEnglishOutput);
            }

            if (recipientsCzechOutput.Count > 0 || bccCzechOutput.Count > 0)
                czechMailer.SendMail();
            if (recipientsEnglishOutput.Count > 0 || bccEnglishOutput.Count > 0)
                englishMailer.SendMail();
        }

        [Action(2010, "Run mailer")]
        public static void RunMailer(COREobject core, string serverName = "Test")
        {
            Mailer mailer = new Mailer(serverName);
            mailer.RunSender();
        }
    }
}

using System;
using System.Collections.Generic;
using System.Collections.Specialized;
using System.Linq;
using System.Net;
using System.Net.Mail;
using System.Text;
using System.Threading.Tasks;
using System.Web.Mvc;
using FSS.Omnius.Modules.Entitron.Entity;
using FSS.Omnius.Modules.Entitron.Entity.Hermes;
using FSS.Omnius.Modules.Entitron.Entity.Tapestry;
using FSS.Omnius.Modules.Watchtower;
using Newtonsoft.Json.Linq;
using S22.Imap;
using FSS.Omnius.Modules.Entitron.Entity.Persona;
using FSS.Omnius.Modules.CORE;

namespace FSS.Omnius.Modules.Hermes
{
    public class IncomingEmailListener
    {
        static Dictionary<string, ImapClient> clients = new Dictionary<string, ImapClient>(); 

        public static void AddListener(IncomingEmail model)
        {
            if(clients.ContainsKey(model.Name)) {
                clients[model.Name].Dispose();
                clients.Remove(model.Name);
            }

            ImapClient client = new ImapClient(model.ImapServer, model.ImapPort ?? 143, model.UserName, model.Password, AuthMethod.Login, model.ImapUseSSL);
            client.NewMessage += (sender, e) => { IncomingEmailListener.onNewMessage(sender, e, model.Name); };

            clients.Add(model.Name, client);
        }

        public static void RemoveListener(string name)
        {
            if(clients.ContainsKey(name)) {
                clients[name].Dispose();
                clients.Remove(name);
            }
        }

        private static void onNewMessage(object sender, IdleMessageEventArgs e, string incomingMailboxName)
        {
            COREobject core = COREobject.i;
            MailMessage mail = e.Client.GetMessage(e.MessageUID, FetchOptions.Normal);

            IncomingEmail email = core.Context.IncomingEmail.SingleOrDefault(m => m.Name == incomingMailboxName);
            if(email != null && email.IncomingEmailRule.Count() > 0) {
                foreach(IncomingEmailRule rule in email.IncomingEmailRule) {
                    bool result = EvaluateRule(rule, mail);

                    if(result) 
                    {
                        Block block = GetBlockWithWF(core.Context, rule.ApplicationId, rule.BlockName.RemoveDiacritics());
                        if (block != null) 
                        {
                            core.Application = rule.Application;

                            try {
                                PersonaAppRole role = core.Context.AppRoles.FirstOrDefault(r => r.Name == "System" && r.ApplicationId == rule.ApplicationId);
                                core.User = core.Context.Users.FirstOrDefault(u => u.Users_Roles.Any(r => r.RoleName == role.Name && r.ApplicationId == role.ApplicationId));
                            }
                            catch (Exception) {
                            }

                            OmniusInfo.Log($"Začátek zpracování mailu: {email.Name} / Pravidlo {rule.Name} / Blok {rule.BlockName} / Button {rule.WorkflowName}", OmniusLogSource.Hermes, rule.Application, core.User);
                                
                            FormCollection fc = new FormCollection(new NameValueCollection()
                            {
                                { "MailFrom", mail.From.Address },
                                { "MailCC", string.Join(";", mail.CC.Select(cc => cc.Address).ToList()) },
                                { "MailSubject", mail.Subject },
                                { "MailBody", mail.Body },
                            });
                                
                            var runResult = new Tapestry.Tapestry(core).run(block, rule.WorkflowName, -1, fc, 0);

                            OmniusInfo.Log($"Konec zpraconání mailu: {email.Name} / Pravidlo {rule.Name} / Blok {rule.BlockName} / Button {rule.WorkflowName}", OmniusLogSource.Hermes, rule.Application, core.User);
                        }
                    }
                }
            }
        }

        public static void Refresh()
        {
            using (DBEntities e = new DBEntities()) {

                if (clients.Count > 0) {
                    foreach (KeyValuePair<string, ImapClient> client in clients) {
                        client.Value.Dispose();
                    }
                    clients.Clear();
                }

                foreach (IncomingEmail mail in e.IncomingEmail) {
                    AddListener(mail);
                }
            }
        }

        private static bool EvaluateRule(IncomingEmailRule rule, MailMessage mail)
        {
            bool result = true;
            JToken conditionSets = JToken.Parse(rule.Rule);
            foreach(JToken set in conditionSets) {
                if((string)set["SetRelation"] == "AND") {
                    result = result && MatchConditionSet(set, mail);
                }
                else if((string)set["SetRelation"] == "OR") {
                    result = result || MatchConditionSet(set, mail);
                }
            }
            
            return result;
        }
        
        private static bool MatchConditionSet(JToken set, MailMessage mail)
        {
            bool result = true;
            foreach (JToken condition in set["Conditions"]) {
                if ((string)condition["Relation"] == "AND") {
                    result = result && MatchCondition(condition, mail);
                }
                else if ((string)condition["Relation"] == "OR") {
                    result = result || MatchCondition(condition, mail);
                }
            }
            return result;
        }
        private static bool MatchCondition(JToken condition, MailMessage mail)
        {
            string variable = "";
            string value = (string)condition["Value"];

            switch((string)condition["Variable"]) {
                case "From": variable = mail.From.Address; break;
                case "CC": variable = string.Join(";", mail.CC.Select(c => c.Address).ToList()); break;
                case "Subject": variable = mail.Subject; break;
                case "Body": variable = mail.Body; break;
            }

            switch((string)condition["Operator"]) {
                case "contains":
                    return variable.Contains(value);
                case "BeginWith":
                    return variable.StartsWith(value);
                case "EndWith":
                    return variable.EndsWith(value);
                case "IsEmpty":
                    return string.IsNullOrEmpty(value);
                case "IsNotEmpty":
                    return !string.IsNullOrEmpty(value);
            }
            return true;
        }

        private static Block GetBlockWithWF(DBEntities context, int appId, string blockName)
        {
            return context.Blocks.FirstOrDefault(b => b.WorkFlow.ApplicationId == appId && b.Name == blockName);
        }
    }
}

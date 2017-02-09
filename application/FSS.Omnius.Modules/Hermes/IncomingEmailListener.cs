using System;
using System.Collections.Generic;
using System.Linq;
using System.Net.Mail;
using System.Text;
using System.Threading.Tasks;
using FSS.Omnius.Modules.Entitron.Entity;
using FSS.Omnius.Modules.Entitron.Entity.Hermes;
using S22.Imap;

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
            DBEntities db = DBEntities.instance;
            MailMessage mail = e.Client.GetMessage(e.MessageUID, FetchOptions.Normal);

            foreach(IncomingEmailRule rule in db.IncomingEmail.SingleOrDefault(m => m.Name == incomingMailboxName).IncomingEmailRule) {
                // LOGIC HERE
            }
        }

        public static void Refresh()
        {
            DBEntities e = DBEntities.instance;

            if(clients.Count > 0) {
                foreach(KeyValuePair<string, ImapClient> client in clients) {
                    client.Value.Dispose();
                }
                clients.Clear();
            }

            foreach(IncomingEmail mail in e.IncomingEmail) {
                AddListener(mail);
            }
        }
    }
}

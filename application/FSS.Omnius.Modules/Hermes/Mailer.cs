using FSS.Omnius.Modules.Entitron.Entity;
using FSS.Omnius.Modules.Entitron.Entity.Hermes;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace FSS.Omnius.Modules.Hermes
{
    public class Mailer
    {
        Smtp server;
        DBEntities e;

        public Mailer(string serverName = "")
        {
            e = new DBEntities();

            if(string.IsNullOrEmpty(serverName)) {
                server = e.SMTPs.Single(s => s.Is_Default == true);
            }
            else {
                server = e.SMTPs.Single(s => s.Name == serverName || s.Server == serverName);
            }
        }

        public string SendMail(string templateName, Object model)
        {
            EmailTemplate template = e.EmailTemplates.Single(t => t.Name == templateName);
            List<EmailPlaceholder> plcs = template.PlaceholderList.ToList();
            string content = template.Content;

            foreach(EmailPlaceholder p in plcs)
            {
                string key = "{" + p.Prop_Name + "}";
                object value = model.GetType().GetProperty(p.Prop_Name).GetValue(model) as object;

                content = content.Replace(key, value == null ? string.Empty : value.ToString());
            }
            
            return content;
        }
    }
}

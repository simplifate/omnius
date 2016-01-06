using FSS.Omnius.Modules.Entitron.Entity;
using FSS.Omnius.Modules.Entitron.Entity.Hermes;
using System;
using System.Collections;
using System.Collections.Generic;
using System.Linq;
using System.Text.RegularExpressions;
using System.Threading.Tasks;

namespace FSS.Omnius.Modules.Hermes
{
    public class Mailer
    {
        Smtp server;
        DBEntities e;
        List<EmailPlaceholder> plcs;

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
            plcs = template.PlaceholderList.ToList();
            string content = "";// template.Content;

            Regex regExpList = new Regex("^\\{list\\.([^\\}]+)}$");

            foreach(EmailPlaceholder p in plcs)
            {
                string key = "{" + p.Prop_Name + "}";
                if(regExpList.IsMatch(key))
                {
                    Match m = regExpList.Match(key);
                    ParseList(ref content, p.Prop_Name, m.Groups[1].ToString(), GetValue(model, m.Groups[1].ToString()));
                }
                else
                {
                    object value = GetValue(model, p.Prop_Name);
                    content = content.Replace(key, value == null ? string.Empty : value.ToString());
                }
            }
            
            return content;
        }

        private void ParseList(ref string content, string listKey, string objectKey, object model)
        {
            Regex regExpList = new Regex("(\\{" + listKey + "\\}(.*?)\\{end." + listKey + "\\})", RegexOptions.Singleline);
            MatchCollection lists = regExpList.Matches(content);

            foreach (Match list in lists)
            {
                string template = list.Groups[2].ToString();
                List<string> items = new List<string>();

                foreach(var item in (IEnumerable)model)
                {
                    string text = template;
                    foreach (EmailPlaceholder p in plcs)
                    {
                        if(p.Prop_Name.StartsWith(objectKey))
                        {
                            string propName = p.Prop_Name.Replace(objectKey + ".", "");
                            object value = GetValue(item, propName);
                            text = text.Replace("{" + p.Prop_Name + "}", value == null ? string.Empty : value.ToString());    
                        } 
                    }
                    items.Add(text);
                }

                content = content.Replace(list.Groups[1].ToString(), String.Join("", items));
            }
        }

        private object GetValue(object model, string propName)
        {
            if(model is IDictionary)
            {
                return ((IDictionary)model)[propName] as object;
            }
            else
            {
                return model.GetType().GetProperty(propName).GetValue(model) as object;
            }
        }
    }
}

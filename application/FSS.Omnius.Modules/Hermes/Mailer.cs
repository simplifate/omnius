﻿using FSS.Omnius.Modules.Entitron.Entity;
using FSS.Omnius.Modules.Entitron.Entity.Hermes;
using System;
using System.Collections;
using System.Collections.Generic;
using System.Linq;
using System.Text.RegularExpressions;
using System.Net.Mail;
using System.Text;
using System.Data.Entity;
using System.Web.Mvc.Html;
using FSS.Omnius.Modules.Watchtower;
using Newtonsoft.Json;
using System.Web.Mvc;
using Newtonsoft.Json.Linq;
using System.Net.Mime;
using System.IO;
using FSS.Omnius.Modules.Entitron.Entity.Nexus;
using System.ComponentModel.DataAnnotations;
using FSS.Omnius.Modules.Entitron.Entity.Master;
using FSS.Omnius.Modules.CORE;

namespace FSS.Omnius.Modules.Hermes
{
    public class Mailer
    {
        private Smtp server;
        private DBEntities e;
        private List<EmailPlaceholder> plcs;
        private Object data;
        private SmtpClient client;
        private JArray attachmentList = new JArray();
        private static Dictionary<int, string> queuStatusNames = new Dictionary<int, string>();
        private Locale mailerLanguage;

        public MailMessage mail;

        public Mailer(string serverName = "")
        {
            mailerLanguage = Locale.cs;
            Init(serverName);
        }

        public Mailer(string serverName, string templateName, Object model, Locale language)
        {
            mailerLanguage = language;
            Init(serverName);
            Prepare(templateName, model);
        }

        private void Init(string serverName = "")
        {
            e = COREobject.i.Context;

            if (string.IsNullOrEmpty(serverName))
            {
                server = e.SMTPs.Single(s => s.Is_Default == true);
            }
            else {
                server = e.SMTPs.Single(s => s.Name == serverName || s.Server == serverName);
            }

            client = new SmtpClient();
            client.Port = server.Use_SSL ? 465 : 25;
            client.EnableSsl = server.Use_SSL;
            client.DeliveryMethod = SmtpDeliveryMethod.Network;
            client.Host = server.Server;
            client.Timeout = 10000;

            if(!string.IsNullOrWhiteSpace(server.Auth_User) && !string.IsNullOrWhiteSpace(server.Auth_Password))
            {
                client.UseDefaultCredentials = false;
                client.Credentials = new System.Net.NetworkCredential(server.Auth_User, server.Auth_Password);
            }
        }

        public void Prepare(string templateName, Object model)
        {
            data = model;

            EmailTemplate template = e.EmailTemplates.Single(t => t.Name == templateName);
            plcs = template.PlaceholderList.OrderBy(p => p.Num_Order).ToList();
            EmailTemplateContent contentModel = template.ContentList.Single(t => t.LanguageId == (int)mailerLanguage);

            string subject = SetData(contentModel.Subject);
            string content = SetData(contentModel.Content);

            mail = new MailMessage();
            mail.BodyEncoding = UTF8Encoding.UTF8;
            mail.DeliveryNotificationOptions = DeliveryNotificationOptions.OnFailure;

            if (!string.IsNullOrWhiteSpace(contentModel.From_Email))
                mail.From = new MailAddress(contentModel.From_Email, contentModel.From_Name);
            else
                OmniusException.Log("Nutno vyplnit email odesílatele.");

            if (!string.IsNullOrWhiteSpace(subject))
                mail.Subject = subject;

            if (!string.IsNullOrWhiteSpace(content))
                mail.Body = content;

            if (template.Is_HTML)
            {
                mail.IsBodyHtml = true;
                if(!string.IsNullOrWhiteSpace(contentModel.Content_Plain))
                {
                    string contentPlain = SetData(contentModel.Content_Plain);
                    mail.AlternateViews.Add(AlternateView.CreateAlternateViewFromString(contentPlain, Encoding.UTF8, MediaTypeNames.Text.Plain));
                }
                mail.AlternateViews.Add(AlternateView.CreateAlternateViewFromString(content, Encoding.UTF8, MediaTypeNames.Text.Html));
            }
        }

        public string SetData(string content)
        {
            Regex regExpList = new Regex("^\\{list@([^\\}]+)}$");
            Regex regExpIf = new Regex("^\\{if@([^\\}]+)}$");

            foreach (EmailPlaceholder p in plcs)
            {
                string key = "{" + p.Prop_Name + "}";
                if (regExpList.IsMatch(key))
                {
                    Match m = regExpList.Match(key);
                    ParseList(ref content, p.Prop_Name, m.Groups[1].ToString(), GetValue(data, m.Groups[1].ToString()));
                }
                else if (regExpIf.IsMatch(key))
                {
                    Match m = regExpIf.Match(key);
                    ParseIf(ref content, p.Prop_Name, GetValue(data, m.Groups[1].ToString()));
                }
                else if (!key.Contains('@'))
                {
                    object value = GetValue(data, p.Prop_Name);
                    content = content.Replace(key, value == null ? string.Empty : value.ToString());
                }
            }

            return content;
        }

        public bool SendBySender(int? applicationId = null, DateTime? sendAfter = null)
        {
            EmailQueue item = new EmailQueue();
            item.ApplicationId = applicationId ?? e.Applications.Single(a => a.IsSystem).Id;
            item.Message = HermesUtils.SerializeMailMessage(mail, Formatting.Indented);
            item.Date_Send_After = sendAfter == null ? DateTime.UtcNow : (DateTime)sendAfter;
            item.Date_Inserted = DateTime.UtcNow;
            item.AttachmentList = attachmentList.ToString();
            item.Status = EmailQueueStatus.waiting;
            
            e.EmailQueueItems.Add(item);
            e.SaveChanges();

            return true;
        }

        public bool SendMail(Application application = null, bool disposeClient = true)
        {
            if (mail.To.Count + mail.Bcc.Count == 0)
            {
                return true;
            }
            bool result;
            string smtpError = "";

            if (attachmentList.Count() > 0)
            {
                mail.Attachments.Clear();
                for(int i = 0; i < attachmentList.Count(); i++)
                {
                    Attachment att;
                    if (!attachmentList[i].GetType().Equals(typeof(JValue)))
                    {
                        FileMetadata fileInfo = e.FileMetadataRecords.Find((int)attachmentList[i]["Key"]);
                        try {
                            byte[] data = fileInfo.CachedCopy.Blob;
                            Stream fileContent = new MemoryStream(data);

                            att = new Attachment(fileContent, fileInfo.Filename);
                        }
                        catch(NullReferenceException ex)
                        {
                            OmniusException.Log($"Odeslání e-mailu se nezdařilo - příloha <b>{attachmentList[i]["Value"].ToString()}</b> nebyla nalezena", OmniusLogSource.Hermes, ex, application);
                            return false;
                        }
                    }
                    else
                    {
                        string path = attachmentList[i].ToString();
                        att = new Attachment(path);
                    }
                    mail.Attachments.Add(att);
                }
            }

            try {
                client.Send(mail);
                result = true;
            }
            catch(Exception e)
            {  
                result = false;
                smtpError = e.Message;
            }

            // Uložíme do logu
            EmailLog log = new EmailLog();
            log.Content = HermesUtils.SerializeMailMessage(mail, Formatting.Indented);
            log.DateSend = DateTime.UtcNow;
            log.Status = result ? EmailSendStatus.success : EmailSendStatus.failed;
            log.SMTP_Error = smtpError;

            e.EmailLogItems.Add(log);
            e.SaveChanges();

            OmniusLog.Log($"Odeslání e-mailu \"{mail.Subject}\" (<a href=\"/Hermes/Log/Detail/{log.Id}\" title=\"Detail e-mailu\">detail e-mailu</a>)", (result ? OmniusLogLevel.Info : OmniusLogLevel.Error), OmniusLogSource.Hermes, application);

            if (disposeClient) {
                client.Dispose();
            }

            return result;
        }

        public void RunSender()
        {
            DateTime now = DateTime.UtcNow;
            List<EmailQueue> rows = e.EmailQueueItems.Where(m => m.Date_Send_After <= now && m.Status != EmailQueueStatus.error).ToList();

            foreach(EmailQueue row in rows)
            {
                try
                {

                    JToken m = JToken.Parse(row.Message);

                    mail = new MailMessage();
                    mail.From = new MailAddress((string)m["From"]["Address"], (string)m["From"]["DisplayName"]);

                    foreach (JToken replyTo in m["ReplyToList"])
                    {
                        mail.ReplyToList.Add(new MailAddress((string)replyTo["Address"], (string)replyTo["DisplayName"]));
                    }
                    foreach (JToken to in m["To"])
                    {
                        mail.To.Add(new MailAddress((string)to["Address"], (string)to["DisplayName"]));
                    }
                    foreach (JToken bcc in m["Bcc"])
                    {
                        mail.Bcc.Add(new MailAddress((string)bcc["Address"], (string)bcc["DisplayName"]));
                    }
                    foreach (JToken cc in m["CC"])
                    {
                        mail.CC.Add(new MailAddress((string)cc["Address"], (string)cc["DisplayName"]));
                    }
                    mail.Priority = (MailPriority)((int)m["Priority"]);
                    mail.DeliveryNotificationOptions = (DeliveryNotificationOptions)((int)m["DeliveryNotificationOptions"]);
                    mail.Subject = (string)m["Subject"];
                    mail.Body = (string)m["Body"];
                    mail.BodyTransferEncoding = (System.Net.Mime.TransferEncoding)((int)m["BodyTransferEncoding"]);
                    mail.IsBodyHtml = (bool)m["IsBodyHtml"];

                    if (m["AlternateViews"].Children().Count() > 0)
                    {
                        foreach (JObject view in m["AlternateViews"].Children())
                        {
                            mail.AlternateViews.Add(AlternateView.CreateAlternateViewFromString(view["Content"].ToString(), Encoding.UTF8, view["Type"]["MediaType"].ToString()));
                        }
                    }

                    attachmentList = JArray.Parse(row.AttachmentList);

                    bool sent = SendMail(row.Application, false);
                    if (sent)
                    {
                        e.EmailQueueItems.Remove(row);
                    }
                    else
                    {
                        row.Status = EmailQueueStatus.error;
                    }
                }
                catch(Exception ex)
                {
                    OmniusException.Log(ex, OmniusLogSource.Hermes);
                }
            }

            e.SaveChanges();
            client.Dispose();
        }

        #region Mail Tools

        public void To(string email, string displayName = "")
        {
            mail.To.Clear();
            mail.To.Add(new MailAddress(email, displayName));
        }

        public void To(Dictionary<string,string>addressList)
        {
            mail.To.Clear();
            foreach(KeyValuePair<string, string> addr in addressList.Where(pair => pair.Key != "")) {
                if (new EmailAddressAttribute().IsValid(addr.Key))
                    mail.To.Add(new MailAddress(addr.Key, addr.Value));
                else
                    OmniusLog.Log($"User passed invalid email address '{addr.Key}'.", OmniusLogLevel.Warning, OmniusLogSource.Hermes);
            }
        }

        public void CC (string email, string displayName = "")
        {
            mail.CC.Clear();
            mail.CC.Add(new MailAddress(email, displayName));
        }

        public void CC (Dictionary<string,string> addressList)
        {
            mail.CC.Clear();
            foreach(KeyValuePair<string, string> addr in addressList) {
                mail.CC.Add(new MailAddress(addr.Key, addr.Value));
            }
        }

        public void BCC(string email, string displayName = "")
        {
            mail.Bcc.Clear();
            mail.Bcc.Add(new MailAddress(email, displayName));
        }

        public void BCC(Dictionary<string, string> addressList)
        {
            mail.Bcc.Clear();
            foreach (KeyValuePair<string, string> addr in addressList.Where(pair => pair.Key != "")) {
                if (new EmailAddressAttribute().IsValid(addr.Key))
                    mail.Bcc.Add(new MailAddress(addr.Key, addr.Value));
                else
                    OmniusLog.Log($"User passed invalid email address '{addr.Key}'.", OmniusLogLevel.Warning, OmniusLogSource.Hermes);
            }
        }

        public void Attachment(string path)
        {
            attachmentList.Clear();
            attachmentList.Add(path);
        }

        public void Attachment(List<string> attList)
        {
            attachmentList.Clear();
            foreach(string path in attList) {
                attachmentList.Add(path);
            }
        }

        public void Attachment(KeyValuePair<int, string> file)
        {
            attachmentList.Clear();
            attachmentList.Add(JToken.FromObject(file));
        }

        public void Attachment(List<KeyValuePair<int, string>> attList)
        {
            attachmentList.Clear();
            foreach(KeyValuePair<int, string> file in attList) {
                attachmentList.Add(JToken.FromObject(file));
            }
        }

        public void AddTo(string email, string displayName = "") { mail.To.Add(new MailAddress(email, displayName)); }
        public void AddCC(string email, string displayName = "") { mail.CC.Add(new MailAddress(email, displayName)); }
        public void AddBCC(string email, string displayName = "") { mail.Bcc.Add(new MailAddress(email, displayName)); }
        public void AddAttachment(string path) { attachmentList.Add(path); }
        public void AddAttachment(KeyValuePair<int, string> file) { attachmentList.Add(JToken.FromObject(file)); }

        public void From(string email, string displayName = "") { mail.From = new MailAddress(email, displayName); }
        public void Subject(string subject) { mail.Subject = subject; }

        #endregion


        #region Tools

        private void ParseList(ref string content, string listKey, string objectKey, object model)
        {
            Regex regExpList = new Regex("(\\{" + listKey + "\\}(.*?)\\{end@" + listKey + "\\})", RegexOptions.Singleline);
            Regex regExpIf = new Regex("^\\{" + objectKey + "@if@([^\\}]+)}$");
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
                            if(regExpIf.IsMatch(p.Prop_Name)) {
                                Match m = regExpIf.Match(p.Prop_Name);
                                ParseIfInList(ref text, p.Prop_Name, GetValue(item, m.Groups[1].ToString()));
                            }
                            else {
                                string propName = p.Prop_Name.Replace(objectKey + "@", "");
                                object value = GetValue(item, propName);
                                text = text.Replace("{" + p.Prop_Name + "}", value == null ? string.Empty : value.ToString());
                            }
                        } 
                    }
                    items.Add(text);
                }

                content = content.Replace(list.Groups[1].ToString(), String.Join("", items));
            }
        }

        private void ParseIf(ref string content, string ifKey, object value)
        {
            Regex regExpIf = new Regex("(\\{" + ifKey + "\\}(.*?)\\{end@" + ifKey + "\\})", RegexOptions.Singleline);
            MatchCollection ifList = regExpIf.Matches(content);

            foreach (Match block in ifList) {
                string template = block.Groups[2].ToString();
                string[] trueFalse = template.Split(new string[] { "{else@" + ifKey + "}" }, StringSplitOptions.None);

                string result = "";
                if (value as bool? == true || value as string == "true" || value as int? == 1 || value as string == "1") {
                    result = trueFalse[0];
                }
                else {
                    if(trueFalse.Length == 2) {
                        result = trueFalse[1];
                    }
                }

                result = SetData(result);
                content = content.Replace(block.Groups[1].ToString(), result);
            }
        }

        private void ParseIfInList(ref string content, string ifKey, object value)
        {
            Regex regExpIf = new Regex("(\\{" + ifKey + "\\}(.*?)\\{end@" + ifKey + "\\})", RegexOptions.Singleline);
            MatchCollection ifList = regExpIf.Matches(content);

            foreach (Match block in ifList) {
                string template = block.Groups[2].ToString();
                string[] trueFalse = template.Split(new string[] { "{else@" + ifKey + "}" }, StringSplitOptions.None);

                string result = "";
                if (value as bool? == true || value as string == "true" || value as int? == 1 || value as string == "1") {
                    result = trueFalse[0];
                }
                else {
                    if (trueFalse.Length == 2) {
                        result = trueFalse[1];
                    }
                }
                content = content.Replace(block.Groups[1].ToString(), result);
            }
        }

        private object GetValue(object model, string propName)
        {
            if(model is IDictionary)
            {
                var data = (IDictionary)model;
                return data.Contains(propName) ? data[propName] as object : null;
            }
            else
            {
                return model.GetType().GetProperty(propName).GetValue(model) as object;
            }
        }

        public static string QueuStatusName(int status)
        {
            if(queuStatusNames.Count() == 0)
            {
                queuStatusNames.Add((int)EmailQueueStatus.error, "Chyba");
                queuStatusNames.Add((int)EmailQueueStatus.waiting, "Čeká na odeslání");
            }

            return queuStatusNames[status];
        }

        #endregion
    }
}

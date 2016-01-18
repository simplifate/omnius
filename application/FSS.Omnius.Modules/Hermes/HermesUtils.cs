using System.Net.Mail;
using Newtonsoft.Json;
using Newtonsoft.Json.Linq;

namespace FSS.Omnius.Modules.Hermes
{
    public static class HermesUtils
    {
        public static string SerializeMailMessage(MailMessage message, Formatting formatting)
        {
            return JsonConvert.SerializeObject(MailMessageToJObject(message), formatting);
        }
        public static JObject MailMessageToJObject(MailMessage message)
        {
            var result = new JObject();
            result.Add("From", GetJTokenOrNull(message.From));
            result.Add("Sender", GetJTokenOrNull(message.Sender));
            result.Add("ReplyTo", null);
            result.Add("ReplyToList", GetJTokenOrNull(message.ReplyToList));
            result.Add("To", GetJTokenOrNull(message.To));
            result.Add("Bcc", GetJTokenOrNull(message.Bcc));
            result.Add("CC", GetJTokenOrNull(message.CC));
            result.Add("Priority", (int)message.Priority);
            result.Add("DeliveryNotificationOptions", (int)message.DeliveryNotificationOptions);
            result.Add("Subject", message.Subject);
            result.Add("SubjectEncoding", GetJTokenOrNull(message.SubjectEncoding));
            result.Add("Headers", GetJTokenOrNull(message.Headers));
            result.Add("HeadersEncoding", GetJTokenOrNull(message.HeadersEncoding));
            result.Add("Body", message.Body);
            result.Add("BodyEncoding", GetJTokenOrNull(message.BodyEncoding));
            result.Add("BodyTransferEncoding", (int)message.BodyTransferEncoding);
            result.Add("IsBodyHtml", message.IsBodyHtml);
            result.Add("Attachments", new JArray());
            result.Add("AlternateViews", GetJTokenOrNull(message.AlternateViews));
            return result;
        }
        public static JToken GetJTokenOrNull(object input)
        {
            if (input != null)
                return JToken.FromObject(input);
            else
                return null;
        }
    }
}

using System;
using System.Data;
using System.Configuration;
using System.Web;
using System.Web.Security;
using System.Web.UI;
using System.Web.UI.WebControls;
using System.Web.UI.WebControls.WebParts;
using System.Web.UI.HtmlControls;

using System.IO;
using System.Xml;
using System.Security.Cryptography.X509Certificates;
using System.Security.Cryptography.Xml;
using System.Security.Cryptography;
using System.IO.Compression;
using System.Web.Mvc;
using System.Collections.Generic;
using System.Linq;
using System.Text;

namespace OneLogin.Saml
{
    public class Certificate
    {
        public X509Certificate2 cert;

        public void LoadCertificate(string certificate)
        {
            cert = new X509Certificate2();
            cert.Import(StringToByteArray(certificate));
        }

        public void LoadCertificate(byte[] certificate)
        {
            cert = new X509Certificate2();
            cert.Import(certificate);
        }

        private byte[] StringToByteArray(string st)
        {
            byte[] bytes = new byte[st.Length];
            for (int i = 0; i < st.Length; i++)
            {
                bytes[i] = (byte)st[i];
            }
            return bytes;
        }
    }

    public class Response
    {
        private XmlDocument xmlDoc;
        private AccountSettings accountSettings;
        private Certificate certificate;

        public Response(AccountSettings accountSettings)
        {
            this.accountSettings = accountSettings;
            certificate = new Certificate();
            certificate.LoadCertificate(accountSettings.certificate);
        }

        public void LoadXml(string xml)
        {
            xmlDoc = new XmlDocument();
            xmlDoc.PreserveWhitespace = true;
            xmlDoc.XmlResolver = null;
            xmlDoc.LoadXml(xml);
        }

        public void LoadXmlFromBase64(string response)
        {
            System.Text.ASCIIEncoding enc = new System.Text.ASCIIEncoding();
            LoadXml(enc.GetString(Convert.FromBase64String(response)));
        }

        public bool IsValid()
        {
            XmlNamespaceManager manager = new XmlNamespaceManager(xmlDoc.NameTable);
            manager.AddNamespace("ds", SignedXml.XmlDsigNamespaceUrl);
            XmlNodeList nodeList = xmlDoc.SelectNodes("//ds:Signature", manager);

            SignedXml signedXml = new SignedXml(xmlDoc);
            signedXml.LoadXml((XmlElement)nodeList[0]);
            return signedXml.CheckSignature(certificate.cert, true);
        }

        public string GetNameID()
        {
            XmlNamespaceManager manager = new XmlNamespaceManager(xmlDoc.NameTable);
            manager.AddNamespace("ds", SignedXml.XmlDsigNamespaceUrl);
            manager.AddNamespace("saml", "urn:oasis:names:tc:SAML:2.0:assertion");
            manager.AddNamespace("samlp", "urn:oasis:names:tc:SAML:2.0:protocol");

            XmlNode node = xmlDoc.SelectSingleNode("/samlp:Response/saml:Assertion/saml:Subject/saml:NameID", manager);
            return node.InnerText;
        }
    }

    public class AuthRequest
    {
        public string id;
        private string issue_instant;
        private AppSettings appSettings;
        private AccountSettings accountSettings;

        public enum AuthRequestFormat
        {
            Base64 = 1,
            DeflatedBase64 = 2
        }

        public AuthRequest(AppSettings appSettings, AccountSettings accountSettings)
        {
            this.appSettings = appSettings;
            this.accountSettings = accountSettings;

            id = "_" + System.Guid.NewGuid().ToString();
            issue_instant = DateTime.Now.ToUniversalTime().ToString("yyyy-MM-ddTHH:mm:ssZ");
        }

        public string GetRequest(AuthRequestFormat format)
        {
            using (StringWriter sw = new StringWriter())
            {
                XmlWriterSettings xws = new XmlWriterSettings();
                xws.OmitXmlDeclaration = true;

                using (XmlWriter xw = XmlWriter.Create(sw, xws))
                {
                    xw.WriteStartElement("samlp", "AuthnRequest", "urn:oasis:names:tc:SAML:2.0:protocol");
                    xw.WriteAttributeString("ID", id);
                    xw.WriteAttributeString("Version", "2.0");
                    xw.WriteAttributeString("IssueInstant", issue_instant);
                    xw.WriteAttributeString("ProtocolBinding", "urn:oasis:names:tc:SAML:2.0:bindings:HTTP-POST");
                    xw.WriteAttributeString("AssertionConsumerServiceURL", appSettings.assertionConsumerServiceUrl);
                    xw.WriteAttributeString("saml", "urn:oasis:names:tc:SAML:2.0:assertion");

                        xw.WriteStartElement("saml", "Issuer", "urn:oasis:names:tc:SAML:2.0:assertion");
                        xw.WriteString(appSettings.issuer);
                        xw.WriteEndElement();

                        xw.WriteStartElement("samlp", "NameIDPolicy", "urn:oasis:names:tc:SAML:2.0:protocol");
                        xw.WriteAttributeString("Format", "urn:oasis:names:tc:SAML:1.1:nameid-format:emailAddress");
                        xw.WriteAttributeString("AllowCreate", "true");
                        xw.WriteEndElement();

                        xw.WriteStartElement("samlp", "RequestedAuthnContext", "urn:oasis:names:tc:SAML:2.0:protocol");
                        xw.WriteAttributeString("Comparison", "exact");
                    
                            xw.WriteStartElement("saml", "AuthnContextClassRef", "urn:oasis:names:tc:SAML:2.0:assertion");
                            xw.WriteString("urn:oasis:names:tc:SAML:2.0:ac:classes:PasswordProtectedTransport");
                            xw.WriteEndElement();

                        xw.WriteEndElement(); // RequestedAuthnContext

                    xw.WriteEndElement();
                }




                if (format == AuthRequestFormat.DeflatedBase64) {
                    return System.Convert.ToBase64String(Deflate(System.Text.ASCIIEncoding.ASCII.GetBytes(sw.ToString())));
                }
                if (format == AuthRequestFormat.Base64)
                {
                    return ConvertStringToBase64(sw.ToString());
                }
                return null;
            }
        }

        public byte[] Deflate(byte[] data)
        {
            using (MemoryStream output = new MemoryStream())
            {
                using (DeflateStream gzip =
                  new DeflateStream(output, CompressionMode.Compress))
                {
                    foreach(byte bajt in data)
                    {
                        gzip.WriteByte(bajt);
                    }
                }

                return output.ToArray();
            }
        }

        public string ConvertStringToBase64(string xml)
        {
            byte[] toEncodeAsBytes = System.Text.ASCIIEncoding.ASCII.GetBytes(xml);
            return System.Convert.ToBase64String(toEncodeAsBytes);
        }
    }

    public class RequestBuilder
    {
        public string GetAuthUrl()
        {
            HttpContextBase context = new HttpContextWrapper(HttpContext.Current);
            UrlHelper url = new UrlHelper(context.Request.RequestContext);
            AppSettings appSettings = new AppSettings();
            AccountSettings accountSettings = new AccountSettings();
            AuthRequest request = new AuthRequest(appSettings, accountSettings);
            return accountSettings.idp_sso_target_url + "?SAMLRequest=" + url.Encode(request.GetRequest(AuthRequest.AuthRequestFormat.DeflatedBase64));
        }
    }

    public static class SamlSession
    {
        public static Dictionary<int, string> SamlResponses = new Dictionary<int, string>();

        public static string AddResponse(string response)
        {
            int id = GetNextDictionaryKey(SamlResponses);
            SamlResponses.Add(id, response);
            return CalculateMD5Hash(id.ToString());
        }

        public static string GetResponse(string id)
        {
            return SamlResponses.FirstOrDefault(x => CalculateMD5Hash(x.Key.ToString()) == id).Value;
        }

        public static void EraseResponse(string id)
        {
            KeyValuePair<int, string> registry = SamlResponses.FirstOrDefault(x => CalculateMD5Hash(x.Key.ToString()) == id);
            SamlResponses.Remove(registry.Key);
        }

        private static int GetNextDictionaryKey(Dictionary<int, string> dictionary)
        {
            int min = dictionary.Keys.Min();
            int max = dictionary.Keys.Max();

            return Enumerable.Range(min, max - min).Except(dictionary.Keys).First();
        }

        private static string CalculateMD5Hash(string input)

        {

            // step 1, calculate MD5 hash from input

            MD5 md5 = System.Security.Cryptography.MD5.Create();

            byte[] inputBytes = System.Text.Encoding.ASCII.GetBytes(input);

            byte[] hash = md5.ComputeHash(inputBytes);

            // step 2, convert byte array to hex string

            StringBuilder sb = new StringBuilder();

            for (int i = 0; i < hash.Length; i++)

            {

                sb.Append(hash[i].ToString("X2"));

            }

            return sb.ToString();

        }

    }
}
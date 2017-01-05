using System;
using System.CodeDom;
using System.CodeDom.Compiler;
using System.Collections.Specialized;
using System.IO;
using System.Linq;
using System.Net;
using System.Reflection;
using System.Security.Permissions;
using System.Web.Services.Description;
using System.Xml;
using Newtonsoft.Json;
using Newtonsoft.Json.Linq;
using FSS.Omnius.Modules.Entitron.Entity;
using System.Text;
using System.Collections.Generic;
using System.Xml.Linq;
using System.Runtime.Serialization.Json;

namespace FSS.Omnius.Modules.Nexus.Gate
{
    public class WS
    {
        private string _soapEnvelope = @"<soap:Envelope {0}><soap:Body>{1}</soap:Body></soap:Envelope>";

        [SecurityPermission(SecurityAction.Demand, Unrestricted = true)]
        public JToken CallWebService(string serviceName, string methodName, object[] args)
        {
            Entitron.Entity.Nexus.WS row = GetModel(serviceName);

            if (row.Type != Entitron.Entity.Nexus.WSType.SOAP) {
                throw new Exception("Neplatné SOAP volání webové služby. Webová služba je typu REST");
            }

            Assembly asm = Assembly.LoadFile(@"C:\Temp\WSProxy" + row.Id + ".dll");
            Type type = asm.GetTypes()[0];
            var ws = Activator.CreateInstance(type);

            if (!string.IsNullOrEmpty(row.Auth_User) && !string.IsNullOrEmpty(row.Auth_Password))
            {
                ws.GetType().GetProperty("PreAuthenticate").SetValue(ws, true);
                ws.GetType().GetProperty("Credentials").SetValue(ws, new NetworkCredential(row.Auth_User, row.Auth_Password));
            }

            MethodInfo mi = ws.GetType().GetMethod(methodName);

            object response = mi.Invoke(ws, args);
            string jsonText;
           
            try
            {
                XmlDocument xml = new XmlDocument();
                xml.LoadXml(response as string);

                jsonText = JsonConvert.SerializeXmlNode(xml);
            }
            catch(Exception e)
            {
                if (e is ArgumentNullException || e is XmlException)
                {
                    jsonText = JsonConvert.SerializeObject(response);
                }
                else
                    throw e;
            }
            JToken json = JToken.Parse(jsonText);

            return json;
        }

        public JToken CallWebService(string serviceName, string methodName, string jsonBody)
        {
            string jsonText;

            Entitron.Entity.Nexus.WS row = GetModel(serviceName);

            if (row.Type != Entitron.Entity.Nexus.WSType.SOAP) {
                throw new Exception("Neplatné SOAP volání webové služby. Webová služba je typu REST");
            }
            
            string data = createSoapEnvelope(row, jsonBody);

            HttpWebRequest request = WebRequest.Create(new Uri(row.SOAP_Endpoint)) as HttpWebRequest;

            if (!string.IsNullOrEmpty(row.Auth_User) && !string.IsNullOrEmpty(row.Auth_Password)) {
                request.Credentials = new NetworkCredential(row.Auth_User, row.Auth_Password);
            }

            // Set type to POST
            request.Method = "POST";
            request.ContentType = string.Format("application/soap+xml;charset=UTF-8;action=\"{0}\"", methodName);
            
            // Create the data we want to send
            byte[] byteData = Encoding.UTF8.GetBytes(data.ToString());
            request.ContentLength = byteData.Length; 

            // Write data to request
            using (Stream postStream = request.GetRequestStream()) {
                postStream.Write(byteData, 0, byteData.Length);
            }

            // Get response and return it
            string result;
            XmlDocument xmlResult = new XmlDocument();
            try {
                using (HttpWebResponse response = request.GetResponse() as HttpWebResponse) {
                    StreamReader reader = new StreamReader(response.GetResponseStream());
                    result = reader.ReadToEnd();
                    reader.Close();
                }
                xmlResult.LoadXml(result);
            }
            catch (WebException webException) {
                if (webException.Response == null)
                    throw webException;

                using(WebResponse webResponse = webException.Response) {
                    StreamReader reader = new StreamReader(webResponse.GetResponseStream());
                    string errorMessage = reader.ReadToEnd();
                    reader.Close();

                    throw new Exception($"SOAP Exception: {errorMessage}");
                }
            }
            catch (Exception e) {
                throw e;
            }

            jsonText = JsonConvert.SerializeXmlNode(xmlResult);
            return JToken.Parse(jsonText);
        }

        public JToken CallRestService(string serviceName, string methodName, NameValueCollection queryParams)
        {
            Entitron.Entity.Nexus.WS row = GetModel(serviceName);

            if(row.Type != Entitron.Entity.Nexus.WSType.REST) {
                throw new Exception("Neplatné REST volání webové služby. Služba je typu SOAP");
            }

            string url = row.REST_Base_Url;
            url += !url.EndsWith("/") && !methodName.StartsWith("/") ? "/" : "";
            url += methodName;

            WebClient client = new WebClient();
            client.QueryString = queryParams;

            if(!string.IsNullOrEmpty(row.Auth_User) && !string.IsNullOrEmpty(row.Auth_Password))
            {
                Uri uri = new Uri(url);
                client.Credentials = new NetworkCredential(row.Auth_User, row.Auth_Password, uri.Host);
            }

            string jsonText = client.DownloadString(url);
            JToken json = JToken.Parse(jsonText);

            return json;
        }

        #region Tools

        [SecurityPermission(SecurityAction.Demand, Unrestricted = true)]
        public bool CreateProxyForWS(Entitron.Entity.Nexus.WS model)
        {
            Stream stream;
            System.Net.WebClient client = new System.Net.WebClient();

            if (model.WSDL_Url != null && model.WSDL_Url.Length > 0)
            {
                stream = client.OpenRead(model.WSDL_Url);
            }
            else if (model.WSDL_File != null && model.WSDL_File.Length > 0)
            {
                stream = new MemoryStream(model.WSDL_File);
            }
            else
            {
                return true;
            }

            ServiceDescription description = ServiceDescription.Read(stream);

            // Initialize a service description importer.
            ServiceDescriptionImporter importer = new ServiceDescriptionImporter();
            importer.ProtocolName = "Soap12"; // Use SOAP 1.2.
            importer.AddServiceDescription(description, null, null);
            importer.Style = ServiceDescriptionImportStyle.Client;
            importer.CodeGenerationOptions = System.Xml.Serialization.CodeGenerationOptions.GenerateProperties;

            // Initialize a Code-DOM tree into which we will import the service.
            CodeNamespace ns = new CodeNamespace();
            CodeCompileUnit unit = new CodeCompileUnit();
            unit.Namespaces.Add(ns);

            // Import the service into the Code-DOM tree. This creates proxy code that uses the service.
            ServiceDescriptionImportWarnings warning = importer.Import(ns, unit);

            if (warning == 0)
            {
                // Generate the proxy code
                CodeDomProvider provider = CodeDomProvider.CreateProvider("CSharp");

                // Compile the assembly proxy with the appropriate references
                string[] assemblyReferences = new string[5] { "System.dll", "System.Web.Services.dll", "System.Web.dll", "System.Xml.dll", "System.Data.dll" };

                // Cesta pro uložení assembly
                string path = System.Reflection.Assembly.GetAssembly(typeof(WS)).Location;
                string dir = Path.GetDirectoryName(path);

                CompilerParameters parameters = new CompilerParameters(assemblyReferences);
                parameters.GenerateExecutable = false;
                parameters.CompilerOptions = " /out:C:\\temp\\WSProxy" + model.Id + ".dll";

                CompilerResults results = provider.CompileAssemblyFromDom(parameters, unit);

                if (results.Errors.Count > 0)
                {
                    foreach (CompilerError oops in results.Errors)
                    {
                        System.Diagnostics.Debug.WriteLine("========Compiler error============");
                        System.Diagnostics.Debug.WriteLine(oops.ErrorText);
                    }
                    throw new System.Exception("Compile Error Occured generating webservice proxy. Check Debug ouput window.");
                }

                return true;

            }
            else
            {
                return false;
            }
        }

        private static Entitron.Entity.Nexus.WS GetModel(string serviceName)
        {
            DBEntities e = DBEntities.instance;
            Entitron.Entity.Nexus.WS row = e.WSs.Single(m => m.Name == serviceName);
            return row;
        }
               
        private string createSoapEnvelope(Entitron.Entity.Nexus.WS model, string jsonBody)
        {
            XmlDocument xmlBody = JsonConvert.DeserializeXmlNode(jsonBody);
            
            string xmlNsList = model.SOAP_XML_NS.Replace("\r\n", " ");

            return string.Format(_soapEnvelope,
                    xmlNsList,
                    xmlBody.OuterXml.Replace("__", ":")
                );
        }

        #endregion
    }
}

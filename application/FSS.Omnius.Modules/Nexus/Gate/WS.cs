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

namespace FSS.Omnius.Modules.Nexus.Gate
{
    public class WS
    {
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
                throw new Exception("Chyba při ukládání webové služby. Nebyl zadán xml soubor s definicí ani URL definice.");
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
               
        #endregion
    }
}

using System;
using System.Collections.Generic;
using System.Text;
using System.Linq;
using System.Reflection;
using System.CodeDom;
using System.CodeDom.Compiler;
using System.IO;
using System.Security.Permissions;
using System.Web.Services.Description;
using FSS.Omnius.Modules.Entitron.Entity;

namespace FSS.Omnius.Modules.Nexus.Gate
{
    public class WS
    {
        [SecurityPermissionAttribute(SecurityAction.Demand, Unrestricted = true)]
        public bool CreateProxyForWS(Entitron.Entity.Nexus.WS model)
        {
            Stream stream;
            System.Net.WebClient client = new System.Net.WebClient();

            if (model.WSDL_Url.Length > 0) {
                stream = client.OpenRead(model.WSDL_Url);
            }
            else { //!!!
                stream = client.OpenRead(model.WSDL_Url);
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


        [SecurityPermissionAttribute(SecurityAction.Demand, Unrestricted = true)]
        public object CallWebService(string serviceName, string methodName, object[] args)
        {
            DBEntities e = new DBEntities();
            Entitron.Entity.Nexus.WS row = e.WSs.Single(m => m.Name == serviceName);

            Assembly asm = Assembly.LoadFile(@"C:\Temp\WSProxy" + row.Id + ".dll");
            Type type = asm.GetTypes()[0];
            var ws = Activator.CreateInstance(type);

            MethodInfo mi = ws.GetType().GetMethod(methodName);
            return mi.Invoke(ws, args);
        }
    }
}

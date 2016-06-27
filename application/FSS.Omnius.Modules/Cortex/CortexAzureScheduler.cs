using FSS.Omnius.Modules.Cortex.Interface;
using FSS.Omnius.Modules.Entitron.Entity.Cortex;
using System;
using System.Collections;
using System.Net;
using System.Web;
using System.Web.Configuration;
using System.IO.Compression;
using System.IO;
using System.Security.Cryptography.X509Certificates;
using System.Collections.Generic;
using Microsoft.IdentityModel.Clients.ActiveDirectory;
using System.Threading;

namespace FSS.Omnius.Modules.Cortex
{
    public class CortexAzureScheduler : ICortexAPI
    {
        private HttpRequestBase Request;

        private string subscriptionId;
        private string cloudServiceId;
        private string jobCollectionId;
        private string baseUrl;
        private string apiVersion;
        private string fullAPIUrl;

        private string systemAccountName;
        private string systemAccountPass;

        public CortexAzureScheduler(HttpRequestBase request)
        {
            Request = request;
            GetSettings();
            SetFullAPIUrl();
        }

        public string List()
        {
            string url = string.Format("{0}?api-version={1}", fullAPIUrl, apiVersion);

            HttpWebRequest r = GetRequest(url);
            r.Method = "GET";

            WebResponse res = r.GetResponse();
            var content = res.GetResponseStream();

            return string.Join("", content);
        }

        public void Create(Task model)
        {
            string url = string.Format("{0}/{1}?api-version={3}", fullAPIUrl, GetJobId(model), apiVersion);

            WebRequest r = GetRequest(url);
            r.Method = "PUT";
            r.Headers.Add("Content-Type", "application/json");




               /* using (WebClient c = new WebClient()) {
                    c.Headers.Add("Content-Disposition: attachement; filename=" + fileName + ".zip");
                    c.Headers.Add("Content-Type: application/zip");
                    //c.Credentials = new NetworkCredential(userName, password);
                    using(Stream putStream = c.OpenWrite(url, "PUT")) {
                        byte[] content = memStream.ToArray();
                        putStream.Write(content, 0, content.Length);
                    }
                }*/
            
        }

        public void Change(Task model, Task original)
        {
            Delete(original);
            Create(model);
        }

        public void Delete(Task model)
        {
            string url = string.Format("{0}/{1}?api-version={3}", fullAPIUrl, GetJobId(model), apiVersion);

            HttpWebRequest r = GetRequest(url);
            r.Method = "DELETE";
            
            
            WebResponse res = r.GetResponse();
        }
        
        #region tools

        private void GetSettings()
        {
            subscriptionId = WebConfigurationManager.AppSettings["CortexAzureSubscriptionId"];
            cloudServiceId = WebConfigurationManager.AppSettings["CortexAzureCloudServiceId"];
            jobCollectionId = WebConfigurationManager.AppSettings["CortexAzureJobCollectionId"];
            baseUrl = WebConfigurationManager.AppSettings["CortexAzureSchedulerBaseUrl"];
            apiVersion = WebConfigurationManager.AppSettings["CortexAzureSchedulerAPIVersion"];

            systemAccountName = WebConfigurationManager.AppSettings["SystemAccountName"];
            systemAccountPass = WebConfigurationManager.AppSettings["SystemAccountPass"];
        }

        private void SetFullAPIUrl()
        {
            fullAPIUrl = string.Format("{0}{1}/cloudServices/{2}/resources/scheduler/~/jobCollections/{3}/jobs", 
                    baseUrl + (baseUrl.EndsWith("/") ? "" : "/"),
                    subscriptionId,
                    cloudServiceId,
                    jobCollectionId
                );
        }

        private string GetJobId(Task model)
        {
            string jobId = string.Format("{0}_{1}", model.Id, model.Name);
            return ExtendMethods.URLSafeString(jobId);
        }

        private HttpWebRequest GetRequest(string url)
        {
            //X509Certificate2 cert = new X509Certificate2(@"C:\Temp\cert-localhost.pfx", "FssOmniusTest");
            //X509Certificate2 cert = GetStoreCertificate("820D1304B011256D8D6A49C80325A854A7422F51");

            string token = GetAuthorizationHeader();

            HttpWebRequest r = (HttpWebRequest)HttpWebRequest.Create(url);
            r.Headers.Add("x-ms-version", "2013-06-01");
            r.Headers.Add(HttpRequestHeader.Authorization, "Bearer " + token);
            r.Host = "localhost";
            //r.ClientCertificates.Add(cert);

            return r;
        }

        private static X509Certificate2 GetStoreCertificate(string thumbprint)
        {
            List<StoreLocation> locations = new List<StoreLocation>{
                StoreLocation.CurrentUser,
                StoreLocation.LocalMachine
              };

            foreach (var location in locations) {
                X509Store store = new X509Store("My", location);
                try {
                    store.Open(OpenFlags.ReadOnly | OpenFlags.OpenExistingOnly);
                    X509Certificate2Collection certificates = store.Certificates.Find(
                      X509FindType.FindByThumbprint, thumbprint, false);
                    if (certificates.Count == 1) {
                        return certificates[0];
                    }
                }
                finally {
                    store.Close();
                }
            }
            throw new ArgumentException(string.Format(
              "A Certificate with Thumbprint '{0}' could not be located.",
              thumbprint));
        }

        private static string GetAuthorizationHeader()
        {
            AuthenticationResult result = null;

            //var context = new AuthenticationContext("https://login.windows.net/1acafdbb-53ac-49c7-8130-2ce9180e4076");
            var context = new AuthenticationContext("https://login.microsoftonline.com/1acafdbb-53ac-49c7-8130-2ce9180e4076/oauth2/authorize");

            var c = new UserCredential("martin.novak@futuresolutionservices.com", "Mnk20051993");

            var thread = new Thread(() =>
            {
                result = context.AcquireToken(
                  "https://management.core.windows.net/",
                  "e2431075-fbf2-4c05-be15-b4d91122c69a",
                  c);
            });
            
            thread.SetApartmentState(ApartmentState.STA);
            thread.Name = "AquireTokenThread";
            thread.Start();
            thread.Join();

            if (result == null) {
                throw new InvalidOperationException("Failed to obtain the JWT token");
            }

            string token = result.AccessToken;
            return token;
        }

        private IEnumerable Enums<T>()
        {
            return Enum.GetValues(typeof(T));
        }

        #endregion
    }
}

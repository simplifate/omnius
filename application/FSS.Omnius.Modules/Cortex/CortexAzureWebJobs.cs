using FSS.Omnius.Modules.Cortex.Interface;
using FSS.Omnius.Modules.Entitron.Entity.Cortex;
using System;
using System.Collections;
using System.Net;
using System.Web;
using System.Web.Configuration;
using System.IO.Compression;
using System.IO;

namespace FSS.Omnius.Modules.Cortex
{
    public class CortexAzureWebJobs : ICortexAPI
    {
        private HttpRequestBase Request;
        private string userName;
        private string password;
        private string endPoint;
        private string protocol;
        private string fullAPIUrl;

        private string systemAccountName;
        private string systemAccountPass;

        public CortexAzureWebJobs(HttpRequestBase request)
        {
            Request = request;
            GetSettings();
            SetFullAPIUrl();
        }

        public void Create(Task model)
        {
            string fileName = string.Format("{0}-{1}.php", model.Id, model.Name.Replace(' ', '_'));
            string url = string.Format("{0}triggeredwebjobs/{1} - {2}", fullAPIUrl, model.Id, model.Name);
            string script = GetRunScript(model);

            using(MemoryStream memStream = new MemoryStream()) {
                using (ZipArchive zip = new ZipArchive(memStream, ZipArchiveMode.Create, true)) {
                    ZipArchiveEntry entry = zip.CreateEntry(fileName);

                    using (Stream entryStream = entry.Open()) {
                        using(StreamWriter writer = new StreamWriter(entryStream)) {
                            writer.Write(script);
                        }
                    }
                }

                memStream.Seek(0, SeekOrigin.Begin);

                using (WebClient c = new WebClient()) {
                    c.Headers.Add("Content-Disposition: attachement; filename=" + fileName + ".zip");
                    c.Headers.Add("Content-Type: application/zip");
                    c.Credentials = new NetworkCredential(userName, password);
                    using(Stream putStream = c.OpenWrite(url, "PUT")) {
                        byte[] content = memStream.ToArray();
                        putStream.Write(content, 0, content.Length);
                    }
                }
            }
        }

        public void Change(Task model, Task original)
        {
            Delete(original);
            Create(model);
        }

        public void Delete(Task model)
        {
            string url = string.Format("{0}triggeredwebjobs/{1} - {2}", fullAPIUrl, model.Id, model.Name);

            WebRequest r = HttpWebRequest.Create(url);
            r.Method = "DELETE";
            r.Credentials = new NetworkCredential(userName, password);

            WebResponse res = r.GetResponse();
        }

        private string GetRunScript(Task model)
        {
            string runUrl = string.Format("{0}//{1}:{2}@{3}{4}{5}",
                    Request.IsSecureConnection ? "https" : "http",
                    systemAccountName,
                    systemAccountPass,
                    Request.Headers["Host"],
                    model.Url.StartsWith("/") ? "" : "/",
                    model.Url
                );

            return $"<?php $url = '{runUrl}'; $r = curl_init($url); curl_exec($r);";
        }

        #region tools

        private void GetSettings()
        {
            userName = WebConfigurationManager.AppSettings["CortexUserName"];
            password = WebConfigurationManager.AppSettings["CortexUserPassword"];
            endPoint = WebConfigurationManager.AppSettings["CortexAzureEndPoint"];
            protocol = WebConfigurationManager.AppSettings["CortexAzureEndPointProtocol"];
            systemAccountName = WebConfigurationManager.AppSettings["SystemAccountName"];
            systemAccountPass = WebConfigurationManager.AppSettings["SystemAccountPass"];
        }

        private void SetFullAPIUrl()
        {
            fullAPIUrl = string.Format("{0}{1}", protocol, endPoint);
        }

        private IEnumerable Enums<T>()
        {
            return Enum.GetValues(typeof(T));
        }

        #endregion
    }
}

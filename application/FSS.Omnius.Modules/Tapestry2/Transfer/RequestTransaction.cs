using System;
using System.Collections.Generic;
using System.IO;
using System.Text;
using System.Web;

namespace FSS.Omnius.Modules.Tapestry2.Transfer
{
    [Serializable]
    public class RequestTransaction
    {
        public string ServerName { get; set; }
        public Dictionary<string, byte[]> Files { get; set; }

        public static RequestTransaction Create(HttpRequest request)
        {
            RequestTransaction result = new RequestTransaction();
            result.ServerName = request.ServerVariables["SERVER_NAME"];
            result.Files = new Dictionary<string, byte[]>();
            for (int i = 0; i < request.Files.Count; i++)
            {
                using (MemoryStream ms = new MemoryStream())
                {
                    request.Files[i].InputStream.CopyTo(ms);

                    result.Files.Add(request.Files[i].FileName, ms.ToArray());
                }
            }

            return result;
        }
    }
}

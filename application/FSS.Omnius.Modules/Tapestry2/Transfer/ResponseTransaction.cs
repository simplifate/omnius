using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using System.Web;

namespace FSS.Omnius.Modules.Tapestry2.Transfer
{
    [Serializable]
    public class ResponseTransaction
    {
        private ResponseTransaction()
        { }

        public string RedirectUrl { get; set; }
        public string FileName { get; set; }
        public string FileType { get; set; }
        public byte[] FileBinary { get; set; }

        public void Apply(HttpResponse httpResponse)
        {
            if (RedirectUrl != null)
            {
                httpResponse.Redirect(RedirectUrl);
                return;
            }

            if (FileName != null)
            {
                httpResponse.Clear();
                httpResponse.StatusCode = 200;
                httpResponse.ContentType = FileType;
                httpResponse.AddHeader("content-disposition", $"attachment; filename={FileName}");
                httpResponse.BinaryWrite(FileBinary);
                httpResponse.Flush();
                httpResponse.Close();
                httpResponse.End();
            }
        }

        public static ResponseTransaction Create(string fileName, string type, byte[] binary)
        {
            return new ResponseTransaction
            {
                FileName = fileName,
                FileType = type,
                FileBinary = binary
            };
        }
        public static ResponseTransaction Create(string redirectUrl)
        {
            return new ResponseTransaction
            {
                RedirectUrl = redirectUrl
            };
        }
    }
}

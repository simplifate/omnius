using FSS.Omnius.Modules.CORE;
using System;
using System.Collections.Generic;
using System.IO;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using System.Web;

namespace FSS.Omnius.Modules.Tapestry.Actions.other
{
    /// <summary>
    /// Přijme content ve stringu a pošle ho zpět ke stažení pod zadaným jménem. 
    /// Odřádkování je standartní '\n'.
    /// </summary>
    class DownloadAsFile : Action
    {
        public override int Id
        {
            get
            {
                return 181666;
            }
        }

        public override string[] InputVar
        {
            get
            {
                return new string[] { "s$FileName","s$Content" };
            }
        }

        public override string Name
        {
            get
            {
                return "Download String as File";
            }
        }

        public override string[] OutputVar
        {
            get
            {
                return new string[0];
            }
        }

        public override int? ReverseActionId
        {
            get
            {
                return null;
            }
        }

        public override void InnerRun(Dictionary<string, object> vars, Dictionary<string, object> outputVars, Dictionary<string, object> InvertedInputVars, Message message)
        {
            string fileName = (string)vars["FileName"];
            string content = (string)vars["Content"];
            content = content.Replace("\\n", "\n");

            HttpContext context = HttpContext.Current;
            HttpResponse response = context.Response;
            response.Clear();
            response.ContentType = "application/csv";
            response.AddHeader("content-disposition", "attachment; filename=" + fileName);
            response.StatusCode = 200;

            response.Write(content);
            response.Flush();
            response.Close();
            response.End();
        }
    }
}

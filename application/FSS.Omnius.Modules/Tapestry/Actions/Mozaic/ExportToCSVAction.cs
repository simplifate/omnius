using FSS.Omnius.Modules.CORE;
using FSS.Omnius.Modules.Entitron;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Net;
using System.Text;
using System.Threading.Tasks;
using System.Web;

namespace FSS.Omnius.Modules.Tapestry.Actions.Mozaic
{
    [MozaicRepository]
    public class ExportToCSVAction : Action
    {
        public override int Id
        {
            get
            {
                return 2003;
            }
        }
        public override int? ReverseActionId
        {
            get
            {
                return null;
            }
        }
        public override string[] InputVar
        {
            get
            {
                return new string[] { "Data", "?Delimiter" };
            }
        }

        public override string Name
        {
            get
            {
                return "Export to CSV";
            }
        }

        public override string[] OutputVar
        {
            get
            {
                return new string[] { };
            }
        }

        public override void InnerRun(Dictionary<string, object> vars, Dictionary<string, object> outputVars, Dictionary<string, object> InvertedInputVars, Message message)
        {
            // Init
            CORE.CORE core = (CORE.CORE)vars["__CORE__"];
            List<string> rows = new List<string>();

            List<DBItem> data = (List<DBItem>)vars["Data"];
            string delimiter = vars.ContainsKey("Delimiter") ? (string)vars["Delimiter"] : ";";

            // Připravíme CSV
            //nastavit header
            List<string> header = new List<string>();
            foreach (string column in data[0].getColumnNames())
            {
                header.Add($"{column}");
            }
            rows.Add(string.Join(delimiter, header));
            
            foreach(DBItem item in data) 
            {
                List<string> row = new List<string>();
                foreach(string column in data[0].getColumnNames()) {
                    row.Add($"{item[column].ToString()}");
                }
                rows.Add(string.Join(delimiter, row));
            }

            string csv = string.Join("\r\n", rows);
            
            HttpContext context = HttpContext.Current;
            HttpResponse response = context.Response;


            response.ContentType = "application/csv";
            response.Charset = Encoding.GetEncoding(1250).EncodingName;
            response.ContentEncoding = Encoding.GetEncoding(1250);
            response.AddHeader("content-disposition", "attachment; filename=export.csv");
            response.Write(csv);
            response.Flush();
            response.Close();
        }
    }
}

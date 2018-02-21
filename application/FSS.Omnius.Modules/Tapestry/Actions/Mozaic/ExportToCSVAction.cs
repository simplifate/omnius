using System.Collections.Generic;
using System.Text;
using System.Web;
using System.IO;
using FSS.Omnius.Modules.CORE;
using FSS.Omnius.Modules.Entitron.DB;

namespace FSS.Omnius.Modules.Tapestry.Actions.Mozaic
{
    [MozaicRepository]
    public class ExportToCSVAction : Action
    {
        public override int Id => 2003;

        public override int? ReverseActionId => null;

        public override string[] InputVar => new string[] { "Data", "?Delimiter", "?ExportPath" };

        public override string Name => "Export to CSV";

        public override string[] OutputVar => new string[] { };

        public override void InnerRun(Dictionary<string, object> vars, Dictionary<string, object> outputVars, Dictionary<string, object> InvertedInputVars, Message message)
        {
            // Init
            List<string> rows = new List<string>();
            string exportPath = vars.ContainsKey("ExportPath") ? (string)vars["ExportPath"] : null;

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

            if (string.IsNullOrEmpty(exportPath)) //send to browser
            {
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
            else //save file to server
            {
                File.WriteAllText(exportPath, csv);
            }
        }
    }
}

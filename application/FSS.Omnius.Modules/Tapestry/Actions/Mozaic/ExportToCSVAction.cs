using System;
using System.Collections.Generic;
using System.Text;
using System.Web;
using System.IO;
using System.Linq;
using FSS.Omnius.Modules.CORE;
using FSS.Omnius.Modules.Entitron.DB;

namespace FSS.Omnius.Modules.Tapestry.Actions.Mozaic
{
    [MozaicRepository]
    public class ExportToCSVAction : Action
    {
        public override int Id => 2003;

        public override int? ReverseActionId => null;

        public override string[] InputVar => new string[] { "?TableName", "?Data", "?Columns", "?Delimiter", "?ExportPath" };

        public override string Name => "Export to CSV";

        public override string[] OutputVar => new string[] { };

        public override void InnerRun(Dictionary<string, object> vars, Dictionary<string, object> outputVars, Dictionary<string, object> InvertedInputVars, Message message)
        {
            /// Init
            DBConnection db = COREobject.i.Entitron;
            string exportPath = vars.ContainsKey("ExportPath") ? (string)vars["ExportPath"] : null;
            string delimiter = vars.ContainsKey("Delimiter") ? (string)vars["Delimiter"] : ";";
            StringBuilder result = new StringBuilder();

            /// get data
            // from vars
            List<DBItem> data = vars.ContainsKey("Data")
                ? (List<DBItem>)vars["Data"]
                : new List<DBItem>();
            // from table
            if (vars.ContainsKey("TableName"))
            {
                var innerOutputVars = new Dictionary<string, object>();
                new Entitron.SelectAction().InnerRun(vars, innerOutputVars, null, message);
                data.AddRange((IEnumerable<DBItem>)innerOutputVars["Data"]);
            }

            /// get columns
            IEnumerable<string> columns = null;
            // from vars
            if (vars.ContainsKey("Columns"))
            {
                if (vars["Columns"] is string)
                    columns = (vars["Columns"] as string).Split(';');
                else if (vars["Columns"] is IEnumerable<string>)
                    columns = (vars["Columns"] as IEnumerable<string>);
            }
            // from item
            else if (data.Count > 0)
                columns = data.First().getColumnNames();

            /// generate csv
            // header
            result.AppendLine(string.Join(delimiter, columns));
            // data
            foreach (var item in data)
            {
                result.AppendLine(string.Join(delimiter, columns.Select(c => item[c].ToString())));
            }

            /// return
            // send to browser
            if (string.IsNullOrEmpty(exportPath))
            {
                HttpContext context = HttpContext.Current;
                HttpResponse response = context.Response;

                response.ContentType = "application/csv";
                response.Charset = Encoding.GetEncoding(1250).EncodingName;
                response.ContentEncoding = Encoding.GetEncoding(1250);
                response.AddHeader("content-disposition", "attachment; filename=export.csv");
                response.Write(result.ToString());
                response.Flush();
                response.Close();
            }
            //save file to server
            else
            {
                File.WriteAllText(exportPath, result.ToString());
            }
        }
    }
}

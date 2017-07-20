using FSS.Omnius.Modules.CORE;
using FSS.Omnius.Modules.Entitron;
using FSS.Omnius.Modules.Entitron.Sql;
using FSS.Omnius.Modules.Watchtower;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using System.Web;
using Microsoft.VisualBasic.FileIO;
using System.Globalization;

namespace FSS.Omnius.Modules.Tapestry.Actions.Entitron
{
    [EntitronRepository]
    public class ImportCSVAction : Action
    {
        public override int Id
        {
            get
            {
                return 1042;
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
                return new string[] { "s$InputName", "s$TableName", "?s$Delimiter", "?b$HasFieldsInQuotes", "?s$UniqueColumns", "?s$DateTimeFormat" };
            }
        }

        public override string Name
        {
            get
            {
                return "Import CSV";
            }
        }

        public override string[] OutputVar
        {
            get
            {
                return new string[] { "Success", "Message" };
            }
        }

        private string typeError = "Řádek {0}, sloupec {1}: hodnota musí být {2}. Řádek vynechán.";
        private string uniqueError = "Řádek {0}: Tabulka již obsahuje takovýto řádek. Řádek vynechán.";

        public override void InnerRun(Dictionary<string, object> vars, Dictionary<string, object> outputVars, Dictionary<string, object> InvertedInputVars, Message message)
        {
            // init
            CORE.CORE core = (CORE.CORE)vars["__CORE__"];

            int countAdded = 0;
            string inputName = (string)vars["InputName"];
            string tableName = (string)vars["TableName"];
            string delimiter = vars.ContainsKey("Delimiter") ? (string)vars["Delimiter"] : ";";
            string dateFormat = vars.ContainsKey("DateTimeFormat") ? (string)vars["DateTimeFormat"] : "yyyy-MM-dd";
            bool enclosed = vars.ContainsKey("b$HasFieldsInQuotes") ? (bool)vars["b$HasFieldsInQuotes"] : false;
            List<string> uniqueColumns = vars.ContainsKey("UniqueColumns") ? ((string)vars["UniqueColumns"]).Split(',').ToList() : new List<string>();

            DBTable table = core.Entitron.GetDynamicTable(tableName, false);
            if (table == null) {
                throw new Exception(string.Format("{0}: Cílová tabulka nebyla nalezena ({1})", Name, tableName));
            }

            DBColumns columns = table.columns;
            Dictionary<int, DBColumn> columnsMap = new Dictionary<int, DBColumn>();

            var files = HttpContext.Current.Request.Files;
            if (files == null)
                return;

            List<string> messages = new List<string>();

            foreach (string fileName in files) {
                HttpPostedFile file = HttpContext.Current.Request.Files[fileName];

                if (file.ContentLength == 0 || fileName != inputName)
                    continue;

                using (TextFieldParser parser = new TextFieldParser(file.InputStream)) {
                    parser.TextFieldType = FieldType.Delimited;
                    parser.SetDelimiters(delimiter);
                    parser.HasFieldsEnclosedInQuotes = enclosed;
                    parser.TrimWhiteSpace = true;

                    while (!parser.EndOfData) {
                        long line = parser.LineNumber;
                        string[] fields = parser.ReadFields();

                        if (line == 1) {
                            int i = 0;
                            foreach (string field in fields) {
                                if (columns.Where(c => c.Name == field).Count() > 0) {
                                    columnsMap.Add(i, columns.Where(c => c.Name == field).First());
                                }
                                i++;
                            }
                        }
                        else {
                            int i = 0;
                            Dictionary<string, object> data = new Dictionary<string, object>();
                            bool isValid = true;

                            foreach (string value in fields) {
                                // Neznámé sloupce ignorujeme
                                if (!columnsMap.ContainsKey(i)) {
                                    i++;
                                    continue;
                                }

                                // Prázdné hodnoty vynecháme
                                if(string.IsNullOrEmpty(value)) {
                                    i++;
                                    continue;
                                }

                                DBColumn col = columnsMap[i];

                                switch (col.type.ToLower())
                                {
                                    case "nvarchar":
                                    case "varchar":
                                        data.Add(col.Name, value);
                                        break;
                                    case "boolean":
                                    case "bit":
                                        bool parsedBool;
                                        if (bool.TryParse(value, out parsedBool)) {
                                            data.Add(col.Name, parsedBool);
                                        }
                                        else {
                                            if (value == "0") {
                                                data.Add(col.Name, false);
                                            }
                                            else if (value == "1") {
                                                data.Add(col.Name, true);
                                            }
                                            else {
                                                isValid = false;
                                                messages.Add(string.Format(typeError, line, col.Name, "logická hodnota"));
                                            }
                                        }
                                        break;
                                    case "int":
                                    case "integer":
                                        int parsedInt;
                                        if (int.TryParse(value, NumberStyles.Any, CultureInfo.InvariantCulture, out parsedInt)) {
                                            data.Add(col.Name, parsedInt);
                                        }
                                        else {
                                            isValid = false;
                                            messages.Add(string.Format(typeError, line, col.Name, "celé číslo"));
                                        }
                                        break;
                                    case "float":
                                        double parsedDouble;
                                        if (double.TryParse(value, NumberStyles.Any, CultureInfo.InvariantCulture, out parsedDouble)) {
                                            data.Add(col.Name, parsedDouble);
                                        }
                                        else {
                                            isValid = false;
                                            messages.Add(string.Format(typeError, line, col.Name, "celé nebo desetinní číslo"));
                                        }
                                        break;
                                    case "datetime":
                                    case "date":
                                    case "time":
                                        DateTime parsedDateTime;
                                        if (DateTime.TryParseExact(value, dateFormat, CultureInfo.InvariantCulture, DateTimeStyles.AdjustToUniversal, out parsedDateTime)) {
                                            data.Add(col.Name, parsedDateTime);
                                        }
                                        else {
                                            isValid = false;
                                            messages.Add(string.Format(typeError, line, col.Name, "platné datum"));
                                        }
                                        break;
                                }
                                i++;
                            }

                            if (!isValid) {
                                continue;
                            }

                            if (uniqueColumns.Count > 0) {
                                var select = table.Select();
                                Conditions condition = new Conditions(select);
                                Condition_concat outCondition = null;

                                // setConditions
                                foreach (string colName in uniqueColumns) {
                                    object condValue = data[colName].ToString().PadLeft(8,'0') ;
                                    DBColumn column = columns.Single(c => c.Name == colName);

                                    outCondition = condition.column(colName).Equal(condValue);
                                    condition = outCondition.and();
                                }

                                // check if row already exists
                                if (select.where(c => outCondition).ToList().Count > 0) {
                                    isValid = false;
                                    messages.Add(string.Format(uniqueError, line));
                                }
                            }

                            if (isValid) {
                                data.Add("Datum_vlozeni",DateTime.Now);
                                data.Add("Cas_editace", DateTime.Now);
                                DBItem item = new DBItem();
                                int j = 0;
                                foreach (KeyValuePair<string, object> kv in data) {
                                    item.createProperty(j, kv.Key, kv.Value);
                                    j++;
                                }
                                table.Add(item);
                                countAdded++;
                            }
                        }
                    }
                }
            }

            if (countAdded > 0) {
                core.Entitron.Application.SaveChanges();
            }

            outputVars["Success"] = messages.Count() == 0;
            outputVars["Message"] = string.Join("<br>", messages);
        }
    }
}
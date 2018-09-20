using System;
using System.Collections.Generic;
using System.Data;
using System.IO;
using System.Linq;
using System.Text;
using System.Web;
using System.Globalization;
using FSS.Omnius.Modules.CORE;
using FSS.Omnius.Modules.Entitron.DB;
using CsvHelper;

namespace FSS.Omnius.Modules.Tapestry.Actions.Entitron
{
    [EntitronRepository]
    public class ImportCSVAction : Action
    {
        public override int Id => 1043;

        public override int? ReverseActionId => null;

        public override string[] InputVar => new string[] { "s$CsvSource", "?isServerFile", "s$TableName", "?s$Delimiter", "?b$HasFieldsInQuotes", "?s$UniqueColumns", "?s$DateTimeFormat" };

        public override string Name => "Import CSV";

        public override string[] OutputVar => new string[] { "Success", "Message", "CountAdded" };

        private string typeError = "Řádek {0}, sloupec {1}: hodnota musí být {2}. Řádek vynechán.";
        private string uniqueError = "Řádek {0}: Tabulka již obsahuje takovýto řádek. Řádek vynechán.";
        private string uniqueColumnMissingError = "Řádek {0}: Unikátní sloupec není vyplněn. Řádek vynechán.";

        public override void InnerRun(Dictionary<string, object> vars, Dictionary<string, object> outputVars, Dictionary<string, object> InvertedInputVars, Message message)
        {
            // init
            COREobject core = COREobject.i;
            DBConnection db = core.Entitron;
            bool isServerFile = vars.ContainsKey("isServerFile") ? (bool)vars["isServerFile"] : false;

            int countAdded = 0;
            //jméno form inputu, nebo cesta k souboru
            string csvSource = (string)vars["CsvSource"];
            string tableName = (string)vars["TableName"];
            string delimiter = vars.ContainsKey("Delimiter") ? (string)vars["Delimiter"] : ";";
            string dateFormat = vars.ContainsKey("DateTimeFormat") ? (string)vars["DateTimeFormat"] : "yyyy-MM-dd";
            bool enclosed = vars.ContainsKey("b$HasFieldsInQuotes") ? (bool)vars["b$HasFieldsInQuotes"] : false;
            List<string> uniqueColumns = vars.ContainsKey("UniqueColumns") ? ((string)vars["UniqueColumns"]).Split(',').ToList() : new List<string>();
            CultureInfo czechCulture = new CultureInfo("cs-CZ");
            var columnMetadataList = db.Application.ColumnMetadata.Where(c => c.TableName == tableName).ToList();

            DBTable table = db.Table(tableName, false);
            Dictionary<int, DBColumn> columnsMap = new Dictionary<int, DBColumn>();

            dynamic files;
            if (!isServerFile)
            {
                files = HttpContext.Current.Request.Files;
                if (files == null)
                    return;
            }
            else
            {
                files = new string[] { csvSource };
            }

            List<string> messages = new List<string>();

            bool isHeader = true;
            foreach (string fileName in files)
            {
                HttpPostedFile file = null;
                if (!isServerFile)
                {
                    file = HttpContext.Current.Request.Files[fileName];
                    if (file.ContentLength == 0 || fileName != csvSource)
                        continue;
                }

                Encoding cp1250 = Encoding.GetEncoding(1250);
                using (StreamReader sr = new StreamReader((isServerFile) ? File.OpenRead(csvSource) : file.InputStream, cp1250))
                {
                    sr.Peek();
                    Encoding enc = sr.CurrentEncoding;

                    using (CsvReader reader = new CsvReader(sr))
                    {
                        reader.Configuration.Delimiter = delimiter;
                        reader.Configuration.TrimFields = true;
                        reader.Configuration.TrimHeaders = true;
                        reader.Configuration.Encoding = cp1250;

                        if (reader.ReadHeader())
                        {
                            string[] fields = reader.FieldHeaders;
                            int i = 0;
                            foreach (string field in fields)
                            {
                                var colMetadata = columnMetadataList.SingleOrDefault(c => c.ColumnDisplayName == field);
                                string colName = colMetadata == null ? field : colMetadata.ColumnName;
                                if (table.Columns.Where(c => c.Name == colName).Count() > 0)
                                {
                                    columnsMap.Add(i, table.Columns.Where(c => c.Name == colName).First());
                                }
                                i++;
                            }
                        }
                        else
                        {
                            messages.Add("Nepodařilo se načíst názvy sloupců. Nelze pokračovat.");
                            isHeader = false;
                        }

                        if (isHeader)
                        {
                            while (reader.Read())
                            {
                                long line = reader.Row;
                                string[] fields = reader.CurrentRecord;

                                int i = 0;
                                Dictionary<string, object> data = new Dictionary<string, object>();
                                bool isValid = true;

                                foreach (string value in fields)
                                {
                                    // Neznámé sloupce ignorujeme
                                    if (!columnsMap.ContainsKey(i))
                                    {
                                        i++;
                                        continue;
                                    }

                                    // Prázdné hodnoty vynecháme
                                    if (string.IsNullOrEmpty(value))
                                    {
                                        i++;
                                        continue;
                                    }

                                    DBColumn col = columnsMap[i];

                                    switch (col.Type)
                                    {
                                        case DbType.String:
                                            data.Add(col.Name, value);
                                            break;
                                        case DbType.Boolean:
                                            bool parsedBool;
                                            if (bool.TryParse(value, out parsedBool))
                                            {
                                                data.Add(col.Name, parsedBool);
                                            }
                                            else
                                            {
                                                if (value == "0")
                                                {
                                                    data.Add(col.Name, false);
                                                }
                                                else if (value == "1")
                                                {
                                                    data.Add(col.Name, true);
                                                }
                                                else
                                                {
                                                    isValid = false;
                                                    messages.Add(string.Format(typeError, line, col.Name, "logická hodnota"));
                                                }
                                            }
                                            break;
                                        case DbType.Int32:
                                            int parsedInt;
                                            if (int.TryParse(value, NumberStyles.Any, CultureInfo.InvariantCulture, out parsedInt))
                                            {
                                                data.Add(col.Name, parsedInt);
                                            }
                                            else
                                            {
                                                isValid = false;
                                                messages.Add(string.Format(typeError, line, col.Name, "celé číslo"));
                                            }
                                            break;
                                        case DbType.Double:
                                            double parsedDouble;
                                            if (double.TryParse(value, NumberStyles.Any, CultureInfo.InvariantCulture, out parsedDouble))
                                            {
                                                data.Add(col.Name, parsedDouble);
                                            }
                                            else
                                            {
                                                isValid = false;
                                                messages.Add(string.Format(typeError, line, col.Name, "celé nebo desetinní číslo"));
                                            }
                                            break;
                                        case DbType.DateTime:
                                            try
                                            {
                                                DateTime parsedDateTime = DateTime.Parse(value, czechCulture);
                                                data.Add(col.Name, parsedDateTime);
                                            }
                                            catch (FormatException)
                                            {
                                                isValid = false;
                                                messages.Add(string.Format(typeError, line, col.Name, "platné datum"));
                                            }
                                            break;
                                    }
                                    i++;
                                }

                                if (!isValid)
                                {
                                    continue;
                                }

                                if (uniqueColumns.Count > 0)
                                {
                                    try
                                    {
                                        var select = table.Select();

                                        // setConditions
                                        foreach (string colName in uniqueColumns)
                                        {
                                            string colName2 = columnMetadataList.SingleOrDefault(c => c.ColumnDisplayName == colName).ColumnName;
                                            object condValue = data[colName2].ToString();
                                            if (colName == "IČO")
                                                condValue = data[colName2].ToString().PadLeft(8, '0');
                                            DBColumn column = table.Columns.Single(c => c.Name == colName2);

                                            select.Where(c => c.Column(colName2).Equal(condValue));
                                        }

                                        // check if row already exists
                                        if (select.ToList().Count > 0)
                                        {
                                            isValid = false;
                                            messages.Add(string.Format(uniqueError, line));
                                        }
                                    }
                                    catch (KeyNotFoundException)
                                    {
                                        isValid = false;
                                        messages.Add(string.Format(uniqueColumnMissingError, line));
                                    }
                                }

                                if (isValid)
                                {
                                    data.Add("Datum_vlozeni", DateTime.Now);
                                    data.Add("Cas_editace", DateTime.Now);
                                    data.Add("Editoval", core.User.Id);
                                    DBItem item = new DBItem(db, table);
                                    foreach (KeyValuePair<string, object> kv in data)
                                    {
                                        item[kv.Key] = kv.Value;
                                    }
                                    table.Add(item);
                                    countAdded++;
                                }
                            }
                        }
                    }
                }
            }
            
            db.SaveChanges();

            outputVars["Success"] = messages.Count() == 0;
            outputVars["Message"] = string.Join("<br>", messages);
            outputVars["CountAdded"] = countAdded;
        }
    }
}

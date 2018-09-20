using DocumentFormat.OpenXml;
using DocumentFormat.OpenXml.Packaging;
using DocumentFormat.OpenXml.Spreadsheet;
using FSS.Omnius.Modules.CORE;
using FSS.Omnius.Modules.Entitron;
using FSS.Omnius.Modules.Entitron.DB;
using FSS.Omnius.Modules.Entitron.Entity;
using FSS.Omnius.Modules.Entitron.Entity.Entitron;
using FSS.Omnius.Modules.Entitron.Entity.Mozaic.Bootstrap;
using FSS.Omnius.Modules.Entitron.Queryable;
using FSS.Omnius.Modules.Watchtower;
using Newtonsoft.Json.Linq;
using System;
using System.Collections.Generic;
using System.Globalization;
using System.IO;
using System.Linq;
using System.Text;
using System.Text.RegularExpressions;

namespace FSS.Omnius.Modules.Tapestry2.Actions
{
    public class Mozaic : ActionManager
    {
        [Action(182, "Show message")]
        public static void ShowMessage(COREobject core, string Message, string Type)
        {
            switch (Type)
            {
                case "Success":
                    core.Message.Success.Add(Message);
                    break;
                case "Warning":
                    core.Message.Warnings.Add(Message);
                    break;
                case "Error":
                    core.Message.Errors.Add(Message);
                    break;
                default:
                    core.Message.Info.Add(Message);
                    break;
            }
        }

        [Action(2002, "Log message")]
        public static void Log(COREobject core, string Message, OmniusLogLevel Level = OmniusLogLevel.Info)
        {
            OmniusLog.Log(Message, Level, OmniusLogSource.User, core.Application, core.User);
        }

        [Action(2003, "Export to CSV")]
        public static void ExportToCSV(COREobject core, List<DBItem> Data = null, string Columns = null, string TableName = null)
        {
            // Init
            DBConnection db = core.Entitron;
            
            // Připravíme CSV
            List<string> rows = new List<string>();

            /// columns
            List<string> columns = new List<string>();
            if (Columns == null && TableName != null)
            {
                foreach (DBColumn col in db.Table(TableName).Columns)
                {
                    columns.Add(col.Name);
                }
            }
            else
            {
                columns = Columns.Split(';').ToList();
            }

            /// data
            if (Data == null && TableName != null)
            {
                Data = db.Table(TableName).Select().ToList();
            }

            /// header to csv
            List<string> header = new List<string>();
            foreach (string column in columns)
            {
                header.Add($"\"{column}\"");
            }
            rows.Add(string.Join(";", header));

            /// data to csv
            foreach (DBItem item in Data)
            {
                List<string> row = new List<string>();
                foreach (string column in columns)
                {
                    row.Add($"\"{item[column].ToString()}\"");
                }
                rows.Add(string.Join(";", row));
            }

            string csv = string.Join("\r\n", rows);
            
            core.HttpResponse("export.csv", "application/csv", Encoding.UTF8.GetBytes(csv));
        }

        #region ExportToExcel
        [Action(2004, "Export to Excel")]
        public static void ExportToExcel(COREobject core, string TableName = null, string ViewName = null, List<DBItem> TableData = null, List<dynamic> Filter = null, string Columns = null, Dictionary<string, string> ForeignKeys = null, string OrderBy = null)
        {
            // Init
            DBConnection db = core.Entitron;
            DBEntities context = core.Context;

            List<DBItem> data = new List<DBItem>();
            List<string> rows = new List<string>();
            List<string> columns = new List<string>();
            List<DbColumn> DBColumns = new List<DbColumn>();
            string abc = "ABCDEFGHIJKLMNOPQRSTUVWXYZ";
            bool isTable = false;
            bool isView = false;

            List<string> chars = new List<string>();
            for (int i = -1; i < abc.Length; i++)
            {
                for (int j = 0; j < abc.Length; j++)
                {
                    string ch = (i >= 0 ? abc[i].ToString() : "") + abc[j].ToString();
                    chars.Add(ch);
                }
            }

            string tableName = TableName ?? core.BlockAttribute.ModelTableName;
            string viewName = ViewName ?? "";

            if (viewName != "")
            {
                if (TableData == null)
                    TableData = ExportFromView(db, viewName, Filter, OrderBy);

                columns = GetColumnsFromTable(db, viewName, Columns);
                isView = true;
            }
            else if (!string.IsNullOrEmpty(tableName))
            {
                data = Entitron.Select(core, TableName: tableName, CondColumn: new string[0], CondOperator: new string[0], CondValue: new string[0]);
                columns = GetColumnsFromTable(db, tableName, Columns);
                isTable = true;
            }
            else
            {
                throw new Exception("Nebylo nalezeno jméno tabulky ani jméno pohledu");
            }

            // Připravíme XLS

            Dictionary<string, Dictionary<int, string>> foreignData = new Dictionary<string, Dictionary<int, string>>();
            Dictionary<string, string> foreignColumnNames = new Dictionary<string, string>();

            if (ForeignKeys != null)
            {
                foreach (KeyValuePair<string, string> key in ForeignKeys)
                {
                    foreignData.Add(key.Key, new Dictionary<int, string>());
                    string[] target = key.Value.Split('.');
                    string foreignTable = target[0];
                    string foreignColumn = target[1];

                    foreach (DBItem fr in db.Table(foreignTable).Select().Where(c => c.Column("id").In(new HashSet<object>(data.Select(i => i[key.Key])))).ToList())
                    {
                        foreignData[key.Key].Add((int)fr["id"], (string)fr[foreignColumn]);
                    }

                    DbColumn foreignDbColumn = context.DbTables.Include("Columns").Where(t => t.Name == foreignTable).OrderByDescending(t => t.DbSchemeCommitId).First().Columns.Where(c => c.Name == foreignColumn).First();
                    foreignColumnNames[key.Key] = foreignDbColumn.DisplayName ?? foreignDbColumn.Name;
                }
            }

            if (isTable)
            {
                DBColumns = context.DbTables.Include("Columns").Where(t => t.Name == tableName).OrderByDescending(t => t.DbSchemeCommitId).First().Columns.ToList();
            }

            using (MemoryStream stream = new MemoryStream())
            {
                using (SpreadsheetDocument xls = SpreadsheetDocument.Create(stream, SpreadsheetDocumentType.Workbook, true))
                {
                    WorkbookPart bookPart = xls.AddWorkbookPart();
                    bookPart.Workbook = new Workbook();
                    bookPart.Workbook.AppendChild<Sheets>(new Sheets());

                    SharedStringTablePart strings = xls.WorkbookPart.AddNewPart<SharedStringTablePart>();
                    WorksheetPart sheetPart = InsertWorksheet("Data", bookPart);

                    int c = 0;
                    if (isTable)
                    {
                        foreach (DbColumn col in DBColumns)
                        {
                            if (columns.Contains(col.Name))
                            {
                                string name;
                                if (foreignColumnNames.ContainsKey(col.Name))
                                {
                                    name = foreignColumnNames[col.Name];
                                }
                                else
                                {
                                    name = col.DisplayName ?? col.Name;
                                }

                                int i = InsertSharedStringItem(name, strings);
                                Cell cell = InsertCellInWorksheet(chars[c], 1, sheetPart);
                                cell.CellValue = new CellValue(i.ToString());
                                cell.DataType = new EnumValue<CellValues>(CellValues.SharedString);
                                c++;
                            }
                        }
                    }
                    if (isView)
                    {
                        foreach (string colName in columns)
                        {
                            int i = InsertSharedStringItem(colName, strings);
                            Cell cell = InsertCellInWorksheet(chars[c], 1, sheetPart);
                            cell.CellValue = new CellValue(i.ToString());
                            cell.DataType = new EnumValue<CellValues>(CellValues.SharedString);
                            c++;
                        }
                    }

                    uint r = 1;
                    foreach (DBItem item in data)
                    {
                        c = 0;
                        r++;

                        if (isTable)
                        {
                            foreach (DbColumn col in DBColumns)
                            {
                                if (columns.Contains(col.Name))
                                {
                                    string value;

                                    if (foreignData.ContainsKey(col.Name))
                                    {
                                        if (foreignData[col.Name].ContainsKey((int)item[col.Name]))
                                        {
                                            value = foreignData[col.Name][(int)item[col.Name]];
                                        }
                                        else
                                        {
                                            value = item[col.Name].ToString();
                                        }
                                    }
                                    else
                                    {
                                        value = item[col.Name].ToString();
                                        if (col.Type == "boolean")
                                        {
                                            value = value == "True" ? "Ano" : "Ne";
                                        }
                                    }

                                    int i = InsertSharedStringItem(value, strings);
                                    Cell cell = InsertCellInWorksheet(chars[c], r, sheetPart);
                                    cell.CellValue = new CellValue(i.ToString());
                                    cell.DataType = new EnumValue<CellValues>(CellValues.SharedString);
                                    c++;
                                }
                            }
                        }
                        if (isView)
                        {
                            foreach (string colName in columns)
                            {
                                string value = item[colName].ToString();

                                int i = InsertSharedStringItem(value, strings);
                                Cell cell = InsertCellInWorksheet(chars[c], r, sheetPart);
                                cell.CellValue = new CellValue(i.ToString());
                                cell.DataType = new EnumValue<CellValues>(CellValues.SharedString);
                                c++;
                            }
                        }
                    }
                    xls.Close();

                    stream.Seek(0, SeekOrigin.Begin);
                    
                    core.HttpResponse("export.xlsx", "application/vnd.openxmlformats-officedocument.spreadsheetml.sheet", stream.ToArray());
                }
            }
        }
        private static List<DBItem> ExportFromView(DBConnection db, string viewName, List<dynamic> filter, string OrderBy)
        {
            Select viewSelect = db.Select(viewName);

            List<DBItem> rows;
            if (filter != null)
            {
                rows = viewSelect.Where(m => m.Column("id").In(filter)).ToList();
            }
            else
            {
                if (OrderBy != null)
                {
                    rows = viewSelect.Order(AscDesc.Asc, OrderBy).ToList();
                }
                else
                {
                    rows = viewSelect.ToList();
                }
            }
            return rows;
        }
        private static List<string> GetColumnsFromTable(DBConnection db, string tableName, string columnsString)
        {
            List<string> columns = new List<string>();

            if (columnsString != null)
            {
                columns = columnsString.Split(';').ToList();
            }
            else
            {
                foreach (DBColumn col in db.Table(tableName).Columns)
                {
                    columns.Add(col.Name);
                }
            }

            return columns;
        }
        private static int InsertSharedStringItem(string text, SharedStringTablePart shareStringPart)
        {
            // If the part does not contain a SharedStringTable, create one.
            if (shareStringPart.SharedStringTable == null)
            {
                shareStringPart.SharedStringTable = new SharedStringTable();
            }

            int i = 0;

            // Iterate through all the items in the SharedStringTable. If the text already exists, return its index.
            foreach (SharedStringItem item in shareStringPart.SharedStringTable.Elements<SharedStringItem>())
            {
                if (item.InnerText == text)
                {
                    return i;
                }

                i++;
            }

            // The text does not exist in the part. Create the SharedStringItem and return its index.
            shareStringPart.SharedStringTable.AppendChild(new SharedStringItem(new DocumentFormat.OpenXml.Spreadsheet.Text(text)));
            //shareStringPart.SharedStringTable.Save();

            return i;
        }
        private static WorksheetPart InsertWorksheet(string name, WorkbookPart workbookPart)
        {
            // Add a new worksheet part to the workbook.
            WorksheetPart newWorksheetPart = workbookPart.AddNewPart<WorksheetPart>();
            newWorksheetPart.Worksheet = new Worksheet(new SheetData());
            //newWorksheetPart.Worksheet.Save();

            Sheets sheets = workbookPart.Workbook.GetFirstChild<Sheets>();
            string relationshipId = workbookPart.GetIdOfPart(newWorksheetPart);

            // Get a unique ID for the new sheet.
            uint sheetId = 1;
            if (sheets.Elements<Sheet>().Count() > 0)
            {
                sheetId = sheets.Elements<Sheet>().Select(s => s.SheetId.Value).Max() + 1;
            }

            // Append the new worksheet and associate it with the workbook.
            Sheet sheet = new Sheet() { Id = relationshipId, SheetId = sheetId, Name = name };
            sheets.Append(sheet);
            //workbookPart.Workbook.Save();

            return newWorksheetPart;
        }
        private static Cell InsertCellInWorksheet(string columnName, uint rowIndex, WorksheetPart worksheetPart)
        {
            Worksheet worksheet = worksheetPart.Worksheet;
            SheetData sheetData = worksheet.GetFirstChild<SheetData>();
            string cellReference = columnName + rowIndex;

            // If the worksheet does not contain a row with the specified row index, insert one.
            Row row;
            if (sheetData.Elements<Row>().Where(r => r.RowIndex == rowIndex).Count() != 0)
            {
                row = sheetData.Elements<Row>().Where(r => r.RowIndex == rowIndex).First();
            }
            else
            {
                row = new Row() { RowIndex = rowIndex };
                sheetData.Append(row);
            }

            // If there is not a cell with the specified column name, insert one.  
            if (row.Elements<Cell>().Where(c => c.CellReference.Value == columnName + rowIndex).Count() > 0)
            {
                return row.Elements<Cell>().Where(c => c.CellReference.Value == cellReference).First();
            }
            else
            {
                // Cells must be in sequential order according to CellReference. Determine where to insert the new cell.
                Cell refCell = null;
                string lettersOnly = Regex.Replace(cellReference, "[\\d]", "");
                foreach (Cell cell in row.Elements<Cell>())
                {
                    string oldCellRefLettersOnly = Regex.Replace(cell.CellReference.Value, "[\\d]", "");
                    if (oldCellRefLettersOnly.Length == lettersOnly.Length && string.Compare(cell.CellReference.Value, cellReference, true) > 0)
                    {
                        refCell = cell;
                        break;
                    }
                }

                Cell newCell = new Cell() { CellReference = cellReference };
                row.InsertBefore(newCell, refCell);

                //worksheet.Save();
                return newCell;
            }
        }
        #endregion

        [Action(2006, "Validate input", "Result")]
        public static bool ValidateInput(COREobject core, string InputName, string Condition = "NotEmpty", string InputAlias = null, bool SuppressMessage = false)
        {
            InputAlias = InputAlias ?? InputName;
            string inputValue = core.Data[InputName].ToString();
            bool isValid = false;
            switch (Condition)
            {
                case "Integer":
                    int tempInt;
                    isValid = int.TryParse(inputValue, out tempInt);
                    if (!isValid && !SuppressMessage)
                        core.Message.Warnings.Add($"Validace: Kolonka {InputAlias} musí obsahovat celé číslo");
                    break;
                case "Float":
                    float tempFloat;
                    isValid = float.TryParse(inputValue, out tempFloat);
                    if (!isValid && !SuppressMessage)
                        core.Message.Warnings.Add($"Validace: Kolonka {InputAlias} musí obsahovat číslo");
                    break;
                case "NotEmpty":
                default:
                    isValid = inputValue.Length > 0;
                    if (!isValid && !SuppressMessage)
                        core.Message.Warnings.Add($"Validace: Kolonka {InputAlias} je povinná, vyplňte ji prosím");
                    break;
            }

            return isValid;
        }

        #region DataTableResponse
        [Action(2007, "DataTable response", "Response")]
        public static JToken DataTableResponse(COREobject core, List<DBItem> Data, int BootstrapPageId, string Columns, string Search, string Order, int draw, int start = -1, int length = -1)
        {
            // Init
            DBEntities context = core.Context;
            List<DBItem> ds = new List<DBItem>();
            string orderColumnName = null;

            if (Data.Count() > 0)
            {
                /**********************************************************************************/
                /* Vytvoříme si datasource - všechny sloupce jako string, přidáme hiddenId a akce */
                /**********************************************************************************/
                // Musíme znovu vytvořit sloupec s akcema
                string actionIcons = "";
                MozaicBootstrapComponent uic = context.MozaicBootstrapComponents.Where(c => c.ElmId == core.Executor && c.MozaicBootstrapPageId == BootstrapPageId).FirstOrDefault();

                if (uic != null)
                {
                    JToken attributes = JToken.Parse(uic.Attributes);
                    foreach (JToken attr in attributes)
                    {
                        if ((string)attr["name"] == "data-actions")
                        {
                            string actionsString = ((string)attr["value"]).Replace('\'', '"');
                            JToken actions = JToken.Parse(actionsString);

                            List<string> actionsIconsList = new List<string>();
                            foreach (JToken a in actions)
                            {
                                actionsIconsList.Add($"<i title=\"{(string)a["title"]}\" class=\"{(string)a["icon"]}\" data-action=\"{(string)a["action"]}\" data-idparam=\"{(string)a["idParam"]}\" data-confirm=\"{(string)a["confirm"]}\"></i>");
                            }
                            if (actionsIconsList.Count() > 0)
                            {
                                actionIcons = "<span class=\"text-nowrap\">" + string.Join(" ", actionsIconsList) + "</span>";
                            }
                            break;
                        }
                    }
                }

                List<string> displayNames = new List<string>();
                List<string> columnsNames = Data[0].getColumnNames();
                try
                {
                    displayNames = Data[0].getColumnDisplayNames().ToList();
                }
                catch (NullReferenceException)
                {
                    displayNames = columnsNames;
                }

                foreach (DBItem row in Data)
                {
                    DBItem newRow = new DBItem(null, null);
                    int i = 0;

                    foreach (string prop in columnsNames)
                    {
                        var value = row[prop];
                        var displayName = displayNames[columnsNames.IndexOf(prop)];
                        if (i == 0)
                            orderColumnName = displayName;

                        // Převedeme všechny hodnoty na string
                        if (value is bool)
                        {
                            newRow[displayName] = (bool)value ? "Ano" : "Ne";
                        }
                        else if (value is DateTime)
                        {
                            newRow[displayName] = ((DateTime)value).ToString("d. M. yyyy H:mm:ss");
                        }
                        else if (value is string)
                        {
                            newRow[displayName] = (string)value;
                        }
                        else
                        {
                            newRow[displayName] = value.ToString();
                        }

                        // pokud je sloupec id nebo Id nastavíme hiddenId
                        if (prop == "id" || prop == "Id")
                        {
                            i++;
                            newRow["hiddenId"] = value.ToString();
                        }
                        i++;
                    }
                    // Pokud existují akce, přidáme je
                    if (!string.IsNullOrEmpty(actionIcons))
                    {
                        newRow["Akce"] = actionIcons;
                    }

                    ds.Add(newRow);
                }
            }

            /**********************************************************************************/
            /* Filtr dat                                                                      */
            /**********************************************************************************/
            List<DBItem> filteredData = new List<DBItem>();
            JToken columnsSearch = JToken.Parse(Columns);
            JToken search = JToken.Parse(Search);

            filteredData.AddRange(ds);

            foreach (JToken cs in JToken.Parse(Columns))
            {
                string searchFor = (string)cs["search"]["value"];
                if (!string.IsNullOrEmpty(searchFor))
                {
                    string columnName = (string)cs["data"];
                    filteredData = filteredData.Where(r => ((string)r[columnName]).ToLowerInvariant().Contains(searchFor.ToLowerInvariant())).ToList();
                }
            }
            string globalSearchFor = (string)search["value"];
            if (!string.IsNullOrEmpty(globalSearchFor))
            {
                filteredData.Clear();

                foreach (DBItem row in ds)
                {
                    foreach (string prop in row.getColumnNames())
                    {
                        if (prop == "id" || prop == "Id" || prop == "hiddenId" || prop == "Akce")
                            continue;

                        List<DBItem> found = ds.Where(r => ((string)r[prop]).ToLowerInvariant().Contains(globalSearchFor.ToLowerInvariant())).ToList();
                        if (found.Count() > 0)
                        {
                            foreach (DBItem item in found)
                            {
                                if (!filteredData.Contains(item))
                                {
                                    filteredData.Add(item);
                                }
                            }
                        }
                    }
                }
            }

            /**********************************************************************************/
            /* Zpracujeme řazení                                                              */
            /**********************************************************************************/
            JToken order = JToken.Parse(Order);
            int j = 0;
            IOrderedEnumerable<DBItem> tmpData = filteredData.OrderBy(r => r[r.getColumnNames().First()]);
            var comparer = new NaturalComparer<string>(CompareNatural);

            foreach (JToken by in order)
            {
                string columnName = (string)columnsSearch[(int)by["column"]]["data"];
                string dir = (string)by["dir"];

                if (j == 0)
                {
                    tmpData = dir == "desc" ?
                        filteredData.OrderByDescending(r => (string)r[columnName], comparer) :
                        filteredData.OrderBy(r => (string)r[columnName], comparer);
                }
                else
                {
                    tmpData = dir == "desc" ?
                        tmpData.ThenByDescending(r => (string)r[columnName], comparer) :
                        tmpData.ThenBy(r => (string)r[columnName], comparer);
                }
                j++;
            }
            filteredData = tmpData.ToList();

            /**********************************************************************************/
            /* Zpracujeme stránkování                                                         */
            /**********************************************************************************/

            var pagedData = length == -1 ? filteredData.ToList() : filteredData.Skip(start).Take(length).ToList();

            /**********************************************************************************/
            /* Vrátíme výsledek                                                               */
            /**********************************************************************************/
            ListJson<DBItem> finalDS = new ListJson<DBItem>();
            finalDS.AddRange(pagedData);

            JToken response = new JObject();
            response["draw"] = draw;
            response["recordsTotal"] = Data.Count();
            response["recordsFiltered"] = filteredData.Count();
            response["data"] = finalDS.ToJson();

            return response;
        }
        public static int CompareNatural(string strA, string strB)
        {
            return CompareNatural(strA, strB, CultureInfo.CurrentCulture, CompareOptions.IgnoreCase);
        }
        public static int CompareNatural(string strA, string strB, CultureInfo culture, CompareOptions options)
        {
            CompareInfo cmp = culture.CompareInfo;
            int iA = 0;
            int iB = 0;
            int softResult = 0;
            int softResultWeight = 0;
            while (iA < strA.Length && iB < strB.Length)
            {
                bool isDigitA = Char.IsDigit(strA[iA]);
                bool isDigitB = Char.IsDigit(strB[iB]);
                if (isDigitA != isDigitB)
                {
                    return cmp.Compare(strA, iA, strB, iB, options);
                }
                else if (!isDigitA && !isDigitB)
                {
                    int jA = iA + 1;
                    int jB = iB + 1;
                    while (jA < strA.Length && !Char.IsDigit(strA[jA])) jA++;
                    while (jB < strB.Length && !Char.IsDigit(strB[jB])) jB++;
                    int cmpResult = cmp.Compare(strA, iA, jA - iA, strB, iB, jB - iB, options);
                    if (cmpResult != 0)
                    {
                        // Certain strings may be considered different due to "soft" differences that are
                        // ignored if more significant differences follow, e.g. a hyphen only affects the
                        // comparison if no other differences follow
                        string sectionA = strA.Substring(iA, jA - iA);
                        string sectionB = strB.Substring(iB, jB - iB);
                        if (cmp.Compare(sectionA + "1", sectionB + "2", options) ==
                            cmp.Compare(sectionA + "2", sectionB + "1", options))
                        {
                            return cmp.Compare(strA, iA, strB, iB, options);
                        }
                        else if (softResultWeight < 1)
                        {
                            softResult = cmpResult;
                            softResultWeight = 1;
                        }
                    }
                    iA = jA;
                    iB = jB;
                }
                else
                {
                    char zeroA = (char)(strA[iA] - (int)Char.GetNumericValue(strA[iA]));
                    char zeroB = (char)(strB[iB] - (int)Char.GetNumericValue(strB[iB]));
                    int jA = iA;
                    int jB = iB;
                    while (jA < strA.Length && strA[jA] == zeroA) jA++;
                    while (jB < strB.Length && strB[jB] == zeroB) jB++;
                    int resultIfSameLength = 0;
                    do
                    {
                        isDigitA = jA < strA.Length && Char.IsDigit(strA[jA]);
                        isDigitB = jB < strB.Length && Char.IsDigit(strB[jB]);
                        int numA = isDigitA ? (int)Char.GetNumericValue(strA[jA]) : 0;
                        int numB = isDigitB ? (int)Char.GetNumericValue(strB[jB]) : 0;
                        if (isDigitA && (char)(strA[jA] - numA) != zeroA) isDigitA = false;
                        if (isDigitB && (char)(strB[jB] - numB) != zeroB) isDigitB = false;
                        if (isDigitA && isDigitB)
                        {
                            if (numA != numB && resultIfSameLength == 0)
                            {
                                resultIfSameLength = numA < numB ? -1 : 1;
                            }
                            jA++;
                            jB++;
                        }
                    }
                    while (isDigitA && isDigitB);
                    if (isDigitA != isDigitB)
                    {
                        // One number has more digits than the other (ignoring leading zeros) - the longer
                        // number must be larger
                        return isDigitA ? 1 : -1;
                    }
                    else if (resultIfSameLength != 0)
                    {
                        // Both numbers are the same length (ignoring leading zeros) and at least one of
                        // the digits differed - the first difference determines the result
                        return resultIfSameLength;
                    }
                    int lA = jA - iA;
                    int lB = jB - iB;
                    if (lA != lB)
                    {
                        // Both numbers are equivalent but one has more leading zeros
                        return lA > lB ? -1 : 1;
                    }
                    else if (zeroA != zeroB && softResultWeight < 2)
                    {
                        softResult = cmp.Compare(strA, iA, 1, strB, iB, 1, options);
                        softResultWeight = 2;
                    }
                    iA = jA;
                    iB = jB;
                }
            }
            if (iA < strA.Length || iB < strB.Length)
            {
                return iA < strA.Length ? 1 : -1;
            }
            else if (softResult != 0)
            {
                return softResult;
            }
            return 0;
        }
        #endregion

        [Action(2008, "Save form state")]
        public static void SaveFormState(COREobject core)
        {
            Dictionary<string, string> state = new Dictionary<string, string>();
            foreach (KeyValuePair<string, object> kv in core.Data)
            {
                state.Add(kv.Key, kv.Value.ToString());
            }

            core.CrossBlockRegistry["FormState"] = state;
        }
    }

    /// <summary>
    /// extension for DataTableResponse
    /// </summary>
    public class NaturalComparer<T> : IComparer<T>
    {
        private Comparison<T> _comparison;

        public NaturalComparer(Comparison<T> comparison)
        {
            _comparison = comparison;
        }

        public int Compare(T x, T y)
        {
            return _comparison(x, y);
        }
    }
}

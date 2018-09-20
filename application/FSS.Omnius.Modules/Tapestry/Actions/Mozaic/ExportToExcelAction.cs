using System;
using System.Collections.Generic;
using System.IO;
using System.Linq;
using System.Web;
using System.Text.RegularExpressions;
using DocumentFormat.OpenXml;
using DocumentFormat.OpenXml.Packaging;
using DocumentFormat.OpenXml.Spreadsheet;
using FSS.Omnius.Modules.Entitron.Entity;
using FSS.Omnius.Modules.Entitron.Entity.Entitron;
using FSS.Omnius.Modules.CORE;
using FSS.Omnius.Modules.Entitron.DB;
using FSS.Omnius.Modules.Entitron.Queryable;

namespace FSS.Omnius.Modules.Tapestry.Actions.Mozaic
{

    [MozaicRepository]
    public class ExportToExcelAction : Action
    {
        private DBConnection _db;

        public override int Id => 2004;

        public override int? ReverseActionId => null;

        public override string[] InputVar => new string[] { "?TableName", "?ViewName", "?TableData", "?Filter", "?Columns", "?OrderBy" };

        public override string Name => "Export to Excel";

        public override string[] OutputVar => new string[] { };

        public int DBSet { get; private set; }

        public override void InnerRun(Dictionary<string, object> vars, Dictionary<string, object> outputVars, Dictionary<string, object> InvertedInputVars, Message message)
        {
            // Init
            COREobject core = COREobject.i;
            DBEntities context = core.Context;
            _db = core.Entitron;
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

            string tableName = vars.ContainsKey("TableName") ? (string)vars["TableName"] : (vars.ContainsKey("__TableName__") ? (string)vars["__TableName__"] : "");
            string viewName = vars.ContainsKey("ViewName") ? (string)vars["ViewName"] : "";

            if (vars.ContainsKey("ViewName"))
            {
                if (vars.ContainsKey("TableData"))
                {
                    data = (List<DBItem>)vars["TableData"];
                }
                else
                {
                    data = ExportFromView(viewName, vars);
                }
                columns = GetColumnsFromView(data, vars);
                isView = true;
            }
            else if (!string.IsNullOrEmpty(tableName))
            {
                data = ExportFromTable(tableName, vars, InvertedInputVars, message);
                columns = GetColumnsFromTable(tableName, vars);
                isTable = true;
            }
            else
            {
                throw new Exception("Nebylo nalezeno jméno tabulky ani jméno pohledu");
            }

            // Připravíme XLS

            Dictionary<string, Dictionary<int, string>> foreignData = new Dictionary<string, Dictionary<int, string>>();
            Dictionary<string, string> foreignColumnNames = new Dictionary<string, string>();
            
            if (vars.ContainsKey("ForeignKeys"))
            {
                foreach (KeyValuePair<string, string> key in (Dictionary<string, string>)vars["ForeignKeys"])
                {
                    foreignData.Add(key.Key, new Dictionary<int, string>());
                    string[] target = key.Value.Split('.');
                    string foreignTable = target[0];
                    string foreignColumn = target[1];

                    foreach (DBItem fr in _db.Table(foreignTable).Select().Where(c => c.Column("id").In(new HashSet<object>(data.Select(i => i[key.Key])))).ToList())
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
                                else {
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
                                        else {
                                            value = item[col.Name].ToString();
                                        }
                                    }
                                    else {
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

                    HttpContext httpContext = HttpContext.Current;
                    HttpResponse response = httpContext.Response;

                    response.Clear();
                    response.StatusCode = 200;
                    response.ContentType = "application/vnd.openxmlformats-officedocument.spreadsheetml.sheet";
                    response.AddHeader("content-disposition", "attachment; filename=export.xlsx");
                    response.BinaryWrite(stream.ToArray());
                    response.Flush();
                    response.Close();
                    response.End();
                }
            }
            
        }

        private List<DBItem> ExportFromTable(string tableName, Dictionary<string, object> vars, Dictionary<string, object> InvertedInputVars, Message message)
        {
            // Získáme data podle podmínek
            Dictionary<string, object> selectOutput = new Dictionary<string, object>();
            Entitron.SelectAction selectAction = new Entitron.SelectAction();
            selectAction.InnerRun(vars, selectOutput, InvertedInputVars, message);

            List<DBItem> rows = (List<DBItem>)selectOutput["Data"];
            return rows;
        }

        private List<DBItem> ExportFromView(string viewName, Dictionary<string, object> vars)
        {
            Select viewSelect = _db.Select(viewName);

            List<DBItem> rows;
            if (vars.ContainsKey("Filter"))
            {
                object filter = vars["Filter"];
                IEnumerable<int> ids;

                if (filter is IEnumerable<object>)
                    ids = (filter as List<object>).Select(f => Convert.ToInt32(f));
                else if (filter is string)
                    ids = (filter as string).Split(',').Select(f => Convert.ToInt32(f));
                else if (filter is IEnumerable<int>)
                    ids = (IEnumerable<int>)filter;
                else
                    throw new InvalidCastException("Filter is in unknown format");

                rows = viewSelect.Where(m => m.Column("id").In(ids.Cast<object>())).ToList();
            }
            else
            {
                if (vars.ContainsKey("OrderBy"))
                {
                    rows = viewSelect.Order(AscDesc.Asc, vars["OrderBy"].ToString()).ToList();
                }
                else
                {
                    rows = viewSelect.ToList();
                }
            }
            return rows;
        }

        private List<string> GetColumnsFromTable(string tableName, Dictionary<string, object> vars)
        {
            List<string> columns = new List<string>();

            if (!vars.ContainsKey("Columns"))
            {
                foreach (DBColumn col in _db.Table(tableName).Columns)
                {
                    columns.Add(col.Name);
                }
            }
            else {
                columns = (vars["Columns"] as string).Split(';').ToList();
            }
            return columns;
        }

        private List<string> GetColumnsFromView(List<DBItem> data, Dictionary<string, object> vars)
        {
            List<string> columns = new List<string>();

            if (!vars.ContainsKey("Columns"))
            {
                if (data.Count > 0)
                {
                    columns = data.First().getColumnNames();
                }
            }
            else {
                columns = (vars["Columns"] as string).Split(';').ToList();
            }
            return columns;
        }

        private int InsertSharedStringItem(string text, SharedStringTablePart shareStringPart)
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

        private WorksheetPart InsertWorksheet(string name, WorkbookPart workbookPart)
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

        private Cell InsertCellInWorksheet(string columnName, uint rowIndex, WorksheetPart worksheetPart)
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
            else {
                row = new Row() { RowIndex = rowIndex };
                sheetData.Append(row);
            }

            // If there is not a cell with the specified column name, insert one.  
            if (row.Elements<Cell>().Where(c => c.CellReference.Value == columnName + rowIndex).Count() > 0)
            {
                return row.Elements<Cell>().Where(c => c.CellReference.Value == cellReference).First();
            }
            else {
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
    }
}

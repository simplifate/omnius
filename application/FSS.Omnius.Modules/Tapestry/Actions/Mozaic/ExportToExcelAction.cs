using FSS.Omnius.Modules.Entitron;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Net;
using System.Text;
using System.Threading.Tasks;
using System.Web;
using DocumentFormat.OpenXml;
using DocumentFormat.OpenXml.Packaging;
using DocumentFormat.OpenXml.Spreadsheet;
using System.IO;
using System.Data.Entity;
using FSS.Omnius.Modules.Entitron.Entity;
using FSS.Omnius.Modules.Entitron.Entity.Entitron;
using FSS.Omnius.Modules.CORE;

namespace FSS.Omnius.Modules.Tapestry.Actions.Mozaic
{
    
    [MozaicRepository]
    public class ExportToExcelAction : Action
    {
        public override int Id
        {
            get
            {
                return 2004;
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
                return new string[] { "TableName" };
            }
        }

        public override string Name
        {
            get
            {
                return "Export to Excel";
            }
        }

        public override string[] OutputVar
        {
            get
            {
                return new string[] { };
            }
        }

        public int DBSet { get; private set; }

        public override void InnerRun(Dictionary<string, object> vars, Dictionary<string, object> outputVars, Dictionary<string, object> InvertedInputVars, Message message)
        {
            // Init
            CORE.CORE core = (CORE.CORE)vars["__CORE__"];
            
            // Získáme data podle podmínek
            Entitron.SelectAction selectAction = new Entitron.SelectAction();
            selectAction.InnerRun(vars, outputVars, InvertedInputVars, message);

            List<DBItem> data = (List<DBItem>)vars["Data"];

            // Připravíme CSV
            List<string> rows = new List<string>();

            List<string> columns = new List<string>();
            if (string.IsNullOrEmpty(core._form["Columns"])) {
                foreach(DBColumn col in core.Entitron.GetDynamicTable((string)vars["TableName"]).columns) {
                    columns.Add(col.Name);
                }
            }
            else {
                columns = core._form["Columns"].Split(';').ToList();
            }

            Dictionary<string, Dictionary<int, string>> foreignData = new Dictionary<string, Dictionary<int, string>>();
            Dictionary<string, string> foreignColumnNames = new Dictionary<string, string>();

            using (DBEntities e = new DBEntities()) {
                if (vars.ContainsKey("ForeignKeys")) {
                    foreach(KeyValuePair<string, string> key in (Dictionary<string, string>)vars["ForeignKeys"]) {
                        foreignData.Add(key.Key, new Dictionary<int, string>());
                        string[] target = key.Value.Split('.');
                        string foreignTable = target[0];
                        string foreignColumn = target[1];

                        foreach(DBItem fr in core.Entitron.GetDynamicTable(foreignTable).Select().where(c => c.column("id").In(new HashSet<object>(data.Select(i => i[key.Key])))).ToList()) {
                            foreignData[key.Key].Add((int)fr["id"], (string)fr[foreignColumn]);
                        }

                        DbColumn foreignDbColumn = e.DbTables.Include("Columns").Where(t => t.Name == foreignTable).OrderByDescending(t => t.DbSchemeCommitId).First().Columns.Where(c => c.Name == foreignColumn).First();
                        foreignColumnNames[key.Key] = foreignDbColumn.DisplayName ?? foreignDbColumn.Name;
                    }
                }
                
                Dictionary<string, string> columnsDisplayName = new Dictionary<string, string>();
            
                string tableName = (string)vars["TableName"];
                var DBColumns = e.DbTables.Include("Columns").Where(t => t.Name == tableName).OrderByDescending(t => t.DbSchemeCommitId).First().Columns;
                
                string abc = "ABCDEFGHIJKLMNOPQRSTUVWXYZ";
            
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
                        foreach (DbColumn col in DBColumns) {
                            if (columns.Contains(col.Name)) {
                                string name;
                                if(foreignColumnNames.ContainsKey(col.Name)) {
                                    name = foreignColumnNames[col.Name];
                                }
                                else {
                                    name = col.DisplayName ?? col.Name;
                                }

                                int i = InsertSharedStringItem(name, strings);
                                Cell cell = InsertCellInWorksheet(abc[c].ToString(), 1, sheetPart);
                                cell.CellValue = new CellValue(i.ToString());
                                cell.DataType = new EnumValue<CellValues>(CellValues.SharedString);
                                c++;
                            }
                        }

                        uint r = 1;
                        foreach (DBItem item in (List<DBItem>)vars["Data"]) {
                            c = 0;
                            r++;

                            foreach (DbColumn col in DBColumns) {
                                if(columns.Contains(col.Name)) {
                                    string value;
                                    
                                    if(foreignData.ContainsKey(col.Name)) {
                                        if(foreignData[col.Name].ContainsKey((int)item[col.Name])) {
                                            value = foreignData[col.Name][(int)item[col.Name]];
                                        }
                                        else {
                                            value = item[col.Name].ToString();
                                        }
                                    }
                                    else { 
                                        value = item[col.Name].ToString();
                                        if(col.Type == "boolean") {
                                            value = value == "True" ? "Ano" : "Ne";
                                        }
                                    }

                                    int i = InsertSharedStringItem(value, strings);
                                    Cell cell = InsertCellInWorksheet(abc[c].ToString(), r, sheetPart);
                                    cell.CellValue = new CellValue(i.ToString());
                                    cell.DataType = new EnumValue<CellValues>(CellValues.SharedString);
                                    c++;
                                }
                            }
                        }
                        //sheetPart.Worksheet.Save();
                        //ystrings.SharedStringTable.Save();
                        //xls.WorkbookPart.Workbook.Save();
                        xls.Close();

                        stream.Seek(0, SeekOrigin.Begin);
                        //new FileStreamResult(stream, "application/vnd.openxmlformats-officedocument.spreadsheetml.sheet");

                        //StreamReader reader = new StreamReader(stream, Encoding.UTF8);

                        HttpContext context = HttpContext.Current;
                        HttpResponse response = context.Response;
                    
                        response.Clear();
                        response.ContentType = "application/vnd.openxmlformats-officedocument.spreadsheetml.sheet";
                        response.AddHeader("content-disposition", "attachment; filename=export.xlsx");
                        response.BinaryWrite(stream.ToArray());
                        response.Flush();
                        response.Close();
                        response.End();
                    }
                }
            }
        }

        private int InsertSharedStringItem(string text, SharedStringTablePart shareStringPart)
        {
            // If the part does not contain a SharedStringTable, create one.
            if (shareStringPart.SharedStringTable == null) {
                shareStringPart.SharedStringTable = new SharedStringTable();
            }

            int i = 0;

            // Iterate through all the items in the SharedStringTable. If the text already exists, return its index.
            foreach (SharedStringItem item in shareStringPart.SharedStringTable.Elements<SharedStringItem>()) {
                if (item.InnerText == text) {
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
            if (sheets.Elements<Sheet>().Count() > 0) {
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
            if (sheetData.Elements<Row>().Where(r => r.RowIndex == rowIndex).Count() != 0) {
                row = sheetData.Elements<Row>().Where(r => r.RowIndex == rowIndex).First();
            }
            else {
                row = new Row() { RowIndex = rowIndex };
                sheetData.Append(row);
            }

            // If there is not a cell with the specified column name, insert one.  
            if (row.Elements<Cell>().Where(c => c.CellReference.Value == columnName + rowIndex).Count() > 0) {
                return row.Elements<Cell>().Where(c => c.CellReference.Value == cellReference).First();
            }
            else {
                // Cells must be in sequential order according to CellReference. Determine where to insert the new cell.
                Cell refCell = null;
                foreach (Cell cell in row.Elements<Cell>()) {
                    if (string.Compare(cell.CellReference.Value, cellReference, true) > 0) {
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

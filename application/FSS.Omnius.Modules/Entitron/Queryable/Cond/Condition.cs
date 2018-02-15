using System.Collections.Generic;
using System.Data;
using System.Linq;
using FSS.Omnius.Modules.Entitron.DB;

namespace FSS.Omnius.Modules.Entitron.Queryable.Cond
{
    public class Condition : IMultiple
    {
        public Condition(DBConnection db)
        {
            _db = db;
            columnNameWithTable = true;
            operation_params = new List<object>();
        }

        private DBConnection _db;

        public string tabloidName { get; set; }
        public string column { get; set; }
        public string operation { get; set; }
        public string operation_columnFunction { get; set; }
        public List<object> operation_params { get; set; }
        public string concat { get; set; }
        public bool columnNameWithTable { get; set; }


        public string Separator => " ";

        public void Start(bool first)
        {
            concat = first
                ? ""
                : "AND";
        }

        public string ToSql(DBCommandSet set, IDbCommand command)
        {
            string column = this.column;
            if (!columnNameWithTable && column.Contains("."))
            {
                column = column.Substring(column.IndexOf('.') + 1);
            }

            switch (operation)
            {
                // no params
                case "IS NULL":
                case "IS NOT NULL":
                    return $"{concat} {columnWithFunction(column, operation_columnFunction, set)} {operation}";
                // 2 params
                case "BETWEEN":
                case "NOT BETWEEN":
                    string value0 = command.AddParam("value", operation_params[0]);
                    string value1 = command.AddParam("value", operation_params[1]);

                    return $"{concat} {columnWithFunction(column, operation_columnFunction, set)} {operation} {columnWithFunction("@value0", operation_columnFunction)} AND {columnWithFunction("@value0", operation_columnFunction)}";
                // list
                case "IN":
                case "NOT IN":
                    // empty
                    if (!operation_params.Any())
                    {
                        if (operation == "IN")
                            return $"{concat} 1=0";
                        else
                            return $"{concat} 1=1";
                    }
                    //
                    return $"{concat} {columnWithFunction(column, operation_columnFunction, set)} {operation} ({string.Join(",", operation_params.Select(p => $"@{command.AddParam("list", p)}"))})";
                // 1 param
                default:
                    if (string.IsNullOrEmpty(operation))
                        operation = "=";
                    return $"{concat} {columnWithFunction(column, operation_columnFunction, set)} {operation} {columnWithFunction($"@{command.AddParam("value", operation_params.First())}", operation_columnFunction)}";
            }
        }

        private string columnWithFunction(string column, string function, DBCommandSet set = null)
        {
            string result = column;
            if (set != null)
                result = $"{set.QuotesBegin}{result}{set.QuotesEnd}";
            if (!string.IsNullOrEmpty(function))
                result = $"{function}({result})";

            return result;
        }
    }
}

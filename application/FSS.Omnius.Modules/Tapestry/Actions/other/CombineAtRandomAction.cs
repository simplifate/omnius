using FSS.Omnius.Modules.CORE;
using FSS.Omnius.Modules.Entitron;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace FSS.Omnius.Modules.Tapestry.Actions.other
{
    /// <summary>
    /// Vezme seznam tabulek, háhodně vybere sloupce a vytvoří z nich string
    /// </summary>
    [OtherRepository]
    class CombineAtRandomAction : Action
    {
        public override int Id
        {
            get
            {
                return 133742069;
            }
        }

        public override string[] InputVar
        {
            get
            {
                return new string[] { "TableName[index]", "Column[index]", "?Separator"};
            }
        }

        public override string Name
        {
            get
            {
                return "Combine at Random";
            }
        }

        public override string[] OutputVar
        {
            get
            {
                return new string[] { "Result" };
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
            CORE.CORE core = (CORE.CORE)vars["__CORE__"];
            string separator = vars.ContainsKey("Separator") ? vars["Separator"].ToString() : " ";
            int tableCount = vars.Keys.Where(k => k.StartsWith("TableName[") && k.EndsWith("]")).Count();
            int colsCount = vars.Keys.Where(k => k.StartsWith("Column[") && k.EndsWith("]")).Count();
            if (tableCount != colsCount)
                throw new Exception( Name + ": Missing table or column name!");

            string result = "";

            for(int i = 0; i < tableCount; i++)
            {
                DBTable table = core.Entitron.GetDynamicTable(vars[$"TableName[{i}]"].ToString());
                Random rnd = new Random();
                var rows = table.Select(vars[$"Column[{i}]"].ToString()).ToList();
                result += rows[rnd.Next(rows.Count)][vars[$"Column[{i}]"].ToString()].ToString() + separator;
            }

            if (result.EndsWith(separator))
                result.Remove(result.Length-separator.Length);
            outputVars["Result"] = result;
        }
    }
}

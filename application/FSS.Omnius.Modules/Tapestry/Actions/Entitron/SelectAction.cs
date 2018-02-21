using System.Collections.Generic;
using System.Linq;
using FSS.Omnius.Modules.CORE;
using FSS.Omnius.Modules.Entitron;
using FSS.Omnius.Modules.Entitron.DB;

namespace FSS.Omnius.Modules.Tapestry.Actions.Entitron
{
	[EntitronRepository]
	public class SelectAction : Action
	{
		public override int Id => 1020;

		public override int? ReverseActionId => null;

		public override string[] InputVar => new string[] { "TableName", "CondColumn[index]", "CondValue[index]", "?CondOperator[index]", "?SearchInShared" };

		public override string Name => "Select (filter)";

		public override string[] OutputVar => new string[] { "Data" };

		public override void InnerRun(Dictionary<string, object> vars, Dictionary<string, object> outputVars, Dictionary<string, object> invertedVars, Message message)
		{
            // init
            DBConnection db = Modules.Entitron.Entitron.i;

			bool searchInShared = vars.ContainsKey("SearchInShared") ? (bool)vars["SearchInShared"] : false;

			DBTable table = db.Table(vars.ContainsKey("TableName") ? (string)vars["TableName"] : (string)vars["__TableName__"], searchInShared);

			//
			var select = table.Select();
			int CondCount = vars.Keys.Where(k => k.StartsWith("CondColumn[") && k.EndsWith("]")).Count();

			// setConditions
			for (int i = 0; i < CondCount; i++)
			{
				string condOperator = vars.ContainsKey($"CondOperator[{i}]") ? (string)vars[$"CondOperator[{i}]"] : "Equal";
				string condColumn = (string)vars[$"CondColumn[{i}]"];
				object condValue = vars[$"CondValue[{i}]"];

				DBColumn column = table.Columns.Single(c => c.Name == condColumn);
				var value = DataType.ConvertTo(column.Type, condValue);

				switch (condOperator)
				{
					case "Less":
						select.Where(c => c.Column(condColumn).Less(value));
						break;
					case "LessOrEqual":
                        select.Where(c => c.Column(condColumn).LessOrEqual(value));
						break;
					case "Greater":
                        select.Where(c => c.Column(condColumn).Greater(value));
						break;
					case "GreaterOrEqual":
                        select.Where(c => c.Column(condColumn).GreaterOrEqual(value));
						break;
					case "Equal":
                        select.Where(c => c.Column(condColumn).Equal(value));
						break;
					case "IsIn":
                        select.Where(c => c.Column(condColumn).In((IEnumerable<object>)value));
						break;
					default: // ==
                        select.Where(c => c.Column(condColumn).Equal(value));
						break;
				}
			}

            // return
            outputVars["Data"] = select.ToList();
        }
	}
}

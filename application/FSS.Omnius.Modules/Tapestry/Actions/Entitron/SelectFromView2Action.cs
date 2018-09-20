using System.Collections.Generic;
using System.Linq;
using FSS.Omnius.Modules.CORE;
using FSS.Omnius.Modules.Entitron.DB;
using FSS.Omnius.Modules.Entitron.Queryable;

namespace FSS.Omnius.Modules.Tapestry.Actions.Entitron
{
	[EntitronRepository]
	public class SelectFromViewAction2 : Action
	{
		public override int Id
		{
			get
			{
				return 1051;
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
				return new string[] { "ViewName", "?SearchInShared", "?OrderBy", "?Descending", "?GroupBy", "?GroupByFunction", "?MaxRows", "CondColumn[index]", "CondValue[index]", "?CondOperator[index]" };
			}
		}

		public override string Name
		{
			get
			{
				return "Select from view 2";
			}
		}

		public override string[] OutputVar
		{
			get
			{
				return new string[] { "Data" };
			}
		}

		public override void InnerRun(Dictionary<string, object> vars, Dictionary<string, object> outputVars, Dictionary<string, object> invertedVars, Message message)
		{
            // init
            COREobject core = COREobject.i;
            DBConnection db = core.Entitron;

            bool searchInShared = vars.ContainsKey("SearchInShared") ? (bool)vars["SearchInShared"] : false;
            string orderBy = vars.ContainsKey("OrderBy") ? (string)vars["OrderBy"] : null;
            AscDesc isDescending = vars.ContainsKey("Descending") && (bool)vars["Descending"] ? AscDesc.Desc : AscDesc.Asc;
            int? maxRows = vars.ContainsKey("MaxRows") ? (int?)vars["MaxRows"] : null;
            string groupBy = vars.ContainsKey("GroupBy") && !string.IsNullOrWhiteSpace((string)vars["GroupBy"]) ? (string)vars["GroupBy"] : null;
            
            //
            Select select = db.Select((string)vars["ViewName"], searchInShared);
            int CondCount = vars.Keys.Where(k => k.StartsWith("CondColumn[") && k.EndsWith("]")).Count();

			// setConditions
			for (int i = 0; i < CondCount; i++)
			{
				string condOperator = vars.ContainsKey($"CondOperator[{i}]") ? (string)vars[$"CondOperator[{i}]"] : "Equal";
				string condColumn = (string)vars[$"CondColumn[{i}]"];
				object condValue = vars[$"CondValue[{i}]"];

				var value = condValue;

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

            // MaxRows
            if (maxRows != null && orderBy != null)
                select.DropStep((int)vars["MaxRows"], ESqlFunction.LAST, isDescending, orderBy);

            // order
            select.Order(isDescending, orderBy);

            // group
            if (groupBy != null)
            {
                ESqlFunction function = ESqlFunction.SUM;
                if (vars.ContainsKey("GroupByFunction"))
                {
                    switch ((string)vars["GroupByFunction"])
                    {
                        case "none":
                            function = ESqlFunction.none;
                            break;
                        case "MAX":
                            function = ESqlFunction.MAX;
                            break;
                        case "MIN":
                            function = ESqlFunction.MIN;
                            break;
                        case "AVG":
                            function = ESqlFunction.AVG;
                            break;
                        case "COUNT":
                            function = ESqlFunction.COUNT;
                            break;
                        case "SUM":
                            function = ESqlFunction.SUM;
                            break;
                        case "FIRST":
                            function = ESqlFunction.FIRST;
                            break;
                        case "LAST":
                            function = ESqlFunction.LAST;
                            break;
                    }
                }
                select.Group(function, columns: (string)vars["GroupBy"]);
            }

            // return
            outputVars["Data"] = select.ToList();
		}
	}
}

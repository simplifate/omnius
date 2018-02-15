using System;
using System.Collections.Generic;
using System.Linq;
using FSS.Omnius.Modules.CORE;
using FSS.Omnius.Modules.Entitron.Entity;
using FSS.Omnius.Modules.Entitron.Entity.Nexus;
using FSS.Omnius.Modules.Nexus.Service;
using FSS.Omnius.Modules.Watchtower;
using Newtonsoft.Json.Linq;

namespace FSS.Omnius.Modules.Tapestry.Actions.Nexus
{
    [NexusRepository]
    class ExtDBSelectAction : Action
	{
		public override int Id => 3004;

		public override string[] InputVar => new string[] { "s$TableName", "?s$CondColumn[]", "?CondValue[]", "?s$CondOperator[][eq|lt|gt|lte|gte|like|not-like|in|not-in]", "?s$OrderBy", "?OrderByIndex", "?i$Limit", "?i$Skip" };

		public override string Name => "ExtDB: Select";

		public override string[] OutputVar => new string[] { "Result", "Error" };

		public override int? ReverseActionId => null;

		public override void InnerRun(Dictionary<string, object> vars, Dictionary<string, object> outputVars, Dictionary<string, object> InvertedInputVars, Message message)
		{
			try
			{
				CORE.CORE core = (CORE.CORE) vars["__CORE__"];
				var context = DBEntities.appInstance(core.Application);

                string dbName = (string)vars["dbName"];
                string tableName = (string)vars["TableName"];

                ExtDB dbInfo = context.ExtDBs.Where(d => d.DB_Alias == dbName).SingleOrDefault();
                if(dbInfo == null) {
                    throw new Exception(string.Format("{0}: Integration was not found", Name));
                }

                bool isOrderedByIndex = (vars.ContainsKey("OrderByIndex")) ? Convert.ToBoolean(vars["OrderByIndex"]) : false;

                NexusExtDBBaseService service;
                switch(dbInfo.DB_Type) {
                    case ExtDBType.RethinkDB:
                        service = new NexusExtDBRethingService(dbInfo);
                        break;
                    default:
                        service = (new NexusExtDBService(dbInfo.DB_Server, dbInfo.DB_Alias)).NewQuery("").Select("*");
                        break;
                }

                var query = service.From(tableName);

                if(service is NexusExtDBRethingService && vars.ContainsKey("OrderBy")) {
                    string orderBy = (string)vars["OrderBy"];
                    if(!string.IsNullOrEmpty(orderBy))
                    {
                        query = isOrderedByIndex ? query.OrderBy($"index:{orderBy}") : query.OrderBy(orderBy);
                    }
                }
                int condCount = vars.Keys.Where(k => k.StartsWith("CondColumn[") && k.EndsWith("]")).Count();
                if(condCount > 0) { 
                    JArray cond = new JArray();
                    // setConditions
                    for (int i = 0; i < condCount; i++) {
                        string condOperator = vars.ContainsKey($"CondOperator[{i}]") ? (string)vars[$"CondOperator[{i}]"] : "eq";
                        string condColumn = (string)vars[$"CondColumn[{i}]"];
                        object condValue = vars[$"CondValue[{i}]"];

                        var c = new JObject();
                        c["column"] = condColumn;
                        c["operator"] = condOperator;
                        c["value"] = JToken.FromObject(condValue);

                        cond.Add(c);
                    }
                    query = query.Where(cond);
                }

                if (service is NexusExtDBService && vars.ContainsKey("OrderBy"))
                {
                    string orderBy = (string)vars["OrderBy"];
                    if (!string.IsNullOrEmpty(orderBy))
                    {
                        query = query.OrderBy(orderBy);
                    }
                }

                if (vars.ContainsKey("Limit")) {
                    query = query.Limit((int)vars["Limit"]);
                }
                if(vars.ContainsKey("Skip")) {
                    query = query.Offset((int)vars["Skip"]);
                }
                
                var data = query.FetchAll();
                outputVars["Result"] = data;
                outputVars["Error"] = false;
			}
			catch (Exception e)
			{
				string errorMsg = e.Message;
				CORE.CORE core = (CORE.CORE)vars["__CORE__"];
				OmniusException.Log(e, OmniusLogSource.Nexus, core.Application, core.User);
				outputVars["Result"] = String.Empty;
				outputVars["Error"] = true;
			}
		}
	}
}

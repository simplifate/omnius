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
    class ExtDBUpdateAction : Action
	{
		public override int Id => 3006;

		public override string[] InputVar => new string[] { "s$TableName", "v$Data", "s$Where" };

		public override string Name => "ExtDB: Update";

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
                JToken data = (JToken)vars["Data"];
                object where = (object)vars["Where"];

                if(where == null || (where is String && string.IsNullOrEmpty((string)where))) {
                    throw new Exception(string.Format("{0}: where is missing. You must provide where clausule or rethingDB item id", Name));
                }

                ExtDB dbInfo = context.ExtDBs.Where(d => d.DB_Alias == dbName).SingleOrDefault();
                if(dbInfo == null) {
                    throw new Exception(string.Format("{0}: Integration was not found", Name));
                }

                NexusExtDBBaseService service;
                switch(dbInfo.DB_Type) {
                    case ExtDBType.RethinkDB:
                        service = new NexusExtDBRethingService(dbInfo);
                        break;
                    default:
                        service = (new NexusExtDBService(dbInfo.DB_Server, dbInfo.DB_Alias)).NewQuery("");
                        break;
                }

                NexusExtDBResult result = service.Update(tableName, data, where);

                if (result.Errors == 0) {
                    outputVars["Result"] = result.Replaced;
                    outputVars["Error"] = false;
                }
                else {
                    outputVars["Result"] = result.FirstError;
                    outputVars["Error"] = true;

                    OmniusException.Log(result.FirstError, OmniusLogSource.Nexus, null, core.Application, core.User);
                }
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

using System;
using System.Collections.Generic;
using FSS.Omnius.Modules.CORE;
using FSS.Omnius.Modules.Watchtower;
using FSS.Omnius.Modules.Entitron;

namespace FSS.Omnius.Modules.Tapestry.Actions.other
{
    [OtherRepository]
    class LogToActivityProtocolAction : Action
    {
        public override int Id
        {
            get
            {
                return 189;
            }
        }

        public override string[] InputVar
        {
            get
            {
                return new string[] { "Message", "?IdOrg", "?IdAuction", "?TableName" };
            }
        }

        public override string Name
        {
            get
            {
                return "Log to activity protocol";
            }
        }

        public override string[] OutputVar
        {
            get
            {
                return null;
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
            string tableName = vars.ContainsKey("TableName") ? (string)vars["TableName"] : "logs";
            DBTable table = core.Entitron.GetDynamicTable(tableName);

            if (table == null)
                throw new Exception($"Požadovaná tabulka nebyla nalezena (Tabulka: {tableName}, Akce: {Name} ({Id}))");

            string msg = (string)vars["Message"];
            int? idOrg = vars.ContainsKey("IdOrg") ? (int?)int.Parse(vars["IdOrg"].ToString()) : null;
            int? idAuction = vars.ContainsKey("IdAuction") ? (int?)int.Parse(vars["IdAuction"].ToString()) : null;

            DBItem item = new DBItem();
            item.createProperty(table.columns.Find(c => c.Name == "id_user").ColumnId, "id_user", core.User.Id);
            if(idOrg == null)
                item.createProperty(table.columns.Find(c => c.Name == "id_org").ColumnId, "id_org", DBNull.Value);
            else
                item.createProperty(table.columns.Find(c => c.Name == "id_org").ColumnId, "id_org", idOrg);
            if (idAuction == null)
                item.createProperty(table.columns.Find(c => c.Name == "id_auction").ColumnId, "id_auction", DBNull.Value);
            else
                item.createProperty(table.columns.Find(c => c.Name == "id_auction").ColumnId, "id_auction", idAuction);
            item.createProperty(table.columns.Find(c => c.Name == "message").ColumnId, "message", msg);

            var time = TimeZoneInfo.ConvertTimeFromUtc(DateTime.SpecifyKind(DateTime.Now.ToUniversalTime(), DateTimeKind.Unspecified),
                    TimeZoneInfo.FindSystemTimeZoneById("Central European Standard Time"));

            item.createProperty(table.columns.Find(c => c.Name == "timestamp").ColumnId, "timestamp", time);

            table.Add(item);
            core.Entitron.Application.SaveChanges();
        }
    }
}

using System;
using System.Collections.Generic;
using FSS.Omnius.Modules.CORE;
using FSS.Omnius.Modules.Entitron.DB;

namespace FSS.Omnius.Modules.Tapestry.Actions.other
{
    [OtherRepository]
    class LogToActivityProtocolAction : Action
    {
        public override int Id => 189;

        public override string[] InputVar => new string[] { "Message", "?IdOrg", "?IdAuction", "?TableName" };

        public override string Name => "Log to activity protocol";

        public override string[] OutputVar => null;

        public override int? ReverseActionId => null;

        public override void InnerRun(Dictionary<string, object> vars, Dictionary<string, object> outputVars, Dictionary<string, object> InvertedInputVars, Message message)
        {
            COREobject core = COREobject.i;
            DBConnection db = core.Entitron;

            string tableName = vars.ContainsKey("TableName") ? (string)vars["TableName"] : "logs";
            DBTable table = db.Table(tableName);

            string msg = (string)vars["Message"];
            int? idOrg = vars.ContainsKey("IdOrg") ? (int?)int.Parse(vars["IdOrg"].ToString()) : null;
            int? idAuction = vars.ContainsKey("IdAuction") ? (int?)int.Parse(vars["IdAuction"].ToString()) : null;

            DBItem item = new DBItem(db, table);
            item["id_user"] = core.User.Id;
            if (idOrg == null)
                item["id_org"] = DBNull.Value;
            else
                item["id_org"] = idOrg;
            if (idAuction == null)
                item["id_auction"] = DBNull.Value;
            else
                item["id_auction"] = idAuction;
            item["message"] = msg;

            var time = TimeZoneInfo.ConvertTimeFromUtc(DateTime.SpecifyKind(DateTime.Now.ToUniversalTime(), DateTimeKind.Unspecified),
                    TimeZoneInfo.FindSystemTimeZoneById("Central European Standard Time"));

            item["timestamp"] = time;

            table.Add(item);
            db.SaveChanges();
        }
    }
}

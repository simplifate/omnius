using System.Collections.Generic;
using System.Linq;
using FSS.Omnius.Modules.CORE;
using FSS.Omnius.Modules.Entitron.Entity;
using FSS.Omnius.Modules.Entitron.DB;

namespace FSS.Omnius.Modules.Tapestry.Actions.other
{
    [OtherRepository]
    class PersonaIdToRweAction : Action
    {
        public override int Id => 4108;
        public override string[] InputVar => new string[] { "UserId" };
        public override string Name => "Persona: UserId to RWE user row";
        public override string[] OutputVar => new string[] { "Result" };
        public override int? ReverseActionId => null;

        public override void InnerRun(Dictionary<string, object> vars, Dictionary<string, object> outputVars, Dictionary<string, object> InvertedInputVars, Message message)
        {
            COREobject core = COREobject.i;
            DBConnection db = core.Entitron;
            DBEntities context = core.AppContext;

            var rweUsersTable = db.Table("Users");
            int userId = (int)vars["UserId"];

            var user = context.Users.Find(userId);
            var results = rweUsersTable.Select().Where(c=> c.Column("ad_email").Equal(user.Email)).ToList();
            if(results.Count > 0)
            {
                outputVars["Result"] = results[0];
            }
            else
            {
                outputVars["Result"] = null;
            }
        }
    }
}

using System;
using System.Collections.Generic;
using FSS.Omnius.Modules.CORE;

namespace FSS.Omnius.Modules.Tapestry.Actions.EPK
{
    class FillOrderAction : Action
    {
        public override int Id => 1000001;

        public override string[] InputVar => new string[0];

        public override string Name => "Fill Order";

        public override string[] OutputVar => new string[] { "PurchaseDate" };

        public override int? ReverseActionId => null;

        public override void InnerRun(Dictionary<string, object> vars, Dictionary<string, object> outputVars, Dictionary<string, object> InvertedInputVars, Message message)
        {
            // CORE.CORE core = (CORE.CORE)vars["__CORE__"];
            // var sap = core.Entitron.GetDynamicTable("Users");
            // string username = core.User.UserName;
            // int i = username.IndexOf('\\') >= 0 ? username.IndexOf('\\') : 0;
            // var thisSap = sap.Select().where(c => c.column("ad_id").Equal(username.Substring(i))).First();

            // var plan = core.Entitron.GetDynamicTable("Plans");
            // var thisSapPlan = plan.Select().where(c => c.column("objid").Equal(thisSap["plans"])).First();

            // outputVars["year"] = DateTime.Now.Year;
            // outputVars["client_sapid2"] = thisSap["sapid2"];
            // outputVars["client_function"] = thisSapPlan["stext"];
            outputVars["PurchaseDate"] = DateTime.Now.ToString("dd.MM.y");
        }
    }
}

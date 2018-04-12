using System;
using System.Collections.Generic;
using FSS.Omnius.Modules.CORE;

namespace FSS.Omnius.Modules.Tapestry.Actions.other
{
    [OtherRepository]
    class ModifyDatetime : Action
    {
        public override int Id => 188;

        public override string[] InputVar => new string[] { "Input", "?AddMinutes", "?AddDays", "?SetToLastDayOfMonth", "?b$ToUtc" };

        public override string Name => "Modify DateTime";

        public override string[] OutputVar => new string[] { "Result" };

        public override int? ReverseActionId => null;

        public override void InnerRun(Dictionary<string, object> vars, Dictionary<string, object> outputVars, Dictionary<string, object> InvertedInputVars, Message message)
        {
            if (vars["Input"] == null || vars["Input"] == DBNull.Value)
            {
                outputVars["Result"] = null;
                return;
            }
            DateTime dateTime;

            try
            {
                dateTime = (DateTime)vars["Input"];
            }
            catch (Exception)
            {
                dateTime = DateTime.Parse((string)vars["Input"]);
            }

            if (vars.ContainsKey("AddMinutes"))
                dateTime = dateTime.AddMinutes((int)vars["AddMinutes"]);

            if (vars.ContainsKey("AddDays"))
                dateTime = dateTime.AddDays((int)vars["AddDays"]);

            if (vars.ContainsKey("SetToLastDayOfMonth") && (bool)vars["SetToLastDayOfMonth"] == true)
            {
                DateTime newDate = new DateTime(dateTime.Year, dateTime.Month, DateTime.DaysInMonth(dateTime.Year, dateTime.Month));
                dateTime = newDate;
            }

            if ((bool)vars["ToUtc"] == true)
            {
                dateTime = TimeZoneInfo.ConvertTimeToUtc(dateTime);
            }

            outputVars["Result"] = dateTime;

            // TODO: Other modifications: add hours, days, etc. + add timespan
        }
    }
}

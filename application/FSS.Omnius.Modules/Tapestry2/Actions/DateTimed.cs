using FSS.Omnius.Modules.CORE;
using System;
using System.Globalization;

namespace FSS.Omnius.Modules.Tapestry2.Actions
{
    public class DateTimed : ActionManager
    {
        [Action(187, "Format DateTime", "Result")]
        public static string FormatDatetime(COREobject core, DateTime Input, string Format = "o")
        {
            return Input.ToUniversalTime().ToString(Format);
        }

        [Action(188, "Modify DateTime", "Result")]
        public static DateTime ModifyDatetime(COREobject core, DateTime Input, int? AddMinutes = null, int? AddDays = null, bool SetToLastDayOfMonth = false)
        {
            if (AddMinutes != null)
                Input = Input.AddMinutes(AddMinutes.Value);

            if (AddDays != null)
                Input = Input.AddDays(AddDays.Value);

            if (SetToLastDayOfMonth)
                Input = Input.AddMonths(1).AddDays(-Input.Day);

            return Input;
        }

        [Action(194, "Merge Date and Time", "Result")]
        public static DateTime MergeDatetime(COREobject core, DateTime Date, DateTime Time)
        {
            return new DateTime(Date.Year, Date.Month, Date.Day, Time.Hour, Time.Minute, Time.Second);
        }

        [Action(200, "Parse Datetime", "Result")]
        public static DateTime ParseDatetime(COREobject core, string Input)
        {
            DateTime datetime;
            DateTime.TryParseExact(Input,
                                new string[] { "d.M.yyyy H:mm:ss", "d.M.yyyy", "H:mm:ss", "yyyy-MM-ddTHH:mm", "dd.MM.yyyy HH:mm", },
                                CultureInfo.InvariantCulture, DateTimeStyles.None, out datetime);

            return datetime;
        }

        [Action(801, "Time span", "Result")]
        public static int TimeSpan(COREobject core, DateTime? From = null, DateTime? To = null)
        {
            return (int)((To ?? DateTime.Now) - (From ?? DateTime.Now)).TotalMilliseconds;
        }

        [Action(802, "Add TimeSpan to DateTime", "Result")]
        public static DateTime AddTimeSpan(COREobject core, int TimeSpan, DateTime? StartTime = null)
        {
            return (StartTime ?? DateTime.Now).Add(System.TimeSpan.FromMilliseconds(TimeSpan));
        }

        [Action(1032, "Get current time", "Result")]
        public static DateTime GetCurrentTime(COREobject core)
        {
            return DateTime.Now;
        }
    }
}

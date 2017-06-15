using System;
using System.ComponentModel.DataAnnotations.Schema;
using FSS.Omnius.Modules.Watchtower;
using System.ComponentModel.DataAnnotations;
using System.Collections.Generic;

namespace FSS.Omnius.Modules.Entitron.Entity.Watchtower
{
    [Table("Watchtower_LogItems")]
    public class LogItem : IEntity
    {
        public LogItem()
        {
            ChildLogItems = new HashSet<LogItem>();
        }

        public int Id { get; set; }
        [Index]
        public DateTime Timestamp { get; set; }
        public int LogLevel { get; set; }
        [Index]
        [StringLength(50)]
        public string UserName { get; set; }

        [Index]
        [StringLength(50)]
        public string Server { get; set; }
        [Index]
        public int Source { get; set; }
        [Index]
        [StringLength(50)]
        public string Application { get; set; }
        [StringLength(100)]
        public string BlockName { get; set; }
        [StringLength(50)]
        public string ActionName { get; set; }

        public string Message { get; set; }
        public string Vars { get; set; }
        public string StackTrace { get; set; }
        
        public int? ParentLogItemId { get; set; }
        public virtual LogItem ParentLogItem { get; set; }
        public virtual ICollection<LogItem> ChildLogItems { get; set; }

        [NotMapped]
        public string TimeString => Timestamp.ToString("d. M. yyyy H:mm:ss");
        [NotMapped]
        public string LogSourceString => OmniusLog.toHumanString[(OmniusLogSource)Source];
        [NotMapped]
        public string LogLevelString => ((OmniusLogLevel)LogLevel).ToString();
        [NotMapped]
        public string LogEventSourceString => Application ?? "Platforma Omnius";

        public int VarsCount()
        {
            if (Vars == null)
                return 0;

            return Vars.Split('\n').Length;
        }
        public string VarHtmlTable()
        {
            if (Vars == null)
                return "";

            string result = "<table>";
            foreach(string line in Vars.Split('\n'))
            {
                string[] pair = line.Split(new string[] { "=>" }, StringSplitOptions.None);

                if (pair.Length == 2)
                    result += $"<tr><th>{pair[0]}</th><td>{pair[1]}</td></tr>";
                else
                    result += $"<tr><td>{line}</td></tr>";
            }

            return result + "</table>";
        }
        public string StackTraceHtml()
        {
            if (StackTrace == null)
                return null;

            return StackTrace.Replace(Environment.NewLine, "<br/><br/>");
        }
    }
}

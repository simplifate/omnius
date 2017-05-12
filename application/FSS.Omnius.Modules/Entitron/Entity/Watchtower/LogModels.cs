using System;
using System.ComponentModel.DataAnnotations.Schema;
using FSS.Omnius.Modules.Watchtower;
using System.ComponentModel.DataAnnotations;

namespace FSS.Omnius.Modules.Entitron.Entity.Watchtower
{
    [Table("Watchtower_LogItems")]
    public class LogItem : IEntity
    {
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
    }
    public class AjaxLogItem : IEntity
    {
        public DateTime Timestamp { get; set; }
        public int Level { get; set; }
        public string UserName { get; set; }

        public string Server { get; set; }
        public int Source { get; set; }
        public string ApplicationName { get; set; }
        public string BlockName { get; set; }
        public string ActionName { get; set; }

        public string Message { get; set; }
        public string Vars { get; set; }
        public string StackTrace { get; set; }

        public string TimeString => Timestamp.ToString("d. M. yyyy H:mm:ss");
        public string LogEventTypeString => OmniusLog.toHumanString[(OmniusLogSource)Source];
        public string LogLevelString => ((OmniusLogLevel)Level).ToString();
        public string LogEventSourceString => ApplicationName ?? "Platforma Omnius";
    }
}

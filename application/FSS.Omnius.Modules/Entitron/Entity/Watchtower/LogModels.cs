using System;
using System.ComponentModel.DataAnnotations.Schema;
using FSS.Omnius.Modules.Watchtower;

namespace FSS.Omnius.Modules.Entitron.Entity.Watchtower
{
    [Table("Watchtower_LogItems")]
    public class LogItem
    {
        public int Id { get; set; }
        public DateTime Timestamp { get; set; }
        public int LogEventType { get; set; }
        public int LogLevel { get; set; }
        public int UserId { get; set; }
        public bool IsPlatformEvent { get; set; }
        public int? AppId { get; set; }
        public string Message { get; set; }
    }
    public class AjaxLogItem
    {
        public int Id { get; set; }
        public DateTime Timestamp { get; set; }
        public int LogEventType { get; set; }
        public int LogLevel { get; set; }
        public int UserId { get; set; }
        public bool IsPlatformEvent { get; set; }
        public int? AppId { get; set; }
        public string Message { get; set; }
        public string TimeString => Timestamp.ToString("d. M. yyyy H:mm:ss");
        public string LogEventTypeString => WatchtowerLogger.Instance.GetEventTypeString(LogEventType);
        public string LogLevelString => WatchtowerLogger.Instance.GetLogLevelString(LogLevel);
        public string LogEventSourceString => WatchtowerLogger.Instance.GetEventSourceString(IsPlatformEvent, AppId);
        public string UserName => WatchtowerLogger.Instance.GetUserName(UserId);
    }
}

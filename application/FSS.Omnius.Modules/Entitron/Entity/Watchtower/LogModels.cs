using System;

namespace FSS.Omnius.Modules.Entitron.Entity.Watchtower
{
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
}

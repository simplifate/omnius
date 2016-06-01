using System;
using System.Collections.Generic;
using System.Data.Linq;
using FSS.Omnius.Modules.Entitron.Entity;
using FSS.Omnius.Modules.Entitron.Entity.Watchtower;

namespace FSS.Omnius.Modules.Watchtower
{
    public enum LogEventType
    {
        NotSpecified, NormalUserAction, AccessDenied, EmailSent, Tapestry
    }
    public enum LogLevel
    {
        Info, Warning, Error, FatalError
    }
    public class WatchtowerLogger
    {
        private Dictionary<int, string> EventTypeMap;
        private Dictionary<int, string> LogLevelMap;
        private DBEntities context;

        public string GetEventTypeString(int id)
        {
            return EventTypeMap[id];
        }
        public string GetLogLevelString(int id)
        {
            return LogLevelMap[id];
        }
        public string GetEventSourceString(bool isPlatformEvent, int? appId)
        {
            if (isPlatformEvent)
                return "Platforma Omnius";
            else if (appId == null)
                return "Neznámá aplikace";
            try
            {
                return context.Applications.Find(appId).DisplayName;
            }
            catch (InvalidOperationException)
            {
                return string.Format("Aplikace {0} (jméno chybí)", appId);
            }
        }
        public string GetUserName(int id)
        {
            try
            {
                return context.Users.Find(id).DisplayName;
            }
            catch (InvalidOperationException)
            {
                return string.Format("Uživatel {0} (jméno chybí)", id);
            }
        }

        public void LoadMaps()
        {
            EventTypeMap.Add((int)LogEventType.NotSpecified, "Neznámá akce");
            EventTypeMap.Add((int)LogEventType.NormalUserAction, "Akce uživatele");
            EventTypeMap.Add((int)LogEventType.AccessDenied, "Přístup odepřen");
            EventTypeMap.Add((int)LogEventType.EmailSent, "Odeslání e-mailu");
            EventTypeMap.Add((int)LogEventType.Tapestry, "Akce tapestry");

            LogLevelMap.Add((int)LogLevel.Info, "Informace");
            LogLevelMap.Add((int)LogLevel.Warning, "Varování");
            LogLevelMap.Add((int)LogLevel.Error, "Chyba");
            LogLevelMap.Add((int)LogLevel.FatalError, "Fatální chyba");
        }

        public void LogEvent(string message, int userId, LogEventType eventType = LogEventType.NotSpecified,
            LogLevel level = LogLevel.Info, bool isPlatormEvent = true, int? appId = null)
        {
            using (var context = new DBEntities())
            {
                var logItem = new LogItem
                {
                    Timestamp = DateTime.UtcNow,
                    LogEventType = (int)eventType,
                    LogLevel = (int)level,
                    UserId = userId,
                    IsPlatformEvent = isPlatormEvent,
                    AppId = appId,
                    Message = message
                };
                context.LogItems.Add(logItem);
                context.SaveChanges();
            }
        }
        private static WatchtowerLogger instance;
        private WatchtowerLogger()
        {
            EventTypeMap = new Dictionary<int, string>();
            LogLevelMap = new Dictionary<int, string>();
            context = new DBEntities();
        }
        public static WatchtowerLogger Instance
        {
            get
            {
                if (instance == null)
                {
                    instance = new WatchtowerLogger();
                    instance.LoadMaps();
                }
                return instance;
            }
        }
    }
}

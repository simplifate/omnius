using System;
using System.Collections.Generic;
using FSS.Omnius.Modules.Entitron.Entity;
using FSS.Omnius.Modules.Entitron.Entity.Watchtower;

namespace FSS.Omnius.Modules.Watchtower
{
    public enum LogEventType
    {
        NotSpecified, NormalUserAction, AccessDenied
    }
    public enum LogLevel
    {
        Info, Warning, Error, FatalError
    }
    public class WatchtowerLogger
    {
        private Dictionary<int, string> EventTypeMap;
        private Dictionary<int, string> LogLevelMap;
        private Dictionary<int, string> AppNameMap;
        private Dictionary<int, string> UserNameMap;

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
            else if (AppNameMap.ContainsKey((int)appId))
                return AppNameMap[(int)appId];
            else
                return String.Format("Aplikace {0} (jméno chybí)", appId);
        }
        public string GetUserName(int id)
        {
            if (UserNameMap.ContainsKey(id))
                return UserNameMap[id];
            else
                return String.Format("Uživatel {0} (jméno chybí)", id);
        }

        public void LoadMaps()
        {
            EventTypeMap.Add((int)LogEventType.NotSpecified, "Neznámá akce");
            EventTypeMap.Add((int)LogEventType.NormalUserAction, "Akce uživatele");
            EventTypeMap.Add((int)LogEventType.AccessDenied, "Přístup odepřen");

            LogLevelMap.Add((int)LogLevel.Info, "Informace");
            LogLevelMap.Add((int)LogLevel.Warning, "Varování");
            LogLevelMap.Add((int)LogLevel.Error, "Chyba");
            LogLevelMap.Add((int)LogLevel.FatalError, "Fatální chyba");

            // TODO: Load real data from Persona
            UserNameMap.Add(0, "Anonym");
            UserNameMap.Add(1, "Samuel Lachman");
            UserNameMap.Add(2, "Martin Novák");

            // TODO: Load real data from app profiles
            AppNameMap.Add(0, "Rezervace služeb");
            AppNameMap.Add(1, "Překlady");
        }

        public void LogEvent(string message, int userId, LogEventType eventType = LogEventType.NotSpecified,
            LogLevel level = LogLevel.Info, bool isPlatormEvent = true, int? appId = null)
        {
            using (var context = new DBEntities())
            {
                var logItem = new LogItem
                {
                    Timestamp = DateTime.Now,
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
            UserNameMap = new Dictionary<int, string>();
            AppNameMap = new Dictionary<int, string>();
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

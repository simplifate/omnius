using FSS.Omnius.Modules.CORE;
using FSS.Omnius.Modules.Entitron.Entity;
using FSS.Omnius.Modules.Entitron.Entity.Master;
using FSS.Omnius.Modules.Entitron.Entity.Persona;
using FSS.Omnius.Modules.Entitron.Entity.Tapestry;
using FSS.Omnius.Modules.Entitron.Entity.Watchtower;
using FSS.Omnius.Modules.Tapestry;
using System;
using System.Collections.Generic;
using System.Data.Entity.Validation;
using System.Linq;
using System.Web;

namespace FSS.Omnius.Modules.Watchtower
{
    public abstract class OmniusLog : Exception
    {
        public static Dictionary<OmniusLogSource, string> toHumanString = new Dictionary<OmniusLogSource, string>
        {
            { OmniusLogSource.none, "neznámá akce" },
            { OmniusLogSource.CORE, "správa modulů" },
            { OmniusLogSource.Tapestry, "akce uživatele" },
            { OmniusLogSource.Mozaic, "frontend" },
            { OmniusLogSource.Entitron, "databáze" },
            { OmniusLogSource.Cortex, "časované úlohy" },
            { OmniusLogSource.Persona, "přístup odepřen" },
            { OmniusLogSource.Hermes, "odeslání e-mailu" },
            { OmniusLogSource.Master, "vytváření a správa aplikace" },
            { OmniusLogSource.Nexus, "integrace" },
            { OmniusLogSource.Watchtower, "záznam do protokolu činností" }
        };
        public static void Log(string Message, OmniusLogLevel level, OmniusLogSource source = OmniusLogSource.none, Application application = null, User user = null)
        {
            switch(level)
            {
                case OmniusLogLevel.Info:
                    new OmniusInfo(Message)
                    {
                        Application = application,
                        User = user,
                        SourceModule = source
                    }.Save();
                    break;
                case OmniusLogLevel.Warning:
                    new OmniusWarning(Message)
                    {
                        Application = application,
                        User = user,
                        SourceModule = source
                    }.Save();
                    break;
                case OmniusLogLevel.Error:
                    new OmniusException(Message)
                    {
                        Application = application,
                        User = user,
                        SourceModule = source
                    }.Save();
                    break;
            }
        }

        public DateTime Timestamp { get; }
        public OmniusLogLevel Level { get; }
        public User User { get; set; }

        public string Server { get; }
        public OmniusLogSource SourceModule { get; set; }
        public Application Application { get; set; }

        public OmniusLog(string Message, Exception innerException, OmniusLogLevel level) : base(Message, innerException)
        {
            Timestamp = DateTime.UtcNow;
            Level = level;
            Server = HttpContext.Current?.Request.Url.Authority;
        }

        //public static string ToString(Exception ex)
        //{
        //    string result = "";

        //    Exception curError = ex;
        //    while (curError != null)
        //    {
        //        result += $"Message: {curError.Message}{Environment.NewLine}";
        //        result += $"Method: {curError.TargetSite.ToString()}{Environment.NewLine}";
        //        result += $"Error type: {curError.GetType()}{Environment.NewLine}";
        //        result += $"Trace: {curError.StackTrace}{Environment.NewLine}";

        //        // validation error
        //        if (curError?.GetType() == typeof(DbEntityValidationException))
        //        {
        //            result += $"Validation errors:{Environment.NewLine}";
        //            foreach (DbEntityValidationResult valE in (curError as DbEntityValidationException).EntityValidationErrors)
        //            {
        //                foreach (var validationMessage in valE.ValidationErrors)
        //                {
        //                    result += $" -> {validationMessage.PropertyName}: {validationMessage.ErrorMessage}{Environment.NewLine}";
        //                }
        //            }
        //        }

        //        result += Environment.NewLine;
        //        // inner error
        //        curError = curError.InnerException;
        //    }

        //    return result;
        //}

        public void Save(DBEntities context = null)
        {
            context = context ?? DBEntities.instance;
            context.LogItems.Add(toLogItem());
            context.SaveChanges();
            Send();
        }
        protected virtual LogItem toLogItem()
        {
            return new LogItem
            {
                Timestamp = Timestamp,
                LogLevel = (int)Level,
                UserName = User?.UserName,
                Server = Server,
                Source = (int)SourceModule,
                Application = Application?.Name,
                Message = Message
            };
        }
        protected virtual void Send()
        {
            #warning ToDo
        }
    }

    public class OmniusInfo : OmniusLog
    {
        public static void Log(string Message, OmniusLogSource source = OmniusLogSource.none, Application application = null, User user = null)
        {
            new OmniusInfo(Message)
            {
                Application = application,
                User = user,
                SourceModule = source
            }.Save();
        }

        public OmniusInfo(string Message) : base(Message, null, OmniusLogLevel.Info)
        {
        }
    }

    public class OmniusWarning : OmniusLog
    {
        public static void Log(string Message, OmniusLogSource source = OmniusLogSource.none, Application application = null, User user = null)
        {
            new OmniusWarning(Message)
            {
                Application = application,
                User = user,
                SourceModule = source
            }.Save();
        }

        public OmniusWarning(string Message) : base(Message, null, OmniusLogLevel.Warning)
        {
        }
    }

    public class OmniusException : OmniusLog
    {
        public static void Log(string Message, OmniusLogSource source = OmniusLogSource.none, Exception innerException = null, Application application = null, User user = null)
        {
            new OmniusException(Message, innerException)
            {
                Application = application,
                User = user,
                SourceModule = source
            }.Save();
        }
        public static void Log(Exception innerException, OmniusLogSource source = OmniusLogSource.none, Application application = null, User user = null)
        {
            new OmniusException(innerException)
            {
                Application = application,
                User = user,
                SourceModule = source
            }.Save();
        }

        public OmniusException(string Message) : base(Message, null, OmniusLogLevel.Error)
        {
        }
        public OmniusException(Exception InnerException) : base(InnerException.Message, InnerException, OmniusLogLevel.Error)
        {
        }
        public OmniusException(string Message, Exception innerException) : base(Message, innerException, OmniusLogLevel.Error)
        {
        }

        protected override LogItem toLogItem()
        {
            LogItem result = base.toLogItem();
            result.StackTrace = InnerException.StackTrace;
            return result;
        }
    }

    public class OmniusApplicationException : OmniusException
    {
        public static void Log(string Message, Exception innerException = null, OmniusLogSource source = OmniusLogSource.none, Application application = null, Block block = null, IActionRule_Action actionRule = null, Dictionary<string, object> vars = null, User user = null)
        {
            new OmniusApplicationException(Message, innerException)
            {
                Application = application,
                Block = block,
                ActionRuleAction = actionRule,
                Vars = vars,
                User = user,
                SourceModule = source
            }.Save();
        }
        public static void Log(Exception innerException, OmniusLogSource source = OmniusLogSource.none, Application application = null, Block block = null, IActionRule_Action actionRule = null, Dictionary<string, object> vars = null, User user = null)
        {
            new OmniusApplicationException(innerException)
            {
                Application = application,
                Block = block,
                ActionRuleAction = actionRule,
                Vars = vars,
                User = user,
                SourceModule = source
            }.Save();
        }

        public Block Block { get; set; }
        public IActionRule_Action ActionRuleAction { get; set; }
        public Dictionary<string, object> Vars { get; set; }

        public OmniusApplicationException(string Message) : base(Message)
        {
            Vars = new Dictionary<string, object>();
        }
        public OmniusApplicationException(Exception InnerException) : base(InnerException)
        {
            Vars = new Dictionary<string, object>();
        }
        public OmniusApplicationException(string Message, Exception innerException) : base(Message, innerException)
        {
            Vars = new Dictionary<string, object>();
        }

        protected override LogItem toLogItem()
        {
            LogItem result = base.toLogItem();
            result.BlockName = Block?.Name;
            result.ActionName = ActionRuleAction != null ? Tapestry.Action.All[ActionRuleAction.ActionId].Name : null;
            result.Vars = string.Join("\n", Vars.Select(pair => $"{pair.Key}=>{pair.Value.ToString().Truncate(1000)}"));
            return result;
        }
    }

    public enum OmniusLogLevel
    {
        Info = 0,
        Warning = 1,
        Error = 2,
        FatalError = 3
    }
    public enum OmniusLogSource
    {
        none = 0,
        CORE,
        Tapestry,
        Mozaic,
        Entitron,
        Cortex,
        Persona,
        Hermes,
        Master,
        Nexus,
        Watchtower,
        User
    }
}

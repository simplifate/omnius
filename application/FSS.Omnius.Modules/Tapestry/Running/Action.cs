using FSS.Omnius.Modules.Tapestry.Actions;
using System;
using System.Collections.Generic;
using System.Dynamic;
using System.Linq;
using System.Reflection;

namespace FSS.Omnius.Modules.Tapestry
{
    public abstract class Action : ActionBase
    {
        private static IEnumerable<Type> _repositories = null;
        private static Dictionary<int, Action> _actions = null;
        public static IEnumerable<Type> Repositories
        {
            get
            {
                if (_repositories == null)
                    INIT();

                return _repositories;
            }
        }
        public static Dictionary<int, Action> All
        {
            get
            {
                if (_actions == null)
                    INIT();

                return _actions;
            }
        }
        public static int getByName(string name)
        {
            if (_actions == null)
                INIT();

            return _actions.SingleOrDefault(a => a.Value.Name == name).Key;
        }
        public static ActionResultCollection RunAction(int id, Dictionary<string, object> vars)
        {
            Action action = All[id];

            return action.run(vars);
        }
        public static void INIT()
        {
            _repositories = Assembly.GetAssembly(typeof(ActionRepositoryAttribute)).GetTypes().Where(t => t.IsSubclassOf(typeof(ActionRepositoryAttribute)));
            _actions = new Dictionary<int, Action>();
            
            foreach (Type type in Assembly.GetAssembly(typeof(Action)).GetTypes().Where(t => t.IsSubclassOf(typeof(Action)) && !t.IsAbstract))
            {
                Action action = (Action)Activator.CreateInstance(type);
                _actions.Add(action.Id, action);
            }
        }
        
        public abstract int? ReverseActionId { get; }
        public abstract string Name { get; }
        
        public override ActionResultCollection run(Dictionary<string, object> vars)
        {
            var outputVars = new Dictionary<string, object>();
            var outputStatus = ActionResultType.Success;
            var outputMessage = "OK";
            var invertedVar = new Dictionary<string, object>();

            try
            {
                InnerRun(vars, outputVars, invertedVar);
            }
            catch (Exception ex)
            {
                outputStatus = ActionResultType.Error;
                outputMessage = ex.Message;
            }

            return new ActionResultCollection(outputStatus, outputMessage, invertedVar, outputVars);
        }

        public override void ReverseRun(Dictionary<string, object> vars)
        {
            if (ReverseActionId != null)
                RunAction(ReverseActionId.Value, vars);
        }
        public abstract void InnerRun(Dictionary<string, object> vars, Dictionary<string, object> outputVars, Dictionary<string,object> InvertedInputVars);

        public void LogError(string message, int userId, int appId)
        {
            Watchtower.WatchtowerLogger logger = Watchtower.WatchtowerLogger.Instance;
            logger.LogEvent(
                    message,
                    userId,
                    Watchtower.LogEventType.Tapestry,
                    Watchtower.LogLevel.Error,
                    false,
                    appId
                );
        }
    }
}

using FSS.Omnius.Modules.CORE;
using FSS.Omnius.Modules.Entitron.Entity.Tapestry;
using FSS.Omnius.Modules.Tapestry.Actions;
using FSS.Omnius.Modules.Watchtower;
using System;
using System.Collections.Generic;
using System.Dynamic;
using System.Linq;
using System.Reflection;

namespace FSS.Omnius.Modules.Tapestry
{
    public abstract class Action : IAction
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
        public static ActionResult RunAction(int id, Dictionary<string, object> vars, IActionRule_Action actionRule_action)
        {
            Action action = All[id];

            return action.run(vars, actionRule_action);
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
        
        public abstract int Id { get; }
        public abstract string[] InputVar { get; }
        public abstract string[] OutputVar { get; }
        public abstract int? ReverseActionId { get; }
        public abstract string Name { get; }
        
        public virtual ActionResult run(Dictionary<string, object> vars, IActionRule_Action actionRule_action)
        {
            Dictionary<string, object> outputVars = new Dictionary<string, object>();
            ActionResultType outputStatus = ActionResultType.Success;
            var invertedVar = new Dictionary<string, object>();
            Message message = new Message();

            try
            {
                InnerRun(vars, outputVars, invertedVar, message);
            }
            catch (Exception ex)
            {
                outputStatus = ActionResultType.Error;
                message.Errors.Add(ex.Message);
                Logger.Log.Error(ex);
                CORE.CORE core = (CORE.CORE)vars["__CORE__"];
                OmniusApplicationException.Log(ex, OmniusLogSource.Tapestry, core.Entitron.Application, actionRule_action.ActionRule.SourceBlock, actionRule_action, vars, core.User);
            }

            return new ActionResult(outputStatus, outputVars, invertedVar, message);
        }

        public void ReverseRun(Dictionary<string, object> vars)
        {
            //if (ReverseActionId != null)
            //    RunAction(ReverseActionId.Value, vars);
        }
        public abstract void InnerRun(Dictionary<string, object> vars, Dictionary<string, object> outputVars, Dictionary<string,object> InvertedInputVars, Message message);
    }
}

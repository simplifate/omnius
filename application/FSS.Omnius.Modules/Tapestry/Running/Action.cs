using FSS.Omnius.Modules.CORE;
using FSS.Omnius.Modules.Tapestry.Actions;
using FSS.Omnius.Modules.Watchtower;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Reflection;
using FSS.Omnius.Modules.Entitron.Entity;

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

        public static ActionResult RunForeach(Dictionary<string, object> vars, IActionRule_Action actionRule_action)
        {
            ActionResult results = new ActionResult();

            if (((IEnumerable<object>)vars["DataSource"]).Count() > 0) {
                CORE.CORE core = (CORE.CORE)vars["__CORE__"];
                DBEntities context = DBEntities.appInstance(core.Entitron.Application);
                //List<ActionRule_Action> actions = context.ActionRule_Action.Where(a => a.VirtualParentId == actionRule_action.VirtualItemId).OrderBy(a => a.Order).ToList();

                var startRule = context.ActionRules.Include("ActionRule_Actions").Where(a => a.ActionRule_Actions.Where(aa => aa.IsForeachStart == true && aa.VirtualParentId == actionRule_action.VirtualItemId).Any()).FirstOrDefault();
                if(startRule == null) {
                    return results;
                }

                foreach (object item in (IEnumerable<object>)vars["DataSource"]) {
                    vars["__item__"] = item;
                    results.OutputData = vars;

                    var nextRule = startRule;
                    while(nextRule != null) {
                        foreach (var nextAction in nextRule.ActionRule_Actions.OrderBy(a => a.Order)) {
                            var remapedParams = nextAction.getInputVariables(results.OutputData);
                            Action action = All[nextAction.ActionId];
                            ActionResult result = action.run(remapedParams, nextAction);

                            if (result.Type == ActionResultType.Error) {
                                foreach (IActionRule_Action reverseActionMap in nextRule.ActionRule_Actions.Where(a => a.Order < nextAction.Order).OrderByDescending(a => a.Order)) {
                                    var reverseAction = Action.All[reverseActionMap.ActionId];
                                    reverseAction.ReverseRun(results.ReverseInputData.Last());
                                    results.ReverseInputData.Remove(results.ReverseInputData.Last());
                                }

                                // do not continue
                                results.Join(result);
                                return results;
                            }

                            nextAction.RemapOutputVariables(result.OutputData);
                            // zpracování výstupů
                            results.Join(result);
                            vars = results.OutputData;
                        }

                        nextRule = Tapestry.GetActionRule(core, nextRule.TargetBlock, results);
                    }
                }
            }
            return results;
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
                OmniusApplicationException.Log(ex, OmniusLogSource.Tapestry, core.Application, actionRule_action.ActionRule.SourceBlock, actionRule_action, vars, core.User);
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

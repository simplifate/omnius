using FSS.Omnius.Modules.Entitron.Entity;
using FSS.Omnius.Modules.Entitron.Entity.Tapestry;
using FSS.Omnius.Modules.Tapestry;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace FSS.Omnius.Modules.Tapestry 
{
    public class ActionSequence : IAction
    {
        private List<IAction> _actions;
        
        public int Id { get; }
        public string[] InputVar { get; }
        public string[] OutputVar { get; }

        public ActionSequence(int Id, DBEntities context)
        {
            this.Id = Id;
            _actions = ActionSequence_Action.GetActions(Id, context);

            List<string> tempInput = new List<string>();
            List<string> tempOutput = new List<string>();
            foreach (Action a in _actions)
            {
                tempInput.AddRange(a.InputVar);
                tempOutput.AddRange(a.OutputVar);
            }

            InputVar = tempInput.ToArray();
            OutputVar = tempOutput.ToArray();
        }

        public ActionResult run(Dictionary<string, object> vars, IActionRule_Action actionRule_action)
        {
            ActionResult result = new ActionResult();

            foreach(Action act in _actions)
            {
                result.Join(act.run(vars, actionRule_action));
            }
            
            return result;
        }
        public void ReverseRun(Dictionary<string, object> vars)
        {
            _actions.Reverse();
            foreach(Action act in _actions)
            {
                act.ReverseRun(vars);
            }
        }
    }
}

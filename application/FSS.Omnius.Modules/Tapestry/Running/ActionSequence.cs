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
    public class ActionSequence : ActionBase
    {
        private List<ActionBase> _actions;
        
        public override int Id { get; }
        public override string[] InputVar { get; }
        public override string[] OutputVar { get; }

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

        public override ActionResult run(Dictionary<string, object> vars)
        {
            ActionResult result = new ActionResult();

            foreach(Action act in _actions)
            {
                result.Join(act.run(vars));
            }
            
            return result;
        }
        public override void ReverseRun(Dictionary<string, object> vars)
        {
            _actions.Reverse();
            foreach(Action act in _actions)
            {
                act.ReverseRun(vars);
            }
        }
    }
}

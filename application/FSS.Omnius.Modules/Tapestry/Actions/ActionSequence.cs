using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace FSS.Omnius.Modules.Tapestry.Actions
{
    public abstract class ActionSequence : Action
    {
        private List<Action> _sequence;

        public ActionSequence()
        {
            _sequence = new List<Action>();
        }

        public override ActionResult run(Dictionary<string, object> vars)
        {
            ActionResult results = new ActionResult();
            foreach(Action action in _sequence)
            {
                results.Join(action.run(vars));
            }

            return results;
        }
    }
}

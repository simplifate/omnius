using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace FSS.Omnius.Modules.Tapestry
{
    public interface IAction
    {
        int Id { get; }
        string[] InputVar { get; }
        string[] OutputVar { get; }
        ActionResult run(Dictionary<string, object> vars, IActionRule_Action actionRule_action);
        void ReverseRun(Dictionary<string, object> vars);
    }
}

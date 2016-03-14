using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace FSS.Omnius.Modules.Tapestry
{
    public abstract class ActionBase
    {
        public abstract int Id { get; }
        public abstract string[] InputVar { get; }
        public abstract string[] OutputVar { get; }

        public abstract ActionResult run(Dictionary<string, object> vars);
        public abstract void ReverseRun(Dictionary<string, object> vars);
    }
}

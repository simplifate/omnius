using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace FSS.Omnius.Modules.Tapestry
{
    public class ActionResult
    {
        public ActionResultType type { get; set; }
        public string Message { get; set; }
        public Dictionary<string, object> ReverseInputData { get; set; }
        public Dictionary<string, object> outputData;
    }

    public enum ActionResultType
    {
        Success,
        Warning,
        Error
    }
}

using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace FSS.Omnius.Modules.Tapestry
{
    public class ActionResult
    {
        public ActionResultType Type { get; set; }
        public Dictionary<string, object> OutputData { get; set; }
        public List<Dictionary<string, object>> ReverseInputData { get; set; }

        public ActionResult()
        {
            Type = ActionResultType.Success;
            OutputData = new Dictionary<string, object>();
            ReverseInputData = new List<Dictionary<string, object>>();
        }
        public ActionResult(ActionResultType type, Dictionary<string, object> outputData, Dictionary<string, object> reverseInputData)
        {
            Type = type;

            OutputData = outputData;
            ReverseInputData = new List<Dictionary<string, object>> { reverseInputData };
        }

        public void Join(ActionResult actionResult)
        {
            if ((int)actionResult.Type > (int)Type)
                Type = actionResult.Type;

            OutputData.AddRange(actionResult.OutputData);
            ReverseInputData.AddRange(actionResult.ReverseInputData);
        }
    }

    public enum ActionResultType
    {
        Success,
        Warning,
        Error
    }
}

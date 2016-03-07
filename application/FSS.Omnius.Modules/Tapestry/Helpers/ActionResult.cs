using FSS.Omnius.Modules.CORE;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace FSS.Omnius.Modules.Tapestry
{
    public class ActionResult
    {
        public Message Message { get; set; }
        public ActionResultType Type { get; set; }
        public Dictionary<string, object> OutputData { get; set; }
        public List<Dictionary<string, object>> ReverseInputData { get; set; }

        public ActionResult()
        {
            Message = new Message();
            Type = ActionResultType.Success;
            OutputData = new Dictionary<string, object>();
            ReverseInputData = new List<Dictionary<string, object>>();
        }
        public ActionResult(ActionResultType type, Dictionary<string, object> outputData, Dictionary<string, object> reverseInputData, Message message)
        {
            Type = type;
            Message = message;

            OutputData = outputData;
            ReverseInputData = new List<Dictionary<string, object>> { reverseInputData };
        }

        public void Join(ActionResult actionResult)
        {
            if ((int)actionResult.Type > (int)Type)
                Type = actionResult.Type;

            Message.Join(actionResult.Message);

            OutputData.AddRange(actionResult.OutputData);
            ReverseInputData.AddRange(actionResult.ReverseInputData);
        }
    }

    public enum ActionResultType
    {
        Success,
        Info,
        Warning,
        Error
    }
}

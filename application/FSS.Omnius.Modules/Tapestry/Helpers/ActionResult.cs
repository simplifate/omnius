using FSS.Omnius.Modules.CORE;
using System;
using System.Collections.Generic;
using System.Linq;

namespace FSS.Omnius.Modules.Tapestry
{
    public class ActionResult
    {
        public Message Message { get; set; }
        public MessageType Type { get; set; }
        public Dictionary<string, object> OutputData { get; set; }
        public List<Dictionary<string, object>> ReverseInputData { get; set; }

        public ActionResult()
        {
            Message = new Message(COREobject.i);
            Type = MessageType.Success;
            OutputData = new Dictionary<string, object>();
            ReverseInputData = new List<Dictionary<string, object>>();
        }
        public ActionResult(MessageType type, Dictionary<string, object> outputData, Dictionary<string, object> reverseInputData, Message message)
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

            OutputData.AddOrUpdateRange(actionResult.OutputData);
            ReverseInputData.AddRange(actionResult.ReverseInputData);
        }

        public Dictionary<string, object> GetLastReverseData()
        {
            Dictionary<string, object> result = new Dictionary<string, object>();
            result.AddOrUpdateRange(OutputData);
            if (ReverseInputData.Count > 0)
            {
                result.AddOrUpdateRange(ReverseInputData.Last());
                ReverseInputData.RemoveAt(ReverseInputData.Count - 1);
            }

            return result;
        }
    }
}

using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace FSS.Omnius.Modules.Tapestry
{
    public class ActionResultCollection
    {
        public int Count { get; set; }
        public List<ActionResultType> types { get; }
        public List<string> Messages { get; }
        public List<Dictionary<string,object>> ReverseInputData { get; } 
        public Dictionary<string, object> outputData { get; }
        
        public ActionResultCollection()
        {
            Count = 0;
            types = new List<ActionResultType>();
            Messages = new List<string>();
            ReverseInputData = new List<Dictionary<string, object>>();
            outputData = new Dictionary<string, object>();
        }
        public ActionResultCollection(ActionResultType type, string Message, Dictionary<string, object> ReverseInputData, Dictionary<string, object> outputData)
        {
            Count = 1;
            types = new List<ActionResultType> { type };
            Messages = new List<string> { Message };
            this.ReverseInputData = new List<Dictionary<string, object>> {ReverseInputData};
            this.outputData = outputData;
        }

        public void Add(ActionResult newActionResult)
        {
            Add(newActionResult.type, newActionResult.Message,newActionResult.ReverseInputData ,newActionResult.outputData);
        }
        public void Add(ActionResultType type, string Message, Dictionary<string,object> ReverseInputData,  Dictionary<string, object> outputData)
        {
            types.Add(type);
            Messages.Add(Message);
            this.ReverseInputData.Add(ReverseInputData);
            this.outputData.AddOrUpdateRange(outputData);
            Count++;
        }

        public ActionResultCollection Join
        {
            set
            {
                for (int i = 0; i < value.Count; i++)
                {
                    Add(value.GetActionResult(i));
                }
                outputData.AddOrUpdateRange(value.outputData);
            }
        }

        public ActionResult GetActionResult(int id)
        {
            if (id >= Count)
                throw new IndexOutOfRangeException();

            return new ActionResult
            {
                type = types[id],
                Message = Messages[id],
                ReverseInputData = ReverseInputData[id]
            };
        }
    }
}

using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace FSS.Omnius.Entitron.Entity.Tapestry
{
    public partial class ActionRule_Action
    {
        public Dictionary<string, object> getInputVariables(Dictionary<string, object> tempVars)
        {
            Dictionary<string, object> output = new Dictionary<string, object>();
            var data = InputVariablesMapping.Split(';').Select(s => s.Split('='));

            foreach (string[] mappingPair in data)
            {
                string target = mappingPair[0];
                string source = mappingPair[1];

                output.Add(target, tempVars[source]);
            }

            return output;
        }
        public Dictionary<string, object> getOutputVariables(Dictionary<string, object> actionVars)
        {
            Dictionary<string, object> output = new Dictionary<string, object>();
            var data = OutputVariablesMapping.Split(';').Select(s => s.Split('='));

            foreach (string[] mappingPair in data)
            {
                string target = mappingPair[0];
                string source = mappingPair[1];

                output.Add(target, actionVars[source]);
            }

            return output;
        }
    }
}

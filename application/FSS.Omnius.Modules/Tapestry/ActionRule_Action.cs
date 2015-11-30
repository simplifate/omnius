using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace FSS.Omnius.Modules.Entitron.Entity.Tapestry
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
                
                // source is value
                if (source[1] == '$')
                {
                    switch (source[0])
                    {
                        case 's':
                            output.Add(target, source.Substring(2));
                            break;
                        case 'b':
                            output.Add(target, Convert.ToBoolean(source.Substring(2)));
                            break;
                        case 'i':
                            output.Add(target, Convert.ToInt32(source.Substring(2)));
                            break;
                        case 'd':
                            output.Add(target, Convert.ToDouble(source.Substring(2)));
                            break;
                        default:
                            throw new ArgumentException("unknown type");
                    }
                }
                // source is link
                else {
                    // can throw error if tempVars doesn't contain source string
                    output.Add(target, tempVars[source]);
                }
            }

            return output;
        }
        public void RemapOutputVariables(Dictionary<string, object> actionVars)
        {
            var data = OutputVariablesMapping.Split(';').Select(s => s.Split('='));

            foreach (string[] mappingPair in data)
            {
                actionVars.ChangeKey(mappingPair[1], mappingPair[0]);
            }
        }
    }
}

using System.Collections.Generic;
using FSS.Omnius.Modules.CORE;

namespace FSS.Omnius.Modules.Tapestry.Actions.Entitron
{
    [EntitronRepository]
    public class FormatStringAction : Action
    {
        public override int Id => 183;

        public override int? ReverseActionId => null;

        public override string[] InputVar => new string[] { "Input", "?var0", "?var1", "?var2", "?var3", "?var4", "?var5", "?var6", "?var7", "?var8", "?var9" };

        public override string Name => "Format string";

        public override string[] OutputVar => new string[] { "Result" };

        public override void InnerRun(Dictionary<string, object> vars, Dictionary<string, object> outputVars, Dictionary<string, object> InvertedInputVars, Message message)
        {
            List<string> replace = new List<string>();
            var stringInput = ((string)vars["Input"]).Replace("{", "{{").Replace("}", "}}");
            for (int i = 0; i < 10; i++)
            {
                string key = "var" + i.ToString();
                if (vars.ContainsKey(key))
                {
                    replace.Add(vars[key].ToString());
                }
                stringInput = stringInput.Replace("{{" + i + "}}","{" +i+"}");

            }
            outputVars["Result"] = string.Format(stringInput, replace.ToArray());
        }
    }
}

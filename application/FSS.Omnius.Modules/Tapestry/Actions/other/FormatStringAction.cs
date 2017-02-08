using FSS.Omnius.Modules.CORE;
using FSS.Omnius.Modules.Entitron;
using FSS.Omnius.Modules.Entitron.Sql;
using FSS.Omnius.Modules.Watchtower;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace FSS.Omnius.Modules.Tapestry.Actions.Entitron
{
    [EntitronRepository]
    public class FormatStringAction : Action
    {
        public override int Id
        {
            get
            {
                return 183;
            }
        }
        public override int? ReverseActionId
        {
            get
            {
                return null;
            }
        }
        public override string[] InputVar
        {
            get
            {
                return new string[] { "Input", "?var0", "?var1", "?var2", "?var3", "?var4", "?var5", "?var6", "?var7", "?var8", "?var9" };
            }
        }

        public override string Name
        {
            get
            {
                return "Format string";
            }
        }

        public override string[] OutputVar
        {
            get
            {
                return new string[] { "Result" };
            }
        }

        public override void InnerRun(Dictionary<string, object> vars, Dictionary<string, object> outputVars, Dictionary<string, object> InvertedInputVars, Message message)
        {
            List<string> replace = new List<string>();
            for (int i = 0; i < 10; i++)
            {
                string key = "var" + i.ToString();
                if (vars.ContainsKey(key))
                {
                    replace.Add(vars[key].ToString());
                }
            }

            outputVars["Result"] = string.Format((string)vars["Input"], replace.ToArray());
        }
    }
}

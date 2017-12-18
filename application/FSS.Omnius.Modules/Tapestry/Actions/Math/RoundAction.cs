using FSS.Omnius.Modules.CORE;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
namespace FSS.Omnius.Modules.Tapestry.Actions.other
{
    [OtherRepository]
    class MathRound : Action
    {
        public override int Id
        {
            get
            {
                return 4015;
            }
        }
        public override string[] InputVar
        {
            get
            {
                return new string[] { "Value", "Precision" };
            }
        }
        public override string Name
        {
            get
            {
                return "Math: Round";
            }
        }
        public override string[] OutputVar
        {
            get
            {
                return new string[] { "Result" };
            }
        }
        public override int? ReverseActionId
        {
            get
            {
                return null;
            }
        }
        public override void InnerRun(Dictionary<string, object> vars, Dictionary<string, object> outputVars, Dictionary<string, object> InvertedInputVars, Message message)
        {
            String zaokrouhleni = vars["Precision"].ToString();
            int x = Convert.ToInt32(zaokrouhleni);
            double y = 0;
            try
            {
                y = Convert.ToDouble(vars["Value"]);
            }
            catch (Exception) { }
            double zaokrouhleno = Math.Round(y, x);
            outputVars["Result"] = zaokrouhleno;
        }
    }
}

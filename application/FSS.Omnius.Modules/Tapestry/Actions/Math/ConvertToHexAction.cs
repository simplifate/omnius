using System;
using System.Collections.Generic;
using System.Globalization;
using System.Runtime.InteropServices;
using FSS.Omnius.Modules.CORE;
using FSS.Omnius.Modules.Tapestry.Actions.Entitron;

namespace FSS.Omnius.Modules.Tapestry.Actions.Math
{
    /// <summary>
    /// Prijme ciselnou hodnotu a vrati string hodnotu v hexadecimalni podobe.
    /// </summary>
    [MathRepository]
    class ConvertToHexAction : Action
    {
        public override int Id
        {
            get
            {
                return 4016;
            }
        }

        public override string[] InputVar
        {
            get
            {
                return new string[] { "Number", "MaxDecimals" };
            }
        }

        public override string Name
        {
            get
            {
                return "Math: Convert to HEX";
            }
        }

        public override string[] OutputVar
        {
            get
            {
                return new string[]
                {
                    "Result"
                };
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
            var number = vars["Number"];
            var maxDecimals = vars["MaxDecimals"];

            outputVars["Result"] = TapestryUtils.DoubleToHex(Convert.ToDouble(number), (int)maxDecimals);
        }
    }
}
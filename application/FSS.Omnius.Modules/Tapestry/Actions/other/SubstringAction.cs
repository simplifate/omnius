﻿using FSS.Omnius.Modules.CORE;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace FSS.Omnius.Modules.Tapestry.Actions.other
{
    [OtherRepository]
    class SubstringAction : Action
    {
        public override int Id
        {
            get
            {
                return 202;
            }
        }

        public override string[] InputVar
        {
            get
            {
                return new string[] { "InputString", "Index", "Length" };
            }
        }

        public override string Name
        {
            get
            {
                return "Substring";
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
            string input = (string)vars["InputString"];
            int index = (int)vars["Index"];
            int length = (int)vars["Length"];

            outputVars["Result"] = input.Substring(index, length);
        }
    }
}

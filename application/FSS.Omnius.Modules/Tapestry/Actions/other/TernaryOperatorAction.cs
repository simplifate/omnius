using FSS.Omnius.Modules.CORE;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace FSS.Omnius.Modules.Tapestry.Actions.other
{
    [OtherRepository]
    class TernaryOperatorAction : Action
    {
        public override int Id
        {
            get
            {
                return 201;
            }
        }

        public override string[] InputVar
        {
            get
            {
                return new string[] { "Input", "?CompareWith", "?ReturnWhenTrue", "?ReturnWhenFalse", "?s$Operator[eq|ne|lt|gt|gte|lte]" };
            }
        }

        public override string Name
        {
            get
            {
                return "Ternary operator";
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

        private object returnWhenTrue;
        private object returnWhenFalse;
        private object input;
        private object compareWith;
        private string compareOperator;

        public override void InnerRun(Dictionary<string, object> vars, Dictionary<string, object> outputVars, Dictionary<string, object> InvertedInputVars, Message message)
        {
            input = vars["Input"];
            compareWith = vars.ContainsKey("CompareWith") ? vars["CompareWith"] : null;
            compareOperator = vars.ContainsKey("Operator") ? (string)vars["Operator"] : "eq";
            returnWhenTrue = vars.ContainsKey("ReturnWhenTrue") ? vars["ReturnWhenTrue"] : true;
            returnWhenFalse = vars.ContainsKey("ReturnWhenFalse") ? vars["ReturnWhenFalse"] : false;

            if(input is string) {
                outputVars["Result"] = Result(TestString());
            }
            else if(input is int) {
                outputVars["Result"] = Result(TestInt());
            }
            else if(input is double) {
                outputVars["Result"] = Result(TestDouble());
            }
            else if(input is bool) {
                outputVars["Result"] = Result(TestBool());
            }
        }

        private bool TestString() {
            return compareWith != null ? Compare() : !string.IsNullOrEmpty((string)input);
        }

        private bool TestInt() {
            return compareWith != null ? Compare() : (int)input != 0;
        }

        private bool TestDouble() {
            return compareWith != null ? Compare() : (double)input != 0;
        }

        private bool TestBool() {
            return compareWith != null ? Compare() : (bool)input == true;
        }

        private bool Compare()
        {
            double nI;
            double nC;
            bool inputIsNumeric = double.TryParse(input.ToString(), out nI);
            bool compareIsNumeric = double.TryParse(compareWith.ToString(), out nC);

            switch(compareOperator) {
                default:
                case "eq": return input == compareWith;
                case "ne": return input != compareWith;
                case "lt": return inputIsNumeric && compareIsNumeric ? nI < nC : false;
                case "gt": return inputIsNumeric && compareIsNumeric ? nI > nC : false;
                case "lte": return inputIsNumeric && compareIsNumeric ? nI <= nC : false;
                case "gte": return inputIsNumeric && compareIsNumeric ? nI >= nC : false;
            }
        }

        private object Result(bool result)
        {
            return result ? returnWhenTrue : returnWhenFalse;
        }
    }
}

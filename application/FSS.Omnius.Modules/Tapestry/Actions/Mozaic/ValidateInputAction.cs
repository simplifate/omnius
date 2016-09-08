using FSS.Omnius.Modules.CORE;
using FSS.Omnius.Modules.Watchtower;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace FSS.Omnius.Modules.Tapestry.Actions.Mozaic
{
    [MozaicRepository]
    public class ValidateInputAction : Action
    {
        public override int Id
        {
            get
            {
                return 2006;
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
                return new string[] { "InputName", "Condition", "?InputAlias", "?SuppressMessage" };
            }
        }

        public override string Name
        {
            get
            {
                return "Validate input";
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
            string inputName = (string)vars["InputName"];
            string condition = vars.ContainsKey("Condition") ? (string)vars["Condition"] : "NotEmpty";
            bool suppressMessage = vars.ContainsKey("SuppressMessage") ? (bool)vars["SuppressMessage"] : false;
            string inputAlias = vars.ContainsKey("InputAlias") ? (string)vars["InputAlias"] : inputName;
            string inputValue = vars[inputName].ToString();
            bool isValid = false;
            switch(condition)
            {
                case "Integer":
                    int tempInt;
                    isValid = int.TryParse(inputValue, out tempInt);
                    if (!isValid && !suppressMessage)
                        message.Warnings.Add($"Validace: Kolonka {inputAlias} musí obsahovat celé číslo");
                    break;
                case "Float":
                    float tempFloat;
                    isValid = float.TryParse(inputValue, out tempFloat);
                    if (!isValid && !suppressMessage)
                        message.Warnings.Add($"Validace: Kolonka {inputAlias} musí obsahovat číslo");
                    break;
                case "NotEmpty":
                default:
                    isValid = inputValue.Length > 0;
                    if (!isValid && !suppressMessage)
                        message.Warnings.Add($"Validace: Kolonka {inputAlias} je povinná, vyplňte ji prosím");
                    break;
            }
            outputVars["Result"] = isValid;
        }
    }
}

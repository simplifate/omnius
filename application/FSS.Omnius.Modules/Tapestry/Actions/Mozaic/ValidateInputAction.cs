using System.Collections.Generic;
using FSS.Omnius.Modules.CORE;

namespace FSS.Omnius.Modules.Tapestry.Actions.Mozaic
{
    [MozaicRepository]
    public class ValidateInputAction : Action
    {
        public override int Id => 2006;

        public override int? ReverseActionId => null;

        public override string[] InputVar => new string[] { "InputName", "Condition", "?InputAlias", "?SuppressMessage" };

        public override string Name => "Validate input";

        public override string[] OutputVar => new string[] { "Result" };

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

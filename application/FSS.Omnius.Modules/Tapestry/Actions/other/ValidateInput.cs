using System;
using System.Collections.Generic;
using System.Globalization;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using FSS.Omnius.Modules.CORE;
using FSS.Omnius.Modules.Cortex;

namespace FSS.Omnius.Modules.Tapestry.Actions.other
{
    [OtherRepository]
    public class ValidateInput : Action
    {
        public override int Id
        {
            get {
                return 200;
            }
        }
        public override int? ReverseActionId
        {
            get {
                return null;
            }
        }
        public override string[] InputVar
        {
            get {
                return new string[] { "s$FieldName", "s$HumanName", "s$Type[string|int|float|bool|date|time|dateTime]", "b$Required", "?Min", "?Max", "b$IsValid", "?s$Messages" };
            }
        }
        public override string Name
        {
            get {
                return "Validate input";
            }
        }
        public override string[] OutputVar
        {
            get {
                return new string[] { "Result", "Messages" };
            }
        }

        private static Dictionary<string, string> m = new Dictionary<string, string>() {
            {"req", "{0} je povinná položka. "},
            {"gt", "{0} musí být větší nebo rovno {1}. "},
            {"lt", "{0} musí být menší nebo rovno {1}. "},
            {"int", "{0} musí být celé číslo. "},
            {"float", "{0} musí být celé nebo desetinné číslo. "},
            {"bool", "{0} musí být platná logická hodnota. "},
            {"date", "{0} musí být platné datum. "},
            {"time", "{0} musí být platný čas. "},
            {"dateTime", "{0} musí být platné datum a čas. "}
        };

        private string[] dateFormats = new string[] {
            "d.M.yyyy",
            "d. M. yyyy",
            "dd.MM.yyyy",
            "dd. MM. yyyy",
            "M/d/yyyy",
            "MM/dd/yyyy"
        };
        private string[] timeFormats = new string[] { "h:mm:ss", "hh:mm:ss", "h:mm", "hh:mm" };
        private string[] dateTimeFormats = new string[] {
            "d.M.yyyy",
            "d.M.yyyy h:mm:ss",
            "d.M.yyyy hh:mm:ss",
            "d.M.yyyy h:mm",
            "d.M.yyyy hh:mm",
            "d. M. yyyy",
            "d. M. yyyy h:mm:ss",
            "d. M. yyyy hh:mm:ss",
            "d. M. yyyy h:mm",
            "d. M. yyyy hh:mm",
            "dd.MM.yyyy",
            "dd.MM.yyyy h:mm:ss",
            "dd.MM.yyyy hh:mm:ss",
            "dd.MM.yyyy h:mm",
            "dd.MM.yyyy hh:mm",
            "dd. MM. yyyy",
            "dd. MM. yyyy h:mm:ss",
            "dd. MM. yyyy hh:mm:ss",
            "dd. MM. yyyy h:mm",
            "dd. MM. yyyy hh:mm",
            "M/d/yyyy",
            "M/d/yyyy h:mm:ss",
            "M/d/yyyy hh:mm:ss",
            "M/d/yyyy h:mm",
            "M/d/yyyy hh:mm",
            "MM/dd/yyyy",
            "MM/dd/yyyy h:mm:ss",
            "MM/dd/yyyy hh:mm:ss",
            "MM/dd/yyyy h:mm",
            "MM/dd/yyyy hh:mm"
        };

        private string fieldName;
        private string humanName;
        private string messages;
        private bool required;
        private bool isValid;

        private Dictionary<string, object> vars;

        public override void InnerRun(Dictionary<string, object> vars, Dictionary<string, object> outputVars, Dictionary<string, object> InvertedInputVars, Message message)
        {
            fieldName = (string)vars["FieldName"];
            humanName = (string)vars["HumanName"];
            messages = vars.ContainsKey("Messages") ? (string)vars["Messages"] : "";
            required = (bool)vars["Required"];
            isValid = (bool)vars["IsValid"];
            this.vars = vars;

            string type = (string)vars["Type"];
            switch(type) {
                case "string": isValid &= ValidateString(); break;
                case "int": isValid &= ValidateInt(); break;
                case "float": isValid &= ValidateFloat(); break;
                case "bool": isValid &= ValidateBool(); break;
                case "date": isValid &= ValidateDate(); break;
                case "time": isValid &= ValidateTime(); break;
                case "dateTime": isValid &= ValidateDateTime(); break;
            }

            outputVars["Result"] = isValid;
            outputVars["Messages"] = messages;
        }

        private bool ValidateString()
        {
            bool isValid = true;
            string value = vars.ContainsKey(fieldName) ? (string)vars[fieldName] : null;

            if(required && string.IsNullOrEmpty(value)) {
                isValid = false;
                messages += string.Format(m["req"], humanName);
            }
            return isValid;
        }

        private bool ValidateInt()
        {
            bool isValid = true;
            int? value;
            try {
                value = vars.ContainsKey(fieldName) ? (int?)Convert.ToInt32(vars[fieldName]) : null;
                if(required && !value.HasValue) {
                    isValid = false;
                    messages += string.Format(m["req"], humanName);
                }

                if (value.HasValue) {
                    int? min = vars.ContainsKey("Min") ? (int?)vars["Min"] : null;
                    int? max = vars.ContainsKey("Max") ? (int?)vars["Max"] : null;

                    if(min.HasValue && value < min) {
                        isValid = false;
                        messages += string.Format(m["gt"], humanName, min);
                    }
                    if(max.HasValue && value > max) {
                        isValid = false;
                        messages += string.Format(m["lt"], humanName, max);
                    }
                }
            }
            catch(Exception) {
                isValid = false;
                messages += string.Format(m["int"], humanName);
            }

            return isValid;
        }

        private bool ValidateFloat()
        {
            bool isValid = true;
            double? value;
            try {
                value = vars.ContainsKey(fieldName) ? (double?)Convert.ToDouble(vars[fieldName]) : null;
                if (required && !value.HasValue) {
                    isValid = false;
                    messages += string.Format(m["req"], humanName);
                }

                if (value.HasValue) {
                    double? min = vars.ContainsKey("Min") ? (double?)vars["Min"] : null;
                    double? max = vars.ContainsKey("Max") ? (double?)vars["Max"] : null;

                    if (min.HasValue && value < min) {
                        isValid = false;
                        messages += string.Format(m["gt"], humanName, min);
                    }
                    if (max.HasValue && value > max) {
                        isValid = false;
                        messages += string.Format(m["lt"], humanName, max);
                    }
                }
            }
            catch (Exception) {
                isValid = false;
                messages += string.Format(m["float"], humanName);
            }
            return isValid;
        }

        private bool ValidateBool()
        {
            bool isValid = true;
            bool? value;
            try {
                value = vars.ContainsKey(fieldName) ? (bool?)Convert.ToBoolean(vars[fieldName]) : null;
                if(required && !value.HasValue) {
                    isValid = false;
                    messages += string.Format(m["req"], humanName);
                }
            }
            catch(Exception) {
                isValid = false;
                messages += string.Format(m["bool"], humanName);
            }

            return isValid;
        }

        private bool ValidateDate()
        {
            bool isValid = true;
            DateTime value;

            try {
                if (vars.ContainsKey(fieldName)) {
                    if (!DateTime.TryParseExact((string)vars[fieldName], dateFormats, CultureInfo.InvariantCulture, DateTimeStyles.None, out value)) {
                        isValid = false;
                        messages += string.Format(m["date"], humanName);
                    }
                }
                else {
                    value = DateTime.MinValue;
                }

                if(required && value == DateTime.MinValue && isValid) {
                    isValid = false;
                    messages += string.Format(m["req"], humanName);
                }

                if(value > DateTime.MinValue) {
                    DateTime min = DateTime.MinValue;
                    DateTime max = DateTime.MinValue;
                    
                    if(vars.ContainsKey("Min")) {
                        if(vars["Min"] is DateTime) {
                            min = (DateTime)vars["Min"];
                        }
                        else {
                            DateTime.TryParseExact((string)vars["Min"], dateFormats, CultureInfo.InvariantCulture, DateTimeStyles.None, out min);
                        }
                    }
                    if (vars.ContainsKey("Max")) {
                        if (vars["Max"] is DateTime) {
                            max = (DateTime)vars["Max"];
                        }
                        else {
                            DateTime.TryParseExact((string)vars["Max"], dateFormats, CultureInfo.InvariantCulture, DateTimeStyles.None, out max);
                        }
                    }

                    if(min > DateTime.MinValue && value < min) {
                        isValid = false;
                        messages += string.Format(m["gt"], humanName, min.ToShortDateString());
                    }
                    if(max > DateTime.MinValue && value > max) {
                        isValid = false;
                        messages += string.Format(m["lt"], humanName, max.ToShortDateString());
                    }
                }
            }
            catch(Exception) {
                isValid = false;
                messages += string.Format(m["date"], humanName);
            }

            return isValid;
        }
        
        private bool ValidateTime()
        {
            bool isValid = true;
            TimeSpan value;
            
            try {
                if (vars.ContainsKey(fieldName)) {
                    if (!TimeSpan.TryParseExact((string)vars[fieldName], timeFormats, CultureInfo.InvariantCulture, TimeSpanStyles.None, out value)) {
                        isValid = false;
                        messages += string.Format(m["time"], humanName);
                    }
                }
                else {
                    value = TimeSpan.Zero;
                }

                if (required && value == TimeSpan.Zero && isValid) {
                    isValid = false;
                    messages += string.Format(m["req"], humanName);
                }

                if (value > TimeSpan.Zero) {
                    TimeSpan min = TimeSpan.Zero;
                    TimeSpan max = TimeSpan.Zero;

                    if (vars.ContainsKey("Min")) {
                        if (vars["Min"] is TimeSpan || vars["Min"] is DateTime) {
                            min = vars["Min"] is DateTime ? ((DateTime)vars["Min"]).TimeOfDay : (TimeSpan)vars["Min"];
                        }
                        else {
                            TimeSpan.TryParseExact((string)vars["Min"], timeFormats, CultureInfo.InvariantCulture, TimeSpanStyles.None, out min);
                        }
                    }
                    if (vars.ContainsKey("Max")) {
                        if (vars["Max"] is TimeSpan || vars["Max"] is DateTime) {
                            max = vars["Max"] is DateTime ? ((DateTime)vars["Max"]).TimeOfDay : (TimeSpan)vars["Max"];
                        }
                        else {
                            TimeSpan.TryParseExact((string)vars["Max"], timeFormats, CultureInfo.InvariantCulture, TimeSpanStyles.None, out max);
                        }
                    }

                    if (min > TimeSpan.Zero && value < min) {
                        isValid = false;
                        messages += string.Format(m["gt"], humanName, min.ToString("hh:mm:ss"));
                    }
                    if (max > TimeSpan.Zero && value > max) {
                        isValid = false;
                        messages += string.Format(m["lt"], humanName, max.ToString("hh:mm:ss"));
                    }
                }
            }
            catch (Exception) {
                isValid = false;
                messages += string.Format(m["time"], humanName);
            }
            return isValid;
        }

        private bool ValidateDateTime()
        {
            bool isValid = true;
            DateTime value;

            try {
                if (vars.ContainsKey(fieldName)) {
                    if (!DateTime.TryParseExact((string)vars[fieldName], dateTimeFormats, CultureInfo.InvariantCulture, DateTimeStyles.None, out value)) {
                        isValid = false;
                        messages += string.Format(m["date"], humanName);
                    }
                }
                else {
                    value = DateTime.MinValue;
                }

                if (required && value == DateTime.MinValue && isValid) {
                    isValid = false;
                    messages += string.Format(m["req"], humanName);
                }

                if (value > DateTime.MinValue) {
                    DateTime min = DateTime.MinValue;
                    DateTime max = DateTime.MinValue;

                    if (vars.ContainsKey("Min")) {
                        if (vars["Min"] is DateTime) {
                            min = (DateTime)vars["Min"];
                        }
                        else {
                            DateTime.TryParseExact((string)vars["Min"], dateTimeFormats, CultureInfo.InvariantCulture, DateTimeStyles.None, out min);
                        }
                    }
                    if (vars.ContainsKey("Max")) {
                        if (vars["Max"] is DateTime) {
                            max = (DateTime)vars["Max"];
                        }
                        else {
                            DateTime.TryParseExact((string)vars["Max"], dateTimeFormats, CultureInfo.InvariantCulture, DateTimeStyles.None, out max);
                        }
                    }

                    if (min > DateTime.MinValue && value < min) {
                        isValid = false;
                        messages += string.Format(m["gt"], humanName, min.ToShortDateString() + " " + min.ToShortTimeString());
                    }
                    if (max > DateTime.MinValue && value > max) {
                        isValid = false;
                        messages += string.Format(m["lt"], humanName, max.ToShortDateString() + " " + min.ToShortTimeString());
                    }
                }
            }
            catch (Exception) {
                isValid = false;
                messages += string.Format(m["date"], humanName);
            }
            return isValid;
        }
    }
}

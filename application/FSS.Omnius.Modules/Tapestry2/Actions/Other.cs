using FSS.Omnius.Modules.CORE;
using FSS.Omnius.Modules.Entitron.DB;
using FSS.Omnius.Modules.Tapestry;
using Newtonsoft.Json.Linq;
using System;
using System.Collections.Generic;
using System.Globalization;
using System.IO;
using System.Linq;
using System.Net;
using System.Text;
using System.Text.RegularExpressions;
using System.Threading;
using System.Web;
using System.Web.Configuration;

namespace FSS.Omnius.Modules.Tapestry2.Actions
{
    public class Other : ActionManager
    {
        [Action(179, "Default value", "Result")]
        public static T DefaultValue<T>(COREobject core, T Variable, T Value)
        {
            if (Variable == null)
                return Value;

            return Variable;
        }

        [Action(181, "No Action")]
        public static void NoAction(COREobject core, int? waitFor = null)
        {
            if (waitFor != null)
            {
                Thread.Sleep(waitFor.Value);
            }
        }

        [Action(183, "Format string", "Result")]
        public static string FormatString(COREobject core, string Input, string[] var)
        {
            Input = Input.Replace("{", "{{").Replace("}", "}}");
            Input = Regex.Replace(Input, "{{(\\d+)}}", "{$1}");
            return string.Format(Input, var);
        }

        [Action(184, "Pass variable", "Result")]
        public static Dictionary<string, object> PassVariable(COREobject core, string Key, object Value)
        {
            // if Value is empty => remove from CrossBlockRegistry
            if (core.CrossBlockRegistry.ContainsKey(Key) &&
                (Value == null
                    || (Value is string
                        && string.IsNullOrEmpty((string)Value))))
                core.CrossBlockRegistry.Remove(Key);
            else
                core.CrossBlockRegistry[Key] = Value;

            return core.CrossBlockRegistry;
        }

        [Action(185, "Copy variable", "Result")]
        public static T CopyVariable<T>(COREobject core, T From)
        {
            if (From is string)
                return (T)(object)(From as string).Replace("\\n", "\n");
            else
                return From;
        }

        [Action(191, "Gather emails to single string", "Result")]
        public static string GatherStrings(COREobject core, List<DBItem> TableData, string ColumnName, string StringSeperator = ",")
        {
            if (TableData.Count == 0)
                return "";

            List<string> listStrings = new List<string>();
            foreach (var row in TableData)
            {
                listStrings.Add((string)row[ColumnName]);
            }

            // gather strings and divide them by "," to single string output
            return string.Join(StringSeperator, listStrings);
        }

        [Action(203, "Validate input", "Result", "Messages")]
        public static (bool, string) ValidateInput(COREobject core, string FieldName, string HumanName, bool Required, string Type, bool IsValid = true, string Messages = "")
        {
            switch (Type)
            {
                case "string": IsValid &= ValidateString(core.Data, FieldName, HumanName, Required, Messages); break;
                case "int": IsValid &= ValidateInt(core.Data, FieldName, HumanName, Required, Messages); break;
                case "float": IsValid &= ValidateFloat(core.Data, FieldName, HumanName, Required, Messages); break;
                case "bool": IsValid &= ValidateBool(core.Data, FieldName, HumanName, Required, Messages); break;
                case "date": IsValid &= ValidateDate(core.Data, FieldName, HumanName, Required, Messages); break;
                case "time": IsValid &= ValidateTime(core.Data, FieldName, HumanName, Required, Messages); break;
                case "dateTime": IsValid &= ValidateDateTime(core.Data, FieldName, HumanName, Required, Messages); break;
            }

            return (IsValid, Messages);
        }
        private static bool ValidateString(Dictionary<string, object> vars, string fieldName, string humanName, bool required, string messages)
        {
            bool isValid = true;
            string value = vars.ContainsKey(fieldName) ? (string)vars[fieldName] : null;

            if (required && string.IsNullOrEmpty(value))
            {
                isValid = false;
                messages += string.Format(m["req"], humanName);
            }
            return isValid;
        }
        private static bool ValidateInt(Dictionary<string, object> vars, string fieldName, string humanName, bool required, string messages)
        {
            bool isValid = true;
            int? value;
            try
            {
                value = vars.ContainsKey(fieldName) ? (int?)Convert.ToInt32(vars[fieldName]) : null;
                if (required && !value.HasValue)
                {
                    isValid = false;
                    messages += string.Format(m["req"], humanName);
                }

                if (value.HasValue)
                {
                    int? min = vars.ContainsKey("Min") ? (int?)vars["Min"] : null;
                    int? max = vars.ContainsKey("Max") ? (int?)vars["Max"] : null;

                    if (min.HasValue && value < min)
                    {
                        isValid = false;
                        messages += string.Format(m["gt"], humanName, min);
                    }
                    if (max.HasValue && value > max)
                    {
                        isValid = false;
                        messages += string.Format(m["lt"], humanName, max);
                    }
                }
            }
            catch (Exception)
            {
                isValid = false;
                messages += string.Format(m["int"], humanName);
            }

            return isValid;
        }
        private static bool ValidateFloat(Dictionary<string, object> vars, string fieldName, string humanName, bool required, string messages)
        {
            bool isValid = true;
            double? value;
            try
            {
                value = vars.ContainsKey(fieldName) ? (double?)Convert.ToDouble(vars[fieldName]) : null;
                if (required && !value.HasValue)
                {
                    isValid = false;
                    messages += string.Format(m["req"], humanName);
                }

                if (value.HasValue)
                {
                    double? min = vars.ContainsKey("Min") ? (double?)vars["Min"] : null;
                    double? max = vars.ContainsKey("Max") ? (double?)vars["Max"] : null;

                    if (min.HasValue && value < min)
                    {
                        isValid = false;
                        messages += string.Format(m["gt"], humanName, min);
                    }
                    if (max.HasValue && value > max)
                    {
                        isValid = false;
                        messages += string.Format(m["lt"], humanName, max);
                    }
                }
            }
            catch (Exception)
            {
                isValid = false;
                messages += string.Format(m["float"], humanName);
            }
            return isValid;
        }
        private static bool ValidateBool(Dictionary<string, object> vars, string fieldName, string humanName, bool required, string messages)
        {
            bool isValid = true;
            bool? value;
            try
            {
                value = vars.ContainsKey(fieldName) ? (bool?)Convert.ToBoolean(vars[fieldName]) : null;
                if (required && !value.HasValue)
                {
                    isValid = false;
                    messages += string.Format(m["req"], humanName);
                }
            }
            catch (Exception)
            {
                isValid = false;
                messages += string.Format(m["bool"], humanName);
            }

            return isValid;
        }
        private static bool ValidateDate(Dictionary<string, object> vars, string fieldName, string humanName, bool required, string messages)
        {
            bool isValid = true;
            DateTime value;

            try
            {
                if (vars.ContainsKey(fieldName))
                {
                    if (!DateTime.TryParseExact((string)vars[fieldName], dateFormats, CultureInfo.InvariantCulture, DateTimeStyles.None, out value))
                    {
                        isValid = false;
                        messages += string.Format(m["date"], humanName);
                    }
                }
                else
                {
                    value = DateTime.MinValue;
                }

                if (required && value == DateTime.MinValue && isValid)
                {
                    isValid = false;
                    messages += string.Format(m["req"], humanName);
                }

                if (value > DateTime.MinValue)
                {
                    DateTime min = DateTime.MinValue;
                    DateTime max = DateTime.MinValue;

                    if (vars.ContainsKey("Min"))
                    {
                        if (vars["Min"] is DateTime)
                        {
                            min = (DateTime)vars["Min"];
                        }
                        else
                        {
                            DateTime.TryParseExact((string)vars["Min"], dateFormats, CultureInfo.InvariantCulture, DateTimeStyles.None, out min);
                        }
                    }
                    if (vars.ContainsKey("Max"))
                    {
                        if (vars["Max"] is DateTime)
                        {
                            max = (DateTime)vars["Max"];
                        }
                        else
                        {
                            DateTime.TryParseExact((string)vars["Max"], dateFormats, CultureInfo.InvariantCulture, DateTimeStyles.None, out max);
                        }
                    }

                    if (min > DateTime.MinValue && value < min)
                    {
                        isValid = false;
                        messages += string.Format(m["gt"], humanName, min.ToShortDateString());
                    }
                    if (max > DateTime.MinValue && value > max)
                    {
                        isValid = false;
                        messages += string.Format(m["lt"], humanName, max.ToShortDateString());
                    }
                }
            }
            catch (Exception)
            {
                isValid = false;
                messages += string.Format(m["date"], humanName);
            }

            return isValid;
        }
        private static bool ValidateTime(Dictionary<string, object> vars, string fieldName, string humanName, bool required, string messages)
        {
            bool isValid = true;
            TimeSpan value;

            try
            {
                if (vars.ContainsKey(fieldName))
                {
                    if (!TimeSpan.TryParseExact((string)vars[fieldName], timeFormats, CultureInfo.InvariantCulture, TimeSpanStyles.None, out value))
                    {
                        isValid = false;
                        messages += string.Format(m["time"], humanName);
                    }
                }
                else
                {
                    value = TimeSpan.Zero;
                }

                if (required && value == TimeSpan.Zero && isValid)
                {
                    isValid = false;
                    messages += string.Format(m["req"], humanName);
                }

                if (value > TimeSpan.Zero)
                {
                    TimeSpan min = TimeSpan.Zero;
                    TimeSpan max = TimeSpan.Zero;

                    if (vars.ContainsKey("Min"))
                    {
                        if (vars["Min"] is TimeSpan || vars["Min"] is DateTime)
                        {
                            min = vars["Min"] is DateTime ? ((DateTime)vars["Min"]).TimeOfDay : (TimeSpan)vars["Min"];
                        }
                        else
                        {
                            TimeSpan.TryParseExact((string)vars["Min"], timeFormats, CultureInfo.InvariantCulture, TimeSpanStyles.None, out min);
                        }
                    }
                    if (vars.ContainsKey("Max"))
                    {
                        if (vars["Max"] is TimeSpan || vars["Max"] is DateTime)
                        {
                            max = vars["Max"] is DateTime ? ((DateTime)vars["Max"]).TimeOfDay : (TimeSpan)vars["Max"];
                        }
                        else
                        {
                            TimeSpan.TryParseExact((string)vars["Max"], timeFormats, CultureInfo.InvariantCulture, TimeSpanStyles.None, out max);
                        }
                    }

                    if (min > TimeSpan.Zero && value < min)
                    {
                        isValid = false;
                        messages += string.Format(m["gt"], humanName, min.ToString("hh:mm:ss"));
                    }
                    if (max > TimeSpan.Zero && value > max)
                    {
                        isValid = false;
                        messages += string.Format(m["lt"], humanName, max.ToString("hh:mm:ss"));
                    }
                }
            }
            catch (Exception)
            {
                isValid = false;
                messages += string.Format(m["time"], humanName);
            }
            return isValid;
        }
        private static bool ValidateDateTime(Dictionary<string, object> vars, string fieldName, string humanName, bool required, string messages)
        {
            bool isValid = true;
            DateTime value;

            try
            {
                if (vars.ContainsKey(fieldName))
                {
                    if (!DateTime.TryParseExact((string)vars[fieldName], dateTimeFormats, CultureInfo.InvariantCulture, DateTimeStyles.None, out value))
                    {
                        isValid = false;
                        messages += string.Format(m["date"], humanName);
                    }
                }
                else
                {
                    value = DateTime.MinValue;
                }

                if (required && value == DateTime.MinValue && isValid)
                {
                    isValid = false;
                    messages += string.Format(m["req"], humanName);
                }

                if (value > DateTime.MinValue)
                {
                    DateTime min = DateTime.MinValue;
                    DateTime max = DateTime.MinValue;

                    if (vars.ContainsKey("Min"))
                    {
                        if (vars["Min"] is DateTime)
                        {
                            min = (DateTime)vars["Min"];
                        }
                        else
                        {
                            DateTime.TryParseExact((string)vars["Min"], dateTimeFormats, CultureInfo.InvariantCulture, DateTimeStyles.None, out min);
                        }
                    }
                    if (vars.ContainsKey("Max"))
                    {
                        if (vars["Max"] is DateTime)
                        {
                            max = (DateTime)vars["Max"];
                        }
                        else
                        {
                            DateTime.TryParseExact((string)vars["Max"], dateTimeFormats, CultureInfo.InvariantCulture, DateTimeStyles.None, out max);
                        }
                    }

                    if (min > DateTime.MinValue && value < min)
                    {
                        isValid = false;
                        messages += string.Format(m["gt"], humanName, min.ToShortDateString() + " " + min.ToShortTimeString());
                    }
                    if (max > DateTime.MinValue && value > max)
                    {
                        isValid = false;
                        messages += string.Format(m["lt"], humanName, max.ToShortDateString() + " " + min.ToShortTimeString());
                    }
                }
            }
            catch (Exception)
            {
                isValid = false;
                messages += string.Format(m["date"], humanName);
            }
            return isValid;
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
        private static string[] dateFormats = new string[] {
            "d.M.yyyy",
            "d. M. yyyy",
            "dd.MM.yyyy",
            "dd. MM. yyyy",
            "M/d/yyyy",
            "MM/dd/yyyy"
        };
        private static string[] timeFormats = new string[] { "h:mm:ss", "hh:mm:ss", "h:mm", "hh:mm" };
        private static string[] dateTimeFormats = new string[] {
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

        [Action(201, "Ternary operator", "Result")]
        public static object TernaryOperator(COREobject core, object Input, object CompareWith = null, string Operator = "eq", object ReturnWhenTrue = null, object ReturnWhenFalse = null)
        {
            ReturnWhenTrue = ReturnWhenTrue ?? true;
            ReturnWhenFalse = ReturnWhenFalse ?? false;

            if (Input is string)
            {
                if (CompareWith != null ? Compare(Input, CompareWith, Operator) : !string.IsNullOrEmpty((string)Input))
                    return ReturnWhenTrue;
                return ReturnWhenFalse;
            }
            else if (Input is int)
            {
                if (CompareWith != null ? Compare(Input, CompareWith, Operator) : (int)Input != 0)
                    return ReturnWhenTrue;
                return ReturnWhenFalse;
            }
            else if (Input is double)
            {
                if (CompareWith != null ? Compare(Input, CompareWith, Operator) : (double)Input != 0)
                    return ReturnWhenTrue;
                return ReturnWhenFalse;
            }
            else if (Input is bool)
            {
                if (CompareWith != null ? Compare(Input, CompareWith, Operator) : (bool)Input == true)
                    return ReturnWhenTrue;
                return ReturnWhenFalse;
            }

            return null;
        }
        private static bool Compare(object Input, object CompareWith, string Operator)
        {
            double nI;
            double nC;
            bool inputIsNumeric = double.TryParse(Input.ToString(), out nI);
            bool compareIsNumeric = double.TryParse(CompareWith.ToString(), out nC);

            switch (Operator)
            {
                default:
                case "eq": return Input == CompareWith;
                case "ne": return Input != CompareWith;
                case "lt": return inputIsNumeric && compareIsNumeric ? nI < nC : false;
                case "gt": return inputIsNumeric && compareIsNumeric ? nI > nC : false;
                case "lte": return inputIsNumeric && compareIsNumeric ? nI <= nC : false;
                case "gte": return inputIsNumeric && compareIsNumeric ? nI >= nC : false;
            }
        }

        [Action(202, "Substring", "Result")]
        public static string Substring(COREobject core, string InputString, int Index, int Length)
        {
            return InputString.Substring(Index, Length);
        }

        [Action(211, "String to JSON", "Result")]
        public static JToken String2JSON(COREobject core, string From)
        {
            return JToken.Parse(From);
        }

        [Action(216, "Pad left by char", "Result")]
        public static string PaddingLeft(COREobject core, string InputString, int Length, char PaddingChar = '0')
        {
            return InputString.PadLeft(Length, PaddingChar);
        }

        [Action(217, "Pad Left Table Column")]
        public static void PadLeftTableColumn(COREobject core, string TableName, string ColumnName, int Length, bool SearchInShared = false, char PaddingChar = '0')
        {
            DBConnection db = core.Entitron;

            DBTable table = db.Table(TableName, SearchInShared);

            var tableRowsList = table.Select().ToList();

            foreach (var tableRow in tableRowsList)
            {
                string columnContent = tableRow[ColumnName].ToString();
                tableRow[ColumnName] = columnContent.PadLeft(Length, PaddingChar);
                table.Update(tableRow, (int)tableRow["id"]);
            }

            db.SaveChanges();
        }

        [Action(218, "Unescape String", "Result")]
        public static string UnescapeString(COREobject core, string EscapedString)
        {
            return Regex.Replace(EscapedString, @"\\[rnt]", m =>
            {
                switch (m.Value)
                {
                    case @"\r": return "\r";
                    case @"\n": return "\n";
                    case @"\t": return "\t";
                    default: return m.Value;
                }
            });
        }

        [Action(219, "Redirect to URL")]
        public static void RedirectToUrl(COREobject core, string URL)
        {
            core.HttpResponse(URL);
        }

        [Action(9119, "Get File Extension", "Extension")]
        public static string GetFileExtension(COREobject core, string FileName)
        {
            return FileName.Split('.')[1];
        }

        [Action(950, "Int To Datetime", "Result")]
        public static DateTime InitToDatetime(COREobject core, int Int)
        {
            DateTime dateTime = new DateTime(1970, 1, 1, 0, 0, 0, System.DateTimeKind.Utc);

            return dateTime.AddSeconds(Int).ToLocalTime();
        }

        [Action(1055, "Get Host URL", "Result")]
        public static string GetHostUrl(COREobject core)
        {
            return TapestryUtils.GetServerHostName();
        }

        [Action(1105, "Number to String", "Result")]
        public static string NumberToString(COREobject core, object Number, string Format = "g")
        {
            return (Convert.ToDouble(Number)).ToString(Format);
        }

        [Action(1111, "Open Url")]
        public static void OpenUrl(COREobject core, string Block, string Button = "", int ModelId = -1)
        {
            DBConnection db = core.Entitron;

            string hostname = TapestryUtils.GetServerHostName();
            string appName = db.Application.Name;
            string serverName = HttpContext.Current.Request.ServerVariables["SERVER_NAME"];

            string systemAccName = WebConfigurationManager.AppSettings["SystemAccountName"];
            string systemAccPass = WebConfigurationManager.AppSettings["SystemAccountPass"];

            string targetUrl;

            if (serverName == "localhost")
                targetUrl = $"https://omnius-as.azurewebsites.net/{appName}/{Block}/Get?modelId={ModelId}&User={systemAccName}&Password={systemAccPass}";
            else
                targetUrl = $"{hostname}/{appName}/{Block}/Get?modelId={ModelId}&User={systemAccName}&Password={systemAccPass}";

            if (Button != "")
                targetUrl += $"&button={Button}";

            HttpWebRequest request = (HttpWebRequest)WebRequest.Create(targetUrl);
            HttpWebResponse response = (HttpWebResponse)request.GetResponse();
        }

        [Action(1149, "JArray to list", "Result")]
        public static List<object> JArrayToList(COREobject core, JArray List)
        {
            if (List.Count > 0)
                return List.ToObject<List<object>>();

            return null;
        }

        [Action(2019, "Object to JToken", "Result")]
        public static JToken ObjectToJToken(COREobject core, object Object = null)
        {
            return Object == null
                ? new JObject()
                : JToken.FromObject(Object);
        }

        [Action(5001, "Split to List", "Result")]
        public static List<string> SplitToList(COREobject core, string SourceString, string Separator = ";")
        {
            return SourceString.Split(new string[] { Separator }, StringSplitOptions.None).ToList();
        }

        [Action(18712, "Parse DateTime", "Result", "HasError")]
        public static (DateTime?, bool) ParseDateTime(COREobject core, string Input, string Format = "")
        {
            try
            {
                CultureInfo provider = CultureInfo.InvariantCulture;
                if (Format != "")
                    return (DateTime.ParseExact(Input, Format, provider), false);

                return (DateTime.Parse(Input), false);
            }
            catch (FormatException)
            {
                return (null, true);
            }
        }

        [Action(181111, "Edit JObject", "Result")]
        public static JObject EditJObject(COREobject core, JObject JObject, string[] PropertyName, object[] Value)
        {
            if (PropertyName.Length != Value.Length)
                throw new Exception("Values count differs from properties count!");

            for (int i = 0; i < PropertyName.Length; i++)
            {
                JObject.Property(PropertyName[i]).Value = new JValue(Value[i]);
            }

            return JObject;
        }

        [Action(118999, "Get Folder Files", "Files")]
        public static List<string> GetFolderFiles(COREobject core, string Path = null, bool WithFullPaths = true)
        {
            Path = Path ?? Directory.GetCurrentDirectory();
            List<string> files = new List<string>(Directory.GetFiles(Path));
            if (!WithFullPaths)
            {
                for (int i = 0; i < files.Count; ++i)
                    files[i] = files[i].Substring(files[i].LastIndexOf('\\') + 1, files[i].Length - files[i].LastIndexOf('\\') - 1);
            }

            return files;
        }

        [Action(181666, "Download String as File")]
        public static void DownloadAsFile(COREobject core, string FileName, string Content)
        {
            Content = Content.Replace("\\n", "\n");

            core.HttpResponse(FileName, "application/csv", Encoding.UTF8.GetBytes(Content));
        }

        [Action(133742069, "Combine at Random", "Result")]
        public static string CombineAtRandom(COREobject core, string[] TableName, string[] Column, string Separator = " ")
        {
            DBConnection db = core.Entitron;

            if (TableName.Length != Column.Length)
                throw new Exception("Missing table or column name!");

            string result = "";

            for (int i = 0; i < TableName.Length; i++)
            {
                DBTable table = db.Table(TableName[i]);
                Random rnd = new Random();
                var rows = table.Select(Column[i]).ToList();
                result += rows[rnd.Next(rows.Count)][Column[i]].ToString() + Separator;
            }

            if (result.EndsWith(Separator))
                result.Remove(result.Length - Separator.Length);

            return result;
        }
    }
}

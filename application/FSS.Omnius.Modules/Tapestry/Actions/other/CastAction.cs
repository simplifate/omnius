using System;
using System.Collections.Generic;
using System.Linq;
using FSS.Omnius.Modules.CORE;
using FSS.Omnius.Modules.Entitron.DB;
using Newtonsoft.Json.Linq;

namespace FSS.Omnius.Modules.Tapestry.Actions.Entitron
{
    [EntitronRepository]
    public class CastAction : Action
    {
        public override int Id => 100;

        public override int? ReverseActionId => null;

        public override string[] InputVar => new string[] { "v$Object", "?s$TargetType[|bool|string|char|int|float|decimal|double|JArray|JToken|JObject|JValue]", "?s$CustomTargetType" };

        public override string Name => "Cast variable";

        public override string[] OutputVar => new string[] { "Result", "Error", "IsNull" };

        private static IEnumerable<Type> classList = AppDomain.CurrentDomain.GetAssemblies().SelectMany(t => t.GetTypes()).Where(t => t.IsClass);

        public override void InnerRun(Dictionary<string, object> vars, Dictionary<string, object> outputVars, Dictionary<string, object> InvertedInputVars, Message message)
        {
            outputVars["Error"] = "";
            outputVars["IsNull"] = false;

            object source = vars["Object"];
            string targetType = "";
            if (vars.ContainsKey("CustomTargetType") && !string.IsNullOrEmpty((string)vars["CustomTargetType"])) {
                targetType = (string)vars["CustomTargetType"];
            }
            else if (vars.ContainsKey("TargetType") && !string.IsNullOrEmpty((string)vars["TargetType"])) {
                targetType = (string)vars["TargetType"];
            }

            if (source == null || source == DBNull.Value) {
                outputVars["Result"] = null;
                outputVars["IsNull"] = true;
                return;
            }

            if(string.IsNullOrEmpty(targetType)) {
                throw new Exception($"{Name}: Target type must by specified");
            }

            try { 
                outputVars["Result"] = Convert(source, targetType);
            }
            catch(Exception e) {
                outputVars["Result"] = null;
                outputVars["Error"] = e.Message;
                outputVars["IsNull"] = true;
            }
        }

        private object ConvertDbItem(DBItem source, string targetType)
        {
            if (targetType == "JToken" || targetType == "JObject") {
                return targetType == "JToken" ? source.ToJson() : (JObject)source.ToJson();
            }
            throw new Exception($"{Name}: Unsupported conversion of DBItem to {targetType}");
        }

        private object ConvertDbItemList(IEnumerable<DBItem> source, string targetType)
        {
            if (targetType == "JToken" || targetType == "JArray") {
                JArray o = new JArray();
                foreach (DBItem item in source) {
                    o.Add(item.ToJson());
                }
                return targetType == "JArray" ? o : (JToken)o;
            }
            throw new Exception($"{Name}: Unsupported conversion of IEnumerable<DBItem> to {targetType}");
        }

        private object Convert(object source, string targetType)
        {
            object output = null;
            if (source is DBItem || source is IEnumerable<DBItem>) {
                return source is DBItem ? ConvertDbItem((DBItem)source, targetType) : ConvertDbItemList((IEnumerable<DBItem>)source, targetType);
            }
            
            JToken sourceToken = source is JToken ? (JToken)source : JToken.FromObject(source);
                
            switch (targetType) {
                case "bool": output = As<bool>(sourceToken); break;
                case "string": output = As<string>(sourceToken); break;
                case "char": output = As<char>(sourceToken); break;
                case "int": output = As<int>(sourceToken); break;
                case "float": output = As<float>(sourceToken); break;
                case "decimal": output = As<decimal>(sourceToken); break;
                case "double": output = As<double>(sourceToken); break;
                case "JArray": output = (JArray)sourceToken; break;
                case "JToken": output = sourceToken; break;
                case "JObject": output = (JObject)sourceToken; break;
                case "JValue": output = (JValue)sourceToken; break;
                default: output = ParseTargetTypeAndConvert(sourceToken, targetType); break;
            }
            
            return output;
        }

        private object ParseTargetTypeAndConvert(JToken source, string targetType)
        {
            bool isSimpleType = !targetType.Contains("<");
            
            Type T = isSimpleType ? ParseSimpleType(targetType) : ParseComplexType(targetType);
            return GetType().GetMethod("As").MakeGenericMethod(T).Invoke(this, new object[] { source });
        }

        private Type ParseComplexType(string type)
        {
            string baseType = type.Substring(0, type.IndexOf("<"));
            string typeFullName = "";
            string[] args = type.Substring(type.IndexOf("<") + 1, type.LastIndexOf(">") - type.IndexOf("<") - 1).Split(',');

            Type tBaseType = ParseSimpleType(baseType + "`" + args.Length);
            typeFullName = tBaseType.FullName;

            List<string> argsFullNames = new List<string>();
            foreach(string arg in args) {
                Type tArgType = arg.Contains("<") ? ParseComplexType(arg) : ParseSimpleType(arg);
                argsFullNames.Add(tArgType.FullName);
            }

            typeFullName = string.Format("{0}[{1}]", typeFullName, string.Join(",", argsFullNames));
            
            return Type.GetType(typeFullName);
        }

        private Type ParseSimpleType(string type)
        {
            bool isNullable = false;
            bool isArray = false;

            type = type.Trim(' ');
            if (type.IndexOf("[]") != -1) {
                isArray = true;
                type = type.Remove(type.IndexOf("[]"), 2);
            }
            if (type.IndexOf("?") != -1) {
                isNullable = true;
                type = type.Remove(type.IndexOf("?"), 1);
            }

            string typeLower = type.ToLower();

            string fullTypeName = null;
            switch(type) {
                case "bool": case "boolean":    fullTypeName = "System.Boolean";        break;
                case "byte":                    fullTypeName = "System.Byte";           break;
                case "char":                    fullTypeName = "System.Char";           break;
                case "datetime":                fullTypeName = "System.DateTime";       break;
                case "datetimeoffset":          fullTypeName = "System.DateTimeOffset"; break;
                case "decimal":                 fullTypeName = "System.Decimal";        break;
                case "double":                  fullTypeName = "System.Double";         break;
                case "float":                   fullTypeName = "System.Single";         break;
                case "int16": case "short":     fullTypeName = "System.Int16";          break;
                case "int32": case "int":       fullTypeName = "System.Int32";          break;
                case "int64": case "long":      fullTypeName = "System.Int64";          break;
                case "object":                  fullTypeName = "System.Object";         break;
                case "sbyte":                   fullTypeName = "System.SByte";          break;
                case "string":                  fullTypeName = "System.String";         break;
                case "timespan":                fullTypeName = "System.TimeSpan";       break;
                case "uint16": case "ushort":   fullTypeName = "System.UInt16";         break;
                case "uint32": case "uint":     fullTypeName = "System.UInt32";         break;
                case "uint64": case "ulong":    fullTypeName = "System.UInt64";         break;
            }

            if(fullTypeName == null) {
                Type T = classList.FirstOrDefault(t => t.Name == type);
                fullTypeName = T != null ? T.FullName : type;
            }

            if (isArray) { fullTypeName += "[]"; }
            if (isNullable) { fullTypeName = string.Format("System.Nullable`1[{0}]", fullTypeName); }
            
            return Type.GetType(fullTypeName);
        }

        public T As<T>(JToken source)
        {
            return source.ToObject<T>();
        }
    }
}

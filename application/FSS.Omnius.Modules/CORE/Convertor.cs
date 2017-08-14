using FSS.Omnius.Modules.Entitron.Entity;
using FSS.Omnius.Modules.Entitron.Entity.CORE;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using HtmlAgilityPack;
using FSS.Omnius.Modules.Tapestry;

namespace FSS.Omnius.Modules.CORE
{
    public class Convertor
    {
        public static object convert(int dataTypeId, object input)
        {
            return convert(DataType.ById(dataTypeId), input);
        }
        public static object convert(char shortcut, object input)
        {
            return convert(DataType.ByShort(shortcut), input);
        }
        public static object convert(DataType type, object input)
        {
            switch (type.CSharpName)
            {
                case "int":
                    return Convert.ToInt32(input);
                case "bool":
                    return Convert.ToBoolean(input);
                case "string":
                    return Convert.ToString(input);
                case "float":
                    return TapestryUtils.ParseDouble(input);
                case "DateTime":
                    return Convert.ToDateTime(input);
                case "blob":
                    return input;
                // none
                default:
                    throw new KeyNotFoundException($"Cannot indentify data type '{type.ToString()}'");
            }
        }

        public static string StripTags(string input)
        {
            var doc = new HtmlDocument();
            doc.LoadHtml(input ?? "");
            return doc.DocumentNode.InnerText;
        }
    }
}

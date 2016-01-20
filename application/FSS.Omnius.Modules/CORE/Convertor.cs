using FSS.Omnius.Modules.Entitron.Entity;
using FSS.Omnius.Modules.Entitron.Entity.CORE;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace FSS.Omnius.Modules.CORE
{
    public class Convertor
    {
        public static object convert(int dataTypeId, object input)
        {
            DBEntities e = new DBEntities();
            DataType type = e.DataTypes.SingleOrDefault(dt => dt.Id == dataTypeId);

            return convert(type, input);
        }
        public static object convert(char shortcut, object input)
        {
            DBEntities e = new DBEntities();
            string s = shortcut.ToString();
            DataType type = e.DataTypes.SingleOrDefault(dt => dt.shortcut == s);

            return convert(type, input);
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
                    return Convert.ToDouble(input);
                case "DateTime":
                    return Convert.ToDateTime(input);
                case "blob":
                    return input;
                // none
                default:
                    throw new KeyNotFoundException($"Cannot indentify data type '{type.ToString()}'");
            }
        }
    }
}

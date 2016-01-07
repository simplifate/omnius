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
            DataType type = e.DataTypes.SingleOrDefault(dt => dt.shortcut[0] == shortcut);

            return convert(type, input);
        }
        public static object convert(DataType type, object input)
        {
            switch (type.Id)
            {
                // int
                case 1:
                    return Convert.ToInt32(input);
                // bool
                case 2:
                    return Convert.ToBoolean(input);
                // string
                case 3:
                    return Convert.ToString(input);
                case 4:
                    return Convert.ToDouble(input);
                // none
                default:
                    throw new KeyNotFoundException($"Cannot indentify data type '{type.ToString()}'");
            }
        }
    }
}

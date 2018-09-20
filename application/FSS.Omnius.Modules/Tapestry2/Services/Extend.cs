using FSS.Omnius.Modules.CORE;
using FSS.Omnius.Modules.Entitron.DB;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Reflection;

namespace FSS.Omnius.Modules.Tapestry2
{
    public static class Extend
    {
        public static T ConvertTo<T>(COREobject core, object value)
        {
            // no conversion needed
            if (value is T)
                return (T)value;

            Type sourceT = value.GetType();
            Type targetT = typeof(T);
            
            // int
            if (sourceT == typeof(int))
            {
                if (targetT == typeof(string))
                    return (T)(value.ToString() as object);
                if (targetT == typeof(bool))
                    return (T)(((int)value != 0) as object);
            }

            // dict
            if (sourceT == typeof(Dictionary<string, object>))
            {
                // DBItem
                if (targetT == typeof(DBItem))
                    return (T)(new DBItem(core.Entitron, null, (Dictionary<string, object>)value) as object);
            }

            // return first item of collection
            if (sourceT.GetInterfaces().Contains(typeof(IEnumerable<T>)))
                return (value as IEnumerable<T>).FirstOrDefault();

            // default
            if (targetT == typeof(int))
                return (T)(Convert.ToInt32(value) as object);
            if (targetT == typeof(bool))
                return (T)(Convert.ToBoolean(value) as object);
            if (targetT == typeof(double))
                return (T)(Convert.ToDouble(value) as object);
            if (targetT == typeof(decimal))
                return (T)(Convert.ToDecimal(value) as object);
            if (targetT == typeof(string))
                return (T)(Convert.ToString(value) as object);

            throw new InvalidOperationException();
        }

        public static bool Compare(object value1, object value2)
        {
            // null
            if (value1 == null || value2 == null)
                return value1 == value2;

            // different types
            if (value1.GetType() != value2.GetType())
                return false;

            // valueTypes
            if (value1 is bool)
                return (bool)value1 == (bool)value2;
            if (value1 is string)
                return (string)value1 == (string)value2;
            if (value1 is int)
                return (int)value1 == (int)value2;
            if (value1 is double)
                return (double)value1 == (double)value2;
            if (value1 is decimal)
                return (decimal)value1 == (decimal)value2;

            // other
            return value1 == value2;
        }

        public static bool GreaterThan(object value1, object value2)
        {
            // null
            if (value1 == null)
                value1 = 0;
            if (value2 == null)
                value2 = 0;

            // type
            if (value1.GetType() != value2.GetType())
            {
                value1 = Convert.ToDecimal(value1);
                value2 = Convert.ToDecimal(value2);
            }

            // numeric
            if (value1 is int)
                return (int)value1 > (int)value2;
            if (value1 is double)
                return (double)value1 > (double)value2;
            if (value1 is decimal)
                return (decimal)value1 > (decimal)value2;

            // other
            return false;
        }
        public static bool GreaterEqThan(object value1, object value2)
        {
            // null
            if (value1 == null)
                value1 = 0;
            if (value2 == null)
                value2 = 0;

            // type
            if (value1.GetType() != value2.GetType())
            {
                value1 = Convert.ToDecimal(value1);
                value2 = Convert.ToDecimal(value2);
            }

            // numeric
            if (value1 is int)
                return (int)value1 >= (int)value2;
            if (value1 is double)
                return (double)value1 >= (double)value2;
            if (value1 is decimal)
                return (decimal)value1 >= (decimal)value2;

            // other
            return false;
        }
        public static bool LessThan(object value1, object value2)
        {
            // null
            if (value1 == null)
                value1 = 0;
            if (value2 == null)
                value2 = 0;

            // type
            if (value1.GetType() != value2.GetType())
            {
                value1 = Convert.ToDecimal(value1);
                value2 = Convert.ToDecimal(value2);
            }

            // numeric
            if (value1 is int)
                return (int)value1 < (int)value2;
            if (value1 is double)
                return (double)value1 < (double)value2;
            if (value1 is decimal)
                return (decimal)value1 < (decimal)value2;

            // other
            return false;
        }
        public static bool LessEqThan(object value1, object value2)
        {
            // null
            if (value1 == null)
                value1 = 0;
            if (value2 == null)
                value2 = 0;

            // type
            if (value1.GetType() != value2.GetType())
            {
                value1 = Convert.ToDecimal(value1);
                value2 = Convert.ToDecimal(value2);
            }

            // numeric
            if (value1 is int)
                return (int)value1 <= (int)value2;
            if (value1 is double)
                return (double)value1 <=(double)value2;
            if (value1 is decimal)
                return (decimal)value1 <= (decimal)value2;

            // other
            return false;
        }

        public static bool IsEmpty(object value)
        {
            if (value == null)
                return true;

            if (value == Activator.CreateInstance(value.GetType()))
                return true;

            return false;
        }

        public static object GetChained(object root, params string[] paramNames)
        {
            object currentObj = root;

            foreach (string paramName in paramNames)
            {
                Type type = currentObj.GetType();
                // property
                PropertyInfo property = type.GetProperty(paramName);
                if (property != null)
                {
                    currentObj = property.GetValue(currentObj);
                    continue;
                }

                // []
                property = type.GetProperty("Item", new Type[] { typeof(string) });
                if (property != null)
                {
                    currentObj = property.GetValue(currentObj, new object[] { paramName });
                    continue;
                }

                throw new TapestryRunOmniusException($"Unknown property[{paramName}] of class[{type.FullName}] in [{paramNames}]", null);
            }

            return currentObj;
        }
    }
}

using System;
using System.Collections.Generic;
using System.Data;
using HtmlAgilityPack;
using FSS.Omnius.Modules.Entitron.DB;

namespace FSS.Omnius.Modules.Entitron
{
    public static class DataType
    {
        public static HashSet<Type> BaseTypes = new HashSet<Type>
        {
            typeof(int),
            typeof(int?),
            typeof(bool),
            typeof(bool?),
            typeof(string),
            typeof(float),
            typeof(float?),
            typeof(decimal),
            typeof(decimal?),
            typeof(DateTime),
            typeof(DateTime?)
        };
        public static string StripTags(string input)
        {
            var doc = new HtmlDocument();
            doc.LoadHtml(input ?? "");
            return doc.DocumentNode.InnerText;
        }

        public static char Shortcut(DbType type)
        {
            switch (type)
            {
                case DbType.String:
                    return 's';
                case DbType.Boolean:
                    return 'b';
                case DbType.Int32:
                    return 'i';
                case DbType.Double:
                    return 'f';
                case DbType.Decimal:
                    return 'c';
                case DbType.DateTime:
                    return 'd';
                case DbType.Binary:
                    return 'l';
                default:
                    throw new InvalidOperationException();
            }
        }
        public static DbType FromShortcut(char type)
        {
            switch (type)
            {
                case 's':
                    return DbType.String;
                case 'b':
                    return DbType.Boolean;
                case 'i':
                    return DbType.Int32;
                case 'f':
                    return DbType.Double;
                case 'c':
                    return DbType.Decimal;
                case 'd':
                    return DbType.DateTime;
                case 'l':
                    return DbType.Binary;
                default:
                    throw new InvalidOperationException();
            }
        }

        public static string CSharpName(DbType type)
        {
            switch (type)
            {
                case DbType.String:
                    return "string";
                case DbType.Boolean:
                    return "bool";
                case DbType.Int32:
                    return "int";
                case DbType.Double:
                    return "float";
                case DbType.Decimal:
                    return "decimal";
                case DbType.DateTime:
                    return "DateTime";
                case DbType.Binary:
                    return "object";
                default:
                    throw new InvalidOperationException();
            }
        }
        public static Type CSharp(DbType type)
        {
            switch (type)
            {
                case DbType.String:
                    return typeof(string);
                case DbType.Boolean:
                    return typeof(bool);
                case DbType.Int32:
                    return typeof(int);
                case DbType.Double:
                    return typeof(float);
                case DbType.Decimal:
                    return typeof(decimal);
                case DbType.DateTime:
                    return typeof(DateTime);
                case DbType.Binary:
                    return typeof(object);
                default:
                    throw new InvalidOperationException();
            }
        }
        public static DbType FromCSharpName(string type)
        {
            switch (type)
            {
                case "string":
                    return DbType.String;
                case "bool":
                    return DbType.Boolean;
                case "int":
                    return DbType.Int32;
                case "float":
                    return DbType.Double;
                case "decimal":
                    return DbType.Decimal;
                case "DateTime":
                    return DbType.DateTime;
                case "object":
                    return DbType.Binary;
                default:
                    throw new InvalidOperationException();
            }
        }

        public static string DesignerName(DbType type)
        {
            switch (type)
            {
                case DbType.String:
                    return "varchar";
                case DbType.Boolean:
                    return "boolean";
                case DbType.Int32:
                    return "integer";
                case DbType.Double:
                    return "float";
                case DbType.Decimal:
                    return "decimal";
                case DbType.DateTime:
                    return "datetime";
                case DbType.Binary:
                    return "blob";
                default:
                    throw new InvalidOperationException();
            }
        }
        public static DbType FromDesignerName(string type)
        {
            switch (type)
            {
                case "varchar":
                    return DbType.String;
                case "boolean":
                    return DbType.Boolean;
                case "integer":
                    return DbType.Int32;
                case "float":
                    return DbType.Double;
                case "decimal":
                    return DbType.Decimal;
                case "datetime":
                    return DbType.DateTime;
                case "binary":
                    return DbType.Binary;
                default:
                    throw new InvalidOperationException();
            }
        }

        public static string DBName(DbType type, ESqlType sqlType)
        {
            switch (type)
            {
                case DbType.String:
                    if (sqlType == ESqlType.MSSQL)
                        return "nvarchar";
                    if (sqlType == ESqlType.MySQL)
                        return "varchar";
                    throw new InvalidOperationException();

                case DbType.Boolean:
                    if (sqlType == ESqlType.MSSQL)
                        return "bit";
                    if (sqlType == ESqlType.MySQL)
                        return "tinyint";
                    throw new InvalidOperationException();

                case DbType.Int32:
                    return "int";
                case DbType.Double:
                    return "float";
                case DbType.Decimal:
                    return "decimal";
                case DbType.DateTime:
                    return "datetime";
                case DbType.Binary:
                    return "binary";
                default:
                    throw new InvalidOperationException();
            }
        }
        public static DbType FromDBName(string type, ESqlType sqlType)
        {
            switch (type)
            {
                case "nvarchar":
                    if (sqlType == ESqlType.MSSQL)
                        return DbType.String;
                    throw new InvalidOperationException($"Unknown type: {type} for {sqlType}");
                case "varchar":
                    #warning inconsistent data types 
                    if (sqlType == ESqlType.MySQL || sqlType == ESqlType.MSSQL)
                        return DbType.String;
                    throw new InvalidOperationException($"Unknown type: {type} for {sqlType}");

                case "bit":
                    if (sqlType == ESqlType.MSSQL)
                        return DbType.Boolean;
                    throw new InvalidOperationException($"Unknown type: {type} for {sqlType}");
                case "boolean":
                    if (sqlType == ESqlType.MySQL)
                        return DbType.Boolean;
                    throw new InvalidOperationException($"Unknown type: {type} for {sqlType}");
                case "tinyint":
                    if (sqlType == ESqlType.MySQL)
                        return DbType.Boolean;
                    throw new InvalidOperationException($"Unknown type: {type} for {sqlType}");

                case "int":
                    return DbType.Int32;
                case "float":
                    return DbType.Double;
                case "decimal":
                    return DbType.Decimal;
                case "datetime":
                    return DbType.DateTime;
                case "binary":
                    return DbType.Binary;
                default:
                    throw new InvalidOperationException($"Unknown type: {type} for {sqlType}");
            }
        }

        public static int Limitation(DbType type)
        {
            switch (type)
            {
                case DbType.Binary:
                case DbType.String:
                    return 1;
                case DbType.Boolean:
                case DbType.Int32:
                case DbType.Double:
                case DbType.Decimal:
                case DbType.DateTime:
                    return 0;
                default:
                    throw new InvalidOperationException();
            }
        }

        public static int MaxLength(DbType type)
        {
            switch (type)
            {
                case DbType.Binary:
                case DbType.String:
                    return 4000;
                case DbType.Boolean:
                case DbType.Int32:
                case DbType.Double:
                case DbType.Decimal:
                case DbType.DateTime:
                    return -1;
                default:
                    throw new InvalidOperationException();
            }
        }

        public static bool IsNumeric(DbType type)
        {
            switch (type)
            {
                case DbType.Binary:
                case DbType.String:
                case DbType.Boolean:
                    return false;
                case DbType.Int32:
                case DbType.Double:
                case DbType.Decimal:
                case DbType.DateTime:
                    return true;
                default:
                    throw new InvalidOperationException();
            }
        }

        public static string FullDefinition(DbType type, ESqlType sqlType, int maxLength = -1)
        {
            switch (type)
            {
                case DbType.String:
                    if (sqlType == ESqlType.MSSQL)
                        return $"nvarchar({(maxLength > 0 ? maxLength : MaxLength(type))})";
                    if (sqlType == ESqlType.MySQL)
                        return $"varchar({(maxLength > 0 ? maxLength : MaxLength(type))})";
                    throw new InvalidOperationException();

                case DbType.Boolean:
                    if (sqlType == ESqlType.MSSQL)
                        return "bit";
                    if (sqlType == ESqlType.MySQL)
                        return "tinyint(1)";
                    throw new InvalidOperationException();

                case DbType.Int32:
                    return "int";
                case DbType.Double:
                    if (sqlType == ESqlType.MSSQL)
                        return "float";
                    if (sqlType == ESqlType.MySQL)
                        return "float(255,30)";
                    throw new InvalidOperationException();
                case DbType.Decimal:
                    if (sqlType == ESqlType.MSSQL)
                        return "decimal(38,20)";
                    if (sqlType == ESqlType.MySQL)
                        return "decimal(38,20)";
                    throw new InvalidOperationException();
                case DbType.DateTime:
                    return "datetime";
                case DbType.Binary:
                    return "binary";
                default:
                    throw new InvalidOperationException();
            }
        }

        public static object ConvertTo(DbType type, object value)
        {
            switch (type)
            {
                case DbType.String:
                    return Convert.ToString(value);
                case DbType.Boolean:
                    return Convert.ToBoolean(value);
                case DbType.Int32:
                    return Convert.ToInt32(value);
                case DbType.Double:
                    return Convert.ToDouble(value);
                case DbType.Decimal:
                    return Convert.ToDecimal(value);
                case DbType.DateTime:
                    return Convert.ToDateTime(value);
                case DbType.Binary:
                    return value;
                default:
                    throw new InvalidOperationException();
            }
        }
    }
}

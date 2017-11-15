using System.Collections.Generic;
using System.ComponentModel.DataAnnotations.Schema;
using System.Linq;

namespace FSS.Omnius.Modules.Entitron.Entity.CORE
{
    [NotMapped]
    public class DataType
    {
        public static HashSet<DataType> All = new HashSet<DataType>
        {
            new DataType { Id = 1, CSharpName = "string", SqlName = "nvarchar", limited = true, shortcut = 's', DBColumnTypeName = "nvarchar" },
            new DataType { Id = 2, CSharpName = "bool", SqlName = "bit", limited = false, shortcut = 'b', DBColumnTypeName = "boolean" },
            new DataType { Id = 3, CSharpName = "int", SqlName = "integer", limited = false, shortcut = 'i', DBColumnTypeName = "integer" },
            new DataType { Id = 4, CSharpName = "float", SqlName = "float", limited = false, shortcut = 'f', DBColumnTypeName = "float" },
            new DataType { Id = 5, CSharpName = "DateTime", SqlName = "datetime", limited = true, shortcut = 'd', DBColumnTypeName = "datetime" },
            new DataType { Id = 8, CSharpName = "string", SqlName = "XML", limited = true, shortcut = 'x', DBColumnTypeName = null },
            new DataType { Id = 9, CSharpName = "blob", SqlName = "Blob", limited = true, shortcut = 'l', DBColumnTypeName = null }
        };

        public static DataType ById(int Id)
        {
            return All.FirstOrDefault(dt => dt.Id == Id);
        }
        public static DataType ByShort(char shortcut)
        {
            return All.FirstOrDefault(dt => dt.shortcut == shortcut);
        }
        public static DataType ByDBColumnTypeName(string DBColumnTypeName)
        {
            return All.FirstOrDefault(dt => dt.DBColumnTypeName.Contains(DBColumnTypeName));
        }
        public static DataType BySqlName(string SqlName)
        {
            return All.FirstOrDefault(dt => dt.SqlName.Contains(SqlName));
        }


        public int Id { get; set; }
        public string CSharpName { get; set; }
        public string SqlName { get; set; }
        public string DBColumnTypeName { get; set; }
        public bool limited { get; set; }
        public char shortcut { get; set; }

        public override string ToString()
        {
            return $"{SqlName}({CSharpName})";
        }
    }
}

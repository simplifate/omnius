using Newtonsoft.Json;
using System;
using System.ComponentModel.DataAnnotations.Schema;
using System.Data;

namespace FSS.Omnius.Modules.Entitron.Entity.Entitron
{
    [Table("Entitron_DbColumn")]
    public class DbColumn : IEntity
    {
        public int Id { get; set; }

        public string Name { get; set; }
        public string DisplayName { get; set; }
        public bool PrimaryKey { get; set; }
        public bool Unique { get; set; }
        public bool AllowNull { get; set; }
        public string Type { get; set; }
        public int ColumnLength { get; set; }
        public bool ColumnLengthIsMax { get; set; }
        public string DefaultValue { get; set; }

        [ImportExport(ELinkType.Parent, typeof(DbTable))]
        public int DbTableId { get; set; }
        [ImportExport(ELinkType.Parent)]
        public virtual DbTable DbTable { get; set; }

        public string RealDefaultValue(DbType type)
        {
            if (string.IsNullOrEmpty(DefaultValue))
            {
                if (AllowNull)
                    return "NULL";

                return null;
            }

            if (type == DbType.String && string.IsNullOrEmpty(DefaultValue))
                return "''";

            if (type == DbType.Boolean)
            {
                if (DefaultValue.ToLower() == "null")
                    return null;
                if (DefaultValue.ToLower() == "true")
                    return "'1'";
                if (DefaultValue.ToLower() == "false")
                    return "'0'";
                if (DefaultValue == "1" || DefaultValue == "0")
                    return $"'{DefaultValue}'";

                return $"'{Convert.ToInt32(DefaultValue).ToString()}'";
            }

            return $"'{DefaultValue}'";
        }
    }
}
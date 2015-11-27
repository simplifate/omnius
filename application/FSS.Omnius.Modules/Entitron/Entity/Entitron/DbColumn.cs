using System.ComponentModel.DataAnnotations.Schema;

namespace FSS.Omnius.Modules.Entitron.Entity.Entitron
{
    [Table("Entitron_DbColumn")]
    public class DbColumn
    {
        public int Id { get; set; }
        public string Name { get; set; }
        public bool PrimaryKey { get; set; }
        public bool Unique { get; set; }
        public bool AllowNull { get; set; }
        public string Type { get; set; }
        public int ColumnLength { get; set; }
        public bool ColumnLengthIsMax { get; set; }
        public string DefaultValue { get; set; }

        public int DbTableId { get; set; }
        public virtual DbTable DbTable { get; set; }
    }
}
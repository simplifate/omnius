using System.ComponentModel.DataAnnotations.Schema;
using FSS.Omnius.Modules.Entitron.Entity.Master;

namespace FSS.Omnius.Modules.Entitron.Entity.Entitron
{
    [Table("Entitron_ColumnMetadata")]
    public class ColumnMetadata
    {
        public int Id { get; set; }
        public string TableName { get; set; }
        public string ColumnName { get; set; }
        public string ColumnDisplayName { get; set; }

        public virtual Application Application { get; set; }
    }
}

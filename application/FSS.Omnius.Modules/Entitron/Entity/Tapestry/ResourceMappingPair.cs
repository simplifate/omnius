using System.ComponentModel.DataAnnotations;
using System.ComponentModel.DataAnnotations.Schema;

namespace FSS.Omnius.Modules.Entitron.Entity.Tapestry
{
    [Table("Tapestry_ResourceMappingPairs")]
    public class ResourceMappingPair : IEntity
    {
        public int Id { get; set; }
        [StringLength(100)]
        public string relationType { get; set; }

        [StringLength(100)]
        public string SourceComponentName { get; set; }
        [StringLength(100)]
        public string SourceTableName { get; set; }
        [StringLength(100)]
        public string SourceColumnName { get; set; }
        public string SourceColumnFilter { get; set; }

        [StringLength(100)]
        public string TargetName { get; set; }
        [StringLength(100)]
        public string TargetTableName { get; set; }
        [StringLength(100)]
        public string TargetColumnName { get; set; }

        public string TargetType { get; set; } // to remove
        public string DataSourceParams { get; set; }
        public virtual TapestryDesignerResourceItem Source { get; set; } // to remove
        public virtual TapestryDesignerResourceItem Target { get; set; } // to remove
        public int BlockId { get; set; }
        public virtual Block Block { get; set; }
    }
}

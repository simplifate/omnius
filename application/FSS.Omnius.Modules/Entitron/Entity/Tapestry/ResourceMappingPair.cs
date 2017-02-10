using System.Collections.Generic;
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
        public bool SourceIsShared { get; set; }
        [StringLength(100)]
        public string SourceColumnName { get; set; }
        public string SourceColumnFilter { get; set; }

        [StringLength(100)]
        public string TargetName { get; set; }
        [StringLength(100)]
        public string TargetTableName { get; set; }
        public bool TargetIsShared { get; set; }
        [StringLength(100)]
        public string TargetColumnName { get; set; }
        
        public string DataSourceParams { get; set; }
        public int BlockId { get; set; }
        public virtual Block Block { get; set; }
        
        // TO remove
        public string TargetType { get; set; }
        public int? Source_Id { get; set; }
        public int? Target_Id { get; set; }
    }
}

using System.ComponentModel.DataAnnotations.Schema;
using FSS.Omnius.Modules.Entitron.Entity.Master;
using Newtonsoft.Json;
using System.ComponentModel.DataAnnotations;

namespace FSS.Omnius.Modules.Entitron.Entity.Entitron
{
    [Table("Entitron_ColumnMetadata")]
    public class ColumnMetadata : IEntity
    {
        public int Id { get; set; }

        [Required]
        [MaxLength(50)]
        [Index("UX_Entitron_ColumnMetadata", IsUnique = true, Order = 2)]
        public string TableName { get; set; }
        [Required]
        [MaxLength(50)]
        [Index("UX_Entitron_ColumnMetadata", IsUnique = true, Order = 3)]
        public string ColumnName { get; set; }
        [MaxLength(100)]
        public string ColumnDisplayName { get; set; }
        
        [Index("UX_Entitron_ColumnMetadata", IsUnique = true, Order = 1)]
        [ImportExport(ELinkType.Parent, typeof(Application))]
        public int ApplicationId { get; set; } 
        [ImportExport(ELinkType.Parent)]
        public virtual Application Application { get; set; }
    }
}

using System.ComponentModel.DataAnnotations;
using System.ComponentModel.DataAnnotations.Schema;

namespace FSS.Omnius.Modules.Entitron.Entity.CORE
{
    [Table("CORE_ConfigPairs")]
    public partial class ConfigPair : IEntity
    {
        public int Id { get; set; }
        [Required]
        [Index(IsUnique = true)]
        [StringLength(100)]
        public string Key { get; set; }
        [Required]
        public string Value { get; set; }
    }
}

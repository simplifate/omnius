using System;
using System.Collections.Generic;
using System.ComponentModel.DataAnnotations;
using System.ComponentModel.DataAnnotations.Schema;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace FSS.Omnius.Modules.Entitron.Entity.CORE
{
    [Table("CORE_ConfigPairs")]
    public partial class ConfigPair
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

using FSS.Omnius.Modules.Entitron.Entity.Tapestry;
using System;
using System.Collections.Generic;
using System.ComponentModel.DataAnnotations;
using System.ComponentModel.DataAnnotations.Schema;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace FSS.Omnius.Modules.Entitron.Entity.CORE
{
    [Table("CORE_DataTypes")]
    public class DataType
    {
        public DataType()
        {
            AttributeRules = new HashSet<AttributeRule>();
        }

        public int Id { get; set; }

        [Required]
        [StringLength(50)]
        public string CSharpName { get; set; }

        [Required]
        [StringLength(50)]
        public string SqlName { get; set; }

        [StringLength(50)]
        public string DBColumnTypeName { get; set; }   

        [Required]
        public bool limited { get; set; }

        [Required]
        [StringLength(1)]
        [Index(IsUnique = true)]
        public string shortcut { get; set; }

        public virtual ICollection<AttributeRule> AttributeRules { get; set; }

        public override string ToString()
        {
            return $"{SqlName}({CSharpName})";
        }
    }
}

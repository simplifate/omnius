using System.Collections.Generic;
using System.ComponentModel.DataAnnotations;
using System.ComponentModel.DataAnnotations.Schema;
using System.Data.Entity.Spatial;

namespace FSS.Omnius.Modules.Entitron.Entity.Tapestry
{
    [Table("Tapestry_ActionRule_Action")]
    public partial class ActionRule_Action
    {
        [Key]
        [Column(Order = 0)]
        [DatabaseGenerated(DatabaseGeneratedOption.None)]
        public int ActionRuleId { get; set; }

        [Key]
        [Column(Order = 1)]
        [DatabaseGenerated(DatabaseGeneratedOption.None)]
        public override int ActionId { get; set; }

        public override int Order { get; set; }

        /// <summary>
        /// Target1=source1;target2=source2
        /// pro vstup dat uvozovky - Target1=s$nìjaký text
        /// s - string
        /// b - boolean
        /// i - int
        /// d - double
        /// </summary>
        [StringLength(2000)]
        public override string InputVariablesMapping { get; set; } // target=source;c=d

        [StringLength(2000)]
        public override string OutputVariablesMapping { get; set; } // target=source;c=d

        public virtual ActionRule ActionRule { get; set; }
        public virtual Action Action { get; set; }
    }
}

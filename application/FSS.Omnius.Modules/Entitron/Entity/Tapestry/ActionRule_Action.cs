using System.ComponentModel.DataAnnotations;
using System.ComponentModel.DataAnnotations.Schema;

namespace FSS.Omnius.Modules.Entitron.Entity.Tapestry
{
    [Table("Tapestry_ActionRule_Action")]
    public partial class ActionRule_Action : IEntity
    {
        public int Id { get; set; }
        public int ActionRuleId { get; set; }
        public int ActionId { get; set; }

        public int Order { get; set; }

        public string VirtualAction { get; set; }
        public int? VirtualItemId { get; set; }
        public int? VirtualParentId { get; set; }
        public bool? IsForeachStart { get; set; }
        public bool? IsForeachEnd { get; set; }

        /// <summary>
        /// Target1=source1;target2=source2
        /// pro vstup dat uvozovky - Target1=s$nìjaký text
        /// s - string
        /// b - boolean
        /// i - int
        /// d - double
        /// </summary>
        [StringLength(2000)]
        public string InputVariablesMapping { get; set; } // target=source;c=d

        [StringLength(2000)]
        public string OutputVariablesMapping { get; set; } // target=source;c=d

        public virtual ActionRule ActionRule { get; set; }
    }
}

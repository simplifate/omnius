using FSS.Omnius.Modules.Entitron.Entity.Persona;
using System.Collections.Generic;
using System.ComponentModel.DataAnnotations.Schema;

namespace FSS.Omnius.Modules.Entitron.Entity.Tapestry
{
    [Table("Tapestry_ActionRule")]
    public partial class ActionRule
    {
        public ActionRule()
        {
            ActionRule_Actions = new HashSet<ActionRule_Action>();
            ActionRuleRights = new HashSet<ActionRuleRight>();
        }

        public int Id { get; set; }
        public string Name { get; set; }
        public int PreFunctionCount { get; set; }
        public string Condition { get; set; }

        public virtual HashSet<ActionRule_Action> ActionRule_Actions { get; set; }

        public int SourceBlockId { get; set; }
        public int TargetBlockId { get; set; }
        public int ActorId { get; set; }
        public virtual Actor Actor { get; set; }
        public virtual Block SourceBlock { get; set; }
        public virtual Block TargetBlock { get; set; }
        public virtual ICollection<ActionRuleRight> ActionRuleRights { get; set; }
    }
}
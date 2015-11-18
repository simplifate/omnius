using System.Collections.Generic;
using Entitron.Entity;

namespace FSS.FSPOC.Entitron.Entity.Tapestry
{
    public class ActionRule
    {
        public int Id { get; set; }
        public string Name { get; set; }

        public virtual ICollection<ActionActionRule> ActionActionRules { get; set; }

        public int SourceBlockId { get; set; }//atribut předán ze třídy actionrule v namespacu Entitron.Entity
        public int TargetBlockId { get; set; }//atribut předán ze třídy actionrule v namespacu Entitron.Entity
        public int ActorId { get; set; }//atribut předán ze třídy actionrule v namespacu Entitron.Entity
        public virtual Actor Actor { get; set; }//atribut předán ze třídy actionrule v namespacu Entitron.Entity
        public virtual Block SourceBlock { get; set; }//atribut předán ze třídy actionrule v namespacu Entitron.Entity
        public virtual Block TargetBlock { get; set; }//atribut předán ze třídy actionrule v namespacu Entitron.Entity


    }
}
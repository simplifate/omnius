using System.Collections.Generic;
using Entitron.Entity;

namespace FSS.FSPOC.Entitron.Entity.Tapestry
{
    public class Action
    {
        public int Id { get; set; }
        public string Name   { get; set; }
        public ActionCategory ActionCategory { get; set; }
        public int ActionCategoryId { get; set; }
        public virtual ICollection<ActionActionRule> ActionActionRules { get; set; }

        public int IdentifierAction { get; set; }

        public string MethodName { get; set; }//atribut předán ze třídy action v namespacu Entitron.Entity
        public string RequiredAttributes { get; set; }//atribut předán ze třídy action v namespacu Entitron.Entity
        public virtual ICollection<ActionRight> Rigths { get; set; }//atribut předán ze třídy action v namespacu Entitron.Entity

    }
}
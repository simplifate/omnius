using System.Collections.Generic;

namespace FSS.FSPOC.BussinesObjects.Entities.Actions
{
    public class ActionRule
    {
        public int Id { get; set; }
        public string Name { get; set; }

        public virtual ICollection<ActionActionRule> ActionActionRules { get; set; } 
    }
}
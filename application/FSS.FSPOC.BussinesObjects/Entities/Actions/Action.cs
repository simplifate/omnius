using System.Collections.Generic;

namespace FSS.FSPOC.BussinesObjects.Entities.Actions
{
    public class Action
    {
        public int Id { get; set; }
        public string Name   { get; set; }
        public ActionCategory ActionCategory { get; set; }
        public int ActionCategoryId { get; set; }
        public virtual ICollection<ActionActionRule> ActionActionRules { get; set; }

    }
}
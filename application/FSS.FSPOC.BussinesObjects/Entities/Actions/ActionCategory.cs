using System.Collections.Generic;

namespace FSS.FSPOC.BussinesObjects.Entities.Actions
{
    public class ActionCategory
    {
        public int Id { get; set; }
        public string Name { get; set; }
        public virtual ICollection<Action> Actions { get; set; }

    }
}
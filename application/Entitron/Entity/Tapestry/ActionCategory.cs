using System.Collections.Generic;

namespace FSS.FSPOC.Entitron.Entity.Tapestry
{
    public class ActionCategory
    {
        public int Id { get; set; }
        public string Name { get; set; }
        public virtual ICollection<Action> Actions { get; set; }
        public int? ParentId { get; set; }

    }
}
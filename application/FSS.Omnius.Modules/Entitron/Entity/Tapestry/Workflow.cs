namespace FSS.Omnius.Modules.Entitron.Entity.Tapestry
{
    using Master;
    using System.Collections.Generic;
    using System.ComponentModel.DataAnnotations.Schema;

    [Table("Tapestry_WorkFlow")]
    public class WorkFlow
    {
        public WorkFlow()
        {
            Blocks = new HashSet<Block>();
            Children = new HashSet<WorkFlow>();
        }

        public int Id { get; set; }
        public int? InitBlockId { get; set; }
        public int? ParentId { get; set; }
        public int ApplicationId { get; set; }
        public int TypeId { get; set; }
        public bool IsInMenu { get; set; }
        public virtual Application Application { get; set; }
        public virtual ICollection<Block> Blocks { get; set; }
        public virtual Block InitBlock { get; set; }
        public virtual WorkFlowType Type { get; set; }
        public virtual ICollection<WorkFlow> Children { get; set; }
        public virtual WorkFlow Parent { get; set; }
    }
}

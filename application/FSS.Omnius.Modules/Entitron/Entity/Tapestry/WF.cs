namespace FSS.Omnius.Modules.Entitron.Entity.Tapestry
{
    using Master;
    using System.Collections.Generic;
    using System.ComponentModel.DataAnnotations.Schema;

    [Table("Tapestry_Run_WF")]
    public class WF
    {
        public WF()
        {
            Blocks = new HashSet<Block>();
            Children = new HashSet<WF>();
        }

        public int Id { get; set; }
        public int InitBlockId { get; set; }
        public int? ParentId { get; set; }
        public int ApplicationId { get; set; }
        public int TypeId { get; set; }
        public virtual Application Application { get; set; }
        public virtual ICollection<Block> Blocks { get; set; }
        public virtual Block InitBlock { get; set; }
        public virtual WFType Type { get; set; }
        public virtual ICollection<WF> Children { get; set; }
        public virtual WF Parent { get; set; }
    }
}

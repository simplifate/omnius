using System.Collections.Generic;
using System.ComponentModel.DataAnnotations.Schema;

namespace FSS.Omnius.Modules.Entitron.Entity.Tapestry
{
    [Table("Tapestry_Activity")]
    public class Activity
    {
        public int Id { get; set; }
        public int Type { get; set; }
        public int PositionX { get; set; }
        public int PositionY { get; set; }

        public virtual ICollection<Input> Inputs { get; set; }
        public virtual ICollection<Output> Outputs { get; set; }

        public virtual WorkflowCommit WorkflowCommit { get; set; }

        public Activity()
        {
            Inputs = new List<Input>();
            Outputs = new List<Output>();
        }
    }
}
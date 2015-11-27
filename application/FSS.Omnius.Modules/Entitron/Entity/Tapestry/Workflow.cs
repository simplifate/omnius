namespace FSS.Omnius.Modules.Entitron.Entity.Tapestry
{
    using System;
    using System.Collections.Generic;
    using Master;
    using System.ComponentModel.DataAnnotations.Schema;

    [Table("Tapestry_WorkFlow")]
    public class Workflow
    {
        public int Id { get; set; }
        public string Name { get; set; }
        public DateTime CreationTime { get; set; }
        public DateTime LastChangeTime { get; set; }
        public virtual ICollection<WorkflowCommit> WorkflowCommits { get; set; }

        public Workflow()
        {
            WorkflowCommits = new HashSet<WorkflowCommit>();
        }

    }
}

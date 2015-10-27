using System;
using System.Collections.Generic;

namespace FSS.FSPOC.BussinesObjects.Entities.Workflow
{
    public class Workflow
    {
        public int Id { get; set; }
        public string Name { get; set; }
        public DateTime CreationTime { get; set; }
        public DateTime LastChangeTime { get; set; }
        public virtual ICollection<WorkflowCommit> WorkflowCommits { get; set; }

        public Workflow()
        {
            WorkflowCommits = new List<WorkflowCommit>();
        }
    }
}

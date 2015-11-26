using System;
using System.Collections.Generic;
using System.ComponentModel.DataAnnotations.Schema;

namespace FSS.Omnius.Entitron.Entity.Tapestry
{
    [Table("Tapestry_WorkFlowCommit")]
    public class WorkflowCommit
    {
        public int Id { get; set; }
        public string CommitMessage { get; set; }
        public DateTime Timestamp { get; set; }
        public virtual ICollection<Activity> Activities { get; set; }

        public virtual Workflow Workflow { get; set; }

        public WorkflowCommit()
        {
            Activities = new List<Activity>();
        }
    }
}
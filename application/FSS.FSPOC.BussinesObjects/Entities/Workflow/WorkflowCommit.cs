using System;
using System.Collections.Generic;

namespace FSS.FSPOC.BussinesObjects.Entities.Workflow
{
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
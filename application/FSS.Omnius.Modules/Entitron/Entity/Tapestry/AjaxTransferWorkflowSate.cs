using System.Collections.Generic;

namespace FSS.Omnius.Modules.Entitron.Entity.Tapestry
{
    public class AjaxTransferWorkflowSate : IEntity
    {
        public string CommitMessage { get; set; }
        public List<AjaxTransferActivity> Activities { get; set; }
        public List<AjaxTransferConnection> Connections { get; set; }

        public int WorkflowId { get; set; }

        public AjaxTransferWorkflowSate()
        {
            Activities = new List<AjaxTransferActivity>();
            Connections = new List<AjaxTransferConnection>();
        }
    }
}

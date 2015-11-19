using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace FSS.Omnius.Entitron.Entity.Tapestry
{
    public class AjaxTransferWorkflowSate
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

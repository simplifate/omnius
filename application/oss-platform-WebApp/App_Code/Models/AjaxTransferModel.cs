using System.Collections.Generic;

namespace FSSWorkflowDesigner.Models
{
    public class AjaxTransferActivity
    {
        public int Id { get; set; }
        public int ActType { get; set; }
        public int PositionX { get; set; }
        public int PositionY { get; set; }
    }
    public class AjaxTransferConnection
    {
        public int Source { get; set; }
        public int SourceSlot { get; set; }
        public int Target { get; set; }
        public int TargetSlot { get; set; }
    }
    public class AjaxTransferWorkflowSate
    {
        public string CommitMessage { get; set; }
        public List<AjaxTransferActivity> Activities { get; set; }
        public List<AjaxTransferConnection> Connections { get; set; }

        public AjaxTransferWorkflowSate()
        {
            Activities = new List<AjaxTransferActivity>();
            Connections = new List<AjaxTransferConnection>();
        }
    }
    public class AjaxTransferCommitHeader
    {
        public int Id { get; set; }
        public string CommitMessage { get; set; }
        public string TimeString { get; set; }
    }
    public class AjaxTransferWorkflowHeader
    {
        public int Id { get; set; }
        public string Name { get; set; }
        public string TimeString { get; set; }
    }
}
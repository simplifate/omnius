using System;

namespace FSS.FSPOC.Entitron.Entity.Tapestry
{
    public class AjaxTransferWorkflowHeader
    {
        public int Id { get; set; }
        public string Name { get; set; }
        public string TimeString => CreationTime.ToString("d.M.yyyy H: mm:ss");
        public DateTime CreationTime { get; set; }
    }
}

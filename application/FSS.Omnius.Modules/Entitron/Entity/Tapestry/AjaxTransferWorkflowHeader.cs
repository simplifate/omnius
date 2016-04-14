using System;

namespace FSS.Omnius.Modules.Entitron.Entity.Tapestry
{
    public class AjaxTransferWorkflowHeader : IEntity
    {
        public int Id { get; set; }
        public string Name { get; set; }
        public string TimeString => CreationTime.ToString("d.M.yyyy H: mm:ss");
        public DateTime CreationTime { get; set; }
    }
}

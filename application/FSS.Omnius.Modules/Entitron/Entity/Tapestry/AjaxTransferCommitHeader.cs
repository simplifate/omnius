using System;

namespace FSS.Omnius.Modules.Entitron.Entity.Tapestry
{
    public class AjaxTransferCommitHeader : IEntity
    {
        public int Id { get; set; }
        public string CommitMessage { get; set; }

        public string TimeString => TimeCommit.ToString("d. M. yyyy H:mm:ss");

        public DateTime TimeCommit { get; set; }
    }
}

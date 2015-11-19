using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace FSS.Omnius.Entitron.Entity.Tapestry
{
    public class AjaxTransferCommitHeader
    {
        public int Id { get; set; }
        public string CommitMessage { get; set; }

        public string TimeString => TimeCommit.ToString("d. M. yyyy H:mm:ss");

        public DateTime TimeCommit { get; set; }
    }
}

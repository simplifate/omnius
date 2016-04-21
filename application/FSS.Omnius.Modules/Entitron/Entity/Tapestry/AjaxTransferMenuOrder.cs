using System.Collections.Generic;

namespace FSS.Omnius.Modules.Entitron.Entity.Tapestry
{
    public class AjaxTransferMenuOrder : IEntity
    {
        public Dictionary<int, int> Metablocks { get; set; }
        public Dictionary<int, int> Blocks { get; set; }

        public AjaxTransferMenuOrder()
        {
            Metablocks = new Dictionary<int, int>();
            Blocks = new Dictionary<int, int>();
        }
    }
}

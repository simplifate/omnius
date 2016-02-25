using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace FSS.Omnius.Modules.Entitron.Entity.Tapestry
{
    public class AjaxTransferMenuOrder
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

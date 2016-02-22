using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace FSS.Omnius.Modules.Entitron.Entity.Tapestry
{
    public class AjaxTransferTapestryActionList
    {
        public List<AjaxTransferTapestryActionItem> Items { get; set; }

        public AjaxTransferTapestryActionList()
        {
            Items = new List<AjaxTransferTapestryActionItem>();
        }
    }

    public class AjaxTransferTapestryActionItem
    {
        public int Id { get; set; }
        public int? ReverseActionId { get; set; }
        public string[] InputVars { get; set; }
        public string[] OutputVars { get; set; }
        public string Name { get; set; }
    }
}

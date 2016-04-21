using System.Collections.Generic;

namespace FSS.Omnius.Modules.Entitron.Entity.Tapestry
{
    public class AjaxTransferTapestryActionList : IEntity
    {
        public List<AjaxTransferTapestryActionItem> Items { get; set; }

        public AjaxTransferTapestryActionList()
        {
            Items = new List<AjaxTransferTapestryActionItem>();
        }
    }

    public class AjaxTransferTapestryActionItem : IEntity
    {
        public int Id { get; set; }
        public int? ReverseActionId { get; set; }
        public string[] InputVars { get; set; }
        public string[] OutputVars { get; set; }
        public string Name { get; set; }
    }
}

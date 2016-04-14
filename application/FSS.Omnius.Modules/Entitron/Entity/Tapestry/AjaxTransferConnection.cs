namespace FSS.Omnius.Modules.Entitron.Entity.Tapestry
{
    public class AjaxTransferConnection : IEntity
    {
        public int Source { get; set; }
        public int SourceSlot { get; set; }
        public int Target { get; set; }
        public int TargetSlot { get; set; }
    }
}
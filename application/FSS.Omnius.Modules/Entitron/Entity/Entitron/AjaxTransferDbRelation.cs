namespace FSS.Omnius.Modules.Entitron.Entity.Entitron
{
    public class AjaxTransferDbRelation : IEntity
    {
        public int Type { get; set; }
        public int SourceTable { get; set; }
        public int SourceColumn { get; set; }
        public int TargetTable { get; set; }
        public int TargetColumn { get; set; }
    }
}
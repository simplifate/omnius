namespace FSS.Omnius.Modules.Entitron.Entity.Entitron
{
    public class AjaxTransferDbRelation
    {
        public int Type { get; set; }
        public int LeftTable { get; set; }
        public int LeftColumn { get; set; }
        public int RightTable { get; set; }
        public int RightColumn { get; set; }
    }
}
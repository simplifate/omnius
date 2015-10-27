namespace FSPOC_WebProject.Models.DbDesigner
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
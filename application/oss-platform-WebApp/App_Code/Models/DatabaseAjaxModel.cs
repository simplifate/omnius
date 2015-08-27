using System.Collections.Generic;

namespace FSPOC.Models
{
    public class AjaxTransferDbColumn
    {
        public int Id { get; set; }
        public string Name { get; set; }
        public bool PrimaryKey { get; set; }
        public string Type { get; set; }
    }
    public class AjaxTransferDbTable
    {
        public int Id { get; set; }
        public string Name { get; set; }
        public int PositionX { get; set; }
        public int PositionY { get; set; }
        public List<AjaxTransferDbColumn> Columns { get; set; }

        public AjaxTransferDbTable()
        {
            Columns = new List<AjaxTransferDbColumn>();
        }
    }
    public class AjaxTransferDbRelation
    {
        public int Type { get; set; }
        public int LeftTable { get; set; }
        public int LeftColumn { get; set; }
        public int RightTable { get; set; }
        public int RightColumn { get; set; }
    }
    public class AjaxTransferDbScheme
    {
        public List<AjaxTransferDbTable> Tables { get; set; }
        public List<AjaxTransferDbRelation> Relations { get; set; }

        public AjaxTransferDbScheme()
        {
            Tables = new List<AjaxTransferDbTable>();
            Relations = new List<AjaxTransferDbRelation>();
        }
    }
}
using System;
using System.Collections.Generic;

namespace FSPOC.Models
{
    public class AjaxTransferDbColumn
    {
        public int Id { get; set; }
        public string Name { get; set; }
        public bool PrimaryKey { get; set; }
        public bool AllowNull { get; set; }
        public string Type { get; set; }
        public int ColumnLength { get; set; }
        public bool ColumnLengthIsMax { get; set; }
        public string DefaultValue { get; set; }
    }
    public class AjaxTransferDbIndex
    {
        public int Id { get; set; }
        public string Name { get; set; }
        public bool Unique { get; set; }
        public string FirstColumnName { get; set; }
        public string SecondColumnName { get; set; }
    }
    public class AjaxTransferDbTable
    {
        public int Id { get; set; }
        public string Name { get; set; }
        public int PositionX { get; set; }
        public int PositionY { get; set; }
        public List<AjaxTransferDbColumn> Columns { get; set; }
        public List<AjaxTransferDbIndex> Indices { get; set; }

        public AjaxTransferDbTable()
        {
            Columns = new List<AjaxTransferDbColumn>();
            Indices = new List<AjaxTransferDbIndex>();
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
    public class AjaxTransferDbView
    {
        public int Id { get; set; }
        public string Name { get; set; }
        public string Query { get; set; }
        public int PositionX { get; set; }
        public int PositionY { get; set; }
    }
    public class AjaxTransferDbScheme
    {
        public string CommitMessage { get; set; }
        public List<AjaxTransferDbTable> Tables { get; set; }
        public List<AjaxTransferDbRelation> Relations { get; set; }
        public List<AjaxTransferDbView> Views { get; set; }

        public AjaxTransferDbScheme()
        {
            Tables = new List<AjaxTransferDbTable>();
            Relations = new List<AjaxTransferDbRelation>();
            Views = new List<AjaxTransferDbView>();
        }
    }
}

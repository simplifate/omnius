using Newtonsoft.Json;
using System.ComponentModel.DataAnnotations.Schema;

namespace FSS.Omnius.Modules.Entitron.Entity.Entitron
{
    [Table("Entitron_DbRelation")]
    public class DbRelation : IEntity
    {
        [ImportExportIgnore(IsKey = true)]
        public int Id { get; set; }
        public string Name { get; set; }
        public int Type { get; set; }
        [ImportExportIgnore(IsLinkKey = true)]
        public int LeftTableId { get; set; }
        [ImportExportIgnore(IsLinkKey = true)]
        public int LeftColumnId { get; set; }
        [ImportExportIgnore(IsLinkKey = true)]
        public int RightTableId { get; set; }
        [ImportExportIgnore(IsLinkKey = true)]
        public int RightColumnId { get; set; }

        [ImportExportIgnore(IsLink = true)]
        public virtual DbTable LeftTable { get; set; }
        [ImportExportIgnore(IsLink = true)]
        public virtual DbColumn LeftColumn { get; set; }
        [ImportExportIgnore(IsLink = true)]
        public virtual DbTable RightTable { get; set; }
        [ImportExportIgnore(IsLink = true)]
        public virtual DbColumn RightColumn { get; set; }

        [ImportExportIgnore(IsParentKey = true)]
        public int DbSchemeCommitId { get; set; }
        [ImportExportIgnore(IsParent = true)]
        public virtual DbSchemeCommit DbSchemeCommit { get; set; }
    }
}
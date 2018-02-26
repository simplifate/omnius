using Newtonsoft.Json;
using System.ComponentModel.DataAnnotations.Schema;

namespace FSS.Omnius.Modules.Entitron.Entity.Entitron
{
    [Table("Entitron_DbRelation")]
    public class DbRelation : IEntity
    {
        public int Id { get; set; }

        public string Name { get; set; }
        public int Type { get; set; }
        [ImportExport(ELinkType.LinkRequired, typeof(DbTable))]
        public int LeftTableId { get; set; }
        [ImportExport(ELinkType.LinkRequired, typeof(DbColumn))]
        public int LeftColumnId { get; set; }
        [ImportExport(ELinkType.LinkRequired, typeof(DbTable))]
        public int RightTableId { get; set; }
        [ImportExport(ELinkType.LinkRequired, typeof(DbColumn))]
        public int RightColumnId { get; set; }

        [ImportExport(ELinkType.LinkRequired)]
        public virtual DbTable LeftTable { get; set; }
        [ImportExport(ELinkType.LinkRequired)]
        public virtual DbColumn LeftColumn { get; set; }
        [ImportExport(ELinkType.LinkRequired)]
        public virtual DbTable RightTable { get; set; }
        [ImportExport(ELinkType.LinkRequired)]
        public virtual DbColumn RightColumn { get; set; }

        [ImportExport(ELinkType.Parent, typeof(DbSchemeCommit))]
        public int DbSchemeCommitId { get; set; }
        [ImportExport(ELinkType.Parent)]
        public virtual DbSchemeCommit DbSchemeCommit { get; set; }
    }
}
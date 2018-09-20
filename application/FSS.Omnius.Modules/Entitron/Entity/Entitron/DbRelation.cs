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
        public int SourceTableId { get; set; }
        [ImportExport(ELinkType.LinkRequired, typeof(DbColumn))]
        public int SourceColumnId { get; set; }
        [ImportExport(ELinkType.LinkRequired, typeof(DbTable))]
        public int TargetTableId { get; set; }
        [ImportExport(ELinkType.LinkRequired, typeof(DbColumn))]
        public int TargetColumnId { get; set; }

        [ImportExport(ELinkType.LinkRequired)]
        public virtual DbTable SourceTable { get; set; }
        [ImportExport(ELinkType.LinkRequired)]
        public virtual DbColumn SourceColumn { get; set; }
        [ImportExport(ELinkType.LinkRequired)]
        public virtual DbTable TargetTable { get; set; }
        [ImportExport(ELinkType.LinkRequired)]
        public virtual DbColumn TargetColumn { get; set; }

        [ImportExport(ELinkType.Parent, typeof(DbSchemeCommit))]
        public int DbSchemeCommitId { get; set; }
        [ImportExport(ELinkType.Parent)]
        public virtual DbSchemeCommit DbSchemeCommit { get; set; }
    }
}
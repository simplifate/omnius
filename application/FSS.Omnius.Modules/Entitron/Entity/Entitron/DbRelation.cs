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
        public int LeftTable { get; set; }
        public int LeftColumn { get; set; }
        public int RightTable { get; set; }
        public int RightColumn { get; set; }

        /*public virtual DbTable LeftTable { get; set; }
        public virtual DbColumn LeftColumn { get; set; }
        public virtual DbTable RightTable { get; set; }
        public virtual DbColumn RightColumn { get; set; }*/

        [ImportExportIgnore(IsParentKey = true)]
        public int DbSchemeCommitId { get; set; }
        [ImportExportIgnore(IsParent = true)]
        public virtual DbSchemeCommit DbSchemeCommit { get; set; }
    }
}
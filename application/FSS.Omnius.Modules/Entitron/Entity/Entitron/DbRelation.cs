using System.ComponentModel.DataAnnotations.Schema;

namespace FSS.Omnius.Modules.Entitron.Entity.Entitron
{
    [Table("Entitron_DbRelation")]
    public class DbRelation
    {
        public int Id { get; set; }
        public string Name { get; set; }
        public int Type { get; set; }
        public int LeftTable { get; set; }
        public int LeftColumn { get; set; }
        public int RightTable { get; set; }
        public int RightColumn { get; set; }

        //public virtual DbTable SourceTable { get; set; }
        //public virtual DbColumn SourceColumn { get; set; }
        //public virtual DbTable TargetTable { get; set; }
        //public virtual DbColumn TargetColumn { get; set; }


        public int DbSchemeCommitId { get; set; }
        public virtual DbSchemeCommit DbSchemeCommit { get; set; }
    }
}
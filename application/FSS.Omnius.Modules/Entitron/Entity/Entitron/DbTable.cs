using Newtonsoft.Json;
using System.Collections.Generic;
using System.ComponentModel.DataAnnotations.Schema;
namespace FSS.Omnius.Modules.Entitron.Entity.Entitron
{
    [Table("Entitron_DbTable")]
    public class DbTable : IEntity
    {
        [ImportExportIgnore(IsKey = true)]
        public int Id { get; set; }
        public string Name { get; set; }
        public int PositionX { get; set; }
        public int PositionY { get; set; }

        public virtual ICollection<DbColumn> Columns { get; set; }
        public virtual ICollection<DbIndex> Indices { get; set; }

        [ImportExportIgnore(IsParentKey = true)]
        public int DbSchemeCommitId { get; set; }
        [ImportExportIgnore(IsParent = true)]
        public virtual DbSchemeCommit DbSchemeCommit { get; set; }

        public DbTable()
        {
            Columns = new List<DbColumn>();
            Indices = new List<DbIndex>();
        }
    }
}
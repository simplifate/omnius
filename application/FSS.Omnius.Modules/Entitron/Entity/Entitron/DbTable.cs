using Newtonsoft.Json;
using System.Collections.Generic;
using System.ComponentModel.DataAnnotations.Schema;
namespace FSS.Omnius.Modules.Entitron.Entity.Entitron
{
    [Table("Entitron_DbTable")]
    public class DbTable : IEntity
    {
        public int Id { get; set; }

        public string Name { get; set; }
        public int PositionX { get; set; }
        public int PositionY { get; set; }

        [ImportExport(ELinkType.Child)]
        public virtual ICollection<DbColumn> Columns { get; set; }
        [ImportExport(ELinkType.Child)]
        public virtual ICollection<DbIndex> Indices { get; set; }

        [ImportExport(ELinkType.Parent, typeof(DbSchemeCommit))]
        public int DbSchemeCommitId { get; set; }
        [ImportExport(ELinkType.Parent)]
        public virtual DbSchemeCommit DbSchemeCommit { get; set; }

        public DbTable()
        {
            Columns = new List<DbColumn>();
            Indices = new List<DbIndex>();
        }
    }
}
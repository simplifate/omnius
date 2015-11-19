using System.Collections.Generic;

namespace FSS.Omnius.Entitron.Entity.Entitron
{
    public class DbTable
    {
        public int Id { get; set; }
        public string Name { get; set; }
        public int PositionX { get; set; }
        public int PositionY { get; set; }

        public virtual ICollection<DbColumn> Columns { get; set; }
        public virtual ICollection<DbIndex> Indices { get; set; }

        public virtual DbSchemeCommit DbSchemeCommit { get; set; }

        public DbTable()
        {
            Columns = new List<DbColumn>();
            Indices = new List<DbIndex>();
        }
    }
}
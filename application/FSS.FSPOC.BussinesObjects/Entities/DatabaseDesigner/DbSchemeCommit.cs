using System;
using System.Collections.Generic;

namespace FSS.FSPOC.BussinesObjects.Entities.DatabaseDesigner
{
    public class DbSchemeCommit
    {
        public int Id { get; set; }
        public string CommitMessage { get; set; }
        public DateTime Timestamp { get; set; }

        public virtual ICollection<DbTable> Tables { get; set; }
        public virtual ICollection<DbRelation> Relations { get; set; }
        public virtual ICollection<DbView> Views { get; set; }

        public DbSchemeCommit()
        {
            Tables    = new List<DbTable>();
            Relations = new List<DbRelation>();
            Views     = new List<DbView>();
        }
    }
}

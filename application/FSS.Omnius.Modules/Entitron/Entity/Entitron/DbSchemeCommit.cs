using Newtonsoft.Json;
using System;
using System.Collections.Generic;
using System.ComponentModel.DataAnnotations.Schema;
using FSS.Omnius.Modules.Entitron.Entity.Master;

namespace FSS.Omnius.Modules.Entitron.Entity.Entitron
{
    [Table("Entitron_DbSchemeCommit")]
    public class DbSchemeCommit : IEntity
    {
        public int Id { get; set; }

        public string CommitMessage { get; set; }
        public DateTime Timestamp { get; set; }
        public bool IsComplete { get; set; }

        [ImportExport(ELinkType.Child)]
        public virtual ICollection<DbTable> Tables { get; set; }
        [ImportExport(ELinkType.Child)]
        public virtual ICollection<DbRelation> Relations { get; set; }
        [ImportExport(ELinkType.Child)]
        public virtual ICollection<DbView> Views { get; set; }

        [ImportExport(ELinkType.Parent, typeof(Application), exportCount = 3, exportOrderColumn = "Timestamp")]
        public int Application_Id { get; set; }
        [ImportExport(ELinkType.Parent)]
        public virtual Application Application { get; set; }

        public DbSchemeCommit()
        {
            Tables    = new List<DbTable>();
            Relations = new List<DbRelation>();
            Views     = new List<DbView>();
            IsComplete = false;
        }
    }
}

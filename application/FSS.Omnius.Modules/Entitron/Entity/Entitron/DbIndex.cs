using Newtonsoft.Json;
using System.Collections.Generic;
using System.ComponentModel.DataAnnotations.Schema;

namespace FSS.Omnius.Modules.Entitron.Entity.Entitron
{
    [Table("Entitron_DbIndex")]
    public class DbIndex : IEntity
    {
        public int Id { get; set; }

        public string Name { get; set; }
        public bool Unique { get; set; }
        public string ColumnNames { get; set; }

        [ImportExport(ELinkType.Parent, typeof(DbTable))]
        public int DbTableId { get; set; }
        [ImportExport(ELinkType.Parent)]
        public virtual DbTable DbTable { get; set; }
    }
}
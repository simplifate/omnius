using Newtonsoft.Json;
using System.Collections.Generic;
using System.ComponentModel.DataAnnotations.Schema;

namespace FSS.Omnius.Modules.Entitron.Entity.Entitron
{
    [Table("Entitron_DbIndex")]
    public class DbIndex : IEntity
    {
        [ImportExportIgnore(IsKey = true)]
        public int Id { get; set; }
        public string Name { get; set; }
        public bool Unique { get; set; }
        public string ColumnNames { get; set; }

        [ImportExportIgnore(IsParentKey = true)]
        public int DbTableId { get; set; }
        [ImportExportIgnore(IsParent = true)]
        public virtual DbTable DbTable { get; set; }
    }
}
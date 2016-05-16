using Newtonsoft.Json;
using System.Collections.Generic;
using System.ComponentModel.DataAnnotations.Schema;

namespace FSS.Omnius.Modules.Entitron.Entity.Entitron
{
    [Table("Entitron_DbIndex")]
    public class DbIndex : IEntity
    {
        [ImportIgnore]
        public int Id { get; set; }
        public string Name { get; set; }
        public bool Unique { get; set; }
        public string ColumnNames { get; set; }

        [ImportExportIgnore]
        public int DbTableId { get; set; }
        [ImportExportIgnore]
        public virtual DbTable DbTable { get; set; }
    }
}
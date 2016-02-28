using Newtonsoft.Json;
using System.Collections.Generic;
using System.ComponentModel.DataAnnotations.Schema;

namespace FSS.Omnius.Modules.Entitron.Entity.Entitron
{
    [Table("Entitron_DbIndex")]
    public class DbIndex
    {
        public int Id { get; set; }
        public string Name { get; set; }
        public bool Unique { get; set; }
        public string ColumnNames { get; set; }

        public int DbTableId { get; set; }
        [JsonIgnore]
        public virtual DbTable DbTable { get; set; }
    }
}
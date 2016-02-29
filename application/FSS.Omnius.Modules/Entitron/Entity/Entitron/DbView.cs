using Newtonsoft.Json;
using System.ComponentModel.DataAnnotations.Schema;

namespace FSS.Omnius.Modules.Entitron.Entity.Entitron
{
    [Table("Entitron_DbView")]
    public class DbView
    {
        public int Id { get; set; }
        public string Name { get; set; }
        public string Query { get; set; }
        public int PositionX { get; set; }
        public int PositionY { get; set; }

        public int DbSchemeCommitId { get; set; }
        [JsonIgnore]
        public virtual DbSchemeCommit DbSchemeCommit { get; set; }
    }
}
using System.ComponentModel.DataAnnotations.Schema;
using FSS.Omnius.Modules.Entitron.Entity.Master;
using Newtonsoft.Json;
using System.ComponentModel.DataAnnotations;

namespace FSS.Omnius.Modules.Entitron.Entity.Entitron
{
    [Table("Entitron_ColumnMetadata")]
    public class ColumnMetadata : IEntity
    {
        public int Id { get; set; }
        
        public string TableName { get; set; }
        public string ColumnName { get; set; }
        public string ColumnDisplayName { get; set; }

        [ForeignKey("Application")]
        [ImportExport(ELinkType.Parent, typeof(Application))]
        public int Application_Id { get; set; } 
        [ImportExport(ELinkType.Parent)]
        public virtual Application Application { get; set; }
    }
}

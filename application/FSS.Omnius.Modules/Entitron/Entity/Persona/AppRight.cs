using System;
using System.Collections.Generic;
using System.ComponentModel.DataAnnotations;
using System.ComponentModel.DataAnnotations.Schema;
using System.Linq;
namespace FSS.Omnius.Modules.Entitron.Entity.Persona
{
    using Master;

    [Table("Persona_AppRights")]
    public partial class AppRight
    {
        [Key]
        [Column(Order = 1)]
        public int GroupId { get; set; }
        [Key]
        [Column(Order = 2)]
        public int ApplicationId { get; set; }

        public bool Readable { get; set; }
        public bool Executable { get; set; }

        public virtual Group Group { get; set; }
        public virtual Application Application { get; set; }
    }
}

using System;
using System.Collections.Generic;
using System.ComponentModel.DataAnnotations;
using System.ComponentModel.DataAnnotations.Schema;
namespace FSS.Omnius.Modules.Entitron.Entity.Persona
{
    using Master;
    [Table("Persona_AppRights")]
    public partial class AppRight
    {
        [Key]
        [Column(Order = 1)]
        public int UserId { get; set; }
        [Key]
        [Column(Order = 2)]
        public int ApplicationId { get; set; }

        public bool hasAccess { get; set; }

        public virtual User User { get; set; }
        public virtual Application Application { get; set; }
    }
}

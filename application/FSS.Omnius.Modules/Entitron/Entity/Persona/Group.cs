using System;
using System.Collections.Generic;
namespace FSS.Omnius.Modules.Entitron.Entity.Persona
{
    using System.ComponentModel.DataAnnotations;
    using System.ComponentModel.DataAnnotations.Schema;
    using Master;

    [Table("Persona_Groups")]
    public partial class Group
    {
        public int Id { get; set; }
        [Required]
        [StringLength(50)]
        public string Name { get; set; }

        public virtual ICollection<User> Users { get; set; }
        public virtual ICollection<ActionRuleRight> ActionRuleRights { get; set; }
        public virtual ICollection<AppRight> ApplicationRights { get; set; }
    }
}

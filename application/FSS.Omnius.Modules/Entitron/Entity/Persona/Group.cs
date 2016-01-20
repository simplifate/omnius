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
        public Group()
        {
            Children = new HashSet<Group>();
            Users = new HashSet<User>();
            ActionRuleRights = new HashSet<ActionRuleRight>();
            ApplicationRights = new HashSet<AppRight>();
        }

        public int Id { get; set; }
        [Required]
        [StringLength(50)]
        public string Name { get; set; }
        [Required]
        public bool IsFromAD { get; set; }
        public int? ParentId { get; set; }

        public virtual Group Parent { get; set; }
        public virtual ICollection<Group> Children { get; set; }
        public virtual ICollection<User> Users { get; set; }
        public virtual ICollection<ActionRuleRight> ActionRuleRights { get; set; }
        public virtual ICollection<AppRight> ApplicationRights { get; set; }
    }
}

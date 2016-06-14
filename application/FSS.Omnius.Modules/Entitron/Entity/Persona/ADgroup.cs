using System.Collections.Generic;
using System.ComponentModel.DataAnnotations;
using System.ComponentModel.DataAnnotations.Schema;

namespace FSS.Omnius.Modules.Entitron.Entity.Persona
{
    using Master;

    [Table("Persona_ADgroups")]
    public partial class ADgroup : IEntity
    {
        public ADgroup()
        {
            ADgroup_Users = new HashSet<ADgroup_User>();
        }

        public int Id { get; set; }

        [Required]
        [StringLength(50)]
        public string Name { get; set; }
        public bool isAdmin { get; set; }
        
        public int? ApplicationId { get; set; }
        public virtual Application Application { get; set; }
        public virtual ICollection<ADgroup_User> ADgroup_Users { get; set; }
    }
}

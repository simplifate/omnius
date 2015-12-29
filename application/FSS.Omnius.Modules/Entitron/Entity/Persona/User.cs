using System;
using System.Collections.Generic;
using System.ComponentModel.DataAnnotations;
using System.ComponentModel.DataAnnotations.Schema;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace FSS.Omnius.Modules.Entitron.Entity.Persona
{
    [Table("Persona_Users")]
    public partial class User
    {
        public int Id { get; set; }
        [Required]
        [Index(IsUnique = true)]
        [StringLength(50)]
        public string username { get; set; }
        [Required]
        [StringLength(100)]
        public string DisplayName { get; set; }
        [StringLength(100)]
        public string Email { get; set; }
        [StringLength(100)]
        public string Company { get; set; }
        [StringLength(100)]
        public string Department { get; set; }
        [StringLength(100)]
        public string Team { get; set; }
        [StringLength(20)]
        public string WorkPhone { get; set; }
        [StringLength(20)]
        public string MobilPhone { get; set; }
        [StringLength(500)]
        public string Address { get; set; }
        [StringLength(100)]
        public string Job { get; set; }
        public DateTime LastLogin { get; set; }
        [Required]
        public DateTime localExpiresAt { get; set; }

        public virtual ICollection<Group> Groups { get; set; }
        public virtual ModuleAccessPermission ModuleAccessPermission { get; set; }
    }
}

using System.Collections.Generic;

namespace FSS.Omnius.Entitron.Entity.Nexus
{
    using System.ComponentModel.DataAnnotations;
    using System.ComponentModel.DataAnnotations.Schema;
    using Master;

    [Table("Nexus_Ldap")]
    public partial class Ldap
    {
        public int Id { get; set; }

        [StringLength(50)]
        public string Domain_Ntlm { get; set; }

        [StringLength(255)]
        public string Domain_Kerberos { get; set; }

        [Required]
        [Display(Name = "Server")]
        public string Domain_Server { get; set; }

        [Required]
        [Display(Name = "Uživatel")]
        public string Bind_User { get; set; }

        public string Bind_Password { get; set; }

        public bool Active { get; set; }
        public bool Use_SSL { get; set; }
    }
}

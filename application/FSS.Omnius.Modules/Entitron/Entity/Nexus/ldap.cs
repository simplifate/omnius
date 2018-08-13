namespace FSS.Omnius.Modules.Entitron.Entity.Nexus
{
    using System.ComponentModel.DataAnnotations;
    using System.ComponentModel.DataAnnotations.Schema;

    [Table("Nexus_Ldap")]
    public partial class Ldap : IEntity
    {
        public int? Id { get; set; }

        [StringLength(50)]
        [Display(Name = "NTLM domain")]
        public string Domain_Ntlm { get; set; }

        [StringLength(255)]
        [Display(Name = "Kerberos domain")]
        public string Domain_Kerberos { get; set; }

        [Required]
        [Display(Name = "Server")]
        public string Domain_Server { get; set; }

        [Required]
        [Display(Name = "User")]
        public string Bind_User { get; set; }

        [Required]
        [Display(Name = "Password")]
        public string Bind_Password { get; set; }

        [Display(Name = "Is active")]
        public bool Active { get; set; }

        [Display(Name = "Use SSL")]
        public bool Use_SSL { get; set; }

        [Display(Name = "Is default")]
        public bool Is_Default { get; set; }
    }
}

namespace FSS.Omnius.Modules.Entitron.Entity.Nexus
{
    using System.ComponentModel.DataAnnotations;
    using System.ComponentModel.DataAnnotations.Schema;

    [Table("Nexus_Ldap")]
    public partial class Ldap
    {
        public int? Id { get; set; }

        [StringLength(50)]
        [Display(Name = "NTLM doména")]
        public string Domain_Ntlm { get; set; }

        [StringLength(255)]
        [Display(Name = "Kerberos doména")]
        public string Domain_Kerberos { get; set; }

        [Required]
        [Display(Name = "Server")]
        public string Domain_Server { get; set; }

        [Required]
        [Display(Name = "Uživatel")]
        public string Bind_User { get; set; }

        [Required]
        [Display(Name = "Heslo")]
        public string Bind_Password { get; set; }

        [Display(Name = "Aktivní")]
        public bool Active { get; set; }

        [Display(Name = "Použít SSL")]
        public bool Use_SSL { get; set; }

        [Display(Name = "Výchozí")]
        public bool Is_Default { get; set; }
    }
}

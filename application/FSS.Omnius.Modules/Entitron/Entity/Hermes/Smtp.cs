namespace FSS.Omnius.Modules.Entitron.Entity.Hermes
{
    using System.ComponentModel.DataAnnotations;
    using System.ComponentModel.DataAnnotations.Schema;

    [Table("Hermes_Smtp")]
    public partial class Smtp
    {
        public int? Id { get; set; }

        [Required]
        [StringLength(100)]
        [Index(IsClustered = false, IsUnique = true)]
        [Display(Name = "Název")]
        public string Name { get; set; }

        [Required]
        [StringLength(255)]
        [Display(Name = "Server")]
        public string Server { get; set; }

        [Required]
        [StringLength(255)]
        [Display(Name = "Uživatelské jméno")]
        public string Auth_User { get; set; }

        [Required]
        [StringLength(255)]
        [Display(Name = "Heslo")]
        public string Auth_Password { get; set; }

        [Index(IsClustered = false, IsUnique = false)]
        [Display(Name = "Šifrované spojení")]
        public bool Use_SSL { get; set; }

        [Index(IsClustered = false, IsUnique = false)]
        [Display(Name = "Výchozí")]
        public bool Is_Default { get; set; }
    }
}

namespace FSS.Omnius.Modules.Entitron.Entity.Hermes
{
    using System.ComponentModel.DataAnnotations;
    using System.ComponentModel.DataAnnotations.Schema;

    [Table("Hermes_Smtp")]
    public partial class Smtp : IEntity
    {
        public int? Id { get; set; }

        [Required]
        [StringLength(100)]
        [Index(IsClustered = false, IsUnique = true)]
        [Display(Name = "Name")]
        public string Name { get; set; }

        [Required]
        [StringLength(255)]
        [Display(Name = "Server")]
        public string Server { get; set; }

        [StringLength(255)]
        [Display(Name = "User name")]
        public string Auth_User { get; set; }

        [StringLength(255)]
        [Display(Name = "Password")]
        public string Auth_Password { get; set; }

        [Index(IsClustered = false, IsUnique = false)]
        [Display(Name = "Using SSL")]
        public bool Use_SSL { get; set; }

        [Index(IsClustered = false, IsUnique = false)]
        [Display(Name = "Is default")]
        public bool Is_Default { get; set; }
    }
}

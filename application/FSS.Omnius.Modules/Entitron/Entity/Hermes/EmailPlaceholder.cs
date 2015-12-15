namespace FSS.Omnius.Modules.Entitron.Entity.Hermes
{
    using System.ComponentModel.DataAnnotations;
    using System.ComponentModel.DataAnnotations.Schema;

    [Table("Hermes_Email_Placeholder")]
    public partial class EmailPlaceholder
    {
        public int? Id { get; set; }

        [Index(IsClustered = false, IsUnique = false)]
        [ForeignKey("Hermes_Email_Template")]
        public int? Hermes_Email_Template_Id { get; set; }

        [Required]
        [StringLength(255)]
        [Index(IsClustered = false, IsUnique = false)]
        [RegularExpression("/^[a-Z0-9_]+$/")]
        public string Prop_Name { get; set; }

        [Required]
        [StringLength(255)]
        public string Description { get; set; }

        public int Num_Order { get; set; }

        public EmailTemplate Hermes_Email_Template { get; set; }
    }
}

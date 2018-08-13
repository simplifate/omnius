namespace FSS.Omnius.Modules.Entitron.Entity.Hermes
{
    using System.ComponentModel.DataAnnotations;
    using System.ComponentModel.DataAnnotations.Schema;

    [Table("Hermes_Email_Placeholder")]
    public partial class EmailPlaceholder : IEntity
    {
        public int Id { get; set; }
        
        [Required]
        [StringLength(255)]
        [Index(IsClustered = false, IsUnique = false)]
        [RegularExpression("^[a-zA-Z0-9_.@]+$")]
        [Display(Name = "Variable")]
        public string Prop_Name { get; set; }
        [Required]
        [StringLength(255)]
        [Display(Name = "Description")]
        public string Description { get; set; }
        public int Num_Order { get; set; }

        [Index(IsClustered = false, IsUnique = false)]
        [ForeignKey("Hermes_Email_Template")]
        [ImportExport(ELinkType.Parent, typeof(EmailTemplate))]
        public int? Hermes_Email_Template_Id { get; set; }
        [ImportExport(ELinkType.Parent)]
        public EmailTemplate Hermes_Email_Template { get; set; }
    }
}

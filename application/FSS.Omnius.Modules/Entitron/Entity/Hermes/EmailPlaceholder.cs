namespace FSS.Omnius.Modules.Entitron.Entity.Hermes
{
    using System.ComponentModel.DataAnnotations;
    using System.ComponentModel.DataAnnotations.Schema;
    using Newtonsoft.Json;

    [Table("Hermes_Email_Placeholder")]
    public partial class EmailPlaceholder : IEntity
    {
        [ImportExportIgnore(IsKey = true)]
        public int? Id { get; set; }

        [Index(IsClustered = false, IsUnique = false)]
        [ForeignKey("Hermes_Email_Template")]
        [ImportExportIgnore(IsParentKey = true)]
        public int? Hermes_Email_Template_Id { get; set; }

        [Required]
        [StringLength(255)]
        [Index(IsClustered = false, IsUnique = false)]
        [RegularExpression("^[a-zA-Z0-9_.@]+$")]
        [Display(Name = "Poměnná")]
        public string Prop_Name { get; set; }

        [Required]
        [StringLength(255)]
        [Display(Name = "Popis")]
        public string Description { get; set; }

        public int Num_Order { get; set; }

        [ImportExportIgnore(IsParent = true)]
        public EmailTemplate Hermes_Email_Template { get; set; }
    }
}

namespace FSS.Omnius.Modules.Entitron.Entity.Hermes
{
    using System.Collections.Generic;
    using System.ComponentModel.DataAnnotations;
    using System.ComponentModel.DataAnnotations.Schema;
    
    [Table("Hermes_Email_Template")]
    public partial class EmailTemplate
    {
        public int? Id { get; set; }

        public int? AppId { get; set; }

        [Required]
        [StringLength(255)]
        [Index(IsClustered = false, IsUnique = true)]
        [Display(Name = "Název")]
        public string Name { get; set; }

        [Display(Name = "HTML e-mail")]
        public bool Is_HTML { get; set; }

        public virtual ICollection<EmailPlaceholder> PlaceholderList { get; set; }
        public virtual ICollection<EmailTemplateContent> ContentList { get; set; }
    }
}

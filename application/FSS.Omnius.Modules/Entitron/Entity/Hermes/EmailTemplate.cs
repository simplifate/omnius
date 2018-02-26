namespace FSS.Omnius.Modules.Entitron.Entity.Hermes
{
    using Master;
    using System.Collections.Generic;
    using System.ComponentModel.DataAnnotations;
    using System.ComponentModel.DataAnnotations.Schema;
    using Newtonsoft.Json;

    [Table("Hermes_Email_Template")]
    public partial class EmailTemplate : IEntity
    {
        public int Id { get; set; }
        
        [Required]
        [StringLength(255)]
        [Index("HermesUniqueness", IsClustered = false, IsUnique = true, Order = 2)]
        [Display(Name = "Název")]
        public string Name { get; set; }

        [Display(Name = "HTML e-mail")]
        public bool Is_HTML { get; set; }

        [ImportExport(ELinkType.Child)]
        public virtual ICollection<EmailPlaceholder> PlaceholderList { get; set; }
        [ImportExport(ELinkType.Child)]
        public virtual ICollection<EmailTemplateContent> ContentList { get; set; }

        [Index("HermesUniqueness", IsClustered = false, IsUnique = true, Order = 1)]
        [ForeignKey("Application")]
        [ImportExport(ELinkType.Parent, typeof(Application))]
        public int? AppId { get; set; }
        [ImportExport(ELinkType.Parent)]
        public virtual Application Application { get; set; }
        
        public EmailTemplate()
        {
            PlaceholderList = new HashSet<EmailPlaceholder>();
            ContentList = new HashSet<EmailTemplateContent>();
        }
    }
}

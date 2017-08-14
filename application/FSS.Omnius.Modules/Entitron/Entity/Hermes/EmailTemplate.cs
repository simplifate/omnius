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
        [ImportExportIgnore(IsKey = true)]
        public int? Id { get; set; }

        [Index(IsClustered = false, IsUnique = false)]
        [ForeignKey("Application")]
        [ImportExportIgnore(IsParentKey = true)]
        public int? AppId { get; set; }

        [Required]
        [StringLength(255)]
        [Index(IsClustered = false, IsUnique = true)]
        [Display(Name = "Název")]
        public string Name { get; set; }

        [Display(Name = "HTML e-mail")]
        public bool Is_HTML { get; set; }

        [ImportExportIgnore(IsParent = true)]
        public virtual Application Application { get; set; }

        public virtual ICollection<EmailPlaceholder> PlaceholderList { get; set; }
        public virtual ICollection<EmailTemplateContent> ContentList { get; set; }

        public EmailTemplate()
        {
            PlaceholderList = new HashSet<EmailPlaceholder>();
            ContentList = new HashSet<EmailTemplateContent>();
        }
    }
}

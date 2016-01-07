namespace FSS.Omnius.Modules.Entitron.Entity.Hermes
{
    using System.Collections.Generic;
    using System.ComponentModel.DataAnnotations;
    using System.ComponentModel.DataAnnotations.Schema;
    using System.Web.Mvc;

    [Table("Hermes_Email_Template_Content")]
    [MetadataType(typeof(Hermes_Email_Template_Content_Metadata))]
    public partial class EmailTemplateContent
    {
        public int? Id { get; set; }

        [Index(IsClustered = false, IsUnique = false)]
        [ForeignKey("Hermes_Email_Template")]
        public int? Hermes_Email_Template_Id { get; set; }

        [Index(IsClustered = false, IsUnique = false)]
        public int? LanguageId { get; set; }

        [Display(Name = "Jméno odesílatele")]
        [MaxLength(255)]
        public string From_Name { get; set; }

        [Display(Name = "E-mail odesílatele")]
        [MaxLength(1000)]
        public string From_Email { get; set; }

        [Display(Name = "Předmět")]
        [MaxLength(1000)]
        public string Subject { get; set; }

        [Display(Name = "Obsah")]
        [DataType(DataType.Text)]
        [AllowHtml]
        public string Content { get; set; }

        public EmailTemplate Hermes_Email_Template { get; set; }
    }

    public class Hermes_Email_Template_Content_Metadata
    {
        [AllowHtml]
        public string Content { get; set; }
    }
}

namespace FSS.Omnius.Modules.Entitron.Entity.Hermes
{
    using System.ComponentModel.DataAnnotations;
    using System.ComponentModel.DataAnnotations.Schema;
    using Master;
    using Newtonsoft.Json;
    using Tapestry;

    [Table("Hermes_Incoming_Email_Rule")]
    public partial class IncomingEmailRule : IEntity
    {
        [ImportExportIgnore(IsKey = true)]
        public int? Id { get; set; }

        [Required]
        [ImportExportIgnore(IsParentKey = true)]
        public int IncomingEmailId { get; set; }

        [Required]
        [Display(Name = "Aplikace")]
        [ImportExportIgnore(IsLinkKey = true)]
        public int ApplicationId { get; set; }

        [Required]
        [Display(Name = "Blok")]
        public string BlockName { get; set; }

        [Required]
        [Display(Name = "Workflow")]
        public string WorkflowName { get; set; }
        
        [Required]
        [Display(Name = "Název")]
        public string Name { get; set; }
        
        [DataType(DataType.MultilineText)]
        public string Rule { get; set; }

        [ImportExportIgnore(IsParent = true)]
        public virtual IncomingEmail IncomingEmail { get; set; }
        [ImportExportIgnore(IsLink = true)]
        public virtual Application Application { get; set; }
    }
}

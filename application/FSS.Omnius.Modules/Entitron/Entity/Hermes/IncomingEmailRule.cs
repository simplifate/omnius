namespace FSS.Omnius.Modules.Entitron.Entity.Hermes
{
    using System.ComponentModel.DataAnnotations;
    using System.ComponentModel.DataAnnotations.Schema;
    using Master;
    using Newtonsoft.Json;

    [Table("Hermes_Incoming_Email_Rule")]
    public partial class IncomingEmailRule : IEntity
    {
        public int? Id { get; set; }

        [Required]
        [Display(Name = "Block")]
        public string BlockName { get; set; }

        [Required]
        [Display(Name = "Workflow")]
        public string WorkflowName { get; set; }
        
        [Required]
        [Display(Name = "Name")]
        public string Name { get; set; }
        
        [DataType(DataType.MultilineText)]
        public string Rule { get; set; }

        [Required]
        [ImportExport(ELinkType.LinkRequired, typeof(IncomingEmail))]
        public int IncomingEmailId { get; set; }
        [ImportExport(ELinkType.LinkRequired)]
        public virtual IncomingEmail IncomingEmail { get; set; }

        [Required]
        [Display(Name = "Application")]
        [ImportExport(ELinkType.Parent, typeof(Application))]
        public int ApplicationId { get; set; }
        [ImportExport(ELinkType.Parent)]
        public virtual Application Application { get; set; }
    }
}

namespace FSS.Omnius.Modules.Entitron.Entity.Hermes
{
    using System.ComponentModel.DataAnnotations;
    using System.ComponentModel.DataAnnotations.Schema;
    using Master;
    using Tapestry;

    [Table("Hermes_Incoming_Email_Rule")]
    public partial class IncomingEmailRule : IEntity
    {
        public int? Id { get; set; }

        [Required]
        public int IncomingEmailId { get; set; }

        [Required]
        public int ApplicationId { get; set; }

        [Required]
        public string BlockName { get; set; }

        [Required]
        public string WorkflowName { get; set; }
        
        [Required]
        [Display(Name = "Název")]
        public string Name { get; set; }
        
        [DataType(DataType.MultilineText)]
        public string Rule { get; set; }

        public virtual IncomingEmail IncomingEmail { get; set; }
        public virtual Application Application { get; set; }
    }
}

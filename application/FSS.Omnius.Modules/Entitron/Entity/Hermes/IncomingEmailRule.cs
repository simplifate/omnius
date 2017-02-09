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

        public int IncomingEmailId { get; set; }
        public int ApplicationId { get; set; }
        public string BlockName { get; set; }
        public string WorkflowName { get; set; }
        
        [DataType(DataType.MultilineText)]
        public string Rule { get; set; }

        public virtual IncomingEmail IncomingEmail { get; set; }
        public virtual Application Application { get; set; }
    }
}

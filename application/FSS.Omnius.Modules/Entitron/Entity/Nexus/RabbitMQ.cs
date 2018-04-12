namespace FSS.Omnius.Modules.Entitron.Entity.Nexus
{
    using Master;
    using System.ComponentModel.DataAnnotations;

    public enum ChannelType
    {
        SEND,
        RECEIVE
    }

    public class RabbitMQ : IEntity
    {
        [Key]
        public int? Id { get; set; }

        [Required]
        [Display(Name = "Name")]
        public string Name { get; set; }

        [Required]
        [Display(Name = "Hostname")]
        public string HostName { get; set; }

        [Required]
        [Display(Name = "Queue name")]
        public string QueueName { get; set; }

        [Required]
        [Display(Name = "Type")]
        public ChannelType Type { get; set; }

        [Required]
        [Display(Name = "Block")]
        public string BlockName { get; set; }

        [Required]
        [Display(Name = "Workflow")]
        public string WorkflowName { get; set; }

        [Display(Name = "Application")]
        [ImportExport(ELinkType.Parent, typeof(Application))]
        public int ApplicationId { get; set; }

        [ImportExport(ELinkType.Parent)]
        public virtual Application Application { get; set; }
    }
}

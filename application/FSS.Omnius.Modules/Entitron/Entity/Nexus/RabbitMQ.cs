namespace FSS.Omnius.Modules.Entitron.Entity.Nexus
{
    using Master;
    using System.ComponentModel.DataAnnotations;
    using System.ComponentModel.DataAnnotations.Schema;

    public enum ChannelType
    {
        SEND,
        RECEIVE
    }

    [Table("Nexus_RabbitMQ")]
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
        [Display(Name = "Port")]
        public int Port { get; set; }

        [Required]
        [Display(Name = "Queue name")]
        public string QueueName { get; set; }

        [Required]
        [Display(Name = "Type")]
        public ChannelType Type { get; set; }

        [Display(Name = "Username")]
        public string UserName { get; set; }
        
        [Display(Name = "Password")]
        public string Password { get; set; }

        [Display(Name = "Block")]
        public string BlockName { get; set; }

        [Display(Name = "Workflow")]
        public string WorkflowName { get; set; }

        [Display(Name = "Application")]
        [ImportExport(ELinkType.Parent, typeof(Application))]
        public int? ApplicationId { get; set; }

        [ImportExport(ELinkType.Parent)]
        public virtual Application Application { get; set; }
    }
}

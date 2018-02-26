namespace FSS.Omnius.Modules.Entitron.Entity.Nexus
{
    using System.ComponentModel.DataAnnotations;
    using System.ComponentModel.DataAnnotations.Schema;
    using Master;
    using Newtonsoft.Json;
    using Tapestry;

    [Table("Nexus_TCP_Socket_Listener")]
    public partial class TCPSocketListener : IEntity
    {
        public int? Id { get; set; }
        
        [Display(Name = "TCP Port")]
        public int Port { get; set; }
        [Display(Name = "Velikost bufferu")]
        public int BufferSize { get; set; }
        [Required]
        [Display(Name = "Blok")]
        public string BlockName { get; set; }
        [Required]
        [Display(Name = "Workflow")]
        public string WorkflowName { get; set; }
        [Required]
        [Display(Name = "Název")]
        public string Name { get; set; }

        [Display(Name = "Aplikace")]
        [ImportExport(ELinkType.Parent, typeof(Application))]
        public int ApplicationId { get; set; }
        [ImportExport(ELinkType.Parent)]
        public virtual Application Application { get; set; }
    }
}

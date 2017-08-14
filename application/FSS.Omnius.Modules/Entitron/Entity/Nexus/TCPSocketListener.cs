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
        [ImportExportIgnore(IsKey = true)]
        public int? Id { get; set; }
        
        [Required]
        [Display(Name = "TCP Port")]
        public int Port { get; set; }

        [Required]
        [Display(Name = "Velikost bufferu")]
        public int BufferSize { get; set; }

        [Required]
        [Display(Name = "Aplikace")]
        [ImportExportIgnore(IsParentKey = true)]
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
        
        [ImportExportIgnore(IsParent = true)]
        public virtual Application Application { get; set; }
    }
}

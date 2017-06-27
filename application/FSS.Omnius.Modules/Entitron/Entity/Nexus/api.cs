namespace FSS.Omnius.Modules.Entitron.Entity.Nexus
{
    using System.ComponentModel.DataAnnotations;
    using System.ComponentModel.DataAnnotations.Schema;
    
    [Table("Nexus_API")]
    public partial class API : IEntity
    {
        public int? Id { get; set; }
       
        [StringLength(255)]
        [Display(Name = "Název")]
        public string Name{ get; set; }
        
        [Display(Name = "Definice")]
        [DataType(DataType.MultilineText)]
        public string Definition { get; set; }
    }
}

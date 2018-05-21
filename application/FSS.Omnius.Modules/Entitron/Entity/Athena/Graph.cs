using System.ComponentModel.DataAnnotations;
using System.ComponentModel.DataAnnotations.Schema;
using DT = System.ComponentModel.DataAnnotations.DataType;

namespace FSS.Omnius.Modules.Entitron.Entity.Athena
{
    [Table("Athena_Graph")]
    public partial class Graph : IEntity
    {
        public int? Id { get; set; }
        
        [Display(Name = "Aktivní")]
        public bool Active { get; set; }
        
        [Required]
        [StringLength(255)]
        [Display(Name = "Název")]
        public string Name { get; set; }

        [Required]
        [StringLength(255)]
        [Display(Name = "Identifikátor")]
        [Index(IsUnique = true)]
        [ImportExportProperty(IsKey = true)]
        public string Ident { get; set; }

        [Required]
        [Display(Name = "JavaScript")]
        [DataType(DT.MultilineText)]
        public string Js { get; set; }
        
        [Display(Name = "CSS")]
        [DataType(DT.MultilineText)]
        public string Css { get; set; }

        [Display(Name = "Demo data")]
        [DataType(DT.MultilineText)]
        public string DemoData { get; set; }

        [Display(Name = "HTML")]
        [DataType(DT.MultilineText)]
        public string Html { get; set; }

        [Display(Name = "Library")]
        public string Library { get; set; }
    }
}

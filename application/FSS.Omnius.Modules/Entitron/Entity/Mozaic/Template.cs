namespace FSS.Omnius.Modules.Entitron.Entity.Mozaic
{
    using System.ComponentModel.DataAnnotations;
    using System.ComponentModel.DataAnnotations.Schema;

    [Table("Mozaic_Template")]
    public partial class Template : IEntity
    {
        [System.Diagnostics.CodeAnalysis.SuppressMessage("Microsoft.Usage", "CA2214:DoNotCallOverridableMethodsInConstructors")]
        public Template()
        {
        }
        
        public int Id { get; set; }

        [Required]
        [StringLength(50)]
        public string Name { get; set; }

        [Required]
        public string Html { get; set; }

        public int CategoryId { get; set; }

        public virtual TemplateCategory Category { get; set; }

    }
}

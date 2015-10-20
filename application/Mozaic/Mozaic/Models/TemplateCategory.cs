using System;
using System.Collections.Generic;
using System.ComponentModel.DataAnnotations;
using System.ComponentModel.DataAnnotations.Schema;
using System.Data.Entity.Spatial;

namespace Mozaic.Models
{
    [Table("Mozaic_TemplateCategories")]
    public class TemplateCategory
    {
        public TemplateCategory()
        {
        }

        [Required]
        public int Id { get; set; }

        [Required]
        [StringLength(50)]
        public string Name { get; set; }

        public int? ParentId { get; set; }
        public virtual TemplateCategory Parent { get; set; }
        public virtual ICollection<TemplateCategory> Children { get; set; }
        public virtual ICollection<Template> Templates { get; set; }
    }
}
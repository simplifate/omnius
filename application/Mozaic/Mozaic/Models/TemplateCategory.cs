using System;
using System.Collections.Generic;
using System.ComponentModel.DataAnnotations;
using System.ComponentModel.DataAnnotations.Schema;
using System.Data.Entity.Spatial;

namespace Mozaic.Models
{
    public class TemplateCategory
    {
        [Required]
        public int Id { get; set; }

        [Required]
        [StringLength(50)]
        public string Name { get; set; }

        public TemplateCategory Parent { get; set; }
        public ICollection<TemplateCategory> Children { get; set; }
    }
}
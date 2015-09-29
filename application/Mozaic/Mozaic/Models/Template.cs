using System;
using System.Collections.Generic;
using System.ComponentModel.DataAnnotations;
using System.ComponentModel.DataAnnotations.Schema;
using System.Data.Entity.Spatial;

namespace Mozaic.Models
{
    public class Template
    {
        [Required]
        public int Id { get; set; }

        [Required]
        [StringLength(50)]
        public string Name { get; set; }
        
        public string Html { get; set; }

        public string Css { get; set; }

        public TemplateCategory Category { get; set; }
    }
}
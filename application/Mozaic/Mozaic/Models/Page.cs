using System;
using System.Collections.Generic;
using System.ComponentModel.DataAnnotations;
using System.ComponentModel.DataAnnotations.Schema;
using System.Data.Entity.Spatial;

namespace Mozaic.Models
{
    public class Page
    {
        [Required]
        public int Id { get; set; }
        
        public string PartialRelations { get; set; }
        public string DatasourceRelations { get; set; }

        [Required]
        public Template MasterTemplate { get; set; }
    }
}
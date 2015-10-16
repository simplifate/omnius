using System;
using System.Collections.Generic;
using System.ComponentModel.DataAnnotations;
using System.ComponentModel.DataAnnotations.Schema;
using System.Linq;
using System.Web;

namespace Tapestry.Models
{
    [Table("Tapestry_WorkFlow_Types")]
    public class WorkFlow_Type
    {
        public int Id { get; set; }
        [Required]
        [StringLength(50)]
        public string Name { get; set; }

        public virtual ICollection<WorkFlow> WorkFlows { get; set; }
    }
}
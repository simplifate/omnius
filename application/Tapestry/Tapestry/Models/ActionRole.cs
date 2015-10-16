using System;
using System.Collections.Generic;
using System.ComponentModel.DataAnnotations;
using System.ComponentModel.DataAnnotations.Schema;
using System.Linq;
using System.Web;

namespace Tapestry.Models
{
    [Table("Tapestry_ActionRoles")]
    public class ActionRole
    {
        public int Id { get; set; }

        [Required]
        public int SourceBlockId { get; set; }
        public virtual Block SourceBlock { get; set; }
        [Required]
        public int TargetBlockId { get; set; }
        public virtual Block TargetBlock { get; set; }
        [Required]
        public int ActorId { get; set; }
        public Actor Actor { get; set; }
        public ICollection<ActionRole_Action> Actions { get; set; }
    }
}
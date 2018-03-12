using System;
using System.Collections.Generic;
using System.ComponentModel.DataAnnotations;
using System.ComponentModel.DataAnnotations.Schema;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace FSS.Omnius.Modules.Entitron.Entity.Persona
{
    [Table("Persona_BadLoginCount")]
    public class BadLoginCount : IEntity
    {
        [Key]
        public int Id { get; set; }

        [StringLength(60)]
        public string IP { get; set; }

        public int AttemptsCount { get; set; }

        public DateTime LastAtempt { get; set; }
    }
}

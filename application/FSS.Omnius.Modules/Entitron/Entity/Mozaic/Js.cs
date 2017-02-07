namespace FSS.Omnius.Modules.Entitron.Entity.Mozaic
{
    using System;
    using System.Collections.Generic;
    using System.ComponentModel.DataAnnotations;
    using System.ComponentModel.DataAnnotations.Schema;
    using System.Data.Entity.Spatial;
    using Master;
    [Table("Mozaic_Js")]
    public partial class Js : IEntity
    {
        public int Id { get; set; }

        public int ApplicationId { get; set; }

        [Required]
        [StringLength(50)]
        public string Name { get; set; }

        [Required]
        public string Value { get; set; }

        public virtual Application Application { get; set; } 
    }
}

namespace Entitron.Entity
{
    using System;
    using System.Collections.Generic;
    using System.ComponentModel.DataAnnotations;
    using System.ComponentModel.DataAnnotations.Schema;
    using System.Data.Entity.Spatial;

    [Table("CORE_Modules")]
    public partial class Module
    {
        public int Id { get; set; }

        [Required]
        public string Name { get; set; }

        public string Description { get; set; }

        public bool IsEnabled { get; set; }


        public Module Update(Module model)
        {
            Name = model.Name;
            Description = model.Description;
            IsEnabled = model.IsEnabled;

            return this;
        }
    }
}

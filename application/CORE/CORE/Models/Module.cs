using System;
using System.Collections.Generic;
using System.ComponentModel.DataAnnotations;
using System.ComponentModel.DataAnnotations.Schema;
using System.Linq;
using System.Web;


namespace CORE.Models
{
    [Table("CORE_Modules")]
    public class Module
    {
        [Required]
        public int Id { get; set; }
        [Required]
        public string Name { get; set; }
        [Required]
        public string Address { get; set; }
        public string Description { get; set; }
        public bool IsEnabled { get; set; }

        public Module Update(Module model)
        {
            Name = model.Name;
            Address = model.Address;
            Description = model.Description;
            IsEnabled = model.IsEnabled;

            return this;
        }
    }
}
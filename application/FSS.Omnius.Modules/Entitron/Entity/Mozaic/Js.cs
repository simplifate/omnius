namespace FSS.Omnius.Modules.Entitron.Entity.Mozaic
{
    using System;
    using System.Collections.Generic;
    using System.ComponentModel.DataAnnotations;
    using System.ComponentModel.DataAnnotations.Schema;
    using System.Data.Entity.Spatial;
    using Bootstrap;
    using Master;
    using Newtonsoft.Json;

    [Table("Mozaic_Js")]
    public partial class Js : IEntity
    {
        [ImportExportIgnore(IsKey = true)]
        public int Id { get; set; }

        [ImportExportIgnore(IsParentKey = true)]
        public int ApplicationId { get; set; }
        [ImportExportIgnore(IsLinkKey = true)]
        public int? MozaicBootstrapPageId { get; set; }

        [Required]
        [StringLength(50)]
        public string Name { get; set; }

        [Required]
        public string Value { get; set; }

        [ImportExportIgnore(IsParent = true)]
        public virtual Application Application { get; set; } 
        [ImportExportIgnore(IsLink = true)]
        public virtual MozaicBootstrapPage Page { get; set; }
    }
}

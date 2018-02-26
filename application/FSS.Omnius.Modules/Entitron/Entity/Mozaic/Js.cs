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
        public int Id { get; set; }
        
        [Required]
        [StringLength(50)]
        public string Name { get; set; }
        [Required]
        public string Value { get; set; }

        [ImportExport(ELinkType.LinkOptional, typeof(MozaicBootstrapPage))]
        public int? MozaicBootstrapPageId { get; set; }
        [ImportExport(ELinkType.LinkOptional)]
        public virtual MozaicBootstrapPage MozaicBootstrapPage { get; set; }

        [ImportExport(ELinkType.Parent, typeof(Application))]
        public int ApplicationId { get; set; }
        [ImportExport(ELinkType.Parent)]
        public virtual Application Application { get; set; }
    }
}

namespace FSS.Omnius.Modules.Entitron.Entity.Mozaic.Bootstrap
{
    using System.Collections.Generic;
    using System.ComponentModel.DataAnnotations;
    using System.ComponentModel.DataAnnotations.Schema;
    using Master;
    using Newtonsoft.Json;

    [Table("MozaicBootstrap_Page")]
    public class MozaicBootstrapPage : IEntity
    {
        [ImportExportIgnore(IsKey = true)]
        public int Id { get; set; }
        public string Name { get; set; }
        [DataType(DataType.Text)]
        public string Content { get; set; }
        public string CompiledPartialView { get; set; }
        public int CompiledPageId { get; set; }
        public VersionEnum Version { get; set; }
        public bool IsDeleted { get; set; }
        public virtual ICollection<MozaicBootstrapComponent> Components { get; set; }
         
        [ImportExportIgnore(IsParent = true)]
        public virtual Application ParentApp { get; set; }

        [ImportExportIgnore]
        public virtual ICollection<Js> Js { get; set; }
        
        public MozaicBootstrapPage()
        {
            Components = new List<MozaicBootstrapComponent>();
            Js = new List<Js>();
        }
    }
}

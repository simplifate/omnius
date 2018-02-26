namespace FSS.Omnius.Modules.Entitron.Entity.Mozaic.Bootstrap
{
    using System.Collections.Generic;
    using System.ComponentModel.DataAnnotations;
    using System.ComponentModel.DataAnnotations.Schema;
    using Master;

    [Table("MozaicBootstrap_Page")]
    public class MozaicBootstrapPage : IEntity
    {
        public int Id { get; set; }

        public string Name { get; set; }
        [DataType(DataType.Text)]
        public string Content { get; set; }
        public string CompiledPartialView { get; set; }
        public int CompiledPageId { get; set; }
        public VersionEnum Version { get; set; }
        public bool IsDeleted { get; set; }

        [ImportExport(ELinkType.Child)]
        public virtual ICollection<MozaicBootstrapComponent> Components { get; set; }
        [ImportExport(ELinkType.LinkChild)]
        public virtual ICollection<Js> Js { get; set; }

        [ForeignKey("ParentApp")]
        [ImportExport(ELinkType.Parent, typeof(Application))]
        public int ParentApp_Id { get; set; }
        [ImportExport(ELinkType.Parent)]
        public virtual Application ParentApp { get; set; }
        
        public MozaicBootstrapPage()
        {
            Components = new List<MozaicBootstrapComponent>();
            Js = new List<Js>();
        }
    }
}

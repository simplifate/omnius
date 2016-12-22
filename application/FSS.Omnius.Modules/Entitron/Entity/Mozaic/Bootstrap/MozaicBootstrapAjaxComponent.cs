namespace FSS.Omnius.Modules.Entitron.Entity.Mozaic.Bootstrap
{
    using System.Collections.Generic;

    public class MozaicBootstrapAjaxComponent : IEntity
    {
        public MozaicBootstrapAjaxComponent()
        {
            ChildComponents = new HashSet<MozaicBootstrapAjaxComponent>();
        }
        
        public int Id { get; set; }
        public string ElmId { get; set; }
        public string Tag { get; set; }
        public string UIC { get; set; }
        public string Attributes { get; set; }
        public string Properties { get; set; }
        public string Content { get; set; }

        public virtual ICollection<MozaicBootstrapAjaxComponent> ChildComponents { get; set; }
    }
}

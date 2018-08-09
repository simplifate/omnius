using System;
using System.Collections.Generic;

using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace FSS.Omnius.Modules.Entitron.Entity.Mozaic.Bootstrap
{
    using System.ComponentModel.DataAnnotations;
    using System.ComponentModel.DataAnnotations.Schema;
    using Newtonsoft.Json;

    [Table("MozaicB_Components")]
    public class MozaicBootstrapComponent : IEntity
    {
        public int Id { get; set; }

        public string ElmId { get; set; }
        public string Tag { get; set; }
        public string UIC { get; set; }
        public int NumOrder { get; set; }

        [DataType(DataType.Text)]
        public string Attributes { get; set; }

        [DataType(DataType.Text)]
        public string Properties { get; set; }

        [DataType(DataType.Text)]
        public string Content { get; set; }

        [ImportExport(ELinkType.LinkChild)]
        public virtual ICollection<MozaicBootstrapComponent> ChildComponents { get; set; }
        [ImportExport(ELinkType.LinkOptional, typeof(MozaicBootstrapComponent))]
        public int? ParentComponentId { get; set; }
        [ImportExport(ELinkType.LinkOptional)]
        public MozaicBootstrapComponent ParentComponent { get; set; }
        [ImportExport(ELinkType.Parent, typeof(MozaicBootstrapPage))]
        public int MozaicBootstrapPageId { get; set; }
        [ImportExport(ELinkType.Parent)]
        public virtual MozaicBootstrapPage MozaicBootstrapPage { get; set; }

        public MozaicBootstrapComponent()
        {
            ChildComponents = new HashSet<MozaicBootstrapComponent>();
        }
    }
}

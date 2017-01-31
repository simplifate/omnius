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

    [Table("MozaicBootstrap_Components")]
    public class MozaicBootstrapComponent : IEntity
    {
        public MozaicBootstrapComponent()
        {
            ChildComponents = new HashSet<MozaicBootstrapComponent>();
        }

        [ImportExportIgnore(IsKey = true)]
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

        [ImportExportIgnore]
        public virtual ICollection<MozaicBootstrapComponent> ChildComponents { get; set; }
        [ImportExportIgnore(IsLinkKey = true)]
        public int? ParentComponentId { get; set; }
        [ImportExportIgnore(IsLink = true)]
        public MozaicBootstrapComponent ParentComponent { get; set; }
        [ImportExportIgnore(IsParentKey = true)]
        public int MozaicBootstrapPageId { get; set; }
        [ImportExportIgnore(IsParent = true)]
        public virtual MozaicBootstrapPage MozaicBootstrapPage { get; set; }  
    }
}

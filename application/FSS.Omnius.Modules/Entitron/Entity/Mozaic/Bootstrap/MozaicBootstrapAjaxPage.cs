using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace FSS.Omnius.Modules.Entitron.Entity.Mozaic.Bootstrap
{
    public class MozaicBootstrapAjaxPage : IEntity
    {
        public int Id { get; set; }
        public string Name { get; set; }
        public string Content { get; set; }
        public bool IsDeleted { get; set; }
        public List<MozaicBootstrapAjaxComponent> Components { get; set; }

        public MozaicBootstrapAjaxPage()
        {
            Components = new List<MozaicBootstrapAjaxComponent>();
        }
    }
}

using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace FSS.Omnius.Modules.Entitron.Entity.Mozaic.Bootstrap
{
    public class MozaicBootstrapAjaxPageHeader : IEntity
    {
        public int Id { get; set; }
        public string Name { get; set; }
        public bool IsBootstrap { get; set; }
    }
}

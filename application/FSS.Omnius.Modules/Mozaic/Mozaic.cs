using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using FSS.Omnius.Modules.Entitron;
using FSS.Omnius.Modules.Entitron.Entity.CORE;
using FSS.Omnius.Modules.Entitron.Entity.Mozaic;
using System.ComponentModel.DataAnnotations.Schema;

namespace FSS.Omnius.Modules.Mozaic
{
    [NotMapped]
    public class Mozaic : Module
    {
        private CORE.CORE _CORE;
        public Mozaic(CORE.CORE core)
        {
            Name = "Mozaic";
            _CORE = core;
        }

        public string Render(int pageId, DBItem model)
        {
            Page page = _CORE.Entitron.GetStaticTables().Pages.FirstOrDefault(p => p.Id == pageId);

            if (page == null)
                return "Page not found";

            return page.Render(model, _CORE.Entitron.GetStaticTables());
        }
    }
}

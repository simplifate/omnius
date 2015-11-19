using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using FSS.Omnius.Entitron;
using FSS.Omnius.Entitron.Entity.CORE;
using FSS.Omnius.Entitron.Entity.Mozaic;
using System.ComponentModel.DataAnnotations.Schema;

namespace FSS.Omnius.Mozaic
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
            Entitron.Entitron entitron = (Entitron.Entitron)_CORE.GetModule("Entitron");
            if (entitron == null)
                throw new ModuleNotFoundOrEnabledException("Entitron");

            Page page = entitron.GetStaticTables().Pages.FirstOrDefault(p => p.Id == pageId);

            if (page == null)
                return "Page not found";

            return page.Render(model, entitron.GetStaticTables());
        }
    }
}

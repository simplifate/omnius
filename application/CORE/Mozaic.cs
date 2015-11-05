using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using Entitron;
using Entitron.Entity;

namespace Mozaic
{
    public class Mozaic : CORE.Module
    {
        private CORE.CORE _CORE;
        public Mozaic(CORE.CORE core) : base("Mozaic")
        {
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

            return new MozaicPage(page).Render(model, entitron.GetStaticTables());
        }
    }
}

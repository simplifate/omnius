using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using Entitron;
using Entitron.Entity;

namespace Mozaic
{
    public class Mozaic
    {
        private CORE.CORE CORE;
        public Mozaic(CORE.CORE core)
        {
            CORE = core;
        }

        public string Render(int pageId, DBItem model)
        {
            DBEntities e = CORE.Entitron().GetStaticTables();
            Entitron.Entity.Page page = e.Pages.FirstOrDefault(p => p.Id == pageId);

            if (page == null)
                return "Page not found";

            return new MozaicPage(page).Render(model, e);
        }
    }
}

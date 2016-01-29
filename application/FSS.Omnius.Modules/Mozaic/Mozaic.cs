using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using FSS.Omnius.Modules.Entitron;
using FSS.Omnius.Modules.Entitron.Entity.CORE;
using FSS.Omnius.Modules.Entitron.Entity.Mozaic;
using System.ComponentModel.DataAnnotations.Schema;
using FSS.Omnius.Modules.Tapestry;

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

        //public string Render(int pageId, ActionResultCollection results)
        //{
        //    Page page = _CORE.Entitron.GetStaticTables().Pages.FirstOrDefault(p => p.Id == pageId);

        //    return Render(page, results);
        //}
        //public string Render(Page page, ActionResultCollection results)
        //{
        //    if (page == null)
        //        return "Page not found";

        //    return page.Render(results, _CORE.Entitron.GetStaticTables());
        //}
    }
}

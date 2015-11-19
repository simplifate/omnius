using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using System.Web;
using System.Web.Mvc;

namespace FSS.Omnius.Controllers.Tapestry
{
    public class RunController : Controller
    {
        public string Index(string url)
        {
            Omnius.CORE.CORE core = new Omnius.CORE.CORE();
            Omnius.Tapestry.Tapestry tapestry = (Omnius.Tapestry.Tapestry)core.GetModule("Tapestry");
            tapestry.run(url);

            return tapestry.GetHtmlOutput();
        }
    }
}

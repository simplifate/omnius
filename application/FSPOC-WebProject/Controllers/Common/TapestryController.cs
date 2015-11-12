using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using System.Web;
using System.Web.Mvc;

namespace FSS.Omnius.Controllers.Common
{
    public class TapestryController : Controller
    {
        public string Index(string url)
        {
            CORE.CORE core = new CORE.CORE();
            Tapestry.Tapestry tapestry = (Tapestry.Tapestry)core.GetModule("Tapestry");
            tapestry.run(url);

            return tapestry.GetHtmlOutput();
        }
    }
}

using System;
using System.Collections.Generic;
using System.Linq;
using System.Web;
using System.Web.Mvc;
using CORE.Models;
using System.Net.Http;
using System.Threading.Tasks;

namespace CORE.Controllers
{
    public class HomeController : Controller
    {
        public string Index(string modul, string app, string url)
        {
            DBEntities e = new DBEntities();
            Module module = e.Modules.SingleOrDefault(m => m.Name == modul);

            if (module == null || !module.IsEnabled)
                return null;
            
            using (var client = new HttpClient())
            {
                var responseString = client.GetStringAsync(string.Format("{0}/{1}/{2}", module.Address, app, url));
                responseString.Wait();
                return responseString.Result;
            }
        }
    }
}
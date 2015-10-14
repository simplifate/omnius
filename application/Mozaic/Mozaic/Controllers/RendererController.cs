using System;
using System.Collections.Generic;
using System.Linq;
using System.Web;
using System.Web.Mvc;
using Mozaic.Models;
using Entitron;

namespace Mozaic.Controllers
{
    [FilterIP(allowedIp = "127.0.0.1;::1")]
    public class RendererController : Controller
    {
        public string Show(string app, int pageId, string tableName, int modelId)
        {
            DBMozaic e = new DBMozaic();
            Page page = e.Pages.SingleOrDefault(p => p.Id == pageId);

            if (page == null)
                return null;

            DBApp application = new DBApp()
            {
                Name = app,
                ConnectionString = e.Database.Connection.ConnectionString
            };
            DBItem item = application.GetTable(tableName).Select().where(c => c.column("Id").Equal(modelId)).ToList().First();

            return page.Render(item, e);
        }
    }
}
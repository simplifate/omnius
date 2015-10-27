using System.Collections.Generic;
using System.IO;
using System.Web.Mvc;
using System.Web.Script.Serialization;
using System.Web.UI;

namespace FSPOC_WebProject.Controllers
{
    public class BlockController : Controller
    {
        // GET: Block
        public ActionResult Index(int id)
        {
            ViewBag.SourceBlock = GetSourceBlock(id);
            var listRouteConfig = new List<object>
            {
                new {id = 1, route = "/api/ActionHandler"},
                new {id = 2, route ="/block/index/3"},
            };
            //http://stackoverflow.com/questions/16503309/how-do-i-get-json-data-from-an-external-url
            var modelConfig = new
            {
                pathSourceFile = $"/api/sourceblock/{id}",
                listConfigRoute = listRouteConfig

            };

            ViewBag.ConfigRoute = new JavaScriptSerializer().Serialize(modelConfig);

            return View();
        }

        private static string GetSourceBlock(int id)
        {
            var stringWriter = new StringWriter();
            using (var writer = new HtmlTextWriter(stringWriter))
            {
                //input
                writer.AddAttribute(HtmlTextWriterAttribute.Type,"text");
                writer.AddAttribute("data-bind", "value: data().Name");
                writer.RenderBeginTag(HtmlTextWriterTag.Input);
                writer.RenderEndTag();
                //button
                writer.AddAttribute("data-bind", "click: onHandleAction");
                writer.AddAttribute("data-actionId", $"{1}");
                writer.AddAttribute(HtmlTextWriterAttribute.Class, "btn btn-primary");
                writer.RenderBeginTag(HtmlTextWriterTag.Button);
                writer.Write("<i class=\"fa fa-fw fa-save\"></i> Save");
                writer.RenderEndTag();

    }
            return stringWriter.ToString();
        }
    }
}
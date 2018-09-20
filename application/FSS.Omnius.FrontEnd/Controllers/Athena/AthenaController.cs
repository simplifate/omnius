using FSS.Omnius.Modules.Entitron.Entity;
using FSS.Omnius.Modules.Entitron.Entity.Cortex;
using FSS.Omnius.Modules.Cortex;
using System;
using System.Collections;
using System.Collections.Generic;
using System.Linq;
using System.Web.Mvc;
using FSS.Omnius.Modules.Entitron.Entity.Athena;
using FSS.Omnius.Modules.CORE;

namespace FSS.Omnius.Controllers.Athena
{
    [PersonaAuthorize(NeedsAdmin = true, Module = "Athena")]
    public class AthenaController : Controller
    {
        public AthenaController()
        {
            ViewData["GraphCount"] = COREobject.i.Context.Graph.Count().ToString();
        }

        public ActionResult Index()
        {
            return View("~/Views/Athena/Index.cshtml", COREobject.i.Context.Graph);
        }

        public ActionResult Create()
        {
            DBEntities e = COREobject.i.Context;
             
            ViewData["ApplicationList"] = e.Applications;

            return View("~/Views/Athena/Form.cshtml");
        }

        public ActionResult Edit(int id)
        {
            DBEntities e = COREobject.i.Context;
            return View("~/Views/Athena/Form.cshtml", e.Graph.Single(g => g.Id == id));
        }
        
        [ValidateInput(false)]
        public ActionResult Save(Graph model)
        {
            DBEntities e = COREobject.i.Context;

            Graph item = model.Id == null ? new Graph() : e.Graph.Single(g => g.Id == model.Id);

            // Pri editaci je nutne prejmenovat komponenty jiz pouzite na strankech a oznacit je jako aktualizovane
            if(model.Id != null) {
                string uic = "athena|" + item.Ident;
                var usedComponentList = e.MozaicBootstrapComponents.Where(c => c.UIC == uic);
                foreach(var component in usedComponentList) {
                    component.UIC = $"athena|{model.Ident}";
                    component.MozaicBootstrapPage.ParentApp.MozaicChangedSinceLastBuild = true;
                }
            }

            item.Name = model.Name;
            item.Ident = model.Ident;
            item.Js = model.Js == null ? " ;" : model.Js;
            item.Css = model.Css;
            item.DemoData = model.DemoData;
            item.Html = model.Html;
            item.Library = model.Library;
            item.Active = true;

            if(model.Id == null) {
                e.Graph.Add(item);
            }
            e.SaveChanges();

            return RedirectToRoute("Athena", new { @action = "Index" });
        }

        public ActionResult Delete(int id)
        {
            DBEntities e = COREobject.i.Context;
            var item = e.Graph.Single(g => g.Id == id);

            e.Graph.Remove(item);
            e.SaveChanges();

            return RedirectToRoute("Athena", new { @action = "Index" });
        }
    }
}

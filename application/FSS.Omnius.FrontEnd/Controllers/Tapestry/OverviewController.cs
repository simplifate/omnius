using System;
using System.Linq;
using System.Web.Mvc;
using FSS.Omnius.Modules.Entitron.Entity;
using FSS.Omnius.Modules.Entitron.Entity.Tapestry;
using FSS.Omnius.Modules.Entitron.Entity.Master;
using System.Collections.Generic;
using Newtonsoft.Json.Linq;
using FSS.Omnius.Modules.CORE;

namespace FSS.Omnius.FrontEnd.Controllers.Tapestry
{
    [PersonaAuthorize(NeedsAdmin = true, Module = "Tapestry")]
    public class OverviewController : Controller
    {
        public ActionResult Index(FormCollection formParams)
        {
            COREobject core = COREobject.i;
            DBEntities context = core.Context;
            JArray metablockList = JArray.Parse("[]");

            if (Request.HttpMethod == "POST")
            {
                int metablockId = 0;
                TapestryDesignerMetablock parentMetablock = null;
                if (formParams["appId"] != null)
                {
                    int appId = int.Parse(formParams["appId"]);
                    core.User.DesignAppId = appId;
                    context.SaveChanges();
                    var app = context.Applications.Find(appId);
                    var rootMetablock = app.TapestryDesignerRootMetablock;
                    if(rootMetablock == null)
                    {
                        var newRootMetablock = new TapestryDesignerMetablock
                        {
                            Name = "Root metablock",
                            ParentApp = app
                        };
                        context.TapestryDesignerMetablocks.Add(newRootMetablock);
                        context.SaveChanges();
                        metablockId = newRootMetablock.Id;
                    }
                    else
                        metablockId = rootMetablock.Id;
                    ViewData["appName"] = context.Applications.Find(appId).DisplayName;
                    ViewData["currentAppId"] = appId;

                    GetMetablockList(rootMetablock, ref metablockList, 0);
                }
                else
                {
                    metablockId = int.Parse(formParams["metablockId"]);
                    parentMetablock = context.TapestryDesignerMetablocks.Include("ParentMetablock")
                        .Where(c => c.Id == metablockId).First().ParentMetablock;

                    Application app = GetApplication(parentMetablock, metablockId, context); 

                    ViewData["appName"] = app.Name;
                    ViewData["currentAppId"] = app.Id;
                    GetMetablockList(app.TapestryDesignerRootMetablock, ref metablockList, 0);
                }
                ViewData["metablockId"] = metablockId;
                if (parentMetablock == null)
                    ViewData["parentMetablockId"] = 0;
                else
                    ViewData["parentMetablockId"] = parentMetablock.Id;
                    
            }
            else
            {
                var userApp = core.User.DesignApp;
                if (userApp == null)
                    userApp = context.Applications.First();
                ViewData["metablockId"] = userApp.TapestryDesignerRootMetablock.Id;
                ViewData["parentMetablockId"] = 0;
                ViewData["appName"] = userApp.DisplayName;
                ViewData["currentAppId"] = userApp.Id;
                GetMetablockList(userApp.TapestryDesignerRootMetablock, ref metablockList, 0);
            }

            ViewData["metablockList"] = metablockList;
            return View();
        }

        public ActionResult MenuOrder(int id)
        {
            TapestryDesignerMetablock parentMetablock;
            List<TapestryDesignerMenuItem> model = new List<TapestryDesignerMenuItem>();

            DBEntities context = COREobject.i.Context;
            parentMetablock = context.TapestryDesignerMetablocks.Include("Metablocks")
                                                                .Include("Blocks")
                                                                .Include("ParentMetablock")
                                                                .Where(m => m.Id == id).First();

            foreach(TapestryDesignerMetablock mb in parentMetablock.Metablocks.Where(mb => !mb.IsDeleted)) {
                model.Add(new TapestryDesignerMenuItem()
                {
                    Id = mb.Id,
                    Name = mb.Name,
                    IsInitial = mb.IsInitial,
                    IsInMenu = mb.IsInMenu,
                    MenuOrder = mb.MenuOrder,
                    IsBlock = false,
                    IsMetablock = true
                });
            }
            foreach(TapestryDesignerBlock b in parentMetablock.Blocks.Where(b => !b.IsDeleted)) {
                model.Add(new TapestryDesignerMenuItem()
                {
                    Id = b.Id,
                    Name = b.Name,
                    IsInitial = b.IsInitial,
                    IsInMenu = b.IsInMenu,
                    MenuOrder = b.MenuOrder,
                    IsBlock = true,
                    IsMetablock = false
                });
            }

            Application app = GetApplication(parentMetablock, id, context);
            ViewData["appName"] = app.Name;
            ViewData["metablockName"] = parentMetablock.Name;
            ViewData["metablockId"] = parentMetablock.Id;

            return View("~/Views/Tapestry/Overview/MenuOrder.cshtml", model);
        }
        
        private Application GetApplication(TapestryDesignerMetablock parentMetablock, int metablockId, DBEntities context)
        {
            int? rootMetablockId = metablockId;
            if (parentMetablock != null) {
                while (parentMetablock.ParentMetablock_Id != null) {
                    parentMetablock = context.TapestryDesignerMetablocks.Include("ParentMetablock")
                                                                        .Where(b => b.Id == parentMetablock.ParentMetablock_Id).First();
                }
                rootMetablockId = parentMetablock.Id;
            }

            return context.Applications.SingleOrDefault(a => a.TapestryDesignerMetablocks.Any(mb => mb.Id == rootMetablockId));
        }

        private void GetMetablockList(TapestryDesignerMetablock item, ref JArray list, int level)
        {
            list.Add(JToken.Parse($"{{ Id: \"{item.Id}\", Name: \"{item.Name}\", Level: \"{level.ToString()}\" }}"));
            foreach(TapestryDesignerMetablock mb in item.Metablocks.Where(m => m.IsDeleted == false)) {
                GetMetablockList(mb, ref list, level + 1);
            }
        }
    }
}

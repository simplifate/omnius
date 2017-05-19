using System;
using System.Collections.Generic;
using System.Web.Mvc;
using System.Linq;
using FSS.Omnius.Modules.Entitron.Entity;
using FSS.Omnius.Modules.Entitron.Entity.Watchtower;
using FSS.Omnius.Modules.Watchtower;
using FSS.Omnius.FrontEnd.Models;

namespace FSS.Omnius.Controllers.Watchtower
{
    [PersonaAuthorize(NeedsAdmin = true, Module = "Watchtower")]
    public class LogController : Controller
    {
        public ActionResult Index(LogFilter filter)
        {
            // INIT
            DBEntities context = DBEntities.instance;
            filter.Fill(context);

            filter.ResultItems = context.LogItems.Where(
                i =>
                   (filter.LevelId == -1 || i.LogLevel == filter.LevelId)
                && (filter.UserName == "All" || i.UserName == filter.UserName)
                && (filter.Server == "All" || i.Server == filter.Server)
                && (filter.SourceId == -1 || i.Source == filter.SourceId)
                && (filter.ApplicationName == "All" || i.Application == filter.ApplicationName)
                && (filter.BlockName == "All" || i.BlockName == filter.BlockName)
                && (filter.ActionName == "All" || i.ActionName == filter.ActionName)
                && (i.Timestamp > filter.TimeSince && i.Timestamp < filter.TimeTo)
            ).OrderByDescending(i => i.Timestamp).Take(100).Select(i =>
                new
                {
                    Id = i.Id,
                    Timestamp = i.Timestamp,
                    UserName = i.UserName,
                    Server = i.Server,
                    Source = i.Source,
                    Application = i.Application,
                    BlockName = i.BlockName,
                    ActionName = i.ActionName,
                    Message = i.Message
                }
            ).ToList().Select(i =>
                new LogItem
                {
                    Id = i.Id,
                    Timestamp = i.Timestamp,
                    UserName = i.UserName,
                    Server = i.Server,
                    Source = i.Source,
                    Application = i.Application,
                    BlockName = i.BlockName,
                    ActionName = i.ActionName,
                    Message = i.Message
                }).ToList();

            return View(filter);
        }

        public JsonResult GetRow(int id)
        {
            DBEntities context = DBEntities.instance;
            LogItem item = context.LogItems.Find(id);

            return Json(new { Vars = item.VarHtmlTable(), StackTrace = item.StackTraceHtml() }, JsonRequestBehavior.AllowGet);
        }
    }
}

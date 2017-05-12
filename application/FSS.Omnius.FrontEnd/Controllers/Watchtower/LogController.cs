using System;
using System.Collections.Generic;
using System.Web.Mvc;
using System.Linq;
using FSS.Omnius.Modules.Entitron.Entity;
using FSS.Omnius.Modules.Entitron.Entity.Watchtower;

namespace FSS.Omnius.Controllers.Watchtower
{
    [PersonaAuthorize(NeedsAdmin = true, Module = "Watchtower")]
    public class LogController : Controller
    {
        public ActionResult Index(FormCollection formParams)
        {
            using (var context = DBEntities.instance)
            {
                List<LogItem> searchResults = new List<LogItem>();
                List<AjaxLogItem> model = new List<AjaxLogItem>();

                //all USers
                ViewData["users"] = context.Users.ToList();
                //all Applications
                ViewData["apps"] = context.Applications.ToList();

                if (Request.HttpMethod == "POST")
                {
                    bool filterByLevel = formParams["level"] != "all";
                    int levelValue = filterByLevel ? Convert.ToInt32(formParams["level"]) : 0;
                    bool filterByType = formParams["type"] != "all";
                    int typeValue = filterByType ? Convert.ToInt32(formParams["type"]) : 0;
                    bool filterBySource = formParams["source"] != "all";
                    bool filterPlatformOnly = formParams["source"] == "platform";
                    bool filterByAppName = filterBySource && !filterPlatformOnly;
                    string appNameValue = filterByAppName ? Convert.ToString(formParams["source"]) : "";
                    bool filterByUsername = formParams["user"] != "all";
                    string userNameValue = filterByUsername ? Convert.ToString(formParams["user"]) : "";
                    bool searchMessage = formParams["message"] != "";
                    string messageValue = formParams["message"];

                    bool limitByTimeFrom = formParams["time-from"] != "";
                    bool limitByTimeTo = formParams["time-to"] != "";
                    DateTime timeFrom = new DateTime();
                    DateTime timeTo = new DateTime();
                    string format = "dd-MM-yyyy HH:mm";
                    bool timeFromOK = DateTime.TryParseExact(formParams["time-from"], format, null, System.Globalization.DateTimeStyles.None, out timeFrom);
                    bool timeToOK = DateTime.TryParseExact(formParams["time-to"], format, null, System.Globalization.DateTimeStyles.None, out timeTo);

                    if (!timeFromOK || !timeToOK)
                        ViewData["timeFormatError"] = true;

                    ViewData["filter"] = filterByLevel || filterByType || filterBySource || filterPlatformOnly || filterByAppName || filterByUsername
                        || searchMessage || limitByTimeFrom || limitByTimeTo;
                    ViewData["level"] = formParams["level"];
                    ViewData["type"] = formParams["type"];
                    ViewData["source"] = formParams["source"];
                    ViewData["user"] = formParams["user"];
                    ViewData["message"] = formParams["message"];
                    ViewData["time-from"] = formParams["time-from"];
                    ViewData["time-to"] = formParams["time-to"];

                    searchResults = context.LogItems.Where(c =>
                        c.LogLevel == (filterByLevel ? levelValue : c.LogLevel)
                        && c.Source == (filterByType ? typeValue : c.Source)
                        && c.Application == (filterByAppName ? appNameValue : c.Application)
                        && c.UserName == (filterByUsername ? userNameValue : c.UserName)
                        && c.Timestamp >= (limitByTimeFrom ? timeFrom : c.Timestamp)
                        && c.Timestamp <= (limitByTimeTo ? timeTo : c.Timestamp)
                        && c.Message.Contains((searchMessage ? messageValue : c.Message)
                        )).OrderByDescending(c => c.Timestamp).Take(100).ToList();
                }
                else
                {
                    searchResults = context.LogItems.OrderByDescending(c => c.Timestamp).Take(100).ToList();
                    ViewData["filter"] = false;
                    ViewData["level"] = "all";
                    ViewData["type"] = "all";
                    ViewData["source"] = "all";
                    ViewData["user"] = "all";
                    ViewData["message"] = "";
                    ViewData["time-from"] = "";
                    ViewData["time-to"] = "";

         

                }

                foreach (var r in searchResults)
                {
                    model.Add(new AjaxLogItem
                    {
                        Timestamp = r.Timestamp,
                        Source = r.Source,
                        Level = r.LogLevel,
                        ActionName = r.ActionName,
                        UserName = r.UserName,
                        ApplicationName = r.Application,
                        BlockName = r.BlockName,
                        Server = r.Server,
                        StackTrace = r.StackTrace,
                        Vars = r.Vars,
                        Message = r.Message
                    });
                }
                return View(model);
            }
        }
    }
}

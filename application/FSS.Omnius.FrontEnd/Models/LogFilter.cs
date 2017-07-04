using FSS.Omnius.Modules.Entitron.Entity;
using FSS.Omnius.Modules.Entitron.Entity.Master;
using FSS.Omnius.Modules.Entitron.Entity.Persona;
using FSS.Omnius.Modules.Entitron.Entity.Tapestry;
using FSS.Omnius.Modules.Entitron.Entity.Watchtower;
using FSS.Omnius.Modules.Watchtower;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Web;
using System.Web.Mvc;

namespace FSS.Omnius.FrontEnd.Models
{
    public class LogFilter
    {
        public LogFilter()
        {
            Levels = new List<SelectListItem>();
            LevelId = -1;
            Sources = new List<SelectListItem>();
            SourceId = -1;
            Users = new List<User>();
            UserName = "All";
            Servers = new List<string>();
            Server = "All";
            Applications = new List<Application>();
            ApplicationName = "All";
            Blocks = new List<Block>();
            BlockName = "All";
            Actions = new List<string>();
            ActionName = "All";
            TimeSince = new DateTime(2000, 1, 1);
            TimeTo = DateTime.UtcNow;
            Message = "";

            ResultItems = new List<LogItem>();
        }

        public List<SelectListItem> Levels { get; set; }
        public int LevelId { get; set; }
        public List<SelectListItem> Sources { get; set; }
        public int SourceId { get; set; }
        public List<User> Users { get; set; }
        public string UserName { get; set; }
        public List<string> Servers { get; set; }
        public string Server { get; set; }
        public List<Application> Applications { get; set; }
        public string ApplicationName { get; set; }
        public List<Block> Blocks { get; set; }
        public string BlockName { get; set; }
        public List<string> Actions { get; set; }
        public string ActionName { get; set; }
        public DateTime TimeSince { get; set; }
        public DateTime TimeTo { get; set; }
        public string Message { get; set; }

        public List<LogItem> ResultItems { get; set; }

        public void Fill(DBEntities context)
        {
            Levels.Add(new SelectListItem { Value = "-1", Text = "All" });
            foreach (OmniusLogLevel level in Enum.GetValues(typeof(OmniusLogLevel)))
                Levels.Add(new SelectListItem { Value = ((int)level).ToString(), Text = level.ToString(), Selected = ((int)level == LevelId) });

            Sources.Add(new SelectListItem { Value = "-1", Text = "All" });
            foreach (OmniusLogSource source in Enum.GetValues(typeof(OmniusLogSource)))
                Sources.Add(new SelectListItem { Value = ((int)source).ToString(), Text = source.ToString(), Selected = ((int)source == SourceId) });

            Users.Add(new User { UserName = "All", DisplayName = "All" });
            Users.AddRange(context.Users.OrderBy(u => u.UserName).ToList());

            Servers.Add("All");
            Servers.AddRange(context.LogItems.Select(i => i.Server).OrderBy(s => s).Distinct().ToList());

            Applications.Add(new Application { Name = "All", DisplayName = "All" });
            Applications.AddRange(context.Applications.OrderBy(a => a.Name).ToList());

            Blocks.Add(new Block { Name = "All", DisplayName = "All" });
            if (ApplicationName != null)
                Blocks.AddRange(context.Blocks.Where(b => b.WorkFlow.Application.Name == ApplicationName).OrderBy(b => b.DisplayName).ToList());

            Actions.Add("All");
            Actions.AddRange(Modules.Tapestry.Action.All.Select(a => a.Value.Name).OrderBy(a => a));
        }
    }
}
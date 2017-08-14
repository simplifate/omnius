using Newtonsoft.Json;
using System.IO;
using System.Linq;
using FSS.Omnius.Modules.Entitron.Entity;
using Newtonsoft.Json.Serialization;
using System;
using System.Collections.Generic;
using System.Collections.Specialized;
using Newtonsoft.Json.Linq;
using System.Text;
using System.Globalization;

namespace FSS.Omnius.Modules.Entitron.Service
{
    public class BackupGeneratorService : IBackupGeneratorService
    {
        private JsonSerializer serializer = new JsonSerializer() { ReferenceLoopHandling = ReferenceLoopHandling.Ignore, ContractResolver = new IgnoreAttributeResolver()  };
        private DBEntities context;
        private int appId;
        private JToken app;

        public void ExportAllDatabaseDesignerData(string filename)
        {
            using (var context = DBEntities.instance)
            {
                var commits = from c in context.DBSchemeCommits orderby c.Timestamp descending select c;
                string jsonOutput = JsonConvert.SerializeObject(commits.ToList(), Formatting.Indented,
                    new JsonSerializerSettings { ReferenceLoopHandling = ReferenceLoopHandling.Ignore, StringEscapeHandling = StringEscapeHandling.EscapeNonAscii });
                File.WriteAllText(filename, jsonOutput);
            }
        }

        public string ExportApplication(int id)
        {
            return ExportApplication(id, new NameValueCollection());
        }

        public string ExportApplication(int id, NameValueCollection form) 
        {
            appId = id;

            context = DBEntities.instance;
            context.Configuration.LazyLoadingEnabled = false;

            var application = context.Applications.SingleOrDefault(a => a.Id == id);
            if (application != null)
            {
                app = JToken.FromObject(application, serializer);

                app["DatabaseDesignerSchemeCommits"]    = form.AllKeys.Contains("ExportDatabaseDesignerSchemeCommits") ? ExportDatabaseDesignerSchemeCommits() : new JArray();
                app["EmailTemplates"]                   = form.AllKeys.Contains("ExportEmailTemplates")                ? ExportEmailTemplates()                : new JArray();
                app["IncomingEmailRule"]                = form.AllKeys.Contains("ExportIncomingEmailRule")             ? ExportIncomingEmailRule()             : new JArray();
                app["Js"]                               = form.AllKeys.Contains("ExportJs")                            ? ExportJs()                            : new JArray();
                app["MozaicBootstrapPages"]             = form.AllKeys.Contains("ExportMozaicBootstrapPages")          ? ExportMozaicBootstrapPages()          : new JArray();
                app["MozaicEditorPages"]                = form.AllKeys.Contains("ExportMozaicEditorPages")             ? ExportMozaicEditorPages()             : new JArray();
                app["Roles"]                            = form.AllKeys.Contains("ExportRoles")                         ? ExportRoles()                         : new JArray();
                app["TapestryDesignerMetablocks"]       = form.AllKeys.Contains("ExportTapestryDesignerMetablocks")    ? ExportTapestryDesignerMetablocks()    : new JArray();
                app["TCPListeners"]                     = form.AllKeys.Contains("ExportTCPListeners")                  ? ExportTCPListeners()                  : new JArray();
                //app["TapestryDesignerConditionGroups"] = form.AllKeys.Contains("ExportTapestryDesignerConditionGroups") ? ExportTapestryDesignerConditionGroups() : new JArray();

                return JsonConvert.SerializeObject(app, Formatting.Indented, new JsonSerializerSettings() { StringEscapeHandling = StringEscapeHandling.EscapeNonAscii });
            }
            else {
                return "";
            }
        }

        private JArray ExportDatabaseDesignerSchemeCommits()
        {
            var rows = context.DBSchemeCommits
                                .Include("Tables")
                                .Include("Tables.Columns")
                                .Include("Tables.Indices")
                                .Include("Relations")
                                .Include("Views")
                              .Where(r => r.Application_Id == appId);
            return JArray.FromObject(rows, serializer);
        }

        private JArray ExportMozaicEditorPages()
        {
            var rows = context.MozaicEditorPages.Include("Components").Where(r => r.ParentApp.Id == appId).ToList();
            return JArray.FromObject(rows, serializer);
        }

        private JArray ExportMozaicBootstrapPages()
        {
            var rows = context.MozaicBootstrapPages.Include("Components").Where(r => r.ParentApp.Id == appId).ToList();
            return JArray.FromObject(rows, serializer);
        }

        private JArray ExportJs()
        {
            var rows = context.Js.Where(r => r.ApplicationId == appId).ToList();
            return JArray.FromObject(rows, serializer);
        }
        
        private JArray ExportEmailTemplates()
        {
            var rows = context.EmailTemplates.Include("PlaceholderList").Include("ContentList").Where(r => r.AppId == appId).ToList();
            return JArray.FromObject(rows, serializer);
        }

        private JArray ExportIncomingEmailRule()
        {
            var rows = context.IncomingEmailRule.Where(r => r.ApplicationId == appId).ToList();
            List<int?> ids = rows.Select(r => r.Id).ToList<int?>();

            app["IncomingEmail"] = JArray.FromObject(context.IncomingEmail.Where(m => ids.Contains(m.Id)).ToList(), serializer);
            return JArray.FromObject(rows, serializer);
        }

        private JArray ExportRoles()
        {
            var rows = context.AppRoles.Where(r => r.ApplicationId == appId).ToList();
            return JArray.FromObject(rows, serializer);
        }

        private JArray ExportTapestryDesignerConditionGroups()
        {
            var rows = context.TapestryDesignerConditionGroups
                                .Include("ConditionSets")
                                .Include("ConditionSets.Conditions")
                              .Where(r => r.ApplicationId == appId).ToList();
            return JArray.FromObject(rows, serializer);
        }

        private JArray ExportTapestryDesignerMetablocks()
        {
            var rows = context.TapestryDesignerMetablocks
                                .Include("Connections")
                                .Include("Metablocks")
                                .Include("Blocks")
                                .Include("Blocks.BlockCommits")
                                .Include("Blocks.BlockCommits.ResourceRules")
                                .Include("Blocks.BlockCommits.ResourceRules.ResourceItems")
                                .Include("Blocks.BlockCommits.ResourceRules.ResourceItems.ConditionGroups")
                                .Include("Blocks.BlockCommits.ResourceRules.ResourceItems.ConditionGroups.ConditionSets")
                                .Include("Blocks.BlockCommits.ResourceRules.ResourceItems.ConditionGroups.ConditionSets.Conditions")
                                .Include("Blocks.BlockCommits.ResourceRules.Connections")
                                .Include("Blocks.BlockCommits.ResourceRules.Connections.Source")
                                .Include("Blocks.BlockCommits.ResourceRules.Connections.Target")
                                .Include("Blocks.BlockCommits.WorkflowRules")
                                .Include("Blocks.BlockCommits.WorkflowRules.Swimlanes")
                                .Include("Blocks.BlockCommits.WorkflowRules.Swimlanes.WorkflowItems")
                                .Include("Blocks.BlockCommits.WorkflowRules.Swimlanes.WorkflowItems.ConditionGroups")
                                .Include("Blocks.BlockCommits.WorkflowRules.Swimlanes.WorkflowItems.ConditionGroups.ConditionSets")
                                .Include("Blocks.BlockCommits.WorkflowRules.Swimlanes.WorkflowItems.ConditionGroups.ConditionSets.Conditions")
                                .Include("Blocks.BlockCommits.WorkflowRules.Swimlanes.Subflow")
                                .Include("Blocks.BlockCommits.WorkflowRules.Swimlanes.Foreach")
                                .Include("Blocks.BlockCommits.WorkflowRules.Connections")
                                .Include("Blocks.BlockCommits.WorkflowRules.Connections.Source")
                                .Include("Blocks.BlockCommits.WorkflowRules.Connections.Target")
                              .Where(r => r.ParentAppId == appId).ToList();

            return JArray.FromObject(rows, serializer);
        }

        private JArray ExportTCPListeners()
        {
            var rows = context.TCPListeners.Where(r => r.ApplicationId == appId).ToList();
            return JArray.FromObject(rows, serializer);
        }
    }

    class IgnoreAttributeResolver : DefaultContractResolver
    {
        protected override IList<JsonProperty> CreateProperties(Type type, MemberSerialization memberSerialization)
        {
            var result = base.CreateProperties(type, memberSerialization);
            result = result.Where(p => !p.AttributeProvider.GetAttributes(true).Any(a =>
                        a.GetType() == typeof(ImportExportIgnoreAttribute)
                        && !(a as ImportExportIgnoreAttribute).IsKey
                        && !(a as ImportExportIgnoreAttribute).IsLinkKey
                    )).ToList();

            return result;
        }
    }
}

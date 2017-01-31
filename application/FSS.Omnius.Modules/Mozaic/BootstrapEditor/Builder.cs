using System;
using System.Collections.Generic;
using System.IO;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using FSS.Omnius.Modules.Entitron.Entity;
using FSS.Omnius.Modules.Entitron.Entity.Mozaic;
using FSS.Omnius.Modules.Entitron.Entity.Mozaic.Bootstrap;
using Newtonsoft.Json.Linq;

namespace FSS.Omnius.Modules.Mozaic.BootstrapEditor
{
    public class Builder
    {
        MozaicBootstrapPage currentPage;
        DBEntities e;
        string destination;
        
        public Builder(DBEntities context, string viewFolder)
        {
            e = context;
            destination = viewFolder;
        }

        public void BuildPage(MozaicBootstrapPage page)
        {
            currentPage = page;
 
            Compile();
            Save();
            
        }

        private void Compile()
        {
            StringBuilder sb = new StringBuilder();

            sb.Append("@{ Layout = \"~/Views/Shared/_OmniusUserAppLayout.cshtml\"; }");
            sb.Append(RenderComponents(currentPage.Components.Where(c => c.ParentComponent == null).OrderBy(c => c.NumOrder).ToList()));

            currentPage.CompiledPartialView = sb.ToString();
            e.SaveChanges();
        }

        private string RenderComponents(ICollection<MozaicBootstrapComponent> componentList)
        {
            StringBuilder sb = new StringBuilder();
            foreach (MozaicBootstrapComponent c in componentList) {
                Dictionary<string, string> p = MozaicPropertiesParser.ParseMozaicPropertiesString(c.Properties);
                
                string html = "";
                switch(c.UIC) {
                    /***** CONTAINERS *****/
                    case "containers|container":                html = RenderDefault(c, p); break;
                    case "containers|panel":                    html = RenderPanel(c, p); break;
                    case "containers|panel-heading":            html = RenderDefault(c, p); break;
                    case "containers|panel-body":               html = RenderDefault(c, p); break;
                    case "containers|panel-footer":             html = RenderDefault(c, p); break;
                    case "containers|tabs":                     html = RenderDefault(c, p); break;
                    case "containers|tab":                      html = RenderDefault(c, p); break;
                    case "containers|tab-items":                html = RenderDefault(c, p); break;
                    case "containers|tab-content":              html = RenderDefault(c, p); break;
                    case "containers|tab-pane":                 html = RenderDefault(c, p); break;
                    case "containers|accordion":                html = RenderDefault(c, p); break;
                    case "containers|well":                     html = RenderDefault(c, p); break;
                    case "containers|list":                     html = RenderDefault(c, p); break;
                    case "containers|list-item":                html = RenderDefault(c, p); break;
                    case "containers|div":                      html = RenderDefault(c, p); break;
                    case "containers|list-group":               html = RenderDefault(c, p); break;
                    case "containers|list-group-div":           html = RenderDefault(c, p); break;
                    case "containers|list-group-item":          html = RenderDefault(c, p); break;
                    case "containers|list-group-item-link":     html = RenderDefault(c, p); break;
                    case "containers|list-group-item-button":   html = RenderDefault(c, p); break;
                    /***** CONTROLS *****/
                    case "controls|button":                     html = RenderButton(c, p); break;
                    case "controls|button-group":               html = RenderDefault(c, p); break;
                    case "controls|button-toolbar":             html = RenderDefault(c, p); break;
                    case "controls|split-button":               html = RenderDefault(c, p); break;
                    case "controls|button-dropdown":            html = RenderDefault(c, p); break;
                    case "controls|dropdown-menu":              html = RenderDefault(c, p); break;
                    case "controls|dropdown-menu-item":         html = RenderDefault(c, p); break;
                    case "controls|dropdown-menu-header":       html = RenderDefault(c, p); break;
                    case "controls|dropdown-menu-divider":      html = RenderDefault(c, p); break;
                    case "controls|link":                       html = RenderDefault(c, p); break;
                    /***** FORM *****/
                    case "form|form":                           html = RenderDefault(c, p); break;
                    case "form|form-group":                     html = RenderDefault(c, p); break;
                    case "form|label":                          html = RenderLabel(c, p); break;
                    case "form|input-text":                     html = RenderInput(c, p); break;
                    case "form|input-email":                    html = RenderInput(c, p); break;
                    case "form|input-color":                    html = RenderInput(c, p); break;
                    case "form|select":                         html = RenderSelect(c, p); break;
                    case "form|input-tel":                      html = RenderInput(c, p); break;
                    case "form|input-date":                     html = RenderInput(c, p); break; /// !!!
                    case "form|input-number":                   html = RenderInput(c, p); break;
                    case "form|input-range":                    html = RenderInput(c, p); break;
                    case "form|input-hidden":                   html = RenderInput(c, p); break;
                    case "form|input-url":                      html = RenderInput(c, p); break;
                    case "form|input-search":                   html = RenderInput(c, p); break;
                    case "form|input-password":                 html = RenderInput(c, p); break;
                    case "form|input-file":                     html = RenderInput(c, p); break;
                    case "form|textarea":                       html = RenderTextarea(c, p); break;
                    case "form|checkbox-group":                 html = RenderDefault(c, p); break;
                    case "form|radio-group":                    html = RenderDefault(c, p); break;
                    case "form|checkbox":                       html = RenderCheckbox(c, p); break;
                    case "form|radio":                          html = RenderRadio(c, p); break;
                    case "form|static-control":                 html = RenderDefault(c, p); break;
                    case "form|help-text":                      html = RenderDefault(c, p); break;
                    case "form|input-group":                    html = RenderDefault(c, p); break;
                    case "form|fieldset":                       html = RenderDefault(c, p); break;
                    case "form|legend":                         html = RenderDefault(c, p); break;
                    case "form|left-addon":                     html = RenderDefault(c, p); break;
                    case "form|right-addon":                    html = RenderDefault(c, p); break;
                    case "form|form-control-feedback":          html = RenderDefault(c, p); break;
                    /***** GRID *****/
                    case "grid|row":                            html = RenderDefault(c, p); break;
                    case "grid|column":                         html = RenderDefault(c, p); break;
                    case "grid|clearfix":                       html = RenderDefault(c, p); break;
                    /***** IMAGE *****/
                    case "image|image":                         html = RenderDefault(c, p); break;
                    case "image|icon":                          html = RenderDefault(c, p); break;
                    case "image|figure":                        html = RenderDefault(c, p); break;
                    case "image|figcaption":                    html = RenderDefault(c, p); break;
                    /***** MISC *****/
                    case "misc|custom-code":                    html = RenderDefault(c, p); break;
                    case "misc|modal":                          html = RenderDefault(c, p); break;
                    case "misc|modal-header":                   html = RenderDefault(c, p); break;
                    case "misc|modal-body":                     html = RenderDefault(c, p); break;
                    case "misc|modal-footer":                   html = RenderDefault(c, p); break;
                    case "misc|badge":                          html = RenderDefault(c, p); break;
                    case "misc|tag":                            html = RenderDefault(c, p); break;
                    case "misc|caret":                          html = RenderDefault(c, p); break;
                    case "misc|close":                          html = RenderDefault(c, p); break;
                    case "misc|hr":                             html = RenderDefault(c, p); break;
                    case "misc|responsive-embed":               html = RenderDefault(c, p); break;
                    case "misc|progressBar":                    html = RenderDefault(c, p); break;
                    case "misc|breadcrumbs":                    html = RenderDefault(c, p); break;
                    case "misc|breadcrumbs-item":               html = RenderDefault(c, p); break;
                    case "misc|breadcrumbs-active":             html = RenderDefault(c, p); break;
                    case "misc|breadcrumbs-inactive":           html = RenderDefault(c, p); break;
                    /***** PAGE *****/
                    case "page|page-header":                    html = RenderDefault(c, p); break;
                    case "page|header":                         html = RenderDefault(c, p); break;
                    case "page|footer":                         html = RenderDefault(c, p); break;
                    case "page|hgroup":                         html = RenderDefault(c, p); break;
                    case "page|section":                        html = RenderDefault(c, p); break;
                    case "page|article":                        html = RenderDefault(c, p); break;
                    case "page|aside":                          html = RenderDefault(c, p); break;
                    /***** TABLE *****/
                    case "table|table":                         html = RenderDefault(c, p); break;
                    case "table|tr":                            html = RenderDefault(c, p); break;
                    case "table|cell":                          html = RenderDefault(c, p); break;
                    case "table|td":                            html = RenderDefault(c, p); break;
                    case "table|th":                            html = RenderDefault(c, p); break;
                    case "table|thead":                         html = RenderDefault(c, p); break;
                    case "table|tbody":                         html = RenderDefault(c, p); break;
                    case "table|tfoot":                         html = RenderDefault(c, p); break;
                    case "table|caption":                       html = RenderDefault(c, p); break;
                    /***** TEXT *****/
                    case "text|heading":                        html = RenderDefault(c, p); break;
                    case "text|paragraph":                      html = RenderDefault(c, p); break;
                    case "text|alert":                          html = RenderDefault(c, p); break;
                    case "text|blockquote":                     html = RenderDefault(c, p); break;
                    case "text|small":                          html = RenderDefault(c, p); break;
                    case "text|strong":                         html = RenderDefault(c, p); break;
                    case "text|italic":                         html = RenderDefault(c, p); break;
                    case "text|span":                           html = RenderDefault(c, p); break;
                    /***** UI *****/
                    case "ui|nv-list":                          html = RenderNVList(c, p); break;
                    case "ui|data-table":                       html = RenderDataTable(c, p); break;
                    case "ui|countdown":                        html = RenderCountdown(c, p); break;
                    case "ui|wizzard":                          html = RenderDefault(c, p); break;
                    case "ui|wizzard-body":                     html = RenderDefault(c, p); break;
                    case "ui|wizzard-phase":                    html = RenderDefault(c, p); break;
                    /***** DEFAULT *****/
                    default: html = RenderDefault(c, p); break; 
                }

                string children = c.ChildComponents.Count > 0 ? RenderComponents(c.ChildComponents.OrderBy(cc => cc.NumOrder).ToList()) : "";
                html = html.Replace("__CHILDREN__", children);

                sb.Append(html);
                
                /*else if (c.Type == "wizard-phases") {
                    var phaseLabelArray = !string.IsNullOrEmpty(c.Content) ? c.Content.Split(';').ToList() : new List<string>();
                    int labelCount = phaseLabelArray.Count;
                    int activePhase = 1;
                    if (!string.IsNullOrEmpty(c.Properties)) {
                        string[] nameValuePair = c.Properties.Split('=');
                        if (nameValuePair.Length == 2) {
                            if (nameValuePair[0].ToLower() == "activephase")
                                activePhase = int.Parse(nameValuePair[1]);
                        }
                    }
                    stringBuilder.Append($"<{c.Tag} id=\"uic_{c.Name}\" name=\"{c.Name}\" {c.Attributes} class=\"uic {c.Classes}\" style=\"left: {c.PositionX}; top: {c.PositionY}; ");
                    stringBuilder.Append($"width: {c.Width}; height: {c.Height}; {c.Styles}\">");
                    stringBuilder.Append($"<div class=\"wizard-phases-frame\"></div><svg class=\"phase-background\" width=\"846px\" height=\"84px\">");
                    stringBuilder.Append($"<defs><linearGradient id=\"grad-light\" x1=\"0%\" y1=\"0%\" x2=\"0%\" y2=\"100%\">");
                    stringBuilder.Append($"<stop offset=\"0%\" style=\"stop-color:#dceffa ;stop-opacity:1\" /><stop offset=\"100%\"");
                    stringBuilder.Append($"style =\"stop-color:#8dceed;stop-opacity:1\" /></linearGradient>");
                    stringBuilder.Append($"<linearGradient id=\"grad-blue\" x1=\"0%\" y1=\"0%\" x2=\"0%\" y2=\"100%\">");
                    stringBuilder.Append($"<stop offset=\"0%\" style=\"stop-color:#0099cc;stop-opacity:1\" />");
                    stringBuilder.Append($"<stop offset=\"100%\" style=\"stop-color:#0066aa;stop-opacity:1\" /></linearGradient></defs>");

                    stringBuilder.Append($"<path d=\"M0 0 L0 88 L 280 88 L324 44 L280 0 Z\"");
                    stringBuilder.Append($"fill =\"url({(activePhase == 1 ? "#grad-blue" : "#grad-light")})\" />");
                    stringBuilder.Append($"<path d=\"M280 88 L324 44 L280 0 L560 0 L604 44 L560 88 Z\"");
                    stringBuilder.Append($"fill =\"url({(activePhase == 2 ? "#grad-blue" : "#grad-light")})\" />");
                    stringBuilder.Append($"<path d=\"M560 0 L604 44 L560 88 L850 88 L850 0 Z\"");
                    stringBuilder.Append($"fill =\"url({(activePhase == 3 ? "#grad-blue" : "#grad-light")})\" /></svg>");

                    stringBuilder.Append($"<div class=\"phase phase1 {(activePhase == 1 ? "phase-active" : "")} {(activePhase > 1 ? "phase-done" : "")}\"><div class=\"phase-icon-circle\">");
                    stringBuilder.Append($"{(activePhase > 1 ? "<div class=\"fa fa-check phase-icon-symbol\"></div>" : "<div class=\"phase-icon-number\">1</div>")}</div>");
                    stringBuilder.Append($"<div class=\"phase-label\">{(labelCount >= 1 ? phaseLabelArray[0] : "Fáze 1")}</div></div>");

                    stringBuilder.Append($"<div class=\"phase phase2 {(activePhase == 2 ? "phase-active" : "")} {(activePhase > 2 ? "phase-done" : "")}\"><div class=\"phase-icon-circle\">");
                    stringBuilder.Append($"{(activePhase > 2 ? "<div class=\"fa fa-check phase-icon-symbol\"></div>" : "<div class=\"phase-icon-number\">2</div>")}</div>");
                    stringBuilder.Append($"<div class=\"phase-label\">{(labelCount >= 2 ? phaseLabelArray[1] : "Fáze 2")}</div></div>");

                    stringBuilder.Append($"<div class=\"phase phase3 {(activePhase == 3 ? "phase-active" : "")}\"><div class=\"phase-icon-circle\">");
                    stringBuilder.Append($"<div class=\"phase-icon-number\">3</div></div>");
                    stringBuilder.Append($"<div class=\"phase-label\">{(labelCount >= 3 ? phaseLabelArray[2] : "Fáze 3")}</div></div>");

                    stringBuilder.Append($"</{c.Tag}>");
                }*/
            }

            return sb.ToString();
        }

        private string BuildAttributes(MozaicBootstrapComponent c)
        {
            return BuildAttributes(c, new List<string>(), new Dictionary<string, string>());
        }

        private string BuildAttributes(MozaicBootstrapComponent c, List<string> ignoreAttrs)
        {
            return BuildAttributes(c, ignoreAttrs, new Dictionary<string, string>());
        }

        private string BuildAttributes(MozaicBootstrapComponent c, List<string> ignoreAttrs, Dictionary<string, string> mergeAttrs)
        {
            JToken jAttrs = JToken.Parse(c.Attributes);
            List<string> attrList = new List<string>();

            if(!string.IsNullOrEmpty(c.ElmId)) {
                attrList.Add($"id=\"{c.ElmId}\"");
            }

            foreach(JToken t in jAttrs) {
                if(ignoreAttrs.Contains((string)t["name"])) {
                    continue;
                }
                string value = (string)t["value"];
                if(mergeAttrs.ContainsKey((string)t["name"])) {
                    value = mergeAttrs[(string)t["name"]];
                    mergeAttrs.Remove((string)t["name"]);
                }

                attrList.Add($"{(string)t["name"]}=\"{value}\"");
            }

            if(mergeAttrs.Count > 0) {
                foreach(KeyValuePair<string, string> kp in mergeAttrs) {
                    attrList.Add($"{kp.Key}=\"{kp.Value}\"");
                }
            }

            return string.Join(" ", attrList);
        }

        private string GetAttribute(string attrs, string attrName)
        {
            JToken jAttrs = JToken.Parse(attrs);

            foreach(JToken t in jAttrs) {
                if((string)t["name"] == attrName) {
                    return (string)t["value"];
                }
            }

            return null;
        }

        private void Save()
        {
            string requestedPath = $"/Views/App/{currentPage.ParentApp.Id}/Page/Bootstrap/{currentPage.Id}.cshtml";
            var oldPage = e.Pages.FirstOrDefault(c => c.ViewPath == requestedPath);

            if (oldPage == null) {
                var newPage = new Page
                {
                    ViewName = currentPage.Name,
                    ViewPath = requestedPath,
                    ViewContent = currentPage.CompiledPartialView,
                    IsBootstrap = true
                };
                e.Pages.Add(newPage);
                e.SaveChanges();
                currentPage.CompiledPageId = newPage.Id;
            }
            else {
                oldPage.ViewName = currentPage.Name;
                oldPage.ViewContent = currentPage.CompiledPartialView;
                oldPage.IsBootstrap = true;
                currentPage.CompiledPageId = oldPage.Id;
            }

            string fileName = destination + $"\\Bootstrap\\{currentPage.Id}.cshtml";

            File.WriteAllText(fileName, currentPage.CompiledPartialView);
        }


        #region RENDER METHODS

        private string RenderDefault(MozaicBootstrapComponent c, Dictionary<string, string> properties)
        {
            StringBuilder html = new StringBuilder();
            string attrs = BuildAttributes(c);

            html.Append($"<{c.Tag} {attrs}>{c.Content}__CHILDREN__</{c.Tag}>");
            return html.ToString();
        }

        private string RenderPanel(MozaicBootstrapComponent c, Dictionary<string, string> properties)
        {
            StringBuilder html = new StringBuilder();

            string generatedAttributes = "";
            string attrs = BuildAttributes(c);

            if (!string.IsNullOrEmpty(c.Properties)) {
                string[] tokenPairs = c.Properties.Split(';');
                foreach (string tokens in tokenPairs) {
                    string[] nameValuePair = tokens.Split('=');
                    if (nameValuePair.Length == 2) {
                        if (nameValuePair[0].ToLower() == "hiddenby")
                            generatedAttributes += $" panelHiddenBy=\"{nameValuePair[1]}\"";
                        else if (nameValuePair[0].ToLower() == "clone")
                            generatedAttributes += $" panelClonedBy=\"{nameValuePair[1]}\"";
                    }
                }
            }
            html.Append($"<{c.Tag} {attrs} {generatedAttributes}>{c.Content}__CHILDREN__</{c.Tag}>");
            return html.ToString();
        }

        private string RenderButton(MozaicBootstrapComponent c, Dictionary<string, string> properties)
        {
            StringBuilder html = new StringBuilder();
            string attrs = BuildAttributes(c);

            var wfItem = e.TapestryDesignerWorkflowItems.Where(i => i.ComponentName == c.ElmId && i.IsBootstrap == true).OrderByDescending(i => i.Id).FirstOrDefault();
            html.Append($"<{c.Tag} {attrs} {(wfItem != null && wfItem.isAjaxAction != null && wfItem.isAjaxAction.Value ? " data-ajax=\"true\"" : "")}>{c.Content}__CHILDREN__</{c.Tag}>");
            
            return html.ToString();
        }

        private string RenderNVList(MozaicBootstrapComponent c, Dictionary<string, string> properties)
        {
            StringBuilder html = new StringBuilder();
            string attrs = BuildAttributes(c);

            html.Append($"<{c.Tag} {attrs}>");
                html.Append($"@{{if(ViewData.ContainsKey(\"tableData_{c.ElmId}\")) {{ var tableData = (System.Data.DataTable)ViewData[\"tableData_{c.ElmId}\"];");
                    html.Append($"foreach (System.Data.DataColumn col in tableData.Columns) {{ if (!col.Caption.StartsWith(\"hidden__\")){{<tr><td class=\"name-cell\">@col.Caption</td>");
                    html.Append($"<td class=\"value-cell\">@(tableData.Rows.Count>0?tableData.Rows[0][col.ColumnName]:\"no data\")</td></tr>");
                html.Append($"}} }} }} }}");
            html.Append($"</{c.Tag}>");
            
            return html.ToString();
        }

        private string RenderDataTable(MozaicBootstrapComponent c, Dictionary<string, string> properties)
        {
            StringBuilder html = new StringBuilder();
            string attrs = BuildAttributes(c, new List<string>() { "data-actions" });
            string actions = GetAttribute(c.Attributes, "data-actions");
            string cellContentTemplate = properties.ContainsKey("allowHtml") ? $"@Html.Raw(cell.ToString())" : $"@cell.ToString()";

            string actionsHeader = actions != null ? "<th>Akce</th>" : "";
            string actionsFooter = actions != null ? "<th>Akce</th>" : "";
            string actionsIcons = "";

            if(actions != null) {
                List<string> actionsIconsList = new List<string>();
                JToken jActions = JToken.Parse(actions.Replace('\'', '"'));
                foreach(JToken a in jActions) {
                    actionsIconsList.Add($"<i title=\"{(string)a["title"]}\" class=\"{(string)a["icon"]}\" data-action=\"{(string)a["action"]}\" data-idparam=\"{(string)a["idParam"]}\"></i>");
                }
                actionsIcons = "<td class=\"actionIcons\">" + string.Join(" ", actionsIconsList) + "</td>";
            }

            html.Append($@"
@{{ if(ViewData.ContainsKey(""tableData_{c.ElmId}"") && ((System.Data.DataTable)(ViewData[""tableData_{c.ElmId}""])).Rows.Count > 0) {{
    <{c.Tag} {attrs}>
        <thead>
            <tr>
            @foreach (System.Data.DataColumn col in ((System.Data.DataTable)(ViewData[""tableData_{c.ElmId}""])).Columns) {{
                <th>@col.Caption</th>
            }}
                {actionsHeader}
            </tr>
        </thead>
        <tfoot>
            <tr>@foreach (System.Data.DataColumn col in ((System.Data.DataTable)(ViewData[""tableData_{c.ElmId}""])).Columns) {{
                <th>@col.Caption</th>
            }}
                {actionsFooter}
            </tr>
        </tfoot>
        <tbody>
        @foreach(System.Data.DataRow row in ((System.Data.DataTable)(ViewData[""tableData_{c.ElmId}""])).Rows) {{
            <tr>@foreach (var cell in row.ItemArray) {{
                <td>{cellContentTemplate}</td>
            }}
                {actionsIcons}
            </tr>
        }}
        </tbody>
    </{c.Tag}>
}} else {{
    <div class=""alert alert-info empty-table-label"">Tabulka neobsahuje žádná data</div>
}} }}
<input type=""hidden"" name=""{c.ElmId}"" />
");
            
            return html.ToString();
        }

        private string RenderCountdown(MozaicBootstrapComponent c, Dictionary<string, string> properties)
        {
            StringBuilder html = new StringBuilder();
            string attrs = BuildAttributes(c);
            string targetDate = $@"@(ViewData.ContainsKey(""countdownTargetData_{c.ElmId}"") ? @ViewData[""countdownTargetData_{c.ElmId}""] : """")";

            html.Append($@"<{c.Tag} {attrs} countdownTarget=""{targetDate}"">{c.Content}</{c.Tag}>");
            
            return html.ToString();
        }

        private string RenderLabel(MozaicBootstrapComponent c, Dictionary<string, string> properties)
        {
            StringBuilder html = new StringBuilder();
            string attrs = BuildAttributes(c);
            string labelText = $"@Html.Raw(ViewData.ContainsKey(\"inputData_{c.ElmId}\") ? \"{c.Content}\".Replace(\"{{var1}}\", ViewData[\"inputData_{c.ElmId}\"].ToString()) : \"{c.Content}\" )";

            html.Append($"<{c.Tag} {attrs}>{labelText}</{c.Tag}>");
            
            return html.ToString();
        }

        private string RenderInput(MozaicBootstrapComponent c, Dictionary<string, string> properties)
        {
            StringBuilder html = new StringBuilder();

            Dictionary<string, string> mergeAttrs = new Dictionary<string, string>();
            mergeAttrs.Add("value", $"@(ViewData.ContainsKey(\"inputData_{c.ElmId}\") ? @ViewData[\"inputData_{c.ElmId}\"] : \"\")");

            string attrs = BuildAttributes(c, new List<string>(), mergeAttrs);

            html.Append($"<{c.Tag} {attrs} />");
                
            return html.ToString();
        }

        private string RenderSelect(MozaicBootstrapComponent c, Dictionary<string, string> properties)
        {
            StringBuilder html = new StringBuilder();
            string attrs = BuildAttributes(c);

            string sortMode = "key";
            foreach(KeyValuePair<string, string> kp in properties) {
                if(kp.Key.ToLower() == "sortby" && kp.Value.ToLower() == "value") {
                    sortMode = "value";
                }
            }

            string sort = sortMode == "value" ? ".OrderBy(p => p.Value)" : "";

            html.Append($@"
<{c.Tag} {attrs}>
    {c.Content}
    @{{ if(ViewData[""dropdownData_{c.ElmId}""] != null) {{
            foreach(var option in ((Dictionary<int, string>)ViewData[""dropdownData_{c.ElmId}""]){sort}) {{
                <option value=""@(option.Key)"" @(ViewData.ContainsKey(""dropdownSelection_{ c.ElmId}"") && ViewData[""dropdownSelection_{c.ElmId}""] is int && (int)ViewData[""dropdownSelection_{c.ElmId}""] == option.Key ? ""selected"" : """")>
                    @(option.Value)
                </option>
            }}
        }}
    }}    
</{c.Tag}>");
            
            return html.ToString();
        }

        private string RenderTextarea(MozaicBootstrapComponent c, Dictionary<string, string> properties)
        {
            StringBuilder html = new StringBuilder();
            string attrs = BuildAttributes(c);

            html.Append($"<{c.Tag} {attrs}>@ViewData[\"inputData_{c.ElmId}\"]</{c.Tag}>");
            
            return html.ToString();
        }

        private string RenderCheckbox(MozaicBootstrapComponent c, Dictionary<string, string> properties)
        {
            StringBuilder html = new StringBuilder();
            string attrs = BuildAttributes(c);

            html.Append($"<input {attrs} @(ViewData.ContainsKey(\"checkboxData_{c.ElmId}\") && (bool)ViewData[\"checkboxData_{c.ElmId}\"] ? \" checked\" : \"\") />");

            return html.ToString();
        }

        private string RenderRadio(MozaicBootstrapComponent c, Dictionary<string, string> properties)
        {
            StringBuilder html = new StringBuilder();
            string attrs = BuildAttributes(c);

            html.Append($"<input {attrs} @(ViewData.ContainsKey(\"checkboxData_{c.ElmId}\") && (bool)ViewData[\"checkboxData_{c.ElmId}\"] ? \" checked\" : \"\") />");

            return html.ToString();
        }


        #endregion
    }
}

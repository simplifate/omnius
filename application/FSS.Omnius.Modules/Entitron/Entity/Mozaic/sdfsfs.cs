//using System.Collections.Generic;
//using System.ComponentModel.DataAnnotations.Schema;
//using System.Text;

//using FSS.Omnius.Modules.Entitron.Entity.Master;
//using System.Linq;
//using FSS.Omnius.Modules.Entitron.Entity.Tapestry;
//using System;
//using System.ComponentModel.DataAnnotations;
//using Newtonsoft.Json;

//namespace FSS.Omnius.Modules.Entitron.Entity.Mozaic
//{
//    [Table("MozaicEditor_Pages")]
//    public class MozaicEditorPage
//    {
//        public int Id { get; set; }
//        public string Name { get; set; }
//        public bool IsModal { get; set; }
//        public int? ModalWidth { get; set; }
//        public int? ModalHeight { get; set; }
//        public string CompiledPartialView { get; set; }
//        public int CompiledPageId { get; set; }
//        public virtual ICollection<MozaicEditorComponent> Components { get; set; }

//        [JsonIgnore]
//        public virtual ICollection<TapestryDesignerResourceItem> ResourceItems { get; set; }

//        public virtual Application ParentApp { get; set; }

//        public void Recompile()
//        {
//            StringBuilder stringBuilder = new StringBuilder();
//            stringBuilder.Append("@using FSS.Omnius.Utils");
//            if (IsModal)
//            {
//                stringBuilder.Append("@{T t = new T( (string)ViewData[\"locale\"] );}");
//                stringBuilder.Append("@{ Layout = \"~/Views/Shared/_PartialViewAjaxLayout.cshtml\"; }");

//            }
//            else
//            {
//                stringBuilder.Append("@{ Layout = \"~/Views/Shared/_OmniusUserAppLayout.cshtml\"; }");
//                stringBuilder.Append("@{T t = new T( (string)ViewData[\"locale\"] );}");
//            }
//            stringBuilder.Append("<form class=\"mozaicForm\" method=\"post\">");

//            RenderComponentList(Components.Where(c => c.ParentComponent == null).ToList(), stringBuilder, true);

//            stringBuilder.Append("<input type=\"hidden\" name=\"registry\" value=\"@ViewData[\"crossBlockRegistry\"]\" />");
//            stringBuilder.Append("</form>");
//            CompiledPartialView = stringBuilder.ToString();
//        }
//        private void RenderComponentList(ICollection<MozaicEditorComponent> ComponentList, StringBuilder stringBuilder, bool allowNesting)
//        {
//            foreach (MozaicEditorComponent c in ComponentList)
//            {
//                if (c.Type == "info-container")
//                {
//                    stringBuilder.Append($"<div id=\"uic_{c.Name}\" name=\"{c.Name}\" class=\"uic info-container {c.Classes}\" style=\"left: {c.PositionX}; top: {c.PositionY}; ");
//                    stringBuilder.Append($"width: {c.Width}; height: {c.Height}; {c.Styles}\">");
//                    stringBuilder.Append($"<div class=\"fa fa-info-circle info-container-icon\"></div>");
//                    stringBuilder.Append($"<div class=\"info-container-header\">@(t._(\"{c.Label}\"))</div>");
//                    stringBuilder.Append($"<div class=\"info-container-body\">@(t._(\"{c.Content}\"))</div></div>");
//                }
//                else if (c.Type == "button-simple")
//                {
//                    bool invisible = false;
//                    if (!string.IsNullOrEmpty(c.Properties))
//                    {
//                        string[] tokenPairs = c.Properties.Split(';');
//                        foreach (string tokens in tokenPairs)
//                        {
//                            string[] nameValuePair = tokens.Split('=');
//                            if (nameValuePair.Length == 2)
//                            {
//                                // No settings yet
//                            }
//                            else
//                            {
//                                if (tokens == "hidden")
//                                    invisible = true;
//                            }
//                        }
//                    }
//                    DBEntities context = DBEntities.instance;

//                    var wfItem = context.TapestryDesignerWorkflowItems.Where(i => i.ComponentName == c.Name).OrderByDescending(i => i.Id).FirstOrDefault();
//                    stringBuilder.Append($"<{c.Tag} id=\"uic_{c.Name}\" name=\"button\" value=\"{c.Name}\" tabindex=\"{c.TabIndex}\" {c.Attributes} class=\"uic {c.Classes}{(wfItem != null && wfItem.isAjaxAction != null && wfItem.isAjaxAction.Value ? " runAjax" : "")}\" buttonName=\"{c.Name}\" style=\"left: {c.PositionX}; top: {c.PositionY}; ");
//                    stringBuilder.Append($"width: {c.Width}; height: {c.Height}; {(invisible ? "display:none;" : "")} {c.Styles}\">@(t._(\"{c.Label}\"))</{c.Tag}>");
//                }
//                else if (c.Type == "dropdown-select")
//                {
//                    stringBuilder.Append($"<{c.Tag} id=\"uic_{c.Name}\" name=\"{c.Name}\" tabindex=\"{c.TabIndex}\" {c.Attributes} class=\"uic {c.Classes}\" style=\"left: {c.PositionX}; top: {c.PositionY}; ");
//                    stringBuilder.Append($"width: {c.Width}; height: {c.Height}; {c.Styles}\">");
//                    string sortMode = "key";
//                    if (!string.IsNullOrEmpty(c.Properties))
//                    {
//                        string[] tokenPairs = c.Properties.Split(';');
//                        foreach (string tokens in tokenPairs)
//                        {
//                            string[] nameValuePair = tokens.Split('=');
//                            if (nameValuePair.Length == 2)
//                            {
//                                if (nameValuePair[0].ToLower() == "defaultoption")
//                                    stringBuilder.Append($"<option value=\"-1\">@(t._(\"{nameValuePair[1]}\"))</option>");
//                                if (nameValuePair[0].ToLower() == "sortby" && nameValuePair[1].ToLower() == "value")
//                                    sortMode = "value";
//                            }
//                        }
//                    }
//                    if (sortMode == "value")
//                    {
//                        stringBuilder.Append($"@{{ if(ViewData[\"dropdownData_{c.Name}\"] != null) ");
//                        stringBuilder.Append($"{{ foreach(var option in ((Dictionary<int, string>)ViewData[\"dropdownData_{c.Name}\"]).OrderBy(p => p.Value))");
//                        stringBuilder.Append($"{{ <option value=\"@(option.Key)\" @(ViewData.ContainsKey(\"dropdownSelection_{c.Name}\") && ViewData[\"dropdownSelection_{c.Name}\"] is int && (int)ViewData[\"dropdownSelection_{c.Name}\"] == option.Key ? \"selected\" : \"\") >");
//                        stringBuilder.Append($"@(t._(option.Value))</option>}}; }} }}");
//                    }
//                    else
//                    {
//                        stringBuilder.Append($"@{{ if(ViewData[\"dropdownData_{c.Name}\"] != null) ");
//                        stringBuilder.Append($"{{ foreach(var option in (Dictionary<int, string>)ViewData[\"dropdownData_{c.Name}\"])");
//                        stringBuilder.Append($"{{ <option value=\"@(option.Key)\" @(ViewData.ContainsKey(\"dropdownSelection_{c.Name}\") && ViewData[\"dropdownSelection_{c.Name}\"] is int && (int)ViewData[\"dropdownSelection_{c.Name}\"] == option.Key ? \"selected\" : \"\") >");
//                        stringBuilder.Append($"@(t._(option.Value))</option>}}; }} }}");
//                    }
//                    stringBuilder.Append($"</{c.Tag}>");
//                }
//                else if (c.Type == "input-single-line")
//                {
//                    stringBuilder.Append($"<{c.Tag} id =\"uic_{c.Name}\" name=\"{c.Name}\" tabindex=\"{c.TabIndex}\" {c.Attributes} placeholder=\"@(t._(\"{c.Placeholder}\"))\" ");
//                    stringBuilder.Append($"value=\"@(ViewData.ContainsKey(\"inputData_{c.Name}\") ? @ViewData[\"inputData_{c.Name}\"] : \"\")\" class=\"uic {c.Classes}\" style=\"left: {c.PositionX}; top: {c.PositionY}; ");
//                    stringBuilder.Append($"width: {c.Width}; height: {c.Height}; {c.Styles}\"");
//                    if (!string.IsNullOrEmpty(c.Properties))
//                    {
//                        string[] nameValuePair = c.Properties.Split('=');
//                        if (nameValuePair.Length == 2)
//                        {
//                            if (nameValuePair[0].ToLower() == "autosum")
//                                stringBuilder.Append($" writeSumInto=\"{nameValuePair[1]}\"");
//                        }
//                    }
//                    if (c.Classes.Contains("input-read-only"))
//                        stringBuilder.Append($" readonly ");
//                    stringBuilder.Append($"/>");
//                }
//                else if (c.Type == "input-multiline")
//                {
//                    stringBuilder.Append($"<{c.Tag} id=\"uic_{c.Name}\" name=\"{c.Name}\" tabindex=\"{c.TabIndex}\" {c.Attributes} placeholder=\"@(t._(\"{c.Placeholder}\"))\" class=\"uic {c.Classes}\" style=\"left: {c.PositionX}; top: {c.PositionY}; ");
//                    stringBuilder.Append($"width: {c.Width}; height: {c.Height}; {c.Styles}\"");
//                    if (!string.IsNullOrEmpty(c.Properties))
//                    {
//                        string[] nameValuePair = c.Properties.Split('=');
//                        if (nameValuePair.Length == 2)
//                        {
//                            if (nameValuePair[0].ToLower() == "autosum")
//                                stringBuilder.Append($" writeSumInto=\"{nameValuePair[1]}\"");
//                        }
//                    }
//                    if (c.Classes.Contains("input-read-only"))
//                        stringBuilder.Append($" readonly ");
//                    stringBuilder.Append($">@ViewData[\"inputData_{c.Name}\"]</{c.Tag}>");
//                }
//                else if (c.Type == "label")
//                {
//                    bool invisible = false;
//                    if (!string.IsNullOrEmpty(c.Properties))
//                    {
//                        string[] tokenPairs = c.Properties.Split(';');
//                        foreach (string tokens in tokenPairs)
//                        {
//                            string[] nameValuePair = tokens.Split('=');
//                            if (nameValuePair.Length == 2)
//                            {
//                                // No settings yet
//                            }
//                            else
//                            {
//                                if (tokens == "hidden")
//                                    invisible = true;
//                            }
//                        }
//                    }
//                    string labelText = $"@Html.Raw(ViewData.ContainsKey(\"inputData_{c.Name}\") ? t._(\"{c.Label.Replace("\"", "\\\"")}\").Replace(\"{{var1}}\", t._(ViewData[\"inputData_{c.Name}\"].ToString())) : t._(\"{c.Label.Replace("\"", "\\\"")}\") )";
//                    stringBuilder.Append($"<{c.Tag} id=\"uic_{c.Name}\" name=\"{c.Name}\" {c.Attributes} class=\"uic {c.Classes}\" contentTemplate=\"{c.Content}\" style=\"left: {c.PositionX}; top: {c.PositionY}; ");
//                    stringBuilder.Append($"width: {c.Width}; height: {c.Height}; {(invisible ? "display:none;" : "")} {c.Styles}\">");
//                    stringBuilder.Append($"{labelText}</{c.Tag}>");
//                }
//                else if (c.Type == "breadcrumb")
//                {
//                    stringBuilder.Append($"<div id=\"uic_{c.Name}\" name=\"{c.Name}\" class=\"uic breadcrumb-navigation {c.Classes}\" style=\"left: {c.PositionX}; top: {c.PositionY}; ");
//                    stringBuilder.Append($"width: {c.Width}; height: {c.Height}; {c.Styles}\">");
//                    stringBuilder.Append($"<div class=\"app-icon fa @ViewData[\"appIcon\"]\"></div>");
//                    stringBuilder.Append($"<div class=\"nav-text\">@ViewData[\"appName\"] &gt; @ViewData[\"pageName\"]</div></div>");
//                }
//                else if (c.Type == "checkbox")
//                {
//                    stringBuilder.Append($"<div id=\"uic_{c.Name}\" class=\"uic {c.Classes}\" tabindex=\"{c.TabIndex}\" style=\"left: {c.PositionX}; top: {c.PositionY}; ");
//                    stringBuilder.Append($"width: {c.Width}; height: {c.Height}; {c.Styles}\">");
//                    stringBuilder.Append($"<input type=\"checkbox\" name=\"{c.Name}\"@(ViewData.ContainsKey(\"checkboxData_{c.Name}\") && ViewData[\"checkboxData_{c.Name}\"] is bool && (bool)ViewData[\"checkboxData_{c.Name}\"] ? \" checked\" : \"\") /><span class=\"checkbox-label\">@(t._(\"{c.Label}\"))</span></div>");
//                }
//                else if (c.Type == "radio")
//                {
//                    stringBuilder.Append($"<div id=\"uic_{c.Name}\" class=\"uic {c.Classes}\" tabindex=\"{c.TabIndex}\" style=\"left: {c.PositionX}; top: {c.PositionY}; ");
//                    stringBuilder.Append($"width: {c.Width}; height: {c.Height}; {c.Styles}\">");
//                    stringBuilder.Append($"<input type=\"radio\" name=\"{c.Name}\" /><span class=\"radio-label\">@(t._(\"{c.Label}\"))</span></div>");
//                }
//                else if (c.Type == "dropdown-select")
//                {
//                    stringBuilder.Append($"<{c.Tag} id=\"uic_{c.Name}\" name=\"{c.Name}\" tabindex=\"{c.TabIndex}\" {c.Attributes} class=\"uic {c.Classes}\" style=\"left: {c.PositionX}; top: {c.PositionY}; ");
//                    stringBuilder.Append($"width: {c.Width}; height: {c.Height}; {c.Styles}\">");
//                    stringBuilder.Append($"@{{ if(ViewData[\"dropdownData_{c.Name}\"] != null) ");
//                    stringBuilder.Append($"{{ foreach(var option in (Dictionary<int, string>)ViewData[\"dropdownData_{c.Name}\"])");
//                    stringBuilder.Append($"{{ <option value=\"@(option.Key)\" @(ViewData.ContainsKey(\"dropdownSelection_{c.Name}\") && (int)ViewData[\"dropdownSelection_{c.Name}\"] == option.Key ? \"selected\" : \"\") >");
//                    stringBuilder.Append($"@(t._(option.Value))</option>}}; }} }}</{c.Tag}>");
//                }
//                else if (c.Type == "data-table-read-only")
//                {
//                    stringBuilder.Append($"<{c.Tag} id=\"uic_{c.Name}\" name=\"{c.Name}\" {c.Attributes} class=\"uic {c.Classes}\" style=\"left: {c.PositionX}; top: {c.PositionY}; ");
//                    stringBuilder.Append($"width: {c.Width}; height: {c.Height}; {c.Styles}\" uicWidth=\"{c.Width}\">");
//                    stringBuilder.Append($"@{{ if(ViewData.ContainsKey(\"tableData_{c.Name}\")) {{");
//                    stringBuilder.Append($"<thead><tr>@foreach (System.Data.DataColumn col in ((System.Data.DataTable)(ViewData[\"tableData_{c.Name}\"])).Columns)");
//                    stringBuilder.Append($"{{<th>@(t._(col.Caption))</th>}}</tr></thead><tfoot><tr>@foreach (System.Data.DataColumn col in ((System.Data.DataTable)(ViewData[\"tableData_{c.Name}\"])).Columns)");
//                    stringBuilder.Append($"{{<th>@col.Caption</th>}}</tr></tfoot><tbody>@foreach(System.Data.DataRow row in ((System.Data.DataTable)(ViewData[\"tableData_{c.Name}\"])).Rows)");
//                    stringBuilder.Append($"{{<tr>@foreach (var cell in row.ItemArray){{<td>@t._(cell.ToString())</td>}}</tr>}}</tbody>}} }}</{c.Tag}>");
//                }
//                else if (c.Type == "name-value-list")
//                {
//                    stringBuilder.Append($"<{c.Tag} id=\"uic_{c.Name}\" name=\"{c.Name}\" {c.Attributes} class=\"uic {c.Classes}\" style=\"left: {c.PositionX}; top: {c.PositionY}; ");
//                    stringBuilder.Append($"width: {c.Width}; height: {c.Height}; {c.Styles}\">");
//                    stringBuilder.Append($"@{{ if(ViewData.ContainsKey(\"tableData_{c.Name}\")) {{ var tableData = (System.Data.DataTable)ViewData[\"tableData_{c.Name}\"];");
//                    stringBuilder.Append($"foreach (System.Data.DataColumn col in tableData.Columns) {{ if (!col.Caption.StartsWith(\"hidden__\")){{<tr><td class=\"name-cell\">@(t._(col.Caption))</td>");
//                    stringBuilder.Append($"<td class=\"value-cell\">@(tableData.Rows.Count>0?tableData.Rows[0][col.ColumnName]:\"no data\")</td></tr>");
//                    stringBuilder.Append($"}} }} }} }}</{c.Tag}>");
//                }
//                else if (c.Type == "data-table-with-actions")
//                {
//                    string actionButtons = "edit-delete";
//                    if (!string.IsNullOrEmpty(c.Properties))
//                    {
//                        string[] nameValuePair = c.Properties.Split('=');
//                        if (nameValuePair.Length == 2)
//                        {
//                            if (nameValuePair[0].ToLower() == "actions")
//                            {
//                                actionButtons = nameValuePair[1].ToLower();
//                            }
//                        }
//                    }
//                    stringBuilder.Append($"<{c.Tag} id=\"uic_{c.Name}\" name=\"{c.Name}\" {c.Attributes} class=\"uic {c.Classes}\" style=\"left: {c.PositionX}; top: {c.PositionY}; ");
//                    stringBuilder.Append($"width: {c.Width}; height: {c.Height}; {c.Styles}\" uicWidth=\"{c.Width}\">");
//                    stringBuilder.Append($"@{{ if(ViewData.ContainsKey(\"tableData_{c.Name}\")) {{");
//                    stringBuilder.Append($"<thead><tr>@foreach (System.Data.DataColumn col in ((System.Data.DataTable)(ViewData[\"tableData_{c.Name}\"])).Columns)");
//                    stringBuilder.Append($"{{<th>@(t._(col.Caption))</th>}}<th>@(t._(\"Akce\"))</th></tr></thead><tfoot><tr>@foreach (System.Data.DataColumn col in ((System.Data.DataTable)(ViewData[\"tableData_{c.Name}\"])).Columns)");
//                    stringBuilder.Append($"{{<th>@col.Caption</th>}}<th>Akce</th></tr></tfoot><tbody>@foreach(System.Data.DataRow row in ((System.Data.DataTable)(ViewData[\"tableData_{c.Name}\"])).Rows)");
//                    stringBuilder.Append($"{{<tr>@foreach (var cell in row.ItemArray){{<td>@t._(cell.ToString())</td>}}<td class=\"actionIcons\">");
//                    switch (actionButtons)
//                    {
//                        case "enter":
//                            stringBuilder.Append($"<i title=\"@(t._(\"Vstoupit\"))\" class=\"fa fa-sign-in rowEditAction\"></i>");
//                            break;
//                        case "enter-details":
//                            stringBuilder.Append($"<i title=\"@(t._(\"Vstoupit\"))\" class=\"fa fa-sign-in rowEditAction\"></i><i title=\"@(t._(\"Detail\"))\" class=\"fa fa-search rowDetailsAction\"></i>");
//                            break;
//                        case "delete":
//                            stringBuilder.Append($"<i title=\"@(t._(\"Smazat\"))\" class=\"fa fa-remove rowDeleteAction\"></i>");
//                            break;
//                        case "details":
//                            stringBuilder.Append($"<i title=\"@(t._(\"Detail\"))\" class=\"fa fa-search rowDetailsAction\"></i>");
//                            break;
//                        case "details-edit-delete":
//                            stringBuilder.Append($"<i title=\"@(t._(\"Detail\"))\" class=\"fa fa-search rowDetailsAction\"></i><i title=\"@(t._(\"Editovat\"))\" class=\"fa fa-edit rowEditAction\"></i><i title=\"@(t._(\"Smazat\"))\" class=\"fa fa-remove rowDeleteAction\"></i>");
//                            break;
//                        case "start-stop-edit-delete":
//                            stringBuilder.Append($"<i title=\"@(t._(\"Spustit aukci\"))\" class=\"fa fa-play rowDetailsAction\"></i><i title=\"@(t._(\"Zastavit aukci\"))\" class=\"fa fa-pause row_A_Action\"></i><i title=\"@(t._(\"Editovat\"))\" class=\"fa fa-edit rowEditAction \"></i><i title=\"@(t._(\"Smazat\"))\" class=\"fa fa-remove rowDeleteAction\"></i>");
//                            break;
//                        case "start-stop-edit-delete-details":
//                            stringBuilder.Append($"<i title=\"@(t._(\"Spustit aukci\"))\" class=\"fa fa-play rowDetailsAction\"></i><i title=\"@(t._(\"Zastavit aukci\"))\" class=\"fa fa-pause row_A_Action\"></i><i title=\"@(t._(\"Editovat\"))\" class=\"fa fa-edit rowEditAction \"></i><i title=\"@(t._(\"Smazat\"))\" class=\"fa fa-remove rowDeleteAction\"></i><i title=\"@(t._(\"Přehled poptávek\"))\" class=\"fa fa-clock-o row_B_Action\"></i>");
//                            break;
//                        case "details-edit-delete-role-restriction":
//                            stringBuilder.Append($"<i title=\"@(t._(\"Detail\"))\" class=\"fa fa-search rowDetailsAction\"></i>" +
//                            "@if(((HttpContextBase)ViewData[\"HttpContext\"]).GetLoggedUser().HasRole(\"Superadmin\") || ((HttpContextBase)ViewData[\"HttpContext\"]).GetLoggedUser().HasRole(\"Coordinator\")) {{" +
//                            "<i title=\"@(t._(\"Editovat\"))\" class=\"fa fa-edit rowEditAction\"></i><i title=\"@(t._(\"Smazat\"))\" class=\"fa fa-remove rowDeleteAction\"></i> }}");
//                            break;
//                        case "edit-delete":
//                        default:
//                            stringBuilder.Append($"<i class=\"fa fa-edit rowEditAction\"></i><i class=\"fa fa-remove rowDeleteAction\"></i>");
//                            break;
//                    }
//                    stringBuilder.Append($"</td></tr>}}</tbody>}} }}</{c.Tag}>");
//                }
//                else if (c.Type == "tab-navigation")
//                {
//                    stringBuilder.Append($"<{c.Tag} id=\"uic_{c.Name}\" name=\"{c.Name}\" {c.Attributes} class=\"uic {c.Classes}\" style=\"left: {c.PositionX}; top: {c.PositionY}; ");
//                    stringBuilder.Append($"width: {c.Width}; height: {c.Height}; {c.Styles}\"><li class=\"active\"><a class=\"fa fa-home\"></a></li>");
//                    var tabLabelArray = c.Content.Split(';').ToList();
//                    foreach (string tabLabel in tabLabelArray)
//                    {
//                        if (tabLabel.Length > 0)
//                            stringBuilder.Append($"<li><a>{tabLabel}</a></li>");
//                    }
//                    stringBuilder.Append($"</{c.Tag}>");
//                }
//                else if (c.Type == "wizard-phases")
//                {
//                    var phaseLabelArray = !string.IsNullOrEmpty(c.Content) ? c.Content.Split(';').ToList() : new List<string>();
//                    int labelCount = phaseLabelArray.Count;
//                    int activePhase = 1;
//                    if (!string.IsNullOrEmpty(c.Properties))
//                    {
//                        string[] nameValuePair = c.Properties.Split('=');
//                        if (nameValuePair.Length == 2)
//                        {
//                            if (nameValuePair[0].ToLower() == "activephase")
//                                activePhase = int.Parse(nameValuePair[1]);
//                        }
//                    }
//                    stringBuilder.Append($"<{c.Tag} id=\"uic_{c.Name}\" name=\"{c.Name}\" {c.Attributes} class=\"uic {c.Classes}\" style=\"left: {c.PositionX}; top: {c.PositionY}; ");
//                    stringBuilder.Append($"width: {c.Width}; height: {c.Height}; {c.Styles}\">");
//                    stringBuilder.Append($"<div class=\"wizard-phases-frame\"></div><svg class=\"phase-background\" width=\"846px\" height=\"84px\">");
//                    /*stringBuilder.Append($"<defs><linearGradient id=\"grad-light\" x1=\"0%\" y1=\"0%\" x2=\"0%\" y2=\"100%\">");
//                    stringBuilder.Append($"<stop offset=\"0%\" style=\"stop-color:#dceffa ;stop-opacity:1\" /><stop offset=\"100%\"");
//                    stringBuilder.Append($"style =\"stop-color:#8dceed;stop-opacity:1\" /></linearGradient>");
//                    stringBuilder.Append($"<linearGradient id=\"grad-blue\" x1=\"0%\" y1=\"0%\" x2=\"0%\" y2=\"100%\">");
//                    stringBuilder.Append($"<stop offset=\"0%\" style=\"stop-color:#0099cc;stop-opacity:1\" />");
//                    stringBuilder.Append($"<stop offset=\"100%\" style=\"stop-color:#0066aa;stop-opacity:1\" /></linearGradient></defs>");*/

//                    string color_dark = "#143C8C";
//                    string color_light = "#00AAE1";

//                    stringBuilder.Append($"<path d=\"M0 0 L0 88 L 280 88 L324 44 L280 0 Z\"");
//                    stringBuilder.Append($"fill =\"{(activePhase == 1 ? color_dark : color_light)}\" />");
//                    stringBuilder.Append($"<path d=\"M280 88 L324 44 L280 0 L560 0 L604 44 L560 88 Z\"");
//                    stringBuilder.Append($"fill =\"{(activePhase == 2 ? color_dark : color_light)}\" />");
//                    stringBuilder.Append($"<path d=\"M560 0 L604 44 L560 88 L850 88 L850 0 Z\"");
//                    stringBuilder.Append($"fill =\"{(activePhase == 3 ? color_dark : color_light)}\" /></svg>");

//                    stringBuilder.Append($"<div class=\"phase phase1 {(activePhase == 1 ? "phase-active" : "")} {(activePhase > 1 ? "phase-done" : "")}\"><div class=\"phase-icon-circle\">");
//                    stringBuilder.Append($"{(activePhase > 1 ? "<div class=\"fa fa-check phase-icon-symbol\"></div>" : "<div class=\"phase-icon-number\">1</div>")}</div>");
//                    stringBuilder.Append($"<div class=\"phase-label\">@(t._(\"{(labelCount >= 1 ? phaseLabelArray[0] : "Fáze 1")}\"))</div></div>");

//                    stringBuilder.Append($"<div class=\"phase phase2 {(activePhase == 2 ? "phase-active" : "")} {(activePhase > 2 ? "phase-done" : "")}\"><div class=\"phase-icon-circle\">");
//                    stringBuilder.Append($"{(activePhase > 2 ? "<div class=\"fa fa-check phase-icon-symbol\"></div>" : "<div class=\"phase-icon-number\">2</div>")}</div>");
//                    stringBuilder.Append($"<div class=\"phase-label\">@(t._(\"{(labelCount >= 2 ? phaseLabelArray[1] : "Fáze 2")}\"))</div></div>");

//                    stringBuilder.Append($"<div class=\"phase phase3 {(activePhase == 3 ? "phase-active" : "")}\"><div class=\"phase-icon-circle\">");
//                    stringBuilder.Append($"<div class=\"phase-icon-number\">3</div></div>");
//                    stringBuilder.Append($"<div class=\"phase-label\">@(t._(\"{(labelCount >= 3 ? phaseLabelArray[2] : "Fáze 3")}\"))</div></div>");

//                    stringBuilder.Append($"</{c.Tag}>");
//                }
//                else if (c.Type == "countdown")
//                {
//                    bool invisible = false;
//                    if (!string.IsNullOrEmpty(c.Properties))
//                    {
//                        string[] tokenPairs = c.Properties.Split(';');
//                        foreach (string tokens in tokenPairs)
//                        {
//                            string[] nameValuePair = tokens.Split('=');
//                            if (nameValuePair.Length == 2)
//                            {
//                                // No settings yet
//                            }
//                            else
//                            {
//                                if (tokens == "hidden")
//                                    invisible = true;
//                            }
//                        }
//                    }
//                    stringBuilder.Append($"<{c.Tag} id=\"uic_{c.Name}\" name=\"{c.Name}\" {c.Attributes} class=\"uic {c.Classes}\" style=\"left: {c.PositionX}; top: {c.PositionY}; ");
//                    stringBuilder.Append($"width: {c.Width}; height: {c.Height}; {(invisible ? "display:none;" : "")} {c.Styles}\" countdownTarget=\"@(ViewData.ContainsKey(\"countdownTargetData_{c.Name}\")");
//                    stringBuilder.Append($" ? @ViewData[\"countdownTargetData_{c.Name}\"] : \"\")\">@(t._(\"{c.Content}\"))</{c.Tag}>");
//                }
//                else if (c.Type == "color-picker")
//                {
//                    stringBuilder.Append($"<{c.Tag} id=\"uic_{c.Name}\" name=\"{c.Name}\" tabindex=\"{c.TabIndex}\" {c.Attributes} type=\"text\" placeholder=\"@(t._(\"{c.Placeholder}\"))\" ");
//                    stringBuilder.Append($"value=\"@(ViewData.ContainsKey(\"inputData_{c.Name}\") ? @ViewData[\"inputData_{c.Name}\"] : \"rgb(255, 0, 0)\")\" class=\"uic {c.Classes}\" style=\"left: {c.PositionX}; top: {c.PositionY}; ");
//                    stringBuilder.Append($"width: {c.Width}; height: {c.Height}; {c.Styles}\" />");
//                }
//                else if (c.Type == "static-html")
//                {
//                    bool invisible = false;
//                    if (!string.IsNullOrEmpty(c.Properties))
//                    {
//                        string[] tokenPairs = c.Properties.Split(';');
//                        foreach (string tokens in tokenPairs)
//                        {
//                            string[] nameValuePair = tokens.Split('=');
//                            if (nameValuePair.Length == 2)
//                            {
//                                // No settings yet
//                            }
//                            else
//                            {
//                                if (tokens == "hidden")
//                                    invisible = true;
//                            }
//                        }
//                    }
//                    stringBuilder.Append($"<{c.Tag} id=\"uic_{c.Name}\" name=\"{c.Name}\" {c.Attributes} class=\"uic {c.Classes}\" style=\"left: {c.PositionX}; top: {c.PositionY}; ");
//                    stringBuilder.Append($"width: {c.Width}; height: {c.Height}; {(invisible ? "display:none;" : "")} {c.Styles}\">@Html.Raw(t._(\"{((c.Content ?? "").Replace("\"", "\\\""))}\"))</{c.Tag}>");
//                }
//                else if (c.Type == "panel" && allowNesting)
//                {
//                    bool invisible = false;
//                    string panelAfter = "";
//                    string conditionVariable = "";

//                    if (!string.IsNullOrEmpty(c.Properties))
//                    {
//                        string[] tokenPairs = c.Properties.Split(';');
//                        foreach (string tokens in tokenPairs)
//                        {
//                            string[] nameValuePair = tokens.Split('=');
//                            if (nameValuePair.Length == 2)
//                            {
//                                if (nameValuePair[0].ToLower() == "hiddenby")
//                                {
//                                    stringBuilder.Append($" panelHiddenBy=\"{nameValuePair[1]}\"");
//                                }
//                                else if (nameValuePair[0].ToLower() == "clone")
//                                {
//                                    stringBuilder.Append($" panelClonedBy=\"{nameValuePair[1]}\"");
//                                }
//                                else if (nameValuePair[0].ToLower() == "roles")
//                                {
//                                    var roles = nameValuePair[1].Split(',');
//                                    List<string> conditions = new List<string>();
//                                    foreach (string role in roles)
//                                    {
//                                        conditions.Add($"((HttpContextBase)ViewData[\"HttpContext\"]).GetLoggedUser().HasRole(\"{role}\")");
//                                    }
//                                    stringBuilder.Append("@if(" + string.Join(" || ", conditions) + ") {");
//                                    panelAfter = "}";
//                                }
//                                else if (nameValuePair[0].ToLower() == "showconditionvariable")
//                                {
//                                    conditionVariable = nameValuePair[1];
//                                }
//                            }
//                            else
//                            {
//                                if (tokens == "hidden")
//                                    invisible = true;
//                            }
//                        }
//                    }
//                    if (conditionVariable != "")
//                    {
//                        stringBuilder.Append($"@if(ViewData[\"{conditionVariable}\"] is bool && (bool)ViewData[\"{conditionVariable}\"]==true){{");
//                    }
//                    stringBuilder.Append($"<{c.Tag} id=\"uic_{c.Name}\" name=\"{c.Name}\" {c.Attributes} class=\"uic {c.Classes}\" style=\"left: {c.PositionX}; top: {c.PositionY}; ");
//                    stringBuilder.Append($"width: {c.Width}; height: {c.Height}; {(invisible ? "display:none;" : "")} {c.Styles}\"");
//                    stringBuilder.Append($">");
//                    RenderComponentList(c.ChildComponents, stringBuilder, false);
//                    stringBuilder.Append($"</{c.Tag}>");
//                    stringBuilder.Append(panelAfter);
//                    if (conditionVariable != "")
//                    {
//                        stringBuilder.Append($"}}");
//                    }
//                }
//                else
//                {
//                    bool invisible = false;
//                    if (!string.IsNullOrEmpty(c.Properties))
//                    {
//                        string[] tokenPairs = c.Properties.Split(';');
//                        foreach (string tokens in tokenPairs)
//                        {
//                            string[] nameValuePair = tokens.Split('=');
//                            if (nameValuePair.Length == 2)
//                            {
//                                // No settings yet
//                            }
//                            else {
//                                if (tokens == "hidden")
//                                    invisible = true;
//                            }
//                        }
//                    }
//                    stringBuilder.Append($"<{c.Tag} id=\"uic_{c.Name}\" name=\"{c.Name}\" {c.Attributes} class=\"uic {c.Classes} {(invisible ? "hidden" : "")}\" style=\"left: {c.PositionX}; top: {c.PositionY}; ");
//                    stringBuilder.Append($"width: {c.Width}; height: {c.Height}; {c.Styles}\">@(t._(\"{c.Content}\"))</{c.Tag}>");
//                }
//            }
//        }

//        public MozaicEditorPage()
//        {
//            Components = new List<MozaicEditorComponent>();
//        }
//    }
//    [Table("MozaicEditor_Components")]
//    public class MozaicEditorComponent
//    {
//        public int Id { get; set; }
//        public string Name { get; set; }
//        public string Type { get; set; }
//        public string PositionX { get; set; }
//        public string PositionY { get; set; }
//        public string Width { get; set; }
//        public string Height { get; set; }

//        public string Tag { get; set; }
//        public string Attributes { get; set; }
//        public string Classes { get; set; }
//        public string Styles { get; set; }
//        public string Content { get; set; }

//        public string Label { get; set; }
//        public string Placeholder { get; set; }
//        public string TabIndex { get; set; }
//        public string Properties { get; set; }

//        [JsonIgnore]
//        public virtual ICollection<MozaicEditorComponent> ChildComponents { get; set; }
//        public int? ParentComponentId { get; set; }
//        [JsonIgnore]
//        public virtual MozaicEditorComponent ParentComponent { get; set; }
//        public int MozaicEditorPageId { get; set; }
//        public virtual MozaicEditorPage MozaicEditorPage { get; set; }
//    }

//    public class MozaicModalMetadataItem
//    {
//        public int Id { get; set; }
//        public string Title { get; set; }
//        public string PartialViewPath { get; set; }
//        public int Width { get; set; }
//        public int Height { get; set; }
//    }
//}
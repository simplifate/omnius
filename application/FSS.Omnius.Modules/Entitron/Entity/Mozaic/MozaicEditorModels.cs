﻿using System.Collections.Generic;
using System.ComponentModel.DataAnnotations.Schema;
using System.Text;
using System.Linq;
using Newtonsoft.Json;

namespace FSS.Omnius.Modules.Entitron.Entity.Mozaic
{
    using Master;
    using Tapestry;

    [Table("MozaicEditor_Pages")]
    public class MozaicEditorPage : IEntity
    {
        public int Id { get; set; }
        public string Name { get; set; }
        public bool IsModal { get; set; }
        public int? ModalWidth { get; set; }
        public int? ModalHeight { get; set; }
        public string CompiledPartialView { get; set; }
        public int CompiledPageId { get; set; }
        public virtual ICollection<MozaicEditorComponent> Components { get; set; }

        [JsonIgnore]
        public virtual ICollection<TapestryDesignerResourceItem> ResourceItems { get; set; }

        [JsonIgnore]
        public virtual Application ParentApp { get; set; }

        public void Recompile()
        {
            StringBuilder stringBuilder = new StringBuilder();
            if (IsModal)
                stringBuilder.Append("@{ Layout = \"~/Views/Shared/_PartialViewAjaxLayout.cshtml\"; }");
            else
                stringBuilder.Append("@{ Layout = \"~/Views/Shared/_OmniusUserAppLayout.cshtml\"; }");
            stringBuilder.Append("<form class=\"mozaicForm\" method=\"post\">");

            RenderComponentList(Components.Where(c => c.ParentComponent == null).ToList(), stringBuilder, true);

            stringBuilder.Append("</form>");
            CompiledPartialView = stringBuilder.ToString();
        }
        private void RenderComponentList(ICollection<MozaicEditorComponent> ComponentList, StringBuilder stringBuilder, bool allowNesting)
        {
            foreach (MozaicEditorComponent c in ComponentList)
            {
                if (c.Type == "info-container")
                {
                    stringBuilder.Append($"<div id=\"uic_{c.Name}\" name=\"{c.Name}\" class=\"uic info-container {c.Classes}\" style=\"left: {c.PositionX}; top: {c.PositionY}; ");
                    stringBuilder.Append($"width: {c.Width}; height: {c.Height}; {c.Styles}\">");
                    stringBuilder.Append($"<div class=\"fa fa-info-circle info-container-icon\"></div>");
                    stringBuilder.Append($"<div class=\"info-container-header\">{c.Label}</div>");
                    stringBuilder.Append($"<div class=\"info-container-body\">{c.Content}</div></div>");
                }
                else if (c.Type == "button-simple")
                {
                    using (DBEntities context = new DBEntities())
                    {
                        var wfItem = context.TapestryDesignerWorkflowItems.Where(i => i.ComponentName == c.Name).OrderByDescending(i => i.Id).FirstOrDefault();
                        stringBuilder.Append($"<{c.Tag} id=\"uic_{c.Name}\" name=\"button\" value=\"{c.Name}\" {c.Attributes} class=\"uic {c.Classes}{(wfItem != null && wfItem.isAjaxAction != null && wfItem.isAjaxAction.Value ? " runAjax" : "")}\" buttonName=\"{c.Name}\" style=\"left: {c.PositionX}; top: {c.PositionY}; ");
                        stringBuilder.Append($"width: {c.Width}; height: {c.Height}; {c.Styles}\">{c.Label}</{c.Tag}>");
                    }
                }
                else if (c.Type == "button-dropdown")
                {
                    stringBuilder.Append($"<{c.Tag} id=\"uic_{c.Name}\" name=\"{c.Name}\" {c.Attributes} class=\"uic {c.Classes}\" buttonName=\"{c.Name}\" style=\"left: {c.PositionX}; top: {c.PositionY}; ");
                    stringBuilder.Append($"width: {c.Width}; height: {c.Height}; {c.Styles}\">{c.Label}<i class=\"fa fa-caret-down\"></i></{c.Tag}>");
                }
                else if (c.Type == "input-single-line")
                {
                    stringBuilder.Append($"<{c.Tag} id =\"uic_{c.Name}\" name=\"{c.Name}\" {c.Attributes} type=\"text\" placeholder=\"{c.Placeholder}\" ");
                    stringBuilder.Append($"value=\"@(ViewData.ContainsKey(\"inputData_{c.Name}\") ? @ViewData[\"inputData_{c.Name}\"] : \"\")\" class=\"uic {c.Classes}\" style=\"left: {c.PositionX}; top: {c.PositionY}; ");
                    stringBuilder.Append($"width: {c.Width}; height: {c.Height}; {c.Styles}\"");
                    if (!string.IsNullOrEmpty(c.Properties))
                    {
                        string[] tokenPairs = c.Properties.Split(';');
                        foreach (string tokens in tokenPairs)
                        {
                            string[] nameValuePair = c.Properties.Split('=');
                            if (nameValuePair.Length == 2)
                            {
                                if (nameValuePair[0].ToLower() == "autosum")
                                    stringBuilder.Append($" writeSumInto=\"{nameValuePair[1]}\"");
                                else if (nameValuePair[0].ToLower() == "role")
                                    stringBuilder.Append($" uicRole=\"{nameValuePair[1]}\"");
                            }
                        }
                    }
                    if (c.Classes.Contains("input-read-only"))
                        stringBuilder.Append($" readonly ");
                    stringBuilder.Append($"/>");
                }
                else if (c.Type == "input-multiline")
                {
                    stringBuilder.Append($"<{c.Tag} id=\"uic_{c.Name}\" name=\"{c.Name}\" {c.Attributes} placeholder=\"{c.Placeholder}\" class=\"uic {c.Classes}\" style=\"left: {c.PositionX}; top: {c.PositionY}; ");
                    stringBuilder.Append($"width: {c.Width}; height: {c.Height}; {c.Styles}\"");
                    if (!string.IsNullOrEmpty(c.Properties))
                    {
                        string[] tokenPairs = c.Properties.Split(';');
                        foreach (string tokens in tokenPairs)
                        {
                            string[] nameValuePair = c.Properties.Split('=');
                            if (nameValuePair.Length == 2)
                            {
                                if (nameValuePair[0].ToLower() == "autosum")
                                    stringBuilder.Append($" writeSumInto=\"{nameValuePair[1]}\"");
                                else if (nameValuePair[0].ToLower() == "role")
                                    stringBuilder.Append($" uicRole=\"{nameValuePair[1]}\"");
                            }
                        }
                    }
                    if (c.Classes.Contains("input-read-only"))
                        stringBuilder.Append($" readonly ");
                    stringBuilder.Append($">@ViewData[\"inputData_{c.Name}\"]</{c.Tag}>");
                }
                else if (c.Type == "label")
                {
                    stringBuilder.Append($"<{c.Tag} id=\"uic_{c.Name}\" name=\"{c.Name}\" {c.Attributes} class=\"uic {c.Classes}\" contentTemplate=\"{c.Content}\" style=\"left: {c.PositionX}; top: {c.PositionY}; ");
                    stringBuilder.Append($"width: {c.Width}; height: {c.Height}; {c.Styles}\">{c.Label}</{c.Tag}>");
                }
                else if (c.Type == "breadcrumb")
                {
                    stringBuilder.Append($"<div id=\"uic_{c.Name}\" name=\"{c.Name}\" class=\"uic breadcrumb-navigation {c.Classes}\" style=\"left: {c.PositionX}; top: {c.PositionY}; ");
                    stringBuilder.Append($"width: {c.Width}; height: {c.Height}; {c.Styles}\">");
                    stringBuilder.Append($"<div class=\"app-icon fa @ViewData[\"appIcon\"]\"></div>");
                    stringBuilder.Append($"<div class=\"nav-text\">@ViewData[\"appName\"] &gt; @ViewData[\"pageName\"]</div></div>");
                }
                else if (c.Type == "checkbox")
                {
                    stringBuilder.Append($"<div id=\"uic_{c.Name}\" class=\"uic {c.Classes}\" style=\"left: {c.PositionX}; top: {c.PositionY}; ");
                    stringBuilder.Append($"width: {c.Width}; height: {c.Height}; {c.Styles}\">");
                    stringBuilder.Append($"<input type=\"checkbox\" name=\"{c.Name}\"@(ViewData.ContainsKey(\"checkboxData_{c.Name}\") && (bool)ViewData[\"checkboxData_{c.Name}\"] ? \" checked\" : \"\") /><span class=\"checkbox-label\">{c.Label}</span></div>");
                }
                else if (c.Type == "radio")
                {
                    stringBuilder.Append($"<div id=\"uic_{c.Name}\" class=\"uic {c.Classes}\" style=\"left: {c.PositionX}; top: {c.PositionY}; ");
                    stringBuilder.Append($"width: {c.Width}; height: {c.Height}; {c.Styles}\">");
                    stringBuilder.Append($"<input type=\"radio\" name=\"{c.Name}\" /><span class=\"radio-label\">{c.Label}</span></div>");
                }
                else if (c.Type == "dropdown-select")
                {
                    stringBuilder.Append($"<{c.Tag} id=\"uic_{c.Name}\" name=\"{c.Name}\" {c.Attributes} class=\"uic {c.Classes}\" style=\"left: {c.PositionX}; top: {c.PositionY}; ");
                    stringBuilder.Append($"width: {c.Width}; height: {c.Height}; {c.Styles}\">");
                    stringBuilder.Append($"@{{ if(ViewData[\"dropdownData_{c.Name}\"] != null) ");
                    stringBuilder.Append($"{{ foreach(var option in (Dictionary<int, string>)ViewData[\"dropdownData_{c.Name}\"])");
                    stringBuilder.Append($"{{ <option value=\"@(option.Key)\" @(ViewData.ContainsKey(\"dropdownSelection_{c.Name}\") && (int)ViewData[\"dropdownSelection_{c.Name}\"] == option.Key ? \"selected\" : \"\") >");
                    stringBuilder.Append($"@(option.Value)</option>}}; }} }}</{c.Tag}>");
                }
                else if (c.Type == "data-table-read-only")
                {
                    stringBuilder.Append($"<{c.Tag} id=\"uic_{c.Name}\" name=\"{c.Name}\" {c.Attributes} class=\"uic {c.Classes}\" style=\"left: {c.PositionX}; top: {c.PositionY}; ");
                    stringBuilder.Append($"width: {c.Width}; height: {c.Height}; {c.Styles}\">");
                    stringBuilder.Append($"@{{ if(ViewData.ContainsKey(\"tableData_{c.Name}\")) {{");
                    stringBuilder.Append($"<thead><tr>@foreach (System.Data.DataColumn col in ((System.Data.DataTable)(ViewData[\"tableData_{c.Name}\"])).Columns)");
                    stringBuilder.Append($"{{<th>@col.Caption</th>}}</tr></thead><tfoot><tr>@foreach (System.Data.DataColumn col in ((System.Data.DataTable)(ViewData[\"tableData_{c.Name}\"])).Columns)");
                    stringBuilder.Append($"{{<th>@col.Caption</th>}}</tr></tfoot><tbody>@foreach(System.Data.DataRow row in ((System.Data.DataTable)(ViewData[\"tableData_{c.Name}\"])).Rows)");
                    stringBuilder.Append($"{{<tr>@foreach (var cell in row.ItemArray){{<td>@cell.ToString()</td>}}</tr>}}</tbody>}} }}</{c.Tag}>");
                    stringBuilder.Append($"<input type=\"hidden\" name=\"{c.Name}\" />");
                }
                else if (c.Type == "name-value-list")
                {
                    stringBuilder.Append($"<{c.Tag} id=\"uic_{c.Name}\" name=\"{c.Name}\" {c.Attributes} class=\"uic {c.Classes}\" style=\"left: {c.PositionX}; top: {c.PositionY}; ");
                    stringBuilder.Append($"width: {c.Width}; height: {c.Height}; {c.Styles}\">");
                    stringBuilder.Append($"@{{ if(ViewData.ContainsKey(\"tableData_{c.Name}\")) {{ var tableData = (System.Data.DataTable)ViewData[\"tableData_{c.Name}\"];");
                    stringBuilder.Append($"foreach (System.Data.DataColumn col in tableData.Columns) {{<tr><td class=\"name-cell\">@col.Caption</td>");
                    stringBuilder.Append($"<td class=\"value-cell\">@(tableData.Rows.Count>0?tableData.Rows[0][col.ColumnName]:\"no data\")</td></tr>");
                    stringBuilder.Append($"}} }} }}</{c.Tag}>");
                }
                else if (c.Type == "data-table-with-actions")
                {
                    stringBuilder.Append($"<{c.Tag} id=\"uic_{c.Name}\" name=\"{c.Name}\" {c.Attributes} class=\"uic {c.Classes}\" style=\"left: {c.PositionX}; top: {c.PositionY}; ");
                    stringBuilder.Append($"width: {c.Width}; height: {c.Height}; {c.Styles}\">");
                    stringBuilder.Append($"@{{ if(ViewData.ContainsKey(\"tableData_{c.Name}\")) {{");
                    stringBuilder.Append($"<thead><tr>@foreach (System.Data.DataColumn col in ((System.Data.DataTable)(ViewData[\"tableData_{c.Name}\"])).Columns)");
                    stringBuilder.Append($"{{<th>@col.Caption</th>}}<th>Akce</th></tr></thead><tfoot><tr>@foreach (System.Data.DataColumn col in ((System.Data.DataTable)(ViewData[\"tableData_{c.Name}\"])).Columns)");
                    stringBuilder.Append($"{{<th>@col.Caption</th>}}<th>Akce</th></tr></tfoot><tbody>@foreach(System.Data.DataRow row in ((System.Data.DataTable)(ViewData[\"tableData_{c.Name}\"])).Rows)");
                    stringBuilder.Append($"{{<tr>@foreach (var cell in row.ItemArray){{<td>@cell.ToString()</td>}}<td class=\"actionIcons\"><i class=\"fa fa-edit rowEditAction\"></i>");
                    stringBuilder.Append($"<i class=\"fa fa-remove rowDeleteAction\"></i></td></tr>}}</tbody>}} }}</{c.Tag}>");
                    stringBuilder.Append($"<input type=\"hidden\" name=\"{c.Name}\" />");
                }
                else if (c.Type == "tab-navigation")
                {
                    stringBuilder.Append($"<{c.Tag} id=\"uic_{c.Name}\" name=\"{c.Name}\" {c.Attributes} class=\"uic {c.Classes}\" style=\"left: {c.PositionX}; top: {c.PositionY}; ");
                    stringBuilder.Append($"width: {c.Width}; height: {c.Height}; {c.Styles}\"><li class=\"active\"><a class=\"fa fa-home\"></a></li>");
                    var tabLabelArray = c.Content.Split(';').ToList();
                    foreach (string tabLabel in tabLabelArray)
                    {
                        if (tabLabel.Length > 0)
                            stringBuilder.Append($"<li><a>{tabLabel}</a></li>");
                    }
                    stringBuilder.Append($"</{c.Tag}>");
                }
                else if (c.Type == "panel" && allowNesting)
                {
                    stringBuilder.Append($"<{c.Tag} id=\"uic_{c.Name}\" name=\"{c.Name}\" {c.Attributes} class=\"uic {c.Classes}\" style=\"left: {c.PositionX}; top: {c.PositionY}; ");
                    stringBuilder.Append($"width: {c.Width}; height: {c.Height}; {c.Styles}\"");
                    if (!string.IsNullOrEmpty(c.Properties))
                    {
                        string[] nameValuePair = c.Properties.Split('=');
                        if (nameValuePair.Length == 2)
                        {
                            if (nameValuePair[0].ToLower() == "hide")
                                stringBuilder.Append($" panelHiddenBy=\"{nameValuePair[1]}\"");
                            else if (nameValuePair[0].ToLower() == "clone")
                                stringBuilder.Append($" panelClonedBy=\"{nameValuePair[1]}\"");
                        }
                    }
                    stringBuilder.Append($">");
                    RenderComponentList(c.ChildComponents, stringBuilder, false);
                    stringBuilder.Append($"</{c.Tag}>");
                }
                else
                {
                    stringBuilder.Append($"<{c.Tag} id=\"uic_{c.Name}\" name=\"{c.Name}\" {c.Attributes} class=\"uic {c.Classes}\" style=\"left: {c.PositionX}; top: {c.PositionY}; ");
                    stringBuilder.Append($"width: {c.Width}; height: {c.Height}; {c.Styles}\">{c.Content}</{c.Tag}>");
                }
            }
        }

        public MozaicEditorPage()
        {
            Components = new List<MozaicEditorComponent>();
        }
    }
    [Table("MozaicEditor_Components")]
    public class MozaicEditorComponent : IEntity
    {
        public int Id { get; set; }
        public string Name { get; set; }
        public string Type { get; set; }
        public string PositionX { get; set; }
        public string PositionY { get; set; }
        public string Width { get; set; }
        public string Height { get; set; }

        public string Tag { get; set; }
        public string Attributes { get; set; }
        public string Classes { get; set; }
        public string Styles { get; set; }
        public string Content { get; set; }

        public string Label { get; set; }
        public string Placeholder { get; set; }
        public string Properties { get; set; }

        [JsonIgnore]
        public virtual ICollection<MozaicEditorComponent> ChildComponents { get; set; }
        public int? ParentComponentId { get; set; }
        [JsonIgnore]
        public virtual MozaicEditorComponent ParentComponent { get; set; }
        [JsonIgnore]
        public int MozaicEditorPageId { get; set; }
        [JsonIgnore]
        public virtual MozaicEditorPage MozaicEditorPage { get; set; }
    }
    
    public class MozaicModalMetadataItem : IEntity
    {
        public int Id { get; set; }
        public string Title { get; set; }
        public string PartialViewPath { get; set; }
        public int Width { get; set; }
        public int Height { get; set; }
    }
}

using System.Collections.Generic;
using System.ComponentModel.DataAnnotations.Schema;
using System.Text;

using FSS.Omnius.Modules.Entitron.Entity.Master;
using System.Linq;

namespace FSS.Omnius.Modules.Entitron.Entity.Mozaic
{
    [Table("MozaicEditor_Pages")]
    public class MozaicEditorPage
    {
        public int Id { get; set; }
        public string Name { get; set; }
        public bool IsModal { get; set; }
        public int? ModalWidth { get; set; }
        public int? ModalHeight { get; set; }
        public string CompiledPartialView { get; set; }
        public int CompiledPageId { get; set; }
        public virtual ICollection<MozaicEditorComponent> Components { get; set; }

        public virtual Application ParentApp { get; set; }

        public void Recompile()
        {
            StringBuilder stringBuilder = new StringBuilder();
            if (IsModal)
                stringBuilder.Append("@{ Layout = \"~/Views/Shared/_PartialViewAjaxLayout.cshtml\"; }");
            else
                stringBuilder.Append("@{ Layout = \"~/Views/Shared/_OmniusUserAppLayout.cshtml\"; }");
            stringBuilder.Append("<form class=\"mozaicForm\" method=\"post\">");

            RenderComponentList(this.Components, stringBuilder, true);

            stringBuilder.Append("</form>");
            CompiledPartialView = stringBuilder.ToString();
        }
        private void RenderComponentList(ICollection<MozaicEditorComponent> ComponentList, StringBuilder stringBuilder, bool allowNesting)
        {
            foreach (MozaicEditorComponent c in ComponentList)
            {
                if (c.Type == "info-container")
                {
                    stringBuilder.Append($"<div id=\"uic{c.Id}\" name=\"{c.Name}\" class=\"uic info-container {c.Classes}\" style=\"left: {c.PositionX}; top: {c.PositionY}; ");
                    stringBuilder.Append($"width: {c.Width}; height: {c.Height}; {c.Styles}\">");
                    stringBuilder.Append($"<div class=\"fa fa-info-circle info-container-icon\"></div>");
                    stringBuilder.Append($"<div class=\"info-container-header\">{c.Label}</div>");
                    stringBuilder.Append($"<div class=\"info-container-body\">{c.Content}</div></div>");
                }
                else if (c.Type == "button-simple")
                {
                    stringBuilder.Append($"<{c.Tag} id=\"uic{c.Id}\" name=\"button\" value=\"{c.Name}\" {c.Attributes} class=\"uic {c.Classes}\" style=\"left: {c.PositionX}; top: {c.PositionY}; ");
                    stringBuilder.Append($"width: {c.Width}; height: {c.Height}; {c.Styles}\">{c.Label}</{c.Tag}>");
                }
                else if (c.Type == "button-dropdown")
                {
                    stringBuilder.Append($"<{c.Tag} id=\"uic{c.Id}\" name=\"{c.Name}\" {c.Attributes} class=\"uic {c.Classes}\" style=\"left: {c.PositionX}; top: {c.PositionY}; ");
                    stringBuilder.Append($"width: {c.Width}; height: {c.Height}; {c.Styles}\">{c.Label}<i class=\"fa fa-caret-down\"></i></{c.Tag}>");
                }
                else if (c.Tag == "input" || c.Tag == "textarea")
                {
                    stringBuilder.Append($"<{c.Tag} id=\"uic{c.Id}\" name=\"{c.Name}\" value=\"@(ViewData.ContainsKey(\"uic{c.Id}\") ? ViewData[\"uic{c.Id}\"].ToString() : \"\")\" {c.Attributes} placeholder=\"{c.Placeholder}\" class=\"uic {c.Classes}\" style=\"left: {c.PositionX}; top: {c.PositionY}; ");
                    stringBuilder.Append($"width: {c.Width}; height: {c.Height}; {c.Styles}\">{c.Label}</{c.Tag}>");
                }
                else if (c.Type == "label")
                {
                    stringBuilder.Append($"<{c.Tag} id=\"uic{c.Id}\" name=\"{c.Name}\" {c.Attributes} class=\"uic {c.Classes}\" style=\"left: {c.PositionX}; top: {c.PositionY}; ");
                    stringBuilder.Append($"width: {c.Width}; height: {c.Height}; {c.Styles}\">{c.Label}</{c.Tag}>");
                }
                else if (c.Type == "breadcrumb")
                {
                    stringBuilder.Append($"<div id=\"uic{c.Id}\" name=\"{c.Name}\" class=\"uic breadcrumb-navigation {c.Classes}\" style=\"left: {c.PositionX}; top: {c.PositionY}; ");
                    stringBuilder.Append($"width: {c.Width}; height: {c.Height}; {c.Styles}\">");
                    stringBuilder.Append($"<div class=\"app-icon fa @ViewData[\"appIcon\"]\"></div>");
                    stringBuilder.Append($"<div class=\"nav-text\">@ViewData[\"appName\"] &gt; @ViewData[\"pageName\"]</div></div>");
                }
                else if (c.Type == "checkbox")
                {
                    stringBuilder.Append($"<div id=\"uic{c.Id}\" class=\"uic {c.Classes}\" style=\"left: {c.PositionX}; top: {c.PositionY}; ");
                    stringBuilder.Append($"width: {c.Width}; height: {c.Height}; {c.Styles}\">");
                    stringBuilder.Append($"<input type=\"checkbox\" name=\"{c.Name}\"@(ViewData.ContainsKey(\"uic{c.Id}\") && (bool)ViewData[\"uic{c.Id}\"] ? \" checked\" : \"\") /><span class=\"checkbox-label\">{c.Label}</span></div>");
                }
                else if (c.Type == "radio")
                {
                    stringBuilder.Append($"<div id=\"uic{c.Id}\" class=\"uic {c.Classes}\" style=\"left: {c.PositionX}; top: {c.PositionY}; ");
                    stringBuilder.Append($"width: {c.Width}; height: {c.Height}; {c.Styles}\">");
                    stringBuilder.Append($"<input type=\"radio\" name=\"{c.Name}\" /><span class=\"radio-label\">{c.Label}</span></div>");
                }
                else if (c.Type == "dropdown-select")
                {
                    stringBuilder.Append($"<{c.Tag} id=\"uic{c.Id}\" name=\"{c.Name}\" {c.Attributes} class=\"uic {c.Classes}\" style=\"left: {c.PositionX}; top: {c.PositionY}; ");
                    stringBuilder.Append($"width: {c.Width}; height: {c.Height}; {c.Styles}\">");
                    stringBuilder.Append($"@{{ if(ViewData[\"dropdownData_{c.Name}\"] != null) ");
                    stringBuilder.Append($"{{ foreach(var option in (Dictionary<int, string>)ViewData[\"dropdownData_{c.Name}\"])");
                    stringBuilder.Append($"{{ <option value=\"@(option.Key)\">@(option.Value)</option>}}; }} }}</{c.Tag}>");
                }
                else if (c.Type == "data-table-read-only")
                {
                    stringBuilder.Append($"<{c.Tag} id=\"uic{c.Id}\" name=\"{c.Name}\" {c.Attributes} class=\"uic {c.Classes}\" style=\"left: {c.PositionX}; top: {c.PositionY}; ");
                    stringBuilder.Append($"width: {c.Width}; height: {c.Height}; {c.Styles}\">");
                    stringBuilder.Append($"@{{ if(ViewData.ContainsKey(\"tableData_{c.Name}\")) {{");
                    stringBuilder.Append($"<thead><tr>@foreach (System.Data.DataColumn col in ((System.Data.DataTable)(ViewData[\"tableData_{c.Name}\"])).Columns)");
                    stringBuilder.Append($"{{<th>@col.Caption</th>}}</tr></thead><tbody>@foreach(System.Data.DataRow row in ((System.Data.DataTable)(ViewData[\"tableData_{c.Name}\"])).Rows)");
                    stringBuilder.Append($"{{<tr>@foreach (var cell in row.ItemArray){{<td>@cell.ToString()</td>}}</tr>}}</tbody>}} }}</{c.Tag}>");
                }
                else if (c.Type == "name-value-list")
                {
                    stringBuilder.Append($"<{c.Tag} id=\"uic{c.Id}\" name=\"{c.Name}\" {c.Attributes} class=\"uic {c.Classes}\" style=\"left: {c.PositionX}; top: {c.PositionY}; ");
                    stringBuilder.Append($"width: {c.Width}; height: {c.Height}; {c.Styles}\">");
                    stringBuilder.Append($"@{{ if(ViewData.ContainsKey(\"tableData_{c.Name}\")) {{ var tableData = (System.Data.DataTable)ViewData[\"tableData_{c.Name}\"];");
                    stringBuilder.Append($"foreach (System.Data.DataColumn col in tableData.Columns) {{<tr><td class=\"name-cell\">@col.Caption</td>");
                    stringBuilder.Append($"<td class=\"value-cell\">@(tableData.Rows.Count>0?tableData.Rows[0][col.ColumnName]:\"no data\")</td></tr>");
                    stringBuilder.Append($"}} }} }}</{c.Tag}>");
                }
                else if (c.Type == "data-table-with-actions")
                {
                    stringBuilder.Append($"<{c.Tag} id=\"uic{c.Id}\" name=\"{c.Name}\" {c.Attributes} class=\"uic {c.Classes}\" style=\"left: {c.PositionX}; top: {c.PositionY}; ");
                    stringBuilder.Append($"width: {c.Width}; height: {c.Height}; {c.Styles}\">");
                    stringBuilder.Append($"@{{ if(ViewData.ContainsKey(\"tableData_{c.Name}\")) {{");
                    stringBuilder.Append($"<thead><tr>@foreach (System.Data.DataColumn col in ((System.Data.DataTable)(ViewData[\"tableData_{c.Name}\"])).Columns)");
                    stringBuilder.Append($"{{<th>@col.Caption</th>}}<th>Akce</th></tr></thead><tbody>@foreach(System.Data.DataRow row in ((System.Data.DataTable)(ViewData[\"tableData_{c.Name}\"])).Rows)");
                    stringBuilder.Append($"{{<tr>@foreach (var cell in row.ItemArray){{<td>@cell.ToString()</td>}}<td class=\"actionIcons\"><a href=\"/EPK/@ViewContext.RouteData.Values[\"controller\"].ToString()/Update/@row.ItemArray.First().ToString()\" ><i class=\"fa fa-edit rowEditAction\"></i></a>");
                    stringBuilder.Append($"<a href=\"/EPK/@ViewContext.RouteData.Values[\"controller\"].ToString()/Delete/@row.ItemArray.First().ToString()\"><i class=\"fa fa-remove rowDeleteAction\"></i></a></td></tr>}}</tbody>}} }}</{c.Tag}>");
                }
                else if (c.Type == "tab-navigation")
                {
                    stringBuilder.Append($"<{c.Tag} id=\"uic{c.Id}\" name=\"{c.Name}\" {c.Attributes} class=\"uic {c.Classes}\" style=\"left: {c.PositionX}; top: {c.PositionY}; ");
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
                    stringBuilder.Append($"<{c.Tag} id=\"uic{c.Id}\" name=\"{c.Name}\" {c.Attributes} class=\"uic {c.Classes}\" style=\"left: {c.PositionX}; top: {c.PositionY}; ");
                    stringBuilder.Append($"width: {c.Width}; height: {c.Height}; {c.Styles}\">");
                    RenderComponentList(c.ChildComponents, stringBuilder, false);
                    stringBuilder.Append($"</{c.Tag}>");
                }
                else
                {
                    stringBuilder.Append($"<{c.Tag} id=\"uic{c.Id}\" name=\"{c.Name}\" {c.Attributes} class=\"uic {c.Classes}\" style=\"left: {c.PositionX}; top: {c.PositionY}; ");
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
    public class MozaicEditorComponent
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

        public virtual ICollection<MozaicEditorComponent> ChildComponents { get; set; }
    }

    public class MozaicModalMetadataItem
    {
        public int Id { get; set; }
        public string Title { get; set; }
        public string PartialViewPath { get; set; }
        public int Width { get; set; }
        public int Height { get; set; }
    }
}

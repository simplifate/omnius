using System.Collections.Generic;
using System.ComponentModel.DataAnnotations.Schema;
using System.Text;
using System.Linq;
using Newtonsoft.Json;

namespace FSS.Omnius.Modules.Entitron.Entity.Mozaic
{
    using Master;
    using System;
    using Tapestry;

    [Table("MozaicEditor_Pages")]
    public class MozaicEditorPage : IEntity
    {
        [ImportExportIgnore(IsKey = true)]
        public int Id { get; set; }
        public string Name { get; set; }
        public bool IsModal { get; set; }
        public int? ModalWidth { get; set; }
        public int? ModalHeight { get; set; }
        public string CompiledPartialView { get; set; }
        public int CompiledPageId { get; set; }
        public VersionEnum Version { get; set; }
        public bool IsDeleted { get; set; }
        public virtual ICollection<MozaicEditorComponent> Components { get; set; }

        [ImportExportIgnore]
        public virtual ICollection<TapestryDesignerResourceItem> ResourceItems { get; set; }

        [ImportExportIgnore(IsParent = true)]
        public virtual Application ParentApp { get; set; }

        public void Recompile()
        {
            StringBuilder stringBuilder = new StringBuilder();
            stringBuilder.Append("@using FSS.Omnius.FrontEnd.Utils");

            if (IsModal)
            {
                stringBuilder.Append("@{T t = new T( (string)ViewData[\"locale\"] );}");
                stringBuilder.Append("@{ Layout = \"~/Views/Shared/_PartialViewAjaxLayout.cshtml\"; }");
            }
            else {
                stringBuilder.Append("@{T t = new T( (string)ViewData[\"locale\"] );}");
                stringBuilder.Append("@{ Layout = \"~/Views/Shared/_OmniusUserAppLayout.cshtml\"; }");
				stringBuilder.Append("@{Dictionary<string, string> formState = (Dictionary<string, string>)ViewData[\"formState\"]; }");
            }

            //Michal Šebela - 11.10. Přidáno enctype
            stringBuilder.Append("<form class=\"mozaicForm\" method=\"post\" enctype = \"multipart/form-data\">");

            RenderComponentList(Components.Where(c => c.ParentComponent == null).ToList(), stringBuilder, true);

            stringBuilder.Append("<input type=\"hidden\" name=\"registry\" value=\"@ViewData[\"crossBlockRegistry\"]\" />");

            // Secure forms with anti cross-site forgery token
            stringBuilder.Append("@Html.AntiForgeryToken()");

            stringBuilder.Append("</form>");
            CompiledPartialView = stringBuilder.ToString();
        }
        private void RenderComponentList(ICollection<MozaicEditorComponent> ComponentList, StringBuilder stringBuilder, bool allowNesting)
        {
            foreach (MozaicEditorComponent c in ComponentList)
            {
                Dictionary<string, string> properties = MozaicPropertiesParser.ParseMozaicPropertiesString(c.Properties);

                if (c.Type == "info-container")
                {
                    stringBuilder.Append($"<div id=\"uic_{c.Name}\" name=\"{c.Name}\" class=\"uic info-container {c.Classes}\" style=\"left: {c.PositionX}; top: {c.PositionY}; ");
                    stringBuilder.Append($"width: {c.Width}; height: {c.Height}; {c.Styles}\">");
                    stringBuilder.Append($"<div class=\"fa fa-info-circle info-container-icon\"></div>");
                    stringBuilder.Append($"<div class=\"info-container-header\">@(t._(\"{c.Label}\"))</div>");
                    stringBuilder.Append($"<div class=\"info-container-body\">@(t._(\"{c.Content}\"))</div></div>");
                }
                else if (c.Type == "button-simple")
                {
                    bool invisible = false;
                    if (!string.IsNullOrEmpty(c.Properties))
                    {
                        string[] tokenPairs = c.Properties.Split(';');
                        foreach (string tokens in tokenPairs)
                        {
                            string[] nameValuePair = tokens.Split('=');
                            if (nameValuePair.Length == 2)
                            {
                                // No settings yet
                            }
                            else
                            {
                                if (tokens == "hidden")
                                    invisible = true;
                            }
                        }
                    }
                    using (DBEntities context = DBEntities.instance)
                    {
                        var wfItem = context.TapestryDesignerWorkflowItems.Where(i => i.ComponentName == c.Name).OrderByDescending(i => i.Id).FirstOrDefault();
                        stringBuilder.Append($"<{c.Tag} id=\"uic_{c.Name}\" name=\"button\" value=\"{c.Name}\" {c.Attributes} class=\"uic {c.Classes}{(wfItem != null && wfItem.isAjaxAction != null && wfItem.isAjaxAction.Value ? " runAjax" : "")}\" buttonName=\"{c.Name}\" tabindex=\"{c.TabIndex}\" style=\"left: {c.PositionX}; top: {c.PositionY}; ");
                        stringBuilder.Append($"width: {c.Width}; height: {c.Height}; {(invisible ? "display:none;" : "")} {c.Styles}\">@(t._(\"{c.Label}\"))</{c.Tag}>");
                    }
                }
                else if (c.Type == "button-dropdown")
                {
                    stringBuilder.Append($"<{c.Tag} id=\"uic_{c.Name}\" name=\"{c.Name}\" tabindex=\"{c.TabIndex}\" {c.Attributes} class=\"uic {c.Classes}\" buttonName=\"{c.Name}\" style=\"left: {c.PositionX}; top: {c.PositionY}; ");
                    stringBuilder.Append($"width: {c.Width}; height: {c.Height}; {c.Styles}\">@(t._(\"{c.Label}\"))<i class=\"fa fa-caret-down\"></i></{c.Tag}>");
                }
                else if (c.Type == "button-browse")
                {
                    stringBuilder.Append($"<{c.Tag} id=\"uic_{c.Name}\" name=\"{c.Name}\" type=\"file\" tabindex=\"{c.TabIndex}\" {c.Attributes} class=\"uic {c.Classes}\" buttonName=\"{c.Name}\" style=\"left: {c.PositionX}; top: {c.PositionY}; ");
                    stringBuilder.Append($"width: {c.Width}; height: {c.Height}; {c.Styles}\">@(t._(\"{c.Label}\"))</{c.Tag}>");
                }
                else if (c.Type == "input-single-line")
                {
                    stringBuilder.Append($"<{c.Tag} id =\"uic_{c.Name}\" name=\"{c.Name}\" {c.Attributes} type=\"text\" placeholder=\"@(t._(\"{c.Label}\"))\" tabindex=\"{c.TabIndex}\" ");
                    stringBuilder.Append($"value=\"@(formState.ContainsKey(\"{c.Name}\") ? formState[\"{c.Name}\"] : (ViewData.ContainsKey(\"inputData_{c.Name}\") ? @ViewData[\"inputData_{c.Name}\"] : \"\"))\" class=\"uic {c.Classes}\" style=\"left: {c.PositionX}; top: {c.PositionY}; ");
                    stringBuilder.Append($"width: {c.Width}; height: {c.Height}; {c.Styles}\"");
                    if (!string.IsNullOrEmpty(c.Properties))
                    {
                        string[] tokenPairs = c.Properties.Split(';');
                        foreach (string tokens in tokenPairs)
                        {
                            string[] nameValuePair = tokens.Split('=');
                            if (nameValuePair.Length == 2)
                            {
                                if (nameValuePair[0].ToLower() == "autosum")
                                    stringBuilder.Append($" writeSumInto=\"@(t._(\"{nameValuePair[1]}\"))\"");
                                else if (nameValuePair[0].ToLower() == "role")
                                    stringBuilder.Append($" uicRole=\"@(t._(\"{nameValuePair[1]}\"))\"");
                            }
                        }
                    }
                    if (c.Classes.Contains("input-read-only"))
                        stringBuilder.Append($" readonly ");
                    stringBuilder.Append($"/>");
                }
                else if (c.Type == "input-multiline")
                {
                    stringBuilder.Append($"<{c.Tag} id=\"uic_{c.Name}\" name=\"{c.Name}\" {c.Attributes} placeholder=\"@(t._(\"{c.Placeholder}\"))\" tabindex=\"{c.TabIndex}\" class=\"uic {c.Classes}\" style=\"left: {c.PositionX}; top: {c.PositionY}; ");
                    stringBuilder.Append($"width: {c.Width}; height: {c.Height}; {c.Styles}\"");
                    if (!string.IsNullOrEmpty(c.Properties))
                    {
                        string[] tokenPairs = c.Properties.Split(';');
                        foreach (string tokens in tokenPairs)
                        {
                            string[] nameValuePair = tokens.Split('=');
                            if (nameValuePair.Length == 2)
                            {
                                if (nameValuePair[0].ToLower() == "autosum")
                                    stringBuilder.Append($" writeSumInto=\"@(t._(\"{nameValuePair[1]}\"))\"");
                                else if (nameValuePair[0].ToLower() == "role")
                                    stringBuilder.Append($" uicRole=\"@(t._(\"{nameValuePair[1]}\"))\"");
                            }

                        }
                    }
                    if (c.Classes.Contains("input-read-only"))
                        stringBuilder.Append($" readonly ");
                    stringBuilder.Append($">@(formState.ContainsKey(\"{c.Name}\") ? formState[\"{c.Name}\"] : ViewData[\"inputData_{c.Name}\"])</{c.Tag}>");
                }
                else if (c.Type == "label")
                {
                    string labelText = $"@Html.Raw(ViewData.ContainsKey(\"inputData_{c.Name}\") ? \"{c.Label}\".Replace(\"{{var1}}\", (ViewData[\"inputData_{c.Name}\"] ?? \"\").ToString()) : t._(\"{c.Label}\") )";
                    stringBuilder.Append($"<{c.Tag} id=\"uic_{c.Name}\" name=\"{c.Name}\" {c.Attributes} class=\"uic {c.Classes}\" contentTemplate=\"{c.Content}\" style=\"left: {c.PositionX}; top: {c.PositionY}; ");
                    stringBuilder.Append($"width: {c.Width}; height: {c.Height}; {c.Styles}\">{labelText}</{c.Tag}>");
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
                    stringBuilder.Append($"<div id=\"uic_{c.Name}\" class=\"uic {c.Classes}\" tabindex=\"{c.TabIndex}\" style=\"left: {c.PositionX}; top: {c.PositionY}; ");
                    stringBuilder.Append($"width: {c.Width}; height: {c.Height}; {c.Styles}\">");
                    stringBuilder.Append($"<input type=\"checkbox\" name=\"{c.Name}\"@((formState.ContainsKey(\"{c.Name}\") && formState[\"{c.Name}\"] == \"on\") || (ViewData.ContainsKey(\"checkboxData_{c.Name}\") && (bool)ViewData[\"checkboxData_{c.Name}\"]) ? \" checked\" : \"\") /><span class=\"checkbox-label\">@(t._(\"{c.Label}\"))</span></div>");
                }
                else if (c.Type == "radio")
                {
                    string value = "";
                    if (!string.IsNullOrEmpty(c.Properties))
                    {
                        string[] tokenPairs = c.Properties.Split(';');
                        foreach (string tokens in tokenPairs)
                        {
                            string[] nameValuePair = tokens.Split('=');
                            if (nameValuePair.Length == 2)
                            {
                                if (nameValuePair[0].ToLower() == "value")
                                    value = nameValuePair[1];
                            }
                        }
                    }
                    stringBuilder.Append($"<div id=\"uic_{c.Name}\" class=\"uic {c.Classes}\" tabindex=\"{c.TabIndex}\" style=\"left: {c.PositionX}; top: {c.PositionY}; ");
                    stringBuilder.Append($"width: {c.Width}; height: {c.Height}; {c.Styles}\">");
                    stringBuilder.Append($"<input type=\"radio\" name=\"{c.Name}\" value=\"{value}\"@(formState.ContainsKey(\"{c.Name}\") && formState[\"{c.Name}\"] == \"{value}\" ? \" checked\" : \"\") /><span class=\"radio-label\">@(t._(\"{c.Label}\"))</span></div>");
                }
                else if (c.Type == "dropdown-select")
                {
                    stringBuilder.Append($"<{c.Tag} id=\"uic_{c.Name}\" name=\"{c.Name}\" tabindex=\"{c.TabIndex}\" {c.Attributes} class=\"uic {c.Classes}\" style=\"left: {c.PositionX}; top: {c.PositionY}; ");
                    stringBuilder.Append($"width: {c.Width}; height: {c.Height}; {c.Styles}\">");
                    string sortMode = "key";
                    if (!string.IsNullOrEmpty(c.Properties))
                    {
                        string[] tokenPairs = c.Properties.Split(';');
                        foreach (string tokens in tokenPairs)
                        {
                            string[] nameValuePair = tokens.Split('=');
                            if (nameValuePair.Length == 2)
                            {
                                if (nameValuePair[0].ToLower() == "defaultoption")
                                    stringBuilder.Append($"<option value=\"-1\">@(t._(\"{nameValuePair[1]}\"))</option>");
                                if (nameValuePair[0].ToLower() == "sortby" && nameValuePair[1].ToLower() == "value")
                                    sortMode = "value";
                            }
                        }
                    }
                    if (sortMode == "value")
                    {
                        stringBuilder.Append($"@{{ if(ViewData[\"dropdownData_{c.Name}\"] != null) ");
                        stringBuilder.Append($"{{ foreach(var option in ((Dictionary<int, string>)ViewData[\"dropdownData_{c.Name}\"]).OrderBy(p => p.Value))");
                        stringBuilder.Append($"{{ <option value=\"@(option.Key)\" @((formState.ContainsKey(\"{c.Name}\") && Convert.ToInt32(formState[\"{c.Name}\"]) == option.Key) || (ViewData.ContainsKey(\"dropdownSelection_{c.Name}\") && ViewData[\"dropdownSelection_{c.Name}\"] is int && (int)ViewData[\"dropdownSelection_{c.Name}\"] == option.Key) ? \"selected\" : \"\") >");
                        stringBuilder.Append($"@(t._(option.Value))</option>}}; }} }}");

                    }
                    else
                    {
                        stringBuilder.Append($"@{{ if(ViewData[\"dropdownData_{c.Name}\"] != null) ");
                        stringBuilder.Append($"{{ foreach(var option in (Dictionary<int, string>)ViewData[\"dropdownData_{c.Name}\"])");
                        stringBuilder.Append($"{{ <option value=\"@(option.Key)\" @((formState.ContainsKey(\"{c.Name}\") && Convert.ToInt32(formState[\"{c.Name}\"]) == option.Key) || (ViewData.ContainsKey(\"dropdownSelection_{c.Name}\") && ViewData[\"dropdownSelection_{c.Name}\"] is int && (int)ViewData[\"dropdownSelection_{c.Name}\"] == option.Key) ? \"selected\" : \"\") >");
                        stringBuilder.Append($"@(t._(option.Value))</option>}}; }} }}");

                    }
                    stringBuilder.Append($"</{c.Tag}>");
                }
                else if (c.Type == "multiple-select")
                {
                    stringBuilder.Append($"<{c.Tag} id=\"uic_{c.Name}\" name=\"{c.Name}\" multiple {c.Attributes} class=\"uic {c.Classes}\" tabindex=\"{c.TabIndex}\" style=\"left: {c.PositionX}; top: {c.PositionY}; ");
                    stringBuilder.Append($"width: {c.Width}; height: {c.Height}; {c.Styles}\">");
                    stringBuilder.Append($"@{{ if(ViewData[\"dropdownData_{c.Name}\"] != null) ");
                    stringBuilder.Append($"{{ foreach(var option in (Dictionary<int, string>)ViewData[\"dropdownData_{c.Name}\"])");
                    stringBuilder.Append($"{{ <option value=\"@(option.Key)\" @(ViewData.ContainsKey(\"dropdownSelection_{c.Name}\") && ViewData[\"dropdownSelection_{c.Name}\"] is int && (int)ViewData[\"dropdownSelection_{c.Name}\"] == option.Key ? \"selected\" : \"\") >");
                    stringBuilder.Append($"@(t._(option.Value))</option>}}; }} }}</{c.Tag}>");
                }
                else if (c.Type == "data-table-read-only")
                {
                    bool allowHtml = properties.ContainsKey("allowHtml") && Convert.ToBoolean(properties["allowHtml"]) == true;
                    bool itemSelection = properties.ContainsKey("itemSelection") && Convert.ToBoolean(properties["itemSelection"]) == true;
                    bool rowSelect = properties.ContainsKey("selectByRow") && Convert.ToBoolean(properties["selectByRow"]) == true;
                    bool columnFilter = properties.ContainsKey("columnFilter") && Convert.ToBoolean(properties["columnFilter"]) == true;

                    string itemPopulation = null;
                    if (properties.ContainsKey("itemPopulation"))
                    {
                        itemPopulation = properties["itemPopulation"];
                    }

                    string columnSearchClass = "";
                    if (properties.ContainsKey("searchInIndividualColumns") && Convert.ToBoolean(properties["searchInIndividualColumns"]) == true)
                    {
                        columnSearchClass = " data-table-individual-columns-search";
                    }

                    // On empty condition
                    stringBuilder.Append($"@{{ if(ViewData.ContainsKey(\"tableData_{c.Name}\") && ((System.Data.DataTable)(ViewData[\"tableData_{c.Name}\"])).Rows.Count > 0) {{");

                        // Opening tag
                        stringBuilder.Append($"<{c.Tag} id=\"uic_{c.Name}\" name=\"{c.Name}\" " + (itemPopulation != null ? "data-item-population-target=\"" + itemPopulation + "\"" : "") + (columnFilter ? "data-column-filter" : "true") + (rowSelect ? "data-select-mode=\"row\"" : "")+$" { c.Attributes} class=\"uic {c.Classes}{columnSearchClass}" + (itemSelection ? " hideSecond" : " hideFirst") + $"\" style=\"left: {c.PositionX}; top: {c.PositionY}; ");
                        stringBuilder.Append($"width: {c.Width}; height: {c.Height}; {c.Styles}\" uicWidth=\"{c.Width}\">");

                            // Open heading
                            stringBuilder.Append($"<thead><tr>");

                                // Item selection (all)
                                if(itemSelection)
                                {
                                    stringBuilder.Append($"<th><input title=\"@t._(\"Vybrat vše\")\" type=\"checkbox\" data-item-selection=\"*\"></th>");
                                }

                                // Item population
                                if (itemPopulation != null)
                                {
                                    stringBuilder.Append($"<th></th>");
                                }

                                // Headers loop #start
                                stringBuilder.Append($"@foreach (System.Data.DataColumn col in ((System.Data.DataTable)(ViewData[\"tableData_{c.Name}\"])).Columns) {{");

                                    // Headers iteration
                                    stringBuilder.Append($"<th data-column-name='@(col.ColumnName)'>@(t._(col.Caption))</th>");

                                // Headers loop #end
                                stringBuilder.Append($"}}");

                            // Close heading
                            stringBuilder.Append("</tr></thead>");

                            // Open footer
                            stringBuilder.Append($"<tfoot><tr>");

                                if (itemSelection)
                                {
                                    stringBuilder.Append($"<th></th>");
                                }
                                // Item population
                                if (itemPopulation != null)
                                {
                                    stringBuilder.Append($"<th></th>");
                                }

                                // Footers loop #start
                                stringBuilder.Append($"@foreach (System.Data.DataColumn col in ((System.Data.DataTable)(ViewData[\"tableData_{c.Name}\"])).Columns) {{");

                                    // Footers iteration
                                    stringBuilder.Append($"<th>@(t._(col.Caption))</th>");

                                // Footers loop #end
                                stringBuilder.Append($"}}");

                            // Close footer
                            stringBuilder.Append("</tr></tfoot>");

                            // Open body
                            stringBuilder.Append($"<tbody>");

                                // Row loop #start
                                stringBuilder.Append($"@foreach(System.Data.DataRow row in ((System.Data.DataTable)(ViewData[\"tableData_{c.Name}\"])).Rows) {{");

                                    // Row iteration #start
                                    stringBuilder.Append($"<tr>");

                                        if (itemSelection)
                                        {
                                            stringBuilder.Append($"<th><input title=\"@t._(\"Vybrat\")\" type=\"checkbox\" data-item-selection=\"row\"></th>");
                                        }
                                        // Item population
                                        if (itemPopulation != null)
                                        {
                                            stringBuilder.Append($"<th><button data-item-populator=true>@t._(\"Vybrat\")</button></th>");
                                        }

                                        // Column loop #start
                                        stringBuilder.Append($"@foreach (var cell in row.ItemArray) {{");

                                            // Column iteration #start
                                            stringBuilder.Append($"<td>");

                                                // Wrap with Html.Raw if required
                                                if(allowHtml)
                                                {
                                                    stringBuilder.Append($"@Html.Raw(");
                                                }
                                                else
                                                {
                                                    stringBuilder.Append($"@");
                                                }
                                
                                                // Column iteration
                                                stringBuilder.Append($"t._(cell.ToString())");

                                                // Wrap with Html.Raw if required
                                                if (allowHtml)
                                                {
                                                    stringBuilder.Append($")");
                                                }

                                            // Column iteration #end
                                            stringBuilder.Append($"</td>");

                                        // Column loop #end
                                        stringBuilder.Append($"}}");

                                    // Row iteration #end
                                    stringBuilder.Append($"</tr>");

                                // Row loop #end
                                stringBuilder.Append($"}}");

                            // Close body
                            stringBuilder.Append("</tbody>");

                        // Closing tag
                        stringBuilder.Append($"</{c.Tag}>");

                    // End of on empty condition
                    stringBuilder.Append($"}} else {{ <div class=\"uic control-label empty-table-label\" style=\"left: {c.PositionX}; top: {c.PositionY};\">@t._(\"Tabulka neobsahuje žádná data\")</div> }} }}");

                    // Input that will be filled with row ids
                    stringBuilder.Append($"<input type=\"hidden\" name=\"{c.Name}\" />");

                    // Input that will be filled with selected column names
                    if (columnFilter)
                    {
                        stringBuilder.Append($"<input type=\"hidden\" name=\"{c.Name}-column-filters\" />");
                    }
                }
                else if (c.Type == "name-value-list")
                {
                    stringBuilder.Append($"<{c.Tag} id=\"uic_{c.Name}\" name=\"{c.Name}\" {c.Attributes} class=\"uic {c.Classes}\" style=\"left: {c.PositionX}; top: {c.PositionY}; ");
                    stringBuilder.Append($"width: {c.Width}; height: {c.Height}; {c.Styles}\">");
                    stringBuilder.Append($"@{{ if(ViewData.ContainsKey(\"tableData_{c.Name}\")) {{ var tableData = (System.Data.DataTable)ViewData[\"tableData_{c.Name}\"];");
                    stringBuilder.Append($"foreach (System.Data.DataColumn col in tableData.Columns) {{ if (!col.Caption.StartsWith(\"hidden__\")){{<tr><td class=\"name-cell\">@(t._(col.Caption))</td>");
                    stringBuilder.Append($"<td class=\"value-cell\">@(tableData.Rows.Count>0?tableData.Rows[0][col.ColumnName]:\"no data\")</td></tr>");
                    stringBuilder.Append($"}} }} }} }}</{c.Tag}>");
                }
                else if (c.Type == "data-table-with-actions")
                {
                    bool allowHtml = properties.ContainsKey("allowHtml") && Convert.ToBoolean(properties["allowHtml"]) == true;
                    bool itemSelection = properties.ContainsKey("itemSelection") && Convert.ToBoolean(properties["itemSelection"]) == true;
                    bool rowSelect = properties.ContainsKey("selectByRow") && Convert.ToBoolean(properties["selectByRow"]) == true;
                    bool columnFilter = properties.ContainsKey("columnFilter") && Convert.ToBoolean(properties["columnFilter"]) == true;

                    string itemPopulation = null;
                    if (properties.ContainsKey("itemPopulation"))
                    {
                        itemPopulation = properties["itemPopulation"];
                    }

                    string columnSearchClass = "";
                    if (properties.ContainsKey("searchInIndividualColumns") && Convert.ToBoolean(properties["searchInIndividualColumns"]) == true)
                    {
                        columnSearchClass = " data-table-individual-columns-search";
                    }

                    string actionButtons = "edit-delete";
                    if (!string.IsNullOrEmpty(c.Properties))
                    {
                        string[] tokenPairs = c.Properties.Split(';');
                        foreach (string tokens in tokenPairs)
                        {
                            string[] nameValuePair = tokens.Split('=');
                            if (nameValuePair.Length == 2)
                            {
                                if (nameValuePair[0].ToLower() == "actions")
                                {
                                    actionButtons = nameValuePair[1].ToLower();
                                }
                            }
                        }
                    }


                    // On empty condition
                    stringBuilder.Append($"@{{ if(ViewData.ContainsKey(\"tableData_{c.Name}\") && ((System.Data.DataTable)(ViewData[\"tableData_{c.Name}\"])).Rows.Count > 0) {{");

                        // Opening tag
                        stringBuilder.Append($"<{c.Tag} id=\"uic_{c.Name}\" name=\"{c.Name}\" " + (itemPopulation != null ? "data-item-population-target=\""+itemPopulation+"\"" : "") + (columnFilter ? "data-column-filter" : "true") + (rowSelect ? "data-select-mode=\"row\"" : "") + $" {c.Attributes} class=\"uic {c.Classes}{columnSearchClass}" + (itemSelection ? " hideSecond" : " hideFirst") + $"\" style=\"left: {c.PositionX}; top: {c.PositionY}; ");
                        stringBuilder.Append($"width: {c.Width}; height: {c.Height}; {c.Styles}\" uicWidth=\"{c.Width}\">");

                            // Open heading
                            stringBuilder.Append($"<thead><tr>");

                                    // Item selection (all)
                                    if (itemSelection)
                                    {
                                        stringBuilder.Append($"<th class=\"text-center\"><input title=\"@t._(\"Vybrat vše\")\" type=\"checkbox\" data-item-selection=\"*\"></th>");
                                    }
                                    // Item population
                                    if (itemPopulation != null)
                                    {
                                        stringBuilder.Append($"<th></th>");
                                    }

                                    // Headers loop #start
                                    stringBuilder.Append($"@foreach (System.Data.DataColumn col in ((System.Data.DataTable)(ViewData[\"tableData_{c.Name}\"])).Columns) {{");

                                    // Headers iteration
                                    stringBuilder.Append($"<th>@(t._(col.Caption))</th>");

                                // Headers loop #end
                                stringBuilder.Append($"}} <th>@(t._(\"Akce\"))</th>");

                            // Close heading
                            stringBuilder.Append("</tr></thead>");

                            // Open footer
                            stringBuilder.Append($"<tfoot><tr>");

                                    // 
                                    if (itemSelection)
                                    {
                                        stringBuilder.Append($"<th></th>");
                                    }
                                    // Item population
                                    if (itemPopulation != null)
                                    {
                                        stringBuilder.Append($"<th></th>");
                                    }

                                    // Footers loop #start
                                    stringBuilder.Append($"@foreach (System.Data.DataColumn col in ((System.Data.DataTable)(ViewData[\"tableData_{c.Name}\"])).Columns) {{");

                                    // Footers iteration
                                    stringBuilder.Append($"<th>@(t._(col.Caption))</th>");

                                // Footers loop #end
                                stringBuilder.Append($"}} <th>@(t._(\"Akce\"))</th>");

                            // Close footer
                            stringBuilder.Append("</tr></tfoot>");

                            // Open body
                            stringBuilder.Append($"<tbody>");

                                // Row loop #start
                                stringBuilder.Append($"@foreach(System.Data.DataRow row in ((System.Data.DataTable)(ViewData[\"tableData_{c.Name}\"])).Rows) {{");

                                    // Row iteration #start
                                    stringBuilder.Append($"<tr>");

                                    if (itemSelection)
                                    {
                                        stringBuilder.Append($"<th class=\"text-center\"><input title=\"@t._(\"Vybrat\")\" type=\"checkbox\" data-item-selection=\"row\"></th>");
                                    }
                                    // Item population
                                    if (itemPopulation != null)
                                    {
                                        stringBuilder.Append($"<th><button data-item-populator=true>@t._(\"Vybrat\")</button></th>");
                                    }

                                    // Column loop #start
                                    stringBuilder.Append($"@foreach (var cell in row.ItemArray) {{");

                                            // Column iteration #start
                                            stringBuilder.Append($"<td>");

                                                // Wrap with Html.Raw if required
                                                if (allowHtml)
                                                {
                                                    stringBuilder.Append($"@Html.Raw(");
                                                }
                                                else
                                                {
                                                    stringBuilder.Append($"@");
                                                }

                                                // Column iteration
                                                stringBuilder.Append($"t._(cell.ToString())");

                                                // Wrap with Html.Raw if required
                                                if (allowHtml)
                                                {
                                                    stringBuilder.Append($")");
                                                }

                                            // Column iteration #end
                                            stringBuilder.Append($"</td>");

                                        // Column loop #end
                                        stringBuilder.Append($"}}");

                                            // Actions #start
                                            stringBuilder.Append("<td class=\"actionIcons\">");
                    
                                                switch (actionButtons)
                                                {
                                                    case "download":
                                                        stringBuilder.Append($"<i title=\"@(t._(\"Stáhnout\"))\" class=\"fa fa-download rowEditAction\"></i>");
                                                        break;
                                                    case "download-delete":
                                                        stringBuilder.Append($"<i title=\"@(t._(\"Stáhnout\"))\" class=\"fa fa-download rowEditAction\"></i><i title=\"@(t._(\"Smazat\"))\" class=\"fa fa-remove rowDeleteAction\"></i>");
                                                        break;
                                                    case "enter":
                                                        stringBuilder.Append($"<i title=\"@(t._(\"Vstoupit\"))\" class=\"fa fa-sign-in rowEditAction\"></i>");
                                                        break;
                                                    case "enter-details":
                                                        stringBuilder.Append($"<i title=\"@(t._(\"Vstoupit\"))\" class=\"fa fa-sign-in rowEditAction\"></i><i title=\"@(t._(\"Detail\"))\" class=\"fa fa-search rowDetailsAction\"></i>");
                                                        break;
                                                    case "delete":
                                                        stringBuilder.Append($"<i title=\"@(t._(\"Smazat\"))\" class=\"fa fa-remove rowDeleteAction\"></i>");
                                                        break;
                                                    case "details":
                                                        stringBuilder.Append($"<i title=\"@(t._(\"Detail\"))\" class=\"fa fa-search rowDetailsAction\"></i>");
                                                        break;
                                                    case "details-edit-delete":
                                                        stringBuilder.Append($"<i title=\"@(t._(\"Detail\"))\" class=\"fa fa-search rowDetailsAction\"></i><i title=\"@(t._(\"Editovat\"))\" class=\"fa fa-edit rowEditAction\"></i><i title=\"@(t._(\"Smazat\"))\" class=\"fa fa-remove rowDeleteAction\"></i>");
                                                        break;
                                                    case "edit":
                                                        stringBuilder.Append($"<i class=\"fa fa-edit rowEditAction\"></i>");
                                                        break;
                                                    case "edit-delete":
                                                    default:
                                                        stringBuilder.Append($"<i class=\"fa fa-edit rowEditAction\"></i><i class=\"fa fa-remove rowDeleteAction\"></i>");
                                                        break;
                                                }

                                            // Actions #end
                                            stringBuilder.Append("</td>");

                                        // Row iteration #end
                                        stringBuilder.Append($"</tr>");

                                // Row loop #end
                                stringBuilder.Append($"}}");

                            // Close body
                            stringBuilder.Append("</tbody>");

                        // Closing tag
                        stringBuilder.Append($"</{c.Tag}>");

                    // End of on empty condition
                    stringBuilder.Append($"}} else {{ <div class=\"uic control-label empty-table-label\" style=\"left: {c.PositionX}; top: {c.PositionY};\">@t._(\"Tabulka neobsahuje žádná data\")</div> }} }}");

                    // Input that will be filled with row ids
                    stringBuilder.Append($"<input type=\"hidden\" name=\"{c.Name}\" />");

                    // Input that will be filled with selected column names
                    if (columnFilter)
                    {
                        stringBuilder.Append($"<input type=\"hidden\" name=\"{c.Name}-column-filters\" />");
                    }
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
                else if (c.Type == "static-html")
                {
                    bool invisible = false;
                    if (!string.IsNullOrEmpty(c.Properties))
                    {
                        string[] tokenPairs = c.Properties.Split(';');
                        foreach (string tokens in tokenPairs)
                        {
                            string[] nameValuePair = tokens.Split('=');
                            if (nameValuePair.Length == 2)
                            {
                                // No settings yet
                            }
                            else
                            {
                                if (tokens == "hidden")
                                    invisible = true;
                            }
                        }
                    }
                    stringBuilder.Append($"<{c.Tag} id=\"uic_{c.Name}\" name=\"{c.Name}\" {c.Attributes} class=\"uic {c.Classes}\" style=\"left: {c.PositionX}; top: {c.PositionY}; ");
                    stringBuilder.Append($"width: {c.Width}; height: {c.Height}; {(invisible ? "display:none;" : "")} {c.Styles}\">@Html.Raw(t._(\"{((c.Content ?? "").Replace("\"", "\\\""))}\"))</{c.Tag}>");
                }
                else if (c.Type == "wizard-phases")
                {
                    var phaseLabelArray = !string.IsNullOrEmpty(c.Content) ? c.Content.Split(';').ToList() : new List<string>();
                    int labelCount = phaseLabelArray.Count;
                    int activePhase = 1;
                    if (!string.IsNullOrEmpty(c.Properties))
                    {
                        string[] nameValuePair = c.Properties.Split('=');
                        if (nameValuePair.Length == 2)
                        {
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
                    stringBuilder.Append($"<div class=\"phase-label\">@(t._(\"{(labelCount >= 1 ? phaseLabelArray[0] : "Fáze 1")}\"))</div></div>");

                    stringBuilder.Append($"<div class=\"phase phase2 {(activePhase == 2 ? "phase-active" : "")} {(activePhase > 2 ? "phase-done" : "")}\"><div class=\"phase-icon-circle\">");
                    stringBuilder.Append($"{(activePhase > 2 ? "<div class=\"fa fa-check phase-icon-symbol\"></div>" : "<div class=\"phase-icon-number\">2</div>")}</div>");
                    stringBuilder.Append($"<div class=\"phase-label\">@(t._(\"{(labelCount >= 2 ? phaseLabelArray[1] : "Fáze 2")}\"))</div></div>");

                    stringBuilder.Append($"<div class=\"phase phase3 {(activePhase == 3 ? "phase-active" : "")}\"><div class=\"phase-icon-circle\">");
                    stringBuilder.Append($"<div class=\"phase-icon-number\">3</div></div>");
                    stringBuilder.Append($"<div class=\"phase-label\">@(t._(\"{(labelCount >= 3 ? phaseLabelArray[2] : "Fáze 3")}\"))</div></div>");

                    stringBuilder.Append($"</{c.Tag}>");
                }
                else if (c.Type == "countdown")
                {
                    stringBuilder.Append($"<{c.Tag} id=\"uic_{c.Name}\" name=\"{c.Name}\" {c.Attributes} class=\"uic {c.Classes}\" style=\"left: {c.PositionX}; top: {c.PositionY}; ");
                    stringBuilder.Append($"width: {c.Width}; height: {c.Height}; {c.Styles}\" countdownTarget=\"@(ViewData.ContainsKey(\"countdownTargetData_{c.Name}\")");
                    stringBuilder.Append($" ? @ViewData[\"countdownTargetData_{c.Name}\"] : \"\")\">{c.Content}</{c.Tag}>");
                }
                else if (c.Type == "panel" && allowNesting)
                {
                    bool invisible = false;
                    string generatedAttributes = "";
                    if (!string.IsNullOrEmpty(c.Properties))
                    {
                        string[] tokenPairs = c.Properties.Split(';');
                        foreach (string tokens in tokenPairs)
                        {
                            string[] nameValuePair = tokens.Split('=');
                            if (nameValuePair.Length == 2)
                            {
                                if (nameValuePair[0].ToLower() == "hiddenby")
                                    generatedAttributes += $" panelHiddenBy=\"{nameValuePair[1]}\"";
                                else if (nameValuePair[0].ToLower() == "clone")
                                    generatedAttributes += $" panelClonedBy=\"{nameValuePair[1]}\"";
                            }
                            else
                            {
                                if (tokens == "hidden")
                                    invisible = true;
                            }
                        }
                    }
                    stringBuilder.Append($"<{c.Tag} id=\"uic_{c.Name}\" name=\"{c.Name}\" {c.Attributes} class=\"uic {c.Classes}\" {generatedAttributes} style=\"left: {c.PositionX}; top: {c.PositionY}; ");
                    stringBuilder.Append($"width: {c.Width}; height: {c.Height}; {(invisible ? "display:none;" : "")} {c.Styles}\"");
                    stringBuilder.Append($">");
                    if (c.Classes.Contains("named-panel"))
                    {
                        stringBuilder.Append($"<div class=\"named-panel-header\">@(t._(\"{c.Label}\"))</div>");
                    }
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

    public enum VersionEnum { Absolute, Bootstrap };

    public static class MozaicPropertiesParser
    {
        public static Dictionary<string, string> ParseMozaicPropertiesString(string propertiesString)
        {
            Dictionary<string, string> registry = new Dictionary<string, string>();
            if (!string.IsNullOrEmpty(propertiesString))
            {
                foreach (string record in propertiesString.Split(';'))
                {
                    string[] splittedRecord = record.Split('=');
                    if (splittedRecord.Count() == 2)
                    {
                        registry.Add(splittedRecord[0], splittedRecord[1]);
                    }
                }
            }
            return registry;
        }
    }

    [Table("MozaicEditor_Components")]
    public class MozaicEditorComponent : IEntity
    {
        public MozaicEditorComponent()
        {
            ChildComponents = new HashSet<MozaicEditorComponent>();
        }

        [ImportExportIgnore(IsKey = true)]
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
        public string BootstrapClasses { get; set; }
        public string Styles { get; set; }
        public string Content { get; set; }

        public string Label { get; set; }
        public string Placeholder { get; set; }
        public string TabIndex { get; set; }
        public string Properties { get; set; }

        [ImportExportIgnore]
        public virtual ICollection<MozaicEditorComponent> ChildComponents { get; set; }
        [ImportExportIgnore(IsLinkKey = true)]
        public int? ParentComponentId { get; set; }
        [ImportExportIgnore(IsLink = true)]
        public virtual MozaicEditorComponent ParentComponent { get; set; }
        [ImportExportIgnore(IsParentKey = true)]
        public int MozaicEditorPageId { get; set; }
        [ImportExportIgnore(IsParent = true)]
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
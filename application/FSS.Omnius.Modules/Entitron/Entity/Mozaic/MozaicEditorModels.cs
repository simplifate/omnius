using System.Collections.Generic;
using System.ComponentModel.DataAnnotations.Schema;
using System.Text;

using FSS.Omnius.Modules.Entitron.Entity.Master;

namespace FSS.Omnius.Modules.Entitron.Entity.Mozaic
{
    [Table("MozaicEditor_Pages")]
    public class MozaicEditorPage
    {
        public int Id { get; set; }
        public string Name { get; set; }
        public string CompiledPartialView { get; set; }
        public virtual ICollection<MozaicEditorComponent> Components { get; set; }

        public virtual Application ParentApp { get; set; }

        public void Recompile()
        {
            StringBuilder stringBuilder = new StringBuilder();
            stringBuilder.Append("<form class=\"mozaicForm\" method=\"post\">");
            foreach(MozaicEditorComponent c in this.Components)
            {
                if (c.Tag == "info-container")
                {
                    stringBuilder.Append($"<div class=\"uic info-container {c.Classes} style=\"left: {c.PositionX}; top: {c.PositionY}; ");
                    stringBuilder.Append($"width: {c.Width}; height: {c.Height}; {c.Styles}\">");
                    stringBuilder.Append($"<div class=\"fa fa-info-circle info-container-icon\"></div>");
                    stringBuilder.Append($"<div class=\"info-container-header\">{c.Label}</div>");
                    stringBuilder.Append($"<div class=\"info-container-body\">{c.Content}</div></div>");
                }
                else
                {
                    stringBuilder.Append($"<{c.Tag} id=\"uic{c.Id}\" {c.Attributes} class=\"uic {c.Classes}\" style=\"left: {c.PositionX}; top: {c.PositionY}; ");
                    stringBuilder.Append($"width: {c.Width}; height: {c.Height}; {c.Styles}\">{c.Content}</{c.Tag}>");
                }
            }
            stringBuilder.Append("</form>");
            CompiledPartialView = stringBuilder.ToString();
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
    }
}

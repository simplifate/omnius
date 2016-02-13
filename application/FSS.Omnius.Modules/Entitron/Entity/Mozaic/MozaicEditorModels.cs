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
                stringBuilder.Append($"<{c.Tag} id=\"uic{c.Id}\" {c.Attributes} class=\"{c.Classes}\" style=\"left: {c.PositionX}px; top: {c.PositionY}px;");
                stringBuilder.Append($"width: {c.Width}px; height: {c.Height}px; {c.Styles}\">{c.Content}</{c.Tag}>");
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
        public int PositionX { get; set; }
        public int PositionY { get; set; }
        public int Width { get; set; }
        public int Height { get; set; }

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

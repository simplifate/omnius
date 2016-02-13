using System.Collections.Generic;

namespace FSS.Omnius.Modules.Entitron.Entity.Mozaic
{
    public class AjaxMozaicEditorPageHeader
    {
        public int Id { get; set; }
        public string Name { get; set; }
    }
    public class AjaxMozaicEditorPage
    {
        public int Id { get; set; }
        public string Name { get; set; }
        public List<AjaxMozaicEditorComponent> Components { get; set; }

        public AjaxMozaicEditorPage()
        {
            Components = new List<AjaxMozaicEditorComponent>();
        }
    }
    public class AjaxMozaicEditorComponent
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

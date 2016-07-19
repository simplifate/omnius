using System.Collections.Generic;

namespace FSS.Omnius.Modules.Entitron.Entity.Mozaic
{
    public class AjaxMozaicEditorPageHeader : IEntity
    {
        public int Id { get; set; }
        public string Name { get; set; }
    }
    public class AjaxMozaicEditorPage : IEntity
    {
        public int Id { get; set; }
        public string Name { get; set; }
        public bool IsModal { get; set; }
        public int? ModalWidth { get; set; }
        public int? ModalHeight { get; set; }
        public bool IsDeleted { get; set; }
        public List<AjaxMozaicEditorComponent> Components { get; set; }

        public AjaxMozaicEditorPage()
        {
            Components = new List<AjaxMozaicEditorComponent>();
        }
    }
    public class AjaxMozaicEditorComponent : IEntity
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
        public string TabIndex { get; set; }
        public string Properties { get; set; }

        public List<AjaxMozaicEditorComponent> ChildComponents { get; set; }
    }
}

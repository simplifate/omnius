namespace FSS.Omnius.Modules.Entitron.Entity.Master
{
    public class AjaxAppProperties : IEntity
    {
        public int Id { get; set; }
        public string DisplayName { get; set; }
        public int CSSTemplateId { get; set; }
        public string Icon { get; set; }
        public int Color { get; set; }
        public int TileWidth { get; set; }
        public int TileHeight { get; set; }
    }
    public class AjaxAppState : IEntity
    {
        public int Id { get; set; }
        public bool IsEnabled { get; set; }
    }
    public class AjaxAppCoordinates : IEntity
    {
        public string positionX { get; set; }
        public string positionY { get; set; }
    }
}
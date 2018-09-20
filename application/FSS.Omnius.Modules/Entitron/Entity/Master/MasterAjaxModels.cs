namespace FSS.Omnius.Modules.Entitron.Entity.Master
{
    public class AjaxAppProperties : IEntity
    {
        public int Id { get; set; }
        public string DisplayName { get; set; }
        public string Icon { get; set; }
        public int Color { get; set; }
        public int TileWidth { get; set; }
        public int TileHeight { get; set; }
        public bool IsAllowedForAll { get; set; }
        public bool IsAllowedGuests { get; set; }

    }
    public class AjaxAppState : IEntity
    {
        public int Id { get; set; }
        public bool IsEnabled { get; set; }
    }
    public class AjaxAppCoordinates : IEntity
    {
        public int positionX { get; set; }
        public int positionY { get; set; }
    }
}
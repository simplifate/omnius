namespace FSPOC_WebProject.Models
{
    public class AppTile
    {
        public int Id { get; set; }
        public string Name { get; set; }
        public string Icon { get; set; }
        public int TitleFontSize { get; set; }
        public int Color { get; set; }
        public string InnerHTML { get; set; }
        public string LaunchCommand { get; set; }
        public int TileWidth { get; set; }
        public int TileHeight { get; set; }
        public int PositionX { get; set; }
        public int PositionY { get; set; }
        public AppTile()
        {
            TileWidth     = 2;
            TileHeight    = 1;
            Color         = 0;
            Icon          = "fa-question";
            TitleFontSize = 20;
        }
    }
}

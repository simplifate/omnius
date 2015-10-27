namespace FSS.FSPOC.BussinesObjects.Entities.DatabaseDesigner
{
    public class DbView
    {
        public int Id { get; set; }
        public string Name { get; set; }
        public string Query { get; set; }
        public int PositionX { get; set; }
        public int PositionY { get; set; }

        public virtual DbSchemeCommit DbSchemeCommit { get; set; }
    }
}
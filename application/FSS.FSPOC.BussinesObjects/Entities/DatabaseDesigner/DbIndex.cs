namespace FSS.FSPOC.BussinesObjects.Entities.DatabaseDesigner
{
    public class DbIndex
    {
        public int Id { get; set; }
        public string Name { get; set; }
        public bool Unique { get; set; }
        public string ColumnNames { get; set; }

        public virtual DbTable DbTable { get; set; }
    }
}
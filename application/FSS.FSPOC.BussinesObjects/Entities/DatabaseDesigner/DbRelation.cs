namespace FSS.FSPOC.BussinesObjects.Entities.DatabaseDesigner
{
    public class DbRelation
    {
        public int Id { get; set; }
        public int Type { get; set; }
        public int LeftTable { get; set; }
        public int LeftColumn { get; set; }
        public int RightTable { get; set; }
        public int RightColumn { get; set; }

        public virtual DbSchemeCommit DbSchemeCommit { get; set; }
    }
}
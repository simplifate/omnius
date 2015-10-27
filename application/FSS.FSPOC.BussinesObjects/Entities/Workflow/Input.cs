namespace FSS.FSPOC.BussinesObjects.Entities.Workflow
{
    public class Input
    {
        public int Id { get; set; }
        public int Source { get; set; }
        public int Slot { get; set; }

        public virtual Activity Activity { get; set; }
    }
}
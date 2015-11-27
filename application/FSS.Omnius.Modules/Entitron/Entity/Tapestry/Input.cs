using System.ComponentModel.DataAnnotations.Schema;

namespace FSS.Omnius.Modules.Entitron.Entity.Tapestry
{
    [Table("Tapestry_Input")]
    public class Input
    {
        public int Id { get; set; }
        public int Source { get; set; }
        public int Slot { get; set; }

        public virtual Activity Activity { get; set; }
    }
}
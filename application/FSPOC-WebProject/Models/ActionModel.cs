using System.Dynamic;

namespace FSPOC_WebProject.Models
{
    public class ActionModel
    {
        public int BlockId { get; set; }
        public dynamic SourceObject { get; set; }
    }
}
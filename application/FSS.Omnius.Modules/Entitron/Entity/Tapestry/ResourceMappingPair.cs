using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace FSS.Omnius.Modules.Entitron.Entity.Tapestry
{
    public class ResourceMappingPair
    {
        public int Id { get; set; }
        public string TargetName { get; set; }
        public string TargetType { get; set; }
        public string SourceColumnFilter { get; set; }
        public virtual TapestryDesignerResourceItem Source { get; set; }
        public virtual TapestryDesignerResourceItem Target { get; set; }
    }
}

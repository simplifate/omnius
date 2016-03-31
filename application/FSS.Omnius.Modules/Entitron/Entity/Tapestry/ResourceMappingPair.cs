﻿using System.ComponentModel.DataAnnotations.Schema;

namespace FSS.Omnius.Modules.Entitron.Entity.Tapestry
{
    [Table("Tapestry_ResourceMappingPairs")]
    public class ResourceMappingPair
    {
        public int Id { get; set; }
        public string TargetName { get; set; }
        public string TargetType { get; set; }
        public string SourceColumnFilter { get; set; }
        public string DataSourceParams { get; set; }
        public virtual TapestryDesignerResourceItem Source { get; set; }
        public virtual TapestryDesignerResourceItem Target { get; set; }
    }
}

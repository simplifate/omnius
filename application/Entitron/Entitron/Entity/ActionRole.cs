namespace Entitron.Entity
{
    using System;
    using System.Collections.Generic;
    using System.ComponentModel.DataAnnotations;
    using System.ComponentModel.DataAnnotations.Schema;
    using System.Data.Entity.Spatial;

    [Table("Tapestry_ActionRoles")]
    public partial class ActionRole
    {
        [System.Diagnostics.CodeAnalysis.SuppressMessage("Microsoft.Usage", "CA2214:DoNotCallOverridableMethodsInConstructors")]
        public ActionRole()
        {
            Tapestry_ActionRole_Action = new HashSet<ActionRole_Action>();
        }

        public int Id { get; set; }

        public int SourceBlockId { get; set; }

        public int TargetBlockId { get; set; }

        public int ActorId { get; set; }

        [System.Diagnostics.CodeAnalysis.SuppressMessage("Microsoft.Usage", "CA2227:CollectionPropertiesShouldBeReadOnly")]
        public virtual ICollection<ActionRole_Action> Tapestry_ActionRole_Action { get; set; }

        public virtual Actor Tapestry_Actors { get; set; }

        public virtual Block Tapestry_Blocks { get; set; }

        public virtual Block Tapestry_Blocks1 { get; set; }
    }
}

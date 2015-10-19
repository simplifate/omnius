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
            ActionRole_Actions = new HashSet<ActionRole_Action>();
        }

        public int Id { get; set; }

        public int SourceBlockId { get; set; }

        public int TargetBlockId { get; set; }

        public int ActorId { get; set; }

        [System.Diagnostics.CodeAnalysis.SuppressMessage("Microsoft.Usage", "CA2227:CollectionPropertiesShouldBeReadOnly")]
        public virtual ICollection<ActionRole_Action> ActionRole_Actions { get; set; }

        public virtual Actor Actor { get; set; }

        public virtual Block SourceBlock { get; set; }

        public virtual Block TargetBlock { get; set; }
    }
}

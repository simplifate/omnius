namespace Entitron.Entity
{
    using System;
    using System.Collections.Generic;
    using System.ComponentModel.DataAnnotations;
    using System.ComponentModel.DataAnnotations.Schema;
    using System.Data.Entity.Spatial;

    [Table("Tapestry_ActionRole_Action")]
    public partial class ActionRole_Action
    {
        [Key]
        [Column(Order = 0)]
        [DatabaseGenerated(DatabaseGeneratedOption.None)]
        public int ActionRoleId { get; set; }

        [Key]
        [Column(Order = 1)]
        [DatabaseGenerated(DatabaseGeneratedOption.None)]
        public int ActionId { get; set; }

        public int Order { get; set; }

        [StringLength(200)]
        public string ResultVariables { get; set; }

        public virtual ActionRole ActionRole { get; set; }

        public virtual Action Action { get; set; }
    }
}

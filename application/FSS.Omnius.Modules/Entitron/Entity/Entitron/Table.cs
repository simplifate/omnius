namespace FSS.Omnius.Modules.Entitron.Entity.Entitron
{
    using System;
    using System.Collections.Generic;
    using System.ComponentModel.DataAnnotations;
    using System.ComponentModel.DataAnnotations.Schema;
    using System.Data.Entity.Spatial;
    using Master;

    [Table("Entitron___META")]
    public partial class Table
    {
        public int Id { get; set; }
        [Required]
        [StringLength(50)]
        [Index("UNIQUE_Entitron___META_Name", IsUnique = true, Order = 2)]
        public string Name { get; set; }

        [Index("UNIQUE_Entitron___META_Name", IsUnique = true, Order = 1)]
        public int ApplicationId { get; set; }

        public int tableId { get; set; }

        public Application Application { get; set; }
    }
}

namespace FSS.Omnius.Modules.Entitron.Entity.Entitron
{
    using System.ComponentModel.DataAnnotations;
    using System.ComponentModel.DataAnnotations.Schema;
    using Master;

    [Table("Entitron___META")]
    public partial class Table : IEntity
    {
        public int Id { get; set; }
        [Required]
        [StringLength(50)]
        [Index("UNIQUE_Entitron___META_Name", IsUnique = true, Order = 2)]
        public string Name { get; set; }

        [Index("UNIQUE_Entitron___META_Name", IsUnique = true, Order = 1)]
        public int? ApplicationId { get; set; }

        public int tableId { get; set; }

        public Application Application { get; set; }
    }
}

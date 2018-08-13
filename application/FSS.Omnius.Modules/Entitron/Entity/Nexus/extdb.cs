namespace FSS.Omnius.Modules.Entitron.Entity.Nexus
{
    using System.ComponentModel.DataAnnotations;
    using System.ComponentModel.DataAnnotations.Schema;

    public enum ExtDBType
    {
        MySQL,
        MSSQL,
        RethinkDB
    }

    [Table("Nexus_Ext_DB")]
    public partial class ExtDB : IEntity
    {
        public int? Id { get; set; }

        [Required]
        [Display(Name = "Type")]
        public ExtDBType DB_Type { get; set; }

        [Required]
        [StringLength(255)]
        [Display(Name = "Server")]
        public string DB_Server { get; set; }

        [Required]
        [StringLength(6)]
        [Display(Name = "Port")]
        public string DB_Port { get; set; }

        [Required]
        [StringLength(255)]
        [Display(Name = "Database name")]
        public string DB_Name { get; set; }

        [Required]
        [StringLength(255)]
        [Display(Name = "User")]
        public string DB_User { get; set; }

        [Required]
        [StringLength(255)]
        [Display(Name = "Password")]
        public string DB_Password { get; set; }

        [Required]
        [StringLength(255)]
        [Display(Name = "Alias")]
        public string DB_Alias { get; set; }
    }
}

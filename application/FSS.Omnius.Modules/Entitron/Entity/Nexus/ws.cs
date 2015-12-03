namespace FSS.Omnius.Modules.Entitron.Entity.Nexus
{
    using System.ComponentModel.DataAnnotations;
    using System.ComponentModel.DataAnnotations.Schema;

    [Table("Nexus_WS")]
    public partial class WS
    {
        public int? Id { get; set; }

        [StringLength(255)]
        [Display(Name = "Název")]
        public string Name{ get; set; }

        [StringLength(255)]
        [Display(Name = "WSDL URL")]
        public string WSDL_Url { get; set; }

        [Display(Name = "WSDL XML soubor")]
        public byte[] WSDL_File { get; set; }

        [Display(Name = "Uživatel")]
        public string Auth_User { get; set; }

        [Display(Name = "Heslo")]
        public string Auth_Password { get; set; }
    }
}

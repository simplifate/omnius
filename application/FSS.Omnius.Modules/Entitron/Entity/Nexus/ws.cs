namespace FSS.Omnius.Modules.Entitron.Entity.Nexus
{
    using System.ComponentModel.DataAnnotations;
    using System.ComponentModel.DataAnnotations.Schema;

    public enum WSType
    {
        SOAP,
        REST
    }

    [Table("Nexus_WS")]
    public partial class WS
    {
        public int? Id { get; set; }

        [Display(Name = "Typ webové služby")]
        public WSType Type { get; set; }

        [StringLength(255)]
        [Display(Name = "Název")]
        public string Name{ get; set; }

        [StringLength(255)]
        [Display(Name = "WSDL URL")]
        public string WSDL_Url { get; set; }

        [Display(Name = "WSDL XML soubor")]
        public byte[] WSDL_File { get; set; }

        [Display(Name = "REST Base URL")]
        public string REST_Base_Url { get; set; }

        [Display(Name = "Uživatel")]
        public string Auth_User { get; set; }

        [Display(Name = "Heslo")]
        public string Auth_Password { get; set; }
    }
}

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
    public partial class WS : IEntity
    {
        public int? Id { get; set; }

        [Display(Name = "Web service type")]
        public WSType Type { get; set; }

        [StringLength(255)]
        [Display(Name = "Name")]
        public string Name{ get; set; }

        [StringLength(255)]
        [Display(Name = "WSDL URL")]
        public string WSDL_Url { get; set; }

        [Display(Name = "WSDL XML file")]
        public byte[] WSDL_File { get; set; }

        [Display(Name = "REST Base URL")]
        public string REST_Base_Url { get; set; }

        [Display(Name = "User")]
        public string Auth_User { get; set; }

        [Display(Name = "Password")]
        public string Auth_Password { get; set; }

        [Display(Name = "SOAP endpoint")]
        public string SOAP_Endpoint { get; set; }

        [Display(Name = "XMLNS List")]
        [DataType(DataType.MultilineText)]
        public string SOAP_XML_NS { get; set; }
    }
}

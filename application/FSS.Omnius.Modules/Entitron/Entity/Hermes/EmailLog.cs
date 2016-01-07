namespace FSS.Omnius.Modules.Entitron.Entity.Hermes
{
    using System.Collections.Generic;
    using System.ComponentModel.DataAnnotations;
    using System.ComponentModel.DataAnnotations.Schema;
    using System.Web.Mvc;

    [Table("Hermes_Email_Log")]
    public partial class EmailLog
    {
        public int? Id { get; set; }

        [DataType(DataType.Text)]
        [AllowHtml]
        public string Content { get; set; }
    }
}

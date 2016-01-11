namespace FSS.Omnius.Modules.Entitron.Entity.Hermes
{
    using System;
    using System.ComponentModel.DataAnnotations;
    using System.ComponentModel.DataAnnotations.Schema;
    using System.Web.Mvc;

    public enum EmailSendStatus
    {
        success,
        failed
    }

    [Table("Hermes_Email_Log")]
    public partial class EmailLog
    {
        public int? Id { get; set; }

        [DataType(DataType.Text)]
        [AllowHtml]
        public string Content { get; set; }

        public DateTime DateSend { get; set; }

        public EmailSendStatus Status { get; set; } 

        public string SMTP_Error { get; set; }
    }
}

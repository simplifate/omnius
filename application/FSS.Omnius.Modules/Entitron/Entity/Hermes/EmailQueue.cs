namespace FSS.Omnius.Modules.Entitron.Entity.Hermes
{
    using System;
    using System.ComponentModel.DataAnnotations;
    using System.ComponentModel.DataAnnotations.Schema;
    using System.Net.Mail;
    [Table("Hermes_Email_Queue")]
    public partial class EmailQueue
    {
        public int? Id { get; set; }
        
        [DataType(DataType.Text)]
        public string Message { get; set; }

        [Index(IsClustered = false, IsUnique = false)]
        public DateTime Date_Send_After { get; set; }

        public DateTime Date_Inserted { get; set; }

        public int? Application_Id { get; set; }
    }
}

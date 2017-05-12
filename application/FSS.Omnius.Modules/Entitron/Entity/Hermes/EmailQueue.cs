namespace FSS.Omnius.Modules.Entitron.Entity.Hermes
{
    using Newtonsoft.Json;
    using Master;
    using System;
    using System.ComponentModel.DataAnnotations;
    using System.ComponentModel.DataAnnotations.Schema;

    public enum EmailQueueStatus
    {
        waiting,
        error
    };

    [Table("Hermes_Email_Queue")]
    public partial class EmailQueue : IEntity
    {
        public int? Id { get; set; }
        
        [DataType(DataType.Text)]
        public string Message { get; set; }

        [Index(IsClustered = false, IsUnique = false)]
        public DateTime Date_Send_After { get; set; }

        public DateTime Date_Inserted { get; set; }

        [DataType(DataType.Text)]
        public string AttachmentList { get; set; }
        
        public int ApplicationId { get; set; }
        public Application Application { get; set; }

        public EmailQueueStatus Status { get; set; }
    }
}

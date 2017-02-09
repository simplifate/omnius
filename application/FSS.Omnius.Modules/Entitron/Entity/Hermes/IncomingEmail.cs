namespace FSS.Omnius.Modules.Entitron.Entity.Hermes
{
    using System.Collections.Generic;
    using System.ComponentModel.DataAnnotations;
    using System.ComponentModel.DataAnnotations.Schema;

    [Table("Hermes_Incoming_Email")]
    public partial class IncomingEmail : IEntity
    {
        public IncomingEmail()
        {
            IncomingEmailRule = new List<IncomingEmailRule>();
        }

        public int? Id { get; set; }

        [Required]
        [Display(Name = "Název")]
        public string Name { get; set; }

        [Required]
        [Display(Name = "IMAP Server")]
        public string ImapServer { get; set; }

        [Display(Name = "Port")]
        public int? ImapPort { get; set; }

        [Required]
        [Display(Name = "Uživatelské jméno (e-mail)")]
        public string UserName { get; set; }

        [Required]
        [Display(Name = "Heslo")]
        public string Password { get; set; }

        public virtual ICollection<IncomingEmailRule> IncomingEmailRule { get; set; }
    }
}

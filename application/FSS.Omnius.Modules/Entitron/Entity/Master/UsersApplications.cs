namespace FSS.Omnius.Modules.Entitron.Entity.Master
{
    using System;
    using System.Collections.Generic;
    using System.ComponentModel.DataAnnotations;
    using System.ComponentModel.DataAnnotations.Schema;
    using System.Data.Entity.Spatial;
    using Tapestry;
    using Mozaic;
    using Entitron;
    using Persona;
    using Newtonsoft.Json;
    [Table("Master_UsersApplications")]
    public partial class UsersApplications
    {
        public int Id { get; set; }
        [Index("IX_userApp", 1, IsUnique = true)]
        public int UserId { get; set; }
        public User User { get; set; }
        [Index("IX_userApp", 2, IsUnique = true)]
        public int ApplicationId { get; set; }
        public Application Application { get; set; }

        public int PositionX { get; set; }
        public int PositionY { get; set; }
    }
}

namespace FSS.Omnius.Modules.Entitron.Entity.Master
{
    using System.ComponentModel.DataAnnotations.Schema;
    using Entitron;
    using Persona;

    [Table("Master_UsersApplications")]
    public partial class UsersApplications : IEntity
    {
        public int Id { get; set; }
        [Index("IX_userApp", 1, IsUnique = true)]
        public int UserId { get; set; }
        public virtual User User { get; set; }
        [Index("IX_userApp", 2, IsUnique = true)]
        public int ApplicationId { get; set; }
        public virtual Application Application { get; set; }

        public int PositionX { get; set; }
        public int PositionY { get; set; }

        public UsersApplications()
        {
            PositionX = 10;
            PositionY = 10;
        }
    }
}

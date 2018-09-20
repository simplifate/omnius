namespace FSS.Omnius.Modules.Migrations.MSSQL
{
    using System;
    using System.Data.Entity.Migrations;
    
    public partial class NexusSocketListenerUpdate : DbMigration
    {
        public override void Up()
        {
            AddColumn("dbo.Nexus_TCP_Socket_Listener", "BufferSize", c => c.Int(nullable: false));
        }
        
        public override void Down()
        {
            DropColumn("dbo.Nexus_TCP_Socket_Listener", "BufferSize");
        }
    }
}

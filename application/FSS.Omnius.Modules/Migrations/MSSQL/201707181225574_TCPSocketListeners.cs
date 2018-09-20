namespace FSS.Omnius.Modules.Migrations.MSSQL
{
    using System;
    using System.Data.Entity.Migrations;
    
    public partial class TCPSocketListeners : DbMigration
    {
        public override void Up()
        {
            CreateTable(
                "dbo.Nexus_TCP_Socket_Listener",
                c => new
                    {
                        Id = c.Int(nullable: false, identity: true),
                        Port = c.Int(nullable: false),
                        ApplicationId = c.Int(nullable: false),
                        BlockName = c.String(nullable: false),
                        WorkflowName = c.String(nullable: false),
                        Name = c.String(nullable: false),
                    })
                .PrimaryKey(t => t.Id)
                .ForeignKey("dbo.Master_Applications", t => t.ApplicationId, cascadeDelete: true)
                .Index(t => t.ApplicationId);
            
        }
        
        public override void Down()
        {
            DropForeignKey("dbo.Nexus_TCP_Socket_Listener", "ApplicationId", "dbo.Master_Applications");
            DropIndex("dbo.Nexus_TCP_Socket_Listener", new[] { "ApplicationId" });
            DropTable("dbo.Nexus_TCP_Socket_Listener");
        }
    }
}

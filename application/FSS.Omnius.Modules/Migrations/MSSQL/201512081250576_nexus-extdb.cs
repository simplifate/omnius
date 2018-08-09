namespace FSS.Omnius.Modules.Migrations.MSSQL
{
    using System;
    using System.Data.Entity.Migrations;
    
    public partial class nexusextdb : DbMigration
    {
        public override void Up()
        {
            CreateTable(
                "dbo.Nexus_Ext_DB",
                c => new
                    {
                        Id = c.Int(nullable: false, identity: true),
                        DB_Type = c.Int(nullable: false),
                        DB_Server = c.String(nullable: false, maxLength: 255),
                        DB_Port = c.String(nullable: false, maxLength: 6),
                        DB_Name = c.String(nullable: false, maxLength: 255),
                        DB_User = c.String(nullable: false, maxLength: 255),
                        DB_Password = c.String(nullable: false, maxLength: 255),
                    })
                .PrimaryKey(t => t.Id);
            
            AddColumn("dbo.Master_Applications", "ShowInAppManager", c => c.Boolean(nullable: false));
            DropColumn("dbo.Master_Applications", "IsPublished");
            DropColumn("dbo.Master_Applications", "IsEnabled");
        }
        
        public override void Down()
        {
            AddColumn("dbo.Master_Applications", "IsEnabled", c => c.Boolean(nullable: false));
            AddColumn("dbo.Master_Applications", "IsPublished", c => c.Boolean(nullable: false));
            DropColumn("dbo.Master_Applications", "ShowInAppManager");
            DropTable("dbo.Nexus_Ext_DB");
        }
    }
}

namespace FSS.Omnius.Modules.Migrations.MSSQL
{
    using System;
    using System.Data.Entity.Migrations;
    
    public partial class Dev_Work_Grid_Merge : DbMigration
    {
        public override void Up()
        {
            CreateTable(
                "dbo.Athena_Graph",
                c => new
                    {
                        Id = c.Int(nullable: false, identity: true),
                        Active = c.Boolean(nullable: false),
                        Name = c.String(nullable: false, maxLength: 255),
                        Ident = c.String(nullable: false, maxLength: 255),
                        Js = c.String(nullable: false),
                        Css = c.String(),
                        DemoData = c.String(),
                        Html = c.String(),
                        Library = c.String(),
                    })
                .PrimaryKey(t => t.Id);
            
            AddColumn("dbo.Persona_Users", "DeletedBySync", c => c.DateTime());
            AlterColumn("dbo.TapestryDesigner_ResourceItems", "IsShared", c => c.Boolean());
            AlterColumn("dbo.Tapestry_ResourceMappingPairs", "SourceIsShared", c => c.Boolean());
            AlterColumn("dbo.Tapestry_ResourceMappingPairs", "TargetIsShared", c => c.Boolean());
        }
        
        public override void Down()
        {
            AlterColumn("dbo.Tapestry_ResourceMappingPairs", "TargetIsShared", c => c.Boolean(nullable: false));
            AlterColumn("dbo.Tapestry_ResourceMappingPairs", "SourceIsShared", c => c.Boolean(nullable: false));
            AlterColumn("dbo.TapestryDesigner_ResourceItems", "IsShared", c => c.Boolean(nullable: false));
            DropColumn("dbo.Persona_Users", "DeletedBySync");
            DropTable("dbo.Athena_Graph");
        }
    }
}

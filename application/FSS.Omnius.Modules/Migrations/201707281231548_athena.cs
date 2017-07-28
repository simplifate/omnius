namespace FSS.Omnius.Modules.Migrations
{
    using System;
    using System.Data.Entity.Migrations;
    
    public partial class athena : DbMigration
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
            
        }
        
        public override void Down()
        {
            DropTable("dbo.Athena_Graph");
        }
    }
}

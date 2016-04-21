namespace FSS.Omnius.Modules.Migrations
{
    using System;
    using System.Data.Entity.Migrations;
    
    public partial class dropModules : DbMigration
    {
        public override void Up()
        {
            DropIndex("dbo.CORE_Modules", new[] { "Name" });
            DropTable("dbo.CORE_Modules");
        }
        
        public override void Down()
        {
            CreateTable(
                "dbo.CORE_Modules",
                c => new
                    {
                        Id = c.Int(nullable: false, identity: true),
                        Name = c.String(nullable: false, maxLength: 50),
                        Description = c.String(),
                        IsEnabled = c.Boolean(nullable: false),
                    })
                .PrimaryKey(t => t.Id);
            
            CreateIndex("dbo.CORE_Modules", "Name", unique: true);
        }
    }
}

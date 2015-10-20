namespace CORE.Migrations
{
    using System;
    using System.Data.Entity.Migrations;
    
    public partial class addModules : DbMigration
    {
        public override void Up()
        {
            CreateTable(
                "dbo.CORE_Modules",
                c => new
                    {
                        Id = c.Int(nullable: false, identity: true),
                        Name = c.String(nullable: false),
                        Address = c.String(nullable: false),
                        Description = c.String(),
                        IsEnabled = c.Boolean(nullable: false, defaultValue: false),
                    })
                .PrimaryKey(t => t.Id);
            
        }
        
        public override void Down()
        {
            DropTable("dbo.CORE_Modules");
        }
    }
}

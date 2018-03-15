namespace FSS.Omnius.Modules.Migrations
{
    using System;
    using System.Data.Entity.Migrations;
    
    public partial class athenaUniqueness : DbMigration
    {
        public override void Up()
        {
            CreateIndex("dbo.Athena_Graph", "Ident", unique: true);
        }
        
        public override void Down()
        {
            DropIndex("dbo.Athena_Graph", new[] { "Ident" });
        }
    }
}

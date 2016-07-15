namespace FSS.Omnius.Modules.Migrations
{
    using System;
    using System.Data.Entity.Migrations;
    
    public partial class mozaiceditorpageversion : DbMigration
    {
        public override void Up()
        {
            AddColumn("dbo.MozaicEditor_Pages", "Version", c => c.Int(nullable: false));
        }
        
        public override void Down()
        {
            DropColumn("dbo.MozaicEditor_Pages", "Version");
        }
    }
}

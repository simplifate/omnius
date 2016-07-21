namespace FSS.Omnius.Modules.Migrations
{
    using System;
    using System.Data.Entity.Migrations;
    
    public partial class addbootstrapclasses : DbMigration
    {
        public override void Up()
        {
            AddColumn("dbo.MozaicEditor_Components", "BootstrapClasses", c => c.String());
        }
        
        public override void Down()
        {
            DropColumn("dbo.MozaicEditor_Components", "BootstrapClasses");
        }
    }
}

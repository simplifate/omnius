namespace FSS.Omnius.Modules.Migrations
{
    using System;
    using System.Data.Entity.Migrations;
    
    public partial class Mozaiceditorproperties : DbMigration
    {
        public override void Up()
        {
            AddColumn("dbo.MozaicEditor_Components", "Name", c => c.String());
            AddColumn("dbo.MozaicEditor_Components", "Type", c => c.String());
            AlterColumn("dbo.MozaicEditor_Components", "PositionX", c => c.String());
            AlterColumn("dbo.MozaicEditor_Components", "PositionY", c => c.String());
            AlterColumn("dbo.MozaicEditor_Components", "Width", c => c.String());
            AlterColumn("dbo.MozaicEditor_Components", "Height", c => c.String());
        }
        
        public override void Down()
        {
            AlterColumn("dbo.MozaicEditor_Components", "Height", c => c.Int(nullable: false));
            AlterColumn("dbo.MozaicEditor_Components", "Width", c => c.Int(nullable: false));
            AlterColumn("dbo.MozaicEditor_Components", "PositionY", c => c.Int(nullable: false));
            AlterColumn("dbo.MozaicEditor_Components", "PositionX", c => c.Int(nullable: false));
            DropColumn("dbo.MozaicEditor_Components", "Type");
            DropColumn("dbo.MozaicEditor_Components", "Name");
        }
    }
}

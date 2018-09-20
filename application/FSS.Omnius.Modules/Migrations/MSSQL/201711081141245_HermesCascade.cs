namespace FSS.Omnius.Modules.Migrations.MSSQL
{
    using System;
    using System.Data.Entity.Migrations;
    
    public partial class HermesCascade : DbMigration
    {
        public override void Up()
        {
            DropForeignKey("dbo.Hermes_Email_Template_Content", "Hermes_Email_Template_Id", "dbo.Hermes_Email_Template");
            DropForeignKey("dbo.Hermes_Email_Placeholder", "Hermes_Email_Template_Id", "dbo.Hermes_Email_Template");
            AddForeignKey("dbo.Hermes_Email_Template_Content", "Hermes_Email_Template_Id", "dbo.Hermes_Email_Template", "Id", cascadeDelete: true);
            AddForeignKey("dbo.Hermes_Email_Placeholder", "Hermes_Email_Template_Id", "dbo.Hermes_Email_Template", "Id", cascadeDelete: true);
        }
        
        public override void Down()
        {
            DropForeignKey("dbo.Hermes_Email_Placeholder", "Hermes_Email_Template_Id", "dbo.Hermes_Email_Template");
            DropForeignKey("dbo.Hermes_Email_Template_Content", "Hermes_Email_Template_Id", "dbo.Hermes_Email_Template");
            AddForeignKey("dbo.Hermes_Email_Placeholder", "Hermes_Email_Template_Id", "dbo.Hermes_Email_Template", "Id");
            AddForeignKey("dbo.Hermes_Email_Template_Content", "Hermes_Email_Template_Id", "dbo.Hermes_Email_Template", "Id");
        }
    }
}

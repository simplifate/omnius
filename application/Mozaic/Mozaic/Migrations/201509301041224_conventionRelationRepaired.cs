namespace Mozaic.Migrations
{
    using System;
    using System.Data.Entity.Migrations;
    
    public partial class conventionRelationRepaired : DbMigration
    {
        public override void Up()
        {
            RenameColumn(table: "dbo.Pages", name: "MasterTemplate_Id", newName: "MasterTemplateId");
            RenameColumn(table: "dbo.Templates", name: "Category_Id", newName: "CategoryId");
            RenameIndex(table: "dbo.Pages", name: "IX_MasterTemplate_Id", newName: "IX_MasterTemplateId");
            RenameIndex(table: "dbo.Templates", name: "IX_Category_Id", newName: "IX_CategoryId");
        }
        
        public override void Down()
        {
            RenameIndex(table: "dbo.Templates", name: "IX_CategoryId", newName: "IX_Category_Id");
            RenameIndex(table: "dbo.Pages", name: "IX_MasterTemplateId", newName: "IX_MasterTemplate_Id");
            RenameColumn(table: "dbo.Templates", name: "CategoryId", newName: "Category_Id");
            RenameColumn(table: "dbo.Pages", name: "MasterTemplateId", newName: "MasterTemplate_Id");
        }
    }
}

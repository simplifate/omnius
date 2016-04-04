namespace FSS.Omnius.Modules.Migrations
{
    using System;
    using System.Data.Entity.Migrations;
    
    public partial class componentParent : DbMigration
    {
        public override void Up()
        {
            RenameColumn(table: "dbo.MozaicEditor_Components", name: "ParentComponent_Id", newName: "ParentComponentId");
            RenameIndex(table: "dbo.MozaicEditor_Components", name: "IX_ParentComponent_Id", newName: "IX_ParentComponentId");
        }
        
        public override void Down()
        {
            RenameIndex(table: "dbo.MozaicEditor_Components", name: "IX_ParentComponentId", newName: "IX_ParentComponent_Id");
            RenameColumn(table: "dbo.MozaicEditor_Components", name: "ParentComponentId", newName: "ParentComponent_Id");
        }
    }
}

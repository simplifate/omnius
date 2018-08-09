namespace FSS.Omnius.Modules.Migrations.MSSQL
{
    using System;
    using System.Data.Entity.Migrations;
    
    public partial class addIdToModel : DbMigration
    {
        public override void Up()
        {
            RenameColumn(table: "dbo.TapestryDesigner_ResourceItems", name: "ParentRule_Id", newName: "ParentRuleId");
            RenameColumn(table: "dbo.TapestryDesigner_WorkflowItems", name: "ParentSwimlane_Id", newName: "ParentSwimlaneId");
            RenameIndex(table: "dbo.TapestryDesigner_ResourceItems", name: "IX_ParentRule_Id", newName: "IX_ParentRuleId");
            RenameIndex(table: "dbo.TapestryDesigner_WorkflowItems", name: "IX_ParentSwimlane_Id", newName: "IX_ParentSwimlaneId");
        }
        
        public override void Down()
        {
            RenameIndex(table: "dbo.TapestryDesigner_WorkflowItems", name: "IX_ParentSwimlaneId", newName: "IX_ParentSwimlane_Id");
            RenameIndex(table: "dbo.TapestryDesigner_ResourceItems", name: "IX_ParentRuleId", newName: "IX_ParentRule_Id");
            RenameColumn(table: "dbo.TapestryDesigner_WorkflowItems", name: "ParentSwimlaneId", newName: "ParentSwimlane_Id");
            RenameColumn(table: "dbo.TapestryDesigner_ResourceItems", name: "ParentRuleId", newName: "ParentRule_Id");
        }
    }
}

namespace FSS.Omnius.Modules.Migrations
{
    using System;
    using System.Data.Entity.Migrations;
    
    public partial class cascadeDelete : DbMigration
    {
        public override void Up()
        {
            DropForeignKey("dbo.MozaicEditor_Components", "MozaicEditorPage_Id", "dbo.MozaicEditor_Pages");
            AddForeignKey("dbo.MozaicEditor_Components", "MozaicEditorPage_Id", "dbo.MozaicEditor_Pages", "Id", cascadeDelete: true);
            DropForeignKey("dbo.TapestryDesigner_ResourceItems", "ParentRule_Id", "dbo.TapestryDesigner_ResourceRules");
            AddForeignKey("dbo.TapestryDesigner_ResourceItems", "ParentRule_Id", "dbo.TapestryDesigner_ResourceRules", "Id", cascadeDelete: true);
            DropForeignKey("dbo.TapestryDesigner_Connections", "ResourceRuleId", "dbo.TapestryDesigner_ResourceRules");
            AddForeignKey("dbo.TapestryDesigner_Connections", "ResourceRuleId", "dbo.TapestryDesigner_ResourceRules", "Id", cascadeDelete: true);
            DropForeignKey("dbo.TapestryDesigner_WorkflowItems", "ParentSwimlane_Id", "dbo.TapestryDesigner_Swimlanes");
            AddForeignKey("dbo.TapestryDesigner_WorkflowItems", "ParentSwimlane_Id", "dbo.TapestryDesigner_Swimlanes", "Id", cascadeDelete: true);
        }
        
        public override void Down()
        {
            DropForeignKey("dbo.MozaicEditor_Components", "MozaicEditorPage_Id", "dbo.MozaicEditor_Pages");
            AddForeignKey("dbo.MozaicEditor_Components", "MozaicEditorPage_Id", "dbo.MozaicEditor_Pages", "Id", cascadeDelete: false);
            DropForeignKey("dbo.TapestryDesigner_ResourceItems", "ParentRule_Id", "dbo.TapestryDesigner_ResourceRules");
            AddForeignKey("dbo.TapestryDesigner_ResourceItems", "ParentRule_Id", "dbo.TapestryDesigner_ResourceRules", "Id", cascadeDelete: false);
            DropForeignKey("dbo.TapestryDesigner_Connections", "ResourceRuleId", "dbo.TapestryDesigner_ResourceRules");
            AddForeignKey("dbo.TapestryDesigner_Connections", "ResourceRuleId", "dbo.TapestryDesigner_ResourceRules", "Id", cascadeDelete: false);
            DropForeignKey("dbo.TapestryDesigner_WorkflowItems", "ParentSwimlane_Id", "dbo.TapestryDesigner_Swimlanes");
            AddForeignKey("dbo.TapestryDesigner_WorkflowItems", "ParentSwimlane_Id", "dbo.TapestryDesigner_Swimlanes", "Id", cascadeDelete: false);
        }
    }
}

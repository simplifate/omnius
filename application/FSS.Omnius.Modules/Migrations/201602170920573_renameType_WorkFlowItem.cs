namespace FSS.Omnius.Modules.Migrations
{
    using System;
    using System.Data.Entity.Migrations;
    
    public partial class renameType_WorkFlowItem : DbMigration
    {
        public override void Up()
        {
            RenameColumn("dbo.TapestryDesigner_WorkflowSymbols", "Type", "TypeClass");
        }
        
        public override void Down()
        {
            RenameColumn("dbo.TapestryDesigner_WorkflowSymbols", "TypeClass", "Type");
        }
    }
}

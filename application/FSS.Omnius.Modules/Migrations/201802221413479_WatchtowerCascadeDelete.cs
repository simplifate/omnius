namespace FSS.Omnius.Modules.Migrations
{
    using System;
    using System.Data.Entity.Migrations;
    
    public partial class WatchtowerCascadeDelete : DbMigration
    {
        public override void Up()
        {
            Sql("CREATE TRIGGER [TRG_C:Watchtower_LogItems] ON [Watchtower_LogItems] INSTEAD OF DELETE AS BEGIN DELETE FROM [Watchtower_LogItems] WHERE [ParentLogItemId] IN (SELECT [Id] FROM deleted); DELETE FROM [Watchtower_LogItems] WHERE [Id] IN (SELECT [Id] FROM deleted); END");
        }
        
        public override void Down()
        {
            Sql("DROP TRIGGER [TRG_C:Watchtower_LogItems];");
        }
    }
}

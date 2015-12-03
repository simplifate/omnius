namespace FSS.Omnius.Modules.Migrations
{
    using System;
    using System.Data.Entity.Migrations;
    
    public partial class addnexusws : DbMigration
    {
        public override void Up()
        {
            CreateTable(
                "dbo.Nexus_WS",
                c => new
                    {
                        Id = c.Int(nullable: false, identity: true),
                        Name = c.String(maxLength: 255),
                        WSDL_Url = c.String(maxLength: 255),
                        WSDL_File = c.Binary(),
                        Auth_User = c.String(),
                        Auth_Password = c.String(),
                    })
                .PrimaryKey(t => t.Id);
            
        }
        
        public override void Down()
        {
            DropTable("dbo.Nexus_WS");
        }
    }
}

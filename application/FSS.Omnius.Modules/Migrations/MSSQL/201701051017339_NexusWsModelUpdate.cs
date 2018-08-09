namespace FSS.Omnius.Modules.Migrations.MSSQL
{
    using System;
    using System.Data.Entity.Migrations;
    
    public partial class NexusWsModelUpdate : DbMigration
    {
        public override void Up()
        {
            AddColumn("dbo.Nexus_WS", "SOAP_Endpoint", c => c.String());
            AddColumn("dbo.Nexus_WS", "XML_NS_SOAP", c => c.String());
            AddColumn("dbo.Nexus_WS", "XML_NS_URN", c => c.String());
        }
        
        public override void Down()
        {
            DropColumn("dbo.Nexus_WS", "XML_NS_URN");
            DropColumn("dbo.Nexus_WS", "XML_NS_SOAP");
            DropColumn("dbo.Nexus_WS", "SOAP_Endpoint");
        }
    }
}

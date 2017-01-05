namespace FSS.Omnius.Modules.Migrations
{
    using System;
    using System.Data.Entity.Migrations;
    
    public partial class NexusWsModelUpdate2 : DbMigration
    {
        public override void Up()
        {
            AddColumn("dbo.Nexus_WS", "SOAP_XML_NS", c => c.String());
            DropColumn("dbo.Nexus_WS", "XML_NS_SOAP");
            DropColumn("dbo.Nexus_WS", "XML_NS_URN");
        }
        
        public override void Down()
        {
            AddColumn("dbo.Nexus_WS", "XML_NS_URN", c => c.String());
            AddColumn("dbo.Nexus_WS", "XML_NS_SOAP", c => c.String());
            DropColumn("dbo.Nexus_WS", "SOAP_XML_NS");
        }
    }
}

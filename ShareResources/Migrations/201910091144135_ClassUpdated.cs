namespace ShareResources.Migrations
{
    using System;
    using System.Data.Entity.Migrations;
    
    public partial class ClassUpdated : DbMigration
    {
        public override void Up()
        {
            AddColumn("dbo.Users", "PhotoFileName", c => c.String());
            AddColumn("dbo.Users", "Data", c => c.Binary());
            DropColumn("dbo.Users", "PhotoFile");
        }
        
        public override void Down()
        {
            AddColumn("dbo.Users", "PhotoFile", c => c.String());
            DropColumn("dbo.Users", "Data");
            DropColumn("dbo.Users", "PhotoFileName");
        }
    }
}

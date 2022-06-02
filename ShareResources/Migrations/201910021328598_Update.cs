namespace ShareResources.Migrations
{
    using System;
    using System.Data.Entity.Migrations;
    
    public partial class Update : DbMigration
    {
        public override void Up()
        {
            AddColumn("dbo.Users", "AccountNum", c => c.String());
            DropColumn("dbo.Users", "AccountId");
        }
        
        public override void Down()
        {
            AddColumn("dbo.Users", "AccountId", c => c.Int(nullable: false));
            DropColumn("dbo.Users", "AccountNum");
        }
    }
}

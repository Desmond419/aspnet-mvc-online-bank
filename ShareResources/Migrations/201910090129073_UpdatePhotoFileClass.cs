namespace ShareResources.Migrations
{
    using System;
    using System.Data.Entity.Migrations;
    
    public partial class UpdatePhotoFileClass : DbMigration
    {
        public override void Up()
        {
            AddColumn("dbo.Users", "PhotoFile", c => c.String());
        }
        
        public override void Down()
        {
            DropColumn("dbo.Users", "PhotoFile");
        }
    }
}

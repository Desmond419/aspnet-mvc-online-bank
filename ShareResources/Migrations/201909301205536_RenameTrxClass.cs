namespace ShareResources.Migrations
{
    using System;
    using System.Data.Entity.Migrations;
    
    public partial class RenameTrxClass : DbMigration
    {
        public override void Up()
        {
            DropForeignKey("dbo.Transactions", "User_Id", "dbo.Users");
            DropIndex("dbo.Transactions", new[] { "User_Id" });
            DropColumn("dbo.Transactions", "UserId");
            RenameColumn(table: "dbo.Transactions", name: "User_Id", newName: "UserId");
            AlterColumn("dbo.Transactions", "UserId", c => c.Int(nullable: false));
            AlterColumn("dbo.Transactions", "UserId", c => c.Int(nullable: false));
            CreateIndex("dbo.Transactions", "UserId");
            AddForeignKey("dbo.Transactions", "UserId", "dbo.Users", "Id", cascadeDelete: true);
        }
        
        public override void Down()
        {
            DropForeignKey("dbo.Transactions", "UserId", "dbo.Users");
            DropIndex("dbo.Transactions", new[] { "UserId" });
            AlterColumn("dbo.Transactions", "UserId", c => c.Int());
            AlterColumn("dbo.Transactions", "UserId", c => c.String());
            RenameColumn(table: "dbo.Transactions", name: "UserId", newName: "User_Id");
            AddColumn("dbo.Transactions", "UserId", c => c.String());
            CreateIndex("dbo.Transactions", "User_Id");
            AddForeignKey("dbo.Transactions", "User_Id", "dbo.Users", "Id");
        }
    }
}

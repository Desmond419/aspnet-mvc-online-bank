namespace ShareResources.Migrations
{
    using System;
    using System.Data.Entity.Migrations;
    
    public partial class AddedTrx : DbMigration
    {
        public override void Up()
        {
            CreateTable(
                "dbo.Transactions",
                c => new
                    {
                        Id = c.Int(nullable: false, identity: true),
                        BankAccName = c.String(),
                        SenderName = c.String(),
                        Type = c.String(),
                        TransactionAmount = c.Int(nullable: false),
                        TransactionDate = c.DateTime(nullable: false),
                        UserId = c.String(),
                        User_Id = c.Int(),
                    })
                .PrimaryKey(t => t.Id)
                .ForeignKey("dbo.Users", t => t.User_Id)
                .Index(t => t.User_Id);
            
        }
        
        public override void Down()
        {
            DropForeignKey("dbo.Transactions", "User_Id", "dbo.Users");
            DropIndex("dbo.Transactions", new[] { "User_Id" });
            DropTable("dbo.Transactions");
        }
    }
}

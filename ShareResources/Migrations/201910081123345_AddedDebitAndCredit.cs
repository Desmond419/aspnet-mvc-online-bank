namespace ShareResources.Migrations
{
    using System;
    using System.Data.Entity.Migrations;
    
    public partial class AddedDebitAndCredit : DbMigration
    {
        public override void Up()
        {
            AddColumn("dbo.Transactions", "Credit", c => c.Decimal(nullable: false, precision: 18, scale: 2));
            AddColumn("dbo.Transactions", "Debit", c => c.Decimal(nullable: false, precision: 18, scale: 2));
        }
        
        public override void Down()
        {
            DropColumn("dbo.Transactions", "Debit");
            DropColumn("dbo.Transactions", "Credit");
        }
    }
}

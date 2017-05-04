namespace Milton.Database.Migrations
{
    using System;
    using System.Data.Entity.Migrations;
    
    public partial class HospitalRates : DbMigration
    {
        public override void Up()
        {
            AddColumn("dbo.Hospital", "BotswanaPrice", c => c.Decimal(precision: 18, scale: 2));
            AddColumn("dbo.Hospital", "BotswanaReturnPrice", c => c.Decimal(precision: 18, scale: 2));
            AddColumn("dbo.Hospital", "SwazilandPrice", c => c.Decimal(precision: 18, scale: 2));
            AddColumn("dbo.Hospital", "SwazilandReturnPrice", c => c.Decimal(precision: 18, scale: 2));
            DropColumn("dbo.Hospital", "Price");
        }
        
        public override void Down()
        {
            AddColumn("dbo.Hospital", "Price", c => c.Decimal(nullable: false, precision: 18, scale: 2));
            DropColumn("dbo.Hospital", "SwazilandReturnPrice");
            DropColumn("dbo.Hospital", "SwazilandPrice");
            DropColumn("dbo.Hospital", "BotswanaReturnPrice");
            DropColumn("dbo.Hospital", "BotswanaPrice");
        }
    }
}

namespace Milton.Database.Migrations
{
    using System;
    using System.Data.Entity.Migrations;
    
    public partial class BorderTrips : DbMigration
    {
        public override void Up()
        {
            //DropIndex("dbo.BorderTrip", new[] { "RegionId" });
            //DropForeignKey("dbo.BorderTrip", "RegionId", "dbo.Region");
            AddColumn("dbo.BorderTrip", "Name", c => c.String(nullable: false, maxLength: 1024, storeType: "nvarchar"));
            AddColumn("dbo.BorderTrip", "Amount", c => c.Decimal(nullable: false, precision: 18, scale: 2));
            AddColumn("dbo.BorderTrip", "Description", c => c.String(nullable: false, maxLength: 1024, storeType: "nvarchar"));
            DropColumn("dbo.BorderTrip", "RegionId");
            DropColumn("dbo.BorderTrip", "ReturnTrip");
        }
        
        public override void Down()
        {
            AddColumn("dbo.BorderTrip", "ReturnTrip", c => c.Boolean(nullable: false));
            AddColumn("dbo.BorderTrip", "RegionId", c => c.Int(nullable: false));
            DropColumn("dbo.BorderTrip", "Description");
            DropColumn("dbo.BorderTrip", "Amount");
            DropColumn("dbo.BorderTrip", "Name");
            CreateIndex("dbo.BorderTrip", "RegionId");
            AddForeignKey("dbo.BorderTrip", "RegionId", "dbo.Region", "RegionId");
        }
    }
}

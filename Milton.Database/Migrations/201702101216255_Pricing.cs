namespace Milton.Database.Migrations
{
    using System;
    using System.Data.Entity.Migrations;
    
    public partial class Pricing : DbMigration
    {
        public override void Up()
        {
            CreateTable(
                "dbo.AccommodationRate",
                c => new
                    {
                        AccommodationRateId = c.Int(nullable: false, identity: true),
                        RegionId = c.Int(nullable: false),
                        AccommodationAmountChild = c.Decimal(nullable: false, precision: 18, scale: 2),
                        AccommodationAmountAdult = c.Decimal(nullable: false, precision: 18, scale: 2),
                        MealAmountChild = c.Decimal(nullable: false, precision: 18, scale: 2),
                        MealAmountAdult = c.Decimal(nullable: false, precision: 18, scale: 2),
                        CreatedOnUtc = c.DateTime(nullable: false, precision: 0),
                        ModifiedOnUtc = c.DateTime(nullable: false, precision: 0),
                        ModifiedByUserId = c.Int(nullable: false),
                    })
                .PrimaryKey(t => t.AccommodationRateId)
                .ForeignKey("dbo.Region", t => t.RegionId)
                .Index(t => t.RegionId);
            
            CreateTable(
                "dbo.TransportRate",
                c => new
                    {
                        TransportRateId = c.Int(nullable: false, identity: true),
                        RegionId = c.Int(nullable: false),
                        HospitalId = c.Int(nullable: false),
                        ServiceTypeEnum = c.String(nullable: false, maxLength: 16, storeType: "nvarchar"),
                        Amount = c.Decimal(nullable: false, precision: 18, scale: 2),
                        CreatedOnUtc = c.DateTime(nullable: false, precision: 0),
                        ModifiedOnUtc = c.DateTime(nullable: false, precision: 0),
                        ModifiedByUserId = c.Int(nullable: false),
                    })
                .PrimaryKey(t => t.TransportRateId)
                .ForeignKey("dbo.Hospital", t => t.HospitalId)
                .ForeignKey("dbo.Region", t => t.RegionId)
                .Index(t => t.RegionId)
                .Index(t => t.HospitalId);
            
        }
        
        public override void Down()
        {
            DropForeignKey("dbo.TransportRate", "RegionId", "dbo.Region");
            DropForeignKey("dbo.TransportRate", "HospitalId", "dbo.Hospital");
            DropForeignKey("dbo.AccommodationRate", "RegionId", "dbo.Region");
            DropIndex("dbo.TransportRate", new[] { "HospitalId" });
            DropIndex("dbo.TransportRate", new[] { "RegionId" });
            DropIndex("dbo.AccommodationRate", new[] { "RegionId" });
            DropTable("dbo.TransportRate");
            DropTable("dbo.AccommodationRate");
        }
    }
}

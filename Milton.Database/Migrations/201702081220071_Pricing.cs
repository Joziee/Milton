namespace Milton.Database.Migrations
{
    using System;
    using System.Data.Entity.Migrations;
    
    public partial class Pricing : DbMigration
    {
        public override void Up()
        {
            CreateTable(
                "dbo.Price",
                c => new
                    {
                        PriceId = c.Int(nullable: false, identity: true),
                        RegionId = c.Int(nullable: false),
                        HospitalId = c.Int(nullable: false),
                        ServiceTypeEnum = c.String(nullable: false, maxLength: 16, storeType: "nvarchar"),
                        Amount = c.Decimal(nullable: false, precision: 18, scale: 2),
                        CreatedOnUtc = c.DateTime(nullable: false, precision: 0),
                        ModifiedOnUtc = c.DateTime(nullable: false, precision: 0),
                        ModifiedByUserId = c.Int(nullable: false),
                    })
                .PrimaryKey(t => t.PriceId)
                .ForeignKey("dbo.Hospital", t => t.HospitalId)
                .ForeignKey("dbo.Region", t => t.RegionId)
                .Index(t => t.RegionId)
                .Index(t => t.HospitalId);
            
        }
        
        public override void Down()
        {
            DropForeignKey("dbo.Price", "RegionId", "dbo.Region");
            DropForeignKey("dbo.Price", "HospitalId", "dbo.Hospital");
            DropIndex("dbo.Price", new[] { "HospitalId" });
            DropIndex("dbo.Price", new[] { "RegionId" });
            DropTable("dbo.Price");
        }
    }
}

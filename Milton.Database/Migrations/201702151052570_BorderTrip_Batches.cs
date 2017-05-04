namespace Milton.Database.Migrations
{
    using System;
    using System.Data.Entity.Migrations;
    
    public partial class BorderTrip_Batches : DbMigration
    {
        public override void Up()
        {
            CreateTable(
                "dbo.BorderTripBatch",
                c => new
                    {
                        BatchId = c.Int(nullable: false),
                        BorderTripId = c.Int(nullable: false),
                    })
                .PrimaryKey(t => new { t.BatchId, t.BorderTripId })
                .ForeignKey("dbo.Batch", t => t.BatchId, cascadeDelete: true)
                .ForeignKey("dbo.BorderTrip", t => t.BorderTripId, cascadeDelete: true)
                .Index(t => t.BatchId)
                .Index(t => t.BorderTripId);
            
        }
        
        public override void Down()
        {
            DropForeignKey("dbo.BorderTripBatch", "BorderTripId", "dbo.BorderTrip");
            DropForeignKey("dbo.BorderTripBatch", "BatchId", "dbo.Batch");
            DropIndex("dbo.BorderTripBatch", new[] { "BorderTripId" });
            DropIndex("dbo.BorderTripBatch", new[] { "BatchId" });
            DropTable("dbo.BorderTripBatch");
        }
    }
}

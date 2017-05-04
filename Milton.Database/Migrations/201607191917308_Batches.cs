namespace Milton.Database.Migrations
{
    using System;
    using System.Data.Entity.Migrations;
    
    public partial class Batches : DbMigration
    {
        public override void Up()
        {
            CreateTable(
                "dbo.Batch",
                c => new
                    {
                        BatchId = c.Int(nullable: false, identity: true),
                        SubmissionDate = c.DateTime(nullable: false, precision: 0),
                        CreatedOnUtc = c.DateTime(nullable: false, precision: 0),
                        ModifiedOnUtc = c.DateTime(nullable: false, precision: 0),
                        ModifiedByUserId = c.Int(nullable: false),
                    })
                .PrimaryKey(t => t.BatchId);
            
            CreateTable(
                "dbo.AccountBatch",
                c => new
                    {
                        BatchId = c.Int(nullable: false),
                        AccountId = c.Int(nullable: false),
                    })
                .PrimaryKey(t => new { t.BatchId, t.AccountId })
                .ForeignKey("dbo.Batch", t => t.BatchId, cascadeDelete: true)
                .ForeignKey("dbo.Account", t => t.AccountId, cascadeDelete: true)
                .Index(t => t.BatchId)
                .Index(t => t.AccountId);
            
        }
        
        public override void Down()
        {
            DropForeignKey("dbo.AccountBatch", "AccountId", "dbo.Account");
            DropForeignKey("dbo.AccountBatch", "BatchId", "dbo.Batch");
            DropIndex("dbo.AccountBatch", new[] { "AccountId" });
            DropIndex("dbo.AccountBatch", new[] { "BatchId" });
            DropTable("dbo.AccountBatch");
            DropTable("dbo.Batch");
        }
    }
}

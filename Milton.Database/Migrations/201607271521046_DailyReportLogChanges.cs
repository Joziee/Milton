namespace Milton.Database.Migrations
{
    using System;
    using System.Data.Entity.Migrations;
    
    public partial class DailyReportLogChanges : DbMigration
    {
        public override void Up()
        {
            CreateTable(
                "dbo.DailyReportLog",
                c => new
                    {
                        DailyReportLogId = c.Int(nullable: false, identity: true),
                        LastSent = c.DateTime(nullable: false, precision: 0),
                        CreatedOnUtc = c.DateTime(nullable: false, precision: 0),
                        ModifiedOnUtc = c.DateTime(nullable: false, precision: 0),
                        ModifiedByUserId = c.Int(nullable: false),
                    })
                .PrimaryKey(t => t.DailyReportLogId);
            
        }
        
        public override void Down()
        {
            DropTable("dbo.DailyReportLog");
        }
    }
}

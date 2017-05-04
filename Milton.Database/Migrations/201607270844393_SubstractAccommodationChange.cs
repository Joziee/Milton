namespace Milton.Database.Migrations
{
    using System;
    using System.Data.Entity.Migrations;
    
    public partial class SubstractAccommodationChange : DbMigration
    {
        public override void Up()
        {
            AlterColumn("dbo.OtherExpense", "SubstractAccommodation", c => c.Boolean());
        }
        
        public override void Down()
        {
            AlterColumn("dbo.OtherExpense", "SubstractAccommodation", c => c.Int());
        }
    }
}

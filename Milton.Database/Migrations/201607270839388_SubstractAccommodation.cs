namespace Milton.Database.Migrations
{
    using System;
    using System.Data.Entity.Migrations;
    
    public partial class SubstractAccommodation : DbMigration
    {
        public override void Up()
        {
            AddColumn("dbo.OtherExpense", "SubstractAccommodation", c => c.Int());
        }
        
        public override void Down()
        {
            DropColumn("dbo.OtherExpense", "SubstractAccommodation");
        }
    }
}

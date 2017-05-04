namespace Milton.Database.Migrations
{
    using System;
    using System.Data.Entity.Migrations;
    
    public partial class BorderTrips_Sent : DbMigration
    {
        public override void Up()
        {
            AddColumn("dbo.BorderTrip", "Sent", c => c.Boolean(nullable: false));
        }
        
        public override void Down()
        {
            DropColumn("dbo.BorderTrip", "Sent");
        }
    }
}

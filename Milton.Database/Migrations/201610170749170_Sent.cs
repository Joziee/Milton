namespace Milton.Database.Migrations
{
    using System;
    using System.Data.Entity.Migrations;
    
    public partial class Sent : DbMigration
    {
        public override void Up()
        {
            AddColumn("dbo.Admit", "Sent", c => c.Boolean(nullable: false));
            AddColumn("dbo.Discharge", "Sent", c => c.Boolean(nullable: false));
            AddColumn("dbo.HospitalTrip", "Sent", c => c.Boolean(nullable: false));
            AddColumn("dbo.OtherExpense", "Sent", c => c.Boolean(nullable: false));
        }
        
        public override void Down()
        {
            DropColumn("dbo.OtherExpense", "Sent");
            DropColumn("dbo.HospitalTrip", "Sent");
            DropColumn("dbo.Discharge", "Sent");
            DropColumn("dbo.Admit", "Sent");
        }
    }
}

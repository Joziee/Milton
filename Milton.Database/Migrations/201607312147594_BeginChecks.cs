namespace Milton.Database.Migrations
{
    using System;
    using System.Data.Entity.Migrations;
    
    public partial class BeginChecks : DbMigration
    {
        public override void Up()
        {
            AddColumn("dbo.Account", "PatientBegin", c => c.Boolean(nullable: false));
            AddColumn("dbo.Account", "GaurdianBegin", c => c.Boolean(nullable: false));
        }
        
        public override void Down()
        {
            DropColumn("dbo.Account", "GaurdianBegin");
            DropColumn("dbo.Account", "PatientBegin");
        }
    }
}

namespace Milton.Database.Migrations
{
    using System;
    using System.Data.Entity.Migrations;
    
    public partial class AdmitDischargePatientGaurdian : DbMigration
    {
        public override void Up()
        {
            AddColumn("dbo.AdmitDischarge", "Patient", c => c.Boolean(nullable: false));
            AddColumn("dbo.AdmitDischarge", "Gaurdian", c => c.Boolean(nullable: false));
        }
        
        public override void Down()
        {
            DropColumn("dbo.AdmitDischarge", "Gaurdian");
            DropColumn("dbo.AdmitDischarge", "Patient");
        }
    }
}

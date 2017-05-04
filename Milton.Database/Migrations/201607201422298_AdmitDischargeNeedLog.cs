namespace Milton.Database.Migrations
{
    using System;
    using System.Data.Entity.Migrations;
    
    public partial class AdmitDischargeNeedLog : DbMigration
    {
        public override void Up()
        {
            AddColumn("dbo.AdmitDischarge", "NeedLog", c => c.Boolean(nullable: false));
        }
        
        public override void Down()
        {
            DropColumn("dbo.AdmitDischarge", "NeedLog");
        }
    }
}

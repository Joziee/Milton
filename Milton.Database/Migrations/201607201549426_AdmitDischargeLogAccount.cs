namespace Milton.Database.Migrations
{
    using System;
    using System.Data.Entity.Migrations;
    
    public partial class AdmitDischargeLogAccount : DbMigration
    {
        public override void Up()
        {
            AddColumn("dbo.Account", "NeedLog", c => c.Boolean(nullable: false));
            DropColumn("dbo.AdmitDischarge", "NeedLog");
        }
        
        public override void Down()
        {
            AddColumn("dbo.AdmitDischarge", "NeedLog", c => c.Boolean(nullable: false));
            DropColumn("dbo.Account", "NeedLog");
        }
    }
}

namespace Milton.Database.Migrations
{
    using System;
    using System.Data.Entity.Migrations;
    
    public partial class GaurdianAuthNumber : DbMigration
    {
        public override void Up()
        {
            AddColumn("dbo.Account", "GaurdianAuthNumber", c => c.String(maxLength: 100, storeType: "nvarchar"));
        }
        
        public override void Down()
        {
            DropColumn("dbo.Account", "GaurdianAuthNumber");
        }
    }
}

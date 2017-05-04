namespace Milton.Database.Migrations
{
    using System;
    using System.Data.Entity.Migrations;
    
    public partial class DoctorName : DbMigration
    {
        public override void Up()
        {
            AddColumn("dbo.Account", "AuthNumber", c => c.String(nullable: false, maxLength: 100, storeType: "nvarchar"));
            AddColumn("dbo.Admit", "DoctorName", c => c.String(nullable: false, maxLength: 100, storeType: "nvarchar"));
        }
        
        public override void Down()
        {
            DropColumn("dbo.Admit", "DoctorName");
            DropColumn("dbo.Account", "AuthNumber");
        }
    }
}

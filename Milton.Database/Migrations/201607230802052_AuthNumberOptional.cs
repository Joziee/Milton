namespace Milton.Database.Migrations
{
    using System;
    using System.Data.Entity.Migrations;
    
    public partial class AuthNumberOptional : DbMigration
    {
        public override void Up()
        {
            AlterColumn("dbo.Account", "AuthNumber", c => c.String(maxLength: 100, storeType: "nvarchar"));
            AlterColumn("dbo.Admit", "DoctorName", c => c.String(maxLength: 100, storeType: "nvarchar"));
        }
        
        public override void Down()
        {
            AlterColumn("dbo.Admit", "DoctorName", c => c.String(nullable: false, maxLength: 100, storeType: "nvarchar"));
            AlterColumn("dbo.Account", "AuthNumber", c => c.String(nullable: false, maxLength: 100, storeType: "nvarchar"));
        }
    }
}

namespace Milton.Database.Migrations
{
    using System;
    using System.Data.Entity.Migrations;
    
    public partial class ReferalHospital : DbMigration
    {
        public override void Up()
        {
            AddColumn("dbo.Account", "HospitalId", c => c.Int());
            CreateIndex("dbo.Account", "HospitalId");
            AddForeignKey("dbo.Account", "HospitalId", "dbo.Hospital", "HospitalId");
        }
        
        public override void Down()
        {
            DropForeignKey("dbo.Account", "HospitalId", "dbo.Hospital");
            DropIndex("dbo.Account", new[] { "HospitalId" });
            DropColumn("dbo.Account", "HospitalId");
        }
    }
}

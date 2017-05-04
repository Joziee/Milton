namespace Milton.Database.Migrations
{
    using System;
    using System.Data.Entity.Migrations;
    
    public partial class AdmitDischarge : DbMigration
    {
        public override void Up()
        {
            //DropForeignKey("dbo.Admissions", "AccountId", "dbo.Account");
            //DropForeignKey("dbo.Admissions", "HospitalId", "dbo.Hospital");
            //DropForeignKey("dbo.Discharges", "AccountId", "dbo.Account");
            //DropForeignKey("dbo.Discharges", "HospitalId", "dbo.Hospital");
            //DropIndex("dbo.Admissions", new[] { "AccountId" });
            //DropIndex("dbo.Admissions", new[] { "HospitalId" });
            //DropIndex("dbo.Discharges", new[] { "AccountId" });
            //DropIndex("dbo.Discharges", new[] { "HospitalId" });
            CreateTable(
                "dbo.AdmitDischarge",
                c => new
                    {
                        AdmitDischargeId = c.Int(nullable: false, identity: true),
                        AccountId = c.Int(nullable: false),
                        HospitalId = c.Int(nullable: false),
                        AdmittedDate = c.DateTime(nullable: false, precision: 0),
                        DischargedDate = c.DateTime(precision: 0),
                        CreatedOnUtc = c.DateTime(nullable: false, precision: 0),
                        ModifiedOnUtc = c.DateTime(nullable: false, precision: 0),
                        ModifiedByUserId = c.Int(nullable: false),
                    })
                .PrimaryKey(t => t.AdmitDischargeId)
                .ForeignKey("dbo.Account", t => t.AccountId)
                .ForeignKey("dbo.Hospital", t => t.HospitalId)
                .Index(t => t.AccountId)
                .Index(t => t.HospitalId);
            
            //DropTable("dbo.Admissions");
            //DropTable("dbo.Discharges");
        }
        
        public override void Down()
        {
            CreateTable(
                "dbo.Discharges",
                c => new
                    {
                        DischargesId = c.Int(nullable: false, identity: true),
                        AccountId = c.Int(nullable: false),
                        HospitalId = c.Int(nullable: false),
                        Date = c.DateTime(nullable: false, precision: 0),
                        CreatedOnUtc = c.DateTime(nullable: false, precision: 0),
                        ModifiedOnUtc = c.DateTime(nullable: false, precision: 0),
                        ModifiedByUserId = c.Int(nullable: false),
                    })
                .PrimaryKey(t => t.DischargesId);
            
            CreateTable(
                "dbo.Admissions",
                c => new
                    {
                        AdmissionsId = c.Int(nullable: false, identity: true),
                        AccountId = c.Int(nullable: false),
                        HospitalId = c.Int(nullable: false),
                        Date = c.DateTime(nullable: false, precision: 0),
                        CreatedOnUtc = c.DateTime(nullable: false, precision: 0),
                        ModifiedOnUtc = c.DateTime(nullable: false, precision: 0),
                        ModifiedByUserId = c.Int(nullable: false),
                    })
                .PrimaryKey(t => t.AdmissionsId);
            
            DropForeignKey("dbo.AdmitDischarge", "HospitalId", "dbo.Hospital");
            DropForeignKey("dbo.AdmitDischarge", "AccountId", "dbo.Account");
            DropIndex("dbo.AdmitDischarge", new[] { "HospitalId" });
            DropIndex("dbo.AdmitDischarge", new[] { "AccountId" });
            DropTable("dbo.AdmitDischarge");
            CreateIndex("dbo.Discharges", "HospitalId");
            CreateIndex("dbo.Discharges", "AccountId");
            CreateIndex("dbo.Admissions", "HospitalId");
            CreateIndex("dbo.Admissions", "AccountId");
            AddForeignKey("dbo.Discharges", "HospitalId", "dbo.Hospital", "HospitalId");
            AddForeignKey("dbo.Discharges", "AccountId", "dbo.Account", "AccountId");
            AddForeignKey("dbo.Admissions", "HospitalId", "dbo.Hospital", "HospitalId");
            AddForeignKey("dbo.Admissions", "AccountId", "dbo.Account", "AccountId");
        }
    }
}

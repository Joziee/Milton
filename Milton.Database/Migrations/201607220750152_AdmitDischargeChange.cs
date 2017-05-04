namespace Milton.Database.Migrations
{
    using System;
    using System.Data.Entity.Migrations;
    
    public partial class AdmitDischargeChange : DbMigration
    {
        public override void Up()
        {
            CreateTable(
                "dbo.Admit",
                c => new
                    {
                        AdmitId = c.Int(nullable: false, identity: true),
                        AccountId = c.Int(nullable: false),
                        HospitalId = c.Int(nullable: false),
                        AdmittedDate = c.DateTime(nullable: false, precision: 0),
                        Patient = c.Boolean(nullable: false),
                        Gaurdian = c.Boolean(nullable: false),
                        CreatedOnUtc = c.DateTime(nullable: false, precision: 0),
                        ModifiedOnUtc = c.DateTime(nullable: false, precision: 0),
                        ModifiedByUserId = c.Int(nullable: false),
                    })
                .PrimaryKey(t => t.AdmitId)
                .ForeignKey("dbo.Account", t => t.AccountId)
                .ForeignKey("dbo.Hospital", t => t.HospitalId)
                .Index(t => t.AccountId)
                .Index(t => t.HospitalId);
            
            CreateTable(
                "dbo.Discharge",
                c => new
                    {
                        DischargeId = c.Int(nullable: false, identity: true),
                        AccountId = c.Int(nullable: false),
                        HospitalId = c.Int(nullable: false),
                        DischargeDate = c.DateTime(nullable: false, precision: 0),
                        Patient = c.Boolean(nullable: false),
                        Gaurdian = c.Boolean(nullable: false),
                        CreatedOnUtc = c.DateTime(nullable: false, precision: 0),
                        ModifiedOnUtc = c.DateTime(nullable: false, precision: 0),
                        ModifiedByUserId = c.Int(nullable: false),
                    })
                .PrimaryKey(t => t.DischargeId)
                .ForeignKey("dbo.Account", t => t.AccountId)
                .ForeignKey("dbo.Hospital", t => t.HospitalId)
                .Index(t => t.AccountId)
                .Index(t => t.HospitalId);
            
        }
        
        public override void Down()
        {
            DropForeignKey("dbo.Discharge", "HospitalId", "dbo.Hospital");
            DropForeignKey("dbo.Discharge", "AccountId", "dbo.Account");
            DropForeignKey("dbo.Admit", "HospitalId", "dbo.Hospital");
            DropForeignKey("dbo.Admit", "AccountId", "dbo.Account");
            DropIndex("dbo.Discharge", new[] { "HospitalId" });
            DropIndex("dbo.Discharge", new[] { "AccountId" });
            DropIndex("dbo.Admit", new[] { "HospitalId" });
            DropIndex("dbo.Admit", new[] { "AccountId" });
            DropTable("dbo.Discharge");
            DropTable("dbo.Admit");
        }
    }
}

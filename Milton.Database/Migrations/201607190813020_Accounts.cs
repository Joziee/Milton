namespace Milton.Database.Migrations
{
    using System;
    using System.Data.Entity.Migrations;
    
    public partial class Accounts : DbMigration
    {
        public override void Up()
        {
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
                .PrimaryKey(t => t.AdmissionsId)
                .ForeignKey("dbo.Account", t => t.AccountId)
                .ForeignKey("dbo.Hospital", t => t.HospitalId)
                .Index(t => t.AccountId)
                .Index(t => t.HospitalId);
            
            CreateTable(
                "dbo.Account",
                c => new
                    {
                        AccountId = c.Int(nullable: false, identity: true),
                        Name = c.String(nullable: false, maxLength: 100, storeType: "nvarchar"),
                        Surname = c.String(nullable: false, maxLength: 100, storeType: "nvarchar"),
                        IdNumber = c.String(nullable: false, maxLength: 30, storeType: "nvarchar"),
                        RegionId = c.Int(nullable: false),
                        DateOfBirth = c.DateTime(nullable: false, precision: 0),
                        Gaurdian = c.Boolean(nullable: false),
                        GaurdianName = c.String(maxLength: 100, storeType: "nvarchar"),
                        GaurdianSurname = c.String(maxLength: 100, storeType: "nvarchar"),
                        GaurdianIdNumber = c.String(maxLength: 30, storeType: "nvarchar"),
                        GaurdianDateOfBirth = c.DateTime(precision: 0),
                        ArrivalDate = c.DateTime(nullable: false, precision: 0),
                        DepartureDate = c.DateTime(precision: 0),
                        AccountClosed = c.Boolean(nullable: false),
                        CreatedOnUtc = c.DateTime(nullable: false, precision: 0),
                        ModifiedOnUtc = c.DateTime(nullable: false, precision: 0),
                        ModifiedByUserId = c.Int(nullable: false),
                    })
                .PrimaryKey(t => t.AccountId)
                .ForeignKey("dbo.Region", t => t.RegionId)
                .Index(t => t.RegionId);
            
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
                .PrimaryKey(t => t.DischargesId)
                .ForeignKey("dbo.Account", t => t.AccountId)
                .ForeignKey("dbo.Hospital", t => t.HospitalId)
                .Index(t => t.AccountId)
                .Index(t => t.HospitalId);
            
            CreateTable(
                "dbo.BorderTrip",
                c => new
                    {
                        BorderTripId = c.Int(nullable: false, identity: true),
                        AccountId = c.Int(nullable: false),
                        RegionId = c.Int(nullable: false),
                        ReturnTrip = c.Boolean(nullable: false),
                        Date = c.DateTime(nullable: false, precision: 0),
                        CreatedOnUtc = c.DateTime(nullable: false, precision: 0),
                        ModifiedOnUtc = c.DateTime(nullable: false, precision: 0),
                        ModifiedByUserId = c.Int(nullable: false),
                    })
                .PrimaryKey(t => t.BorderTripId)
                .ForeignKey("dbo.Account", t => t.AccountId)
                .ForeignKey("dbo.Region", t => t.RegionId)
                .Index(t => t.AccountId)
                .Index(t => t.RegionId);
            
            CreateTable(
                "dbo.HospitalTrip",
                c => new
                    {
                        HospitalTripId = c.Int(nullable: false, identity: true),
                        AccountId = c.Int(nullable: false),
                        HospitalId = c.Int(nullable: false),
                        ReturnTrip = c.Boolean(nullable: false),
                        Date = c.DateTime(nullable: false, precision: 0),
                        CreatedOnUtc = c.DateTime(nullable: false, precision: 0),
                        ModifiedOnUtc = c.DateTime(nullable: false, precision: 0),
                        ModifiedByUserId = c.Int(nullable: false),
                    })
                .PrimaryKey(t => t.HospitalTripId)
                .ForeignKey("dbo.Account", t => t.AccountId)
                .ForeignKey("dbo.Hospital", t => t.HospitalId)
                .Index(t => t.AccountId)
                .Index(t => t.HospitalId);
            
            CreateTable(
                "dbo.OtherExpense",
                c => new
                    {
                        OtherExpenseId = c.Int(nullable: false, identity: true),
                        AccountId = c.Int(nullable: false),
                        Amount = c.Decimal(nullable: false, precision: 18, scale: 2),
                        Description = c.String(nullable: false, maxLength: 1024, storeType: "nvarchar"),
                        Date = c.DateTime(nullable: false, precision: 0),
                        CreatedOnUtc = c.DateTime(nullable: false, precision: 0),
                        ModifiedOnUtc = c.DateTime(nullable: false, precision: 0),
                        ModifiedByUserId = c.Int(nullable: false),
                    })
                .PrimaryKey(t => t.OtherExpenseId)
                .ForeignKey("dbo.Account", t => t.AccountId)
                .Index(t => t.AccountId);
        }
        
        public override void Down()
        {
            DropForeignKey("dbo.OtherExpense", "AccountId", "dbo.Account");
            DropForeignKey("dbo.HospitalTrip", "HospitalId", "dbo.Hospital");
            DropForeignKey("dbo.HospitalTrip", "AccountId", "dbo.Account");
            DropForeignKey("dbo.BorderTrip", "RegionId", "dbo.Region");
            DropForeignKey("dbo.BorderTrip", "AccountId", "dbo.Account");
            DropForeignKey("dbo.Patient", "RegionId", "dbo.Region");
            DropForeignKey("dbo.Patient", "HospitalId3", "dbo.Hospital");
            DropForeignKey("dbo.Patient", "HospitalId2", "dbo.Hospital");
            DropForeignKey("dbo.Patient", "HospitalId", "dbo.Hospital");
            DropForeignKey("dbo.Discharges", "HospitalId", "dbo.Hospital");
            DropForeignKey("dbo.Discharges", "AccountId", "dbo.Account");
            DropForeignKey("dbo.Admissions", "HospitalId", "dbo.Hospital");
            DropForeignKey("dbo.Admissions", "AccountId", "dbo.Account");
            DropForeignKey("dbo.Account", "RegionId", "dbo.Region");
            DropForeignKey("dbo.EntityPicture", "PictureId", "dbo.Picture");
            DropForeignKey("dbo.PictureInstance", "PictureId", "dbo.Picture");
            DropForeignKey("dbo.RolePerson", "RoleId", "dbo.Role");
            DropForeignKey("dbo.RolePerson", "PersonId", "dbo.Person");
            DropIndex("dbo.RolePerson", new[] { "RoleId" });
            DropIndex("dbo.RolePerson", new[] { "PersonId" });
            DropIndex("dbo.OtherExpense", new[] { "AccountId" });
            DropIndex("dbo.HospitalTrip", new[] { "HospitalId" });
            DropIndex("dbo.HospitalTrip", new[] { "AccountId" });
            DropIndex("dbo.BorderTrip", new[] { "RegionId" });
            DropIndex("dbo.BorderTrip", new[] { "AccountId" });
            DropIndex("dbo.Patient", new[] { "HospitalId3" });
            DropIndex("dbo.Patient", new[] { "HospitalId2" });
            DropIndex("dbo.Patient", new[] { "HospitalId" });
            DropIndex("dbo.Patient", new[] { "RegionId" });
            DropIndex("dbo.Discharges", new[] { "HospitalId" });
            DropIndex("dbo.Discharges", new[] { "AccountId" });
            DropIndex("dbo.Account", new[] { "RegionId" });
            DropIndex("dbo.Admissions", new[] { "HospitalId" });
            DropIndex("dbo.Admissions", new[] { "AccountId" });
            DropIndex("dbo.PictureInstance", new[] { "PictureId" });
            DropIndex("dbo.EntityPicture", new[] { "PictureId" });
            DropTable("dbo.RolePerson");
            DropTable("dbo.OtherExpense");
            DropTable("dbo.HospitalTrip");
            DropTable("dbo.BorderTrip");
            DropTable("dbo.Payment");
            DropTable("dbo.Recon");
            DropTable("dbo.Patient");
            DropTable("dbo.HealthShareRecon");
            DropTable("dbo.Discharges");
            DropTable("dbo.Hospital");
            DropTable("dbo.Region");
            DropTable("dbo.Account");
            DropTable("dbo.Admissions");
            DropTable("dbo.Setting");
            DropTable("dbo.PictureInstance");
            DropTable("dbo.Picture");
            DropTable("dbo.EntityPicture");
            DropTable("dbo.Download");
            DropTable("dbo.Person");
            DropTable("dbo.Role");
        }
    }
}

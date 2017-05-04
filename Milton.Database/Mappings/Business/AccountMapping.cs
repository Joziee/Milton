using Milton.Database.Models.Business;
using System;
using System.Collections.Generic;
using System.Data.Entity.ModelConfiguration;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace Milton.Database.Mappings.Business
{
    public class AccountMapping : EntityTypeConfiguration<Account>
    {
        public AccountMapping()
        {
            //Map to table
            this.ToTable("Account");

            //Define primary key
            this.HasKey(a => a.AccountId);

            #region Foreign Keys

            this.HasRequired(x => x.Region)
                .WithMany()
                .HasForeignKey(x => x.RegionId)
                .WillCascadeOnDelete(false);

            this.HasOptional(x => x.Hospital)
                .WithMany()
                .HasForeignKey(x => x.HospitalId)
                .WillCascadeOnDelete(false);

            #endregion

            #region Column Specifications

            this.Property(a => a.CreatedOnUtc).IsRequired();
            this.Property(a => a.ModifiedOnUtc).IsRequired();
            this.Property(a => a.ModifiedByUserId).IsRequired();

            this.Property(a => a.Name).IsRequired().HasMaxLength(100);
            this.Property(a => a.Surname).IsRequired().HasMaxLength(100);
            this.Property(a => a.IdNumber).IsRequired().HasMaxLength(30);
            this.Property(a => a.GaurdianName).IsOptional().HasMaxLength(100);
            this.Property(a => a.GaurdianSurname).IsOptional().HasMaxLength(100);
            this.Property(a => a.GaurdianIdNumber).IsOptional().HasMaxLength(30);
            this.Property(a => a.ArrivalDate).IsRequired();
            this.Property(a => a.Gaurdian).IsRequired();
            this.Property(a => a.DepartureDate).IsOptional();
            this.Property(a => a.AccountClosed).IsRequired();
            this.Property(a => a.DateOfBirth).IsRequired();
            this.Property(a => a.GaurdianDateOfBirth).IsOptional();
            this.Property(a => a.NeedLog).IsRequired();
            this.Property(a => a.AuthNumber).IsOptional().HasMaxLength(100);
            this.Property(a => a.GaurdianAuthNumber).IsOptional().HasMaxLength(100);
            this.Property(a => a.PatientBegin).IsRequired();
            this.Property(a => a.GaurdianBegin).IsRequired();

            #endregion
        }
    }
}

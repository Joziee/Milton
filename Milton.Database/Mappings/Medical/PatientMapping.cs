using Milton.Database.Models.Medical;
using System.Data.Entity.ModelConfiguration;

namespace Milton.Database.Mappings.Medical
{
    public class PatientMapping : EntityTypeConfiguration<Patient>
    {
        public PatientMapping()
        {
            //Map to table
            this.ToTable("Patient");

            //Define primary key
            this.HasKey(a => a.PatientId);

            #region Foreign Keys
            this.HasRequired(x => x.Region)
                .WithMany()
                .HasForeignKey(x => x.RegionId)
                .WillCascadeOnDelete(false);

            this.HasOptional(x => x.Hospital)
                .WithMany()
                .HasForeignKey(x => x.HospitalId)
                .WillCascadeOnDelete(false);

            this.HasOptional(x => x.Hospital2)
                .WithMany()
                .HasForeignKey(x => x.HospitalId2)
                .WillCascadeOnDelete(false);

            this.HasOptional(x => x.Hospital3)
                .WithMany()
                .HasForeignKey(x => x.HospitalId3)
                .WillCascadeOnDelete(false);

            #endregion

            #region Column Specifications

            this.Property(a => a.CreatedOnUtc).IsRequired();
            this.Property(a => a.ModifiedOnUtc).IsRequired();

            this.Property(a => a.Name).IsRequired().HasMaxLength(100);
            this.Property(a => a.Surname).IsRequired().HasMaxLength(100);

            this.Property(a => a.IdNumber).IsRequired().HasMaxLength(50);

            this.Property(a => a.ArrivalDate).IsRequired();
            this.Property(a => a.DepartureDate).IsRequired();

            this.Property(a => a.DaysAccommodation).IsRequired();
            this.Property(a => a.NormalTransport).IsRequired();
            this.Property(a => a.AdmittedTransport).IsRequired();
            this.Property(a => a.DischargedTransport).IsRequired();

            this.Property(a => a.PharmacyAmount).IsOptional();
            this.Property(a => a.Gaurdian).IsRequired();
            this.Property(a => a.GaurdianIdNumber).IsOptional().HasMaxLength(50);
            this.Property(a => a.GaurdianName).IsOptional().HasMaxLength(100);
            this.Property(a => a.GaurdianSurname).IsOptional().HasMaxLength(100);
            this.Property(a => a.GaurdianDaysAccommodation).IsOptional();
            this.Property(a => a.GaurdianNormalTransport).IsOptional();
            this.Property(a => a.GaurdianAdmittedTransport).IsOptional();
            this.Property(a => a.GaurdianDischargedTransport).IsOptional();

            this.Property(a => a.DateOfBirth).IsOptional();
            this.Property(a => a.GaurdianDateOfBirth).IsOptional();
            this.Property(a => a.Done).IsRequired();

            #endregion
        }
    }
}

using Milton.Database.Models.Medical;
using System;
using System.Collections.Generic;
using System.Data.Entity.ModelConfiguration;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace Milton.Database.Mappings.Medical
{
    public class AdmitMapping : EntityTypeConfiguration<Admit>
    {
        public AdmitMapping()
        {
            //Map to table
            this.ToTable("Admit");

            //Define primary key
            this.HasKey(a => a.AdmitId);


            #region Foreign Keys

            this.HasRequired(a => a.Account)
                .WithMany(x => x.Admissions)
                .HasForeignKey(a => a.AccountId)
                .WillCascadeOnDelete(false);

            this.HasRequired(a => a.Hospital)
                .WithMany()
                .HasForeignKey(a => a.HospitalId)
                .WillCascadeOnDelete(false);

            #endregion

            #region Column Specifications

            this.Property(a => a.CreatedOnUtc).IsRequired();
            this.Property(a => a.ModifiedOnUtc).IsRequired();

            this.Property(a => a.AdmittedDate).IsRequired();
            this.Property(a => a.Patient).IsRequired();
            this.Property(a => a.Gaurdian).IsRequired();
            this.Property(a => a.DoctorName).IsOptional().HasMaxLength(100);
            this.Property(a => a.Sent).IsRequired();

            #endregion
        }
    }
}

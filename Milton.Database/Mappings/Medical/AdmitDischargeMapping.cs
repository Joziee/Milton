using Milton.Database.Models.Medical;
using System;
using System.Collections.Generic;
using System.Data.Entity.ModelConfiguration;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace Milton.Database.Mappings.Medical
{
    public class AdmitDischargeMapping : EntityTypeConfiguration<AdmitDischarge>
    {
        public AdmitDischargeMapping()
        {
            //Map to table
            this.ToTable("AdmitDischarge");

            //Define primary key
            this.HasKey(a => a.AdmitDischargeId);


            #region Foreign Keys

            this.HasRequired(a => a.Account)
                .WithMany()
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
            this.Property(a => a.DischargedDate).IsOptional();
            this.Property(a => a.Patient).IsRequired();
            this.Property(a => a.Gaurdian).IsRequired();

            #endregion
        }
    }
}

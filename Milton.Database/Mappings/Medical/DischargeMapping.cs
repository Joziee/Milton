using Milton.Database.Models.Medical;
using System;
using System.Collections.Generic;
using System.Data.Entity.ModelConfiguration;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace Milton.Database.Mappings.Medical
{
    public class DischargeMapping : EntityTypeConfiguration<Discharge>
    {
        public DischargeMapping()
        {
            //Map to table
            this.ToTable("Discharge");

            //Define primary key
            this.HasKey(a => a.DischargeId);


            #region Foreign Keys

            this.HasRequired(a => a.Account)
                .WithMany(x => x.Discharges)
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

            this.Property(a => a.DischargeDate).IsRequired();
            this.Property(a => a.Patient).IsRequired();
            this.Property(a => a.Gaurdian).IsRequired();
            this.Property(a => a.Sent).IsRequired();

            #endregion
        }
    }
}

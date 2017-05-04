using Milton.Database.Models.Business;
using System;
using System.Collections.Generic;
using System.Data.Entity.ModelConfiguration;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace Milton.Database.Mappings.Business
{
    public class HospitalTripMapping : EntityTypeConfiguration<HospitalTrip>
    {
        public HospitalTripMapping()
        {
            //Map to table
            this.ToTable("HospitalTrip");

            //Define primary key
            this.HasKey(a => a.HospitalTripId);

            #region Foreign Keys

            this.HasRequired(x => x.Account)
                .WithMany(x => x.HospitalTrips)
                .HasForeignKey(x => x.AccountId)
                .WillCascadeOnDelete(false);

            this.HasRequired(x => x.Hospital)
                .WithMany()
                .HasForeignKey(x => x.HospitalId)
                .WillCascadeOnDelete(false);

            #endregion

            #region Column Specifications

            this.Property(a => a.CreatedOnUtc).IsRequired();
            this.Property(a => a.ModifiedOnUtc).IsRequired();
            this.Property(a => a.ModifiedByUserId).IsRequired();

            this.Property(a => a.Date).IsRequired();
            this.Property(a => a.ReturnTrip).IsRequired();
            this.Property(a => a.Sent).IsRequired();

            #endregion
        }
    }
}

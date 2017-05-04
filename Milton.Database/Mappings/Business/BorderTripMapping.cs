using Milton.Database.Models.Business;
using System;
using System.Collections.Generic;
using System.Data.Entity.ModelConfiguration;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace Milton.Database.Mappings.Business
{
    public class BorderTripMapping : EntityTypeConfiguration<BorderTrip>
    {
        public BorderTripMapping()
        {
            //Map to table
            this.ToTable("BorderTrip");

            //Define primary key
            this.HasKey(a => a.BorderTripId);

            #region Foreign Keys

            this.HasRequired(x => x.Account)
                .WithMany(x => x.BorderTrips)
                .HasForeignKey(x => x.AccountId)
                .WillCascadeOnDelete(false);

            #endregion

            #region Column Specifications

            this.Property(a => a.CreatedOnUtc).IsRequired();
            this.Property(a => a.ModifiedOnUtc).IsRequired();
            this.Property(a => a.ModifiedByUserId).IsRequired();

            this.Property(a => a.Date).IsRequired();
            this.Property(a => a.Amount).IsRequired();
            this.Property(x => x.Description).IsRequired().HasMaxLength(1024);
            this.Property(x => x.Name).IsRequired().HasMaxLength(1024);

            #endregion
        }
    }
}

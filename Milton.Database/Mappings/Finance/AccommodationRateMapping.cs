using Milton.Database.Models.Finance;
using System;
using System.Collections.Generic;
using System.Data.Entity.ModelConfiguration;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace Milton.Database.Mappings.Finance
{
    public class AccommodationRateMapping : EntityTypeConfiguration<AccommodationRate>
    {
        public AccommodationRateMapping()
        {
            //Map to table
            this.ToTable("AccommodationRate");

            //Define primary key
            this.HasKey(a => a.AccommodationRateId);

            #region Foreign Keys

            this.HasRequired(x => x.Region)
                .WithMany()
                .HasForeignKey(x => x.RegionId)
                .WillCascadeOnDelete(false);

            #endregion

            #region Column Specifications

            this.Property(a => a.CreatedOnUtc).IsRequired();
            this.Property(a => a.ModifiedOnUtc).IsRequired();

            this.Property(a => a.AccommodationAmountAdult).IsRequired();
            this.Property(a => a.AccommodationAmountChild).IsRequired();

            this.Property(a => a.MealAmountAdult).IsRequired();
            this.Property(a => a.MealAmountChild).IsRequired();

            #endregion
        }
    }
}

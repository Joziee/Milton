using Milton.Database.Models.Finance;
using System;
using System.Collections.Generic;
using System.Data.Entity.ModelConfiguration;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace Milton.Database.Mappings.Finance
{
    public class TransportRateMapping : EntityTypeConfiguration<TransportRate>
    {
        public TransportRateMapping()
        {
            //Map to table
            this.ToTable("TransportRate");

            //Define primary key
            this.HasKey(a => a.TransportRateId);

            #region Foreign Keys

            this.HasRequired(x => x.Hospital)
                .WithMany()
                .HasForeignKey(x => x.HospitalId)
                .WillCascadeOnDelete(false);

            this.HasRequired(x => x.Region)
                .WithMany()
                .HasForeignKey(x => x.RegionId)
                .WillCascadeOnDelete(false);

            #endregion

            #region Column Specifications

            this.Property(a => a.CreatedOnUtc).IsRequired();
            this.Property(a => a.ModifiedOnUtc).IsRequired();

            this.Property(a => a.Amount).IsRequired();

            this.Property(a => a.ServiceTypeEnum).IsRequired().HasMaxLength(16);

            this.Ignore(a => a.ServiceType);

            #endregion
        }
    }
}

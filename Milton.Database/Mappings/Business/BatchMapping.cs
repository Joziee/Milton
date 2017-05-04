using Milton.Database.Models.Business;
using System;
using System.Collections.Generic;
using System.Data.Entity.ModelConfiguration;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace Milton.Database.Mappings.Business
{
    public class BatchMapping : EntityTypeConfiguration<Batch>
    {
        public BatchMapping()
        {
            //Map to table
            this.ToTable("Batch");

            //Define primary key
            this.HasKey(a => a.BatchId);

            #region Foreign Keys

            //Roles
            this.HasMany(a => a.Accounts)
                .WithMany(a => a.Batches)
                .Map(a =>
                {
                    a.MapLeftKey("BatchId");
                    a.MapRightKey("AccountId");
                    a.ToTable("AccountBatch");
                });

            //Roles
            this.HasMany(a => a.BorderTrips)
                .WithMany(a => a.Batches)
                .Map(a =>
                {
                    a.MapLeftKey("BatchId");
                    a.MapRightKey("BorderTripId");
                    a.ToTable("BorderTripBatch");
                });

            #endregion

            #region Column Specifications

            this.Property(a => a.CreatedOnUtc).IsRequired();
            this.Property(a => a.ModifiedOnUtc).IsRequired();
            this.Property(a => a.ModifiedByUserId).IsRequired();

            this.Property(a => a.SubmissionDate).IsRequired();

            #endregion
        }
    }
}

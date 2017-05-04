using Milton.Database.Models.Business;
using System.Data.Entity.ModelConfiguration;

namespace Milton.Database.Mappings.Business
{
    public class RegionMapping : EntityTypeConfiguration<Region>
    {
        public RegionMapping()
        {
            //Map to table
            this.ToTable("Region");

            //Define primary key
            this.HasKey(a => a.RegionId);

            #region Column Specifications

            this.Property(a => a.CreatedOnUtc).IsRequired();
            this.Property(a => a.ModifiedOnUtc).IsRequired();
            this.Property(a => a.ModifiedByUserId).IsRequired();

            this.Property(a => a.Name).IsRequired().HasMaxLength(100);

            #endregion
        }
    }
}

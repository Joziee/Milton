using Milton.Database.Models.Medical;
using System;
using System.Collections.Generic;
using System.Data.Entity.ModelConfiguration;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace Milton.Database.Mappings.Medical
{
    public class HospitalMapping : EntityTypeConfiguration<Hospital>
    {
        public HospitalMapping()
        {
            //Map to table
            this.ToTable("Hospital");

            //Define primary key
            this.HasKey(a => a.HospitalId);

            #region Column Specifications

            this.Property(a => a.CreatedOnUtc).IsRequired();
            this.Property(a => a.ModifiedOnUtc).IsRequired();

            this.Property(a => a.Name).IsRequired().HasMaxLength(100);

            this.Property(a => a.BotswanaPrice).IsOptional();
            this.Property(a => a.BotswanaReturnPrice).IsOptional();
            this.Property(a => a.SwazilandPrice).IsOptional();
            this.Property(a => a.SwazilandReturnPrice).IsOptional();

            #endregion
        }
    }
}

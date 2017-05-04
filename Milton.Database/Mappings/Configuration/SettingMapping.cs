using Milton.Database.Models.Configuration;
using System;
using System.Collections.Generic;
using System.Data.Entity.ModelConfiguration;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace Milton.Database.Mappings.Configuration
{
    public partial class SettingConfiguration : EntityTypeConfiguration<Setting>
    {
        public SettingConfiguration()
        {
            //Map to table
            this.ToTable("Setting");

            //Define primary key
            this.HasKey(a => a.SettingId);

            #region Column Specifications

            this.Property(a => a.CreatedOnUtc).IsRequired();
            this.Property(a => a.ModifiedOnUtc).IsRequired();
            this.Property(a => a.Key).IsRequired().HasMaxLength(255);
            this.Property(a => a.Value).IsRequired().HasMaxLength(1024);

            #endregion
        }
    }
}

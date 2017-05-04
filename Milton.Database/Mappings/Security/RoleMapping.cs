using Milton.Database.Models.Security;
using System;
using System.Collections.Generic;
using System.Data.Entity.ModelConfiguration;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace Milton.Database.Mappings.Security
{
    public class RoleMapping : EntityTypeConfiguration<Role>
    {
        public RoleMapping()
        {
            //Map to table
            this.ToTable("Role");

            //Define primary key
            this.HasKey(a => a.RoleId);

            #region Column Specifications

            this.Property(a => a.Name).IsRequired().HasMaxLength(255);
            this.Property(a => a.SystemName).HasMaxLength(255);
            this.Property(a => a.IsSystemRole).IsRequired();
            this.Property(a => a.Enabled).IsRequired();

            #endregion
        }
    }
}

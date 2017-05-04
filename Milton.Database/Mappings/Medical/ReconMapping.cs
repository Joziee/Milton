using Milton.Database.Models.Medical;
using System;
using System.Collections.Generic;
using System.Data.Entity.ModelConfiguration;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace Milton.Database.Mappings.Medical
{
    public class ReconMapping : EntityTypeConfiguration<Recon>
    {
        public ReconMapping()
        {
            //Map to table
            this.ToTable("Recon");

            //Define primary key
            this.HasKey(a => a.ReconId);

            #region Column Specifications

            this.Property(a => a.CreatedOnUtc).IsRequired();
            this.Property(a => a.ModifiedOnUtc).IsRequired();

            this.Property(a => a.Date).IsRequired();
            this.Property(a => a.InvoiceNumber).IsRequired().HasMaxLength(50);
            this.Property(a => a.Total).IsRequired();

            #endregion
        }
    }
}

using Milton.Database.Models.Medical;
using System;
using System.Collections.Generic;
using System.Data.Entity.ModelConfiguration;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace Milton.Database.Mappings.Medical
{
    public class HealthShareReconMapping : EntityTypeConfiguration<HealthShareRecon>
    {
        public HealthShareReconMapping()
        {
            //Map to table
            this.ToTable("HealthShareRecon");

            //Define primary key
            this.HasKey(a => a.HealthShareReconId);

            #region Column Specifications

            this.Property(a => a.CreatedOnUtc).IsRequired();
            this.Property(a => a.ModifiedOnUtc).IsRequired();

            this.Property(a => a.CaptureDate).IsRequired();
            this.Property(a => a.Code).IsRequired().HasMaxLength(100);
            this.Property(a => a.Description).IsRequired().HasMaxLength(500);
            this.Property(a => a.IdNumber).IsRequired().HasMaxLength(50);
            this.Property(a => a.InvoiceDate).IsRequired();
            this.Property(a => a.InvoiceNumber).IsRequired().HasMaxLength(50);
            this.Property(a => a.MedicalAid).IsRequired().HasMaxLength(500);
            this.Property(a => a.Patient).IsRequired().HasMaxLength(500);
            this.Property(a => a.Qty).IsRequired();
            this.Property(a => a.Total).IsRequired();
            this.Property(a => a.Paid).IsRequired();
            this.Property(a => a.TreatingPractitioner).IsRequired().HasMaxLength(500);


            #endregion
        }
    }
}

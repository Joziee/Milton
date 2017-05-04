using Milton.Database.Models.Finance;
using System;
using System.Collections.Generic;
using System.Data.Entity.ModelConfiguration;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace Milton.Database.Mappings.Finance
{
    public class PaymentMapping : EntityTypeConfiguration<Payment>
    {
        public PaymentMapping()
        {
            //Map to table
            this.ToTable("Payment");

            //Define primary key
            this.HasKey(a => a.PaymentId);

            #region Column Specifications

            this.Property(a => a.CreatedOnUtc).IsRequired();
            this.Property(a => a.ModifiedOnUtc).IsRequired();

            this.Property(a => a.InvoiceNumber).IsRequired().HasMaxLength(50);
            this.Property(a => a.Amount).IsRequired();
            this.Property(a => a.ActionDate).IsRequired();


            #endregion
        }
    }
}

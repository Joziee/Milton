using Milton.Database.Models.Business;
using System;
using System.Collections.Generic;
using System.Data.Entity.ModelConfiguration;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace Milton.Database.Mappings.Business
{
    public class OtherExpenseMapping : EntityTypeConfiguration<OtherExpense>
    {
        public OtherExpenseMapping()
        {
            //Map to table
            this.ToTable("OtherExpense");

            //Define primary key
            this.HasKey(a => a.OtherExpenseId);

            #region Foreign Keys

            this.HasRequired(x => x.Account)
                .WithMany(x => x.OtherExpenses)
                .HasForeignKey(x => x.AccountId)
                .WillCascadeOnDelete(false);

            #endregion

            #region Column Specifications

            this.Property(a => a.CreatedOnUtc).IsRequired();
            this.Property(a => a.ModifiedOnUtc).IsRequired();
            this.Property(a => a.ModifiedByUserId).IsRequired();

            this.Property(a => a.Amount).IsRequired();
            this.Property(a => a.Date).IsRequired();
            this.Property(a => a.Description).IsRequired().HasMaxLength(1024);
            this.Property(a => a.SubstractAccommodation).IsOptional();
            this.Property(a => a.Sent).IsRequired();

            #endregion
        }
    }
}

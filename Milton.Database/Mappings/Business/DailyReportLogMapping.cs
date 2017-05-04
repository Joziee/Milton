using Milton.Database.Models.Business;
using System;
using System.Collections.Generic;
using System.Data.Entity.ModelConfiguration;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace Milton.Database.Mappings.Business
{
    public class DailyReportLogMapping : EntityTypeConfiguration<DailyReportLog>
    {
        public DailyReportLogMapping()
        {
            //Map to table
            this.ToTable("DailyReportLog");

            //Define primary key
            this.HasKey(a => a.DailyReportLogId);

            #region Column Specifications

            this.Property(a => a.CreatedOnUtc).IsRequired();
            this.Property(a => a.ModifiedOnUtc).IsRequired();
            this.Property(a => a.ModifiedByUserId).IsRequired();

            this.Property(a => a.LastSent).IsRequired();

            #endregion
        }
    }
}

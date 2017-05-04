using Milton.Database.Models.Media;
using System;
using System.Collections.Generic;
using System.Data.Entity.ModelConfiguration;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace Milton.Database.Mappings.Media
{
    public class DownloadMapping : EntityTypeConfiguration<Download>
    {
        public DownloadMapping()
        {
            //Map to table
            this.ToTable("Download");

            //Define primary key
            this.HasKey(a => a.DownloadId);


            #region Column Specifications
            this.Property(a => a.CreatedOnUtc).IsRequired();
            this.Property(a => a.ModifiedOnUtc).IsRequired();
            this.Property(a => a.FileName).IsRequired().HasMaxLength(255);
            this.Property(a => a.Extension).IsRequired().HasMaxLength(8);
            this.Property(a => a.MediaStorageEnum).IsRequired().HasMaxLength(16);
            this.Property(a => a.RelativeFolderPath).HasMaxLength(255);
            this.Property(a => a.ContentType).IsRequired().HasMaxLength(255);
            this.Property(a => a.Size).IsRequired();
            this.Property(a => a.DownloadTypeEnum).IsRequired().HasMaxLength(10);

            this.Ignore(a => a.MediaStorage);
            this.Ignore(a => a.DownloadType);
            #endregion
        }
    }
}

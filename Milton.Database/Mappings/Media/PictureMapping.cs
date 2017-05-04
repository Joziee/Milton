using Milton.Database.Models.Media;
using System;
using System.Collections.Generic;
using System.Data.Entity.ModelConfiguration;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace Milton.Database.Mappings.Media
{
    public partial class PictureConfiguration : EntityTypeConfiguration<Picture>
    {
        public PictureConfiguration()
        {
            //Map to table
            this.ToTable("Picture");

            //Define primary key
            this.HasKey(a => a.PictureId);

            #region Column Specifications
            this.Property(a => a.CreatedOnUtc).IsRequired();
            this.Property(a => a.ModifiedOnUtc).IsRequired();
            this.Property(a => a.FormatEnum).HasMaxLength(32).IsRequired();
            this.Property(a => a.FileName).HasMaxLength(255).IsRequired();
            this.Property(a => a.SEOFriendlyName).HasMaxLength(255);
            this.Property(a => a.AlternateText).HasMaxLength(255);
            this.Property(a => a.MimeType).HasMaxLength(50);

            this.Ignore(a => a.Format);
            #endregion
        }
    }
}

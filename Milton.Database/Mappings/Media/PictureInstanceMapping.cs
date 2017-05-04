using Milton.Database.Models.Media;
using System;
using System.Collections.Generic;
using System.Data.Entity.ModelConfiguration;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace Milton.Database.Mappings.Media
{
    public partial class PictureInstanceConfiguration : EntityTypeConfiguration<PictureInstance>
    {
        public PictureInstanceConfiguration()
        {
            //Map to table
            this.ToTable("PictureInstance");

            //Define primary key
            this.HasKey(a => a.PictureInstanceId);

            #region Foreign Keys
            this.HasRequired(a => a.Picture)
                .WithMany(a => a.PictureInstances)
                .WillCascadeOnDelete(true);
            #endregion

            #region Column Specifications
            this.Property(a => a.CreatedOnUtc).IsRequired();
            this.Property(a => a.ModifiedOnUtc).IsRequired();
            this.Property(a => a.RelativePath).HasMaxLength(255);
            this.Property(a => a.Width).IsRequired();
            this.Property(a => a.Height).IsRequired();
            this.Property(a => a.Size).IsRequired();
            this.Property(a => a.StorageEnum).IsRequired().HasMaxLength(32);
            this.Property(a => a.PictureSizeEnum).IsRequired().HasMaxLength(32);

            this.Ignore(a => a.Storage);
            this.Ignore(a => a.PictureSize);
            #endregion
        }
    }
}

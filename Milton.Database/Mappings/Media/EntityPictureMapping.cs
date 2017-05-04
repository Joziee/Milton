using Milton.Database.Models.Media;
using System;
using System.Collections.Generic;
using System.Data.Entity.ModelConfiguration;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace Milton.Database.Mappings.Media
{
    public partial class EntityPictureConfiguration : EntityTypeConfiguration<EntityPicture>
    {
        public EntityPictureConfiguration()
        {
            //Map to table
            this.ToTable("EntityPicture");

            //Define primary key
            this.HasKey(a => new { a.EntityId, a.PictureId, a.EntityTypeEnum });

            #region Foreign Keys
            this.HasRequired(a => a.Picture)
                .WithMany()
                .WillCascadeOnDelete(true);
            #endregion

            #region Column Specifications
            this.Property(a => a.CreatedOnUtc).IsRequired();
            this.Property(a => a.ModifiedOnUtc).IsRequired();
            this.Property(a => a.IsCoverPicture).IsRequired();
            this.Property(a => a.DisplayOrder).IsRequired();

            this.Property(a => a.EntityTypeEnum)
                .IsRequired()
                .HasMaxLength(32);

            this.Property(a => a.EntityId).IsRequired();
            this.Property(a => a.PictureId).IsRequired();

            this.Ignore(a => a.EntityType);
            #endregion
        }
    }
}

using Milton.Database.Common;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace Milton.Database.Models.Media
{
    /// <summary>
	/// Represets an association between an entity and pictures
	/// </summary>
	public partial class EntityPicture : BaseEntity
    {
        #region Constructor
        public EntityPicture()
        {
            this.CreatedOnUtc = DateTime.Now;
            this.ModifiedOnUtc = DateTime.Now;
            this.EntityType = EntityType.None;
        }
        #endregion

        #region Value Properties
        public Int32 EntityId { get; set; }
        public Int32 PictureId { get; set; }
        public DateTime CreatedOnUtc { get; set; }
        public DateTime ModifiedOnUtc { get; set; }
        public Boolean IsCoverPicture { get; set; }
        public Int32 DisplayOrder { get; set; }
        #endregion

        #region Navigation Properties
        public Picture Picture { get; set; }
        #endregion

        #region Enumeration Properties
        /// <summary>
        /// Gets or sets the entity type of the attribute set (internal use only).
        /// </summary>
        public String EntityTypeEnum { get; set; }

        /// <summary>
        /// Gets or sets the entity type associated with this attribute set
        /// </summary>
        public EntityType EntityType
        {
            get { return (EntityType)Enum.Parse(typeof(EntityType), this.EntityTypeEnum); }
            set { this.EntityTypeEnum = value.ToString(); }
        }
        #endregion
    }
}

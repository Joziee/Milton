using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace Milton.Database.Models.Media
{
    /// <summary>
	/// Represents an abstract of a picture of a product, category etc.
	/// </summary>
	/// <remarks>
	/// The Picture object represents an abstract of a picture and contains only the meta data associated with the picture, but not the actual picture data.
	/// The actual picture data is stored either on disk, in the cloud or in the database.
	/// Each picture can have multiple instances, where each instance is resized to approximately the correct dimensions.
	/// </remarks>
	public partial class Picture : BaseEntity
    {
        #region Constructor
        public Picture()
        {
            this.CreatedOnUtc = DateTime.Now;
            this.ModifiedOnUtc = DateTime.Now;
            this.PictureInstances = new HashSet<PictureInstance>();
        }
        #endregion

        #region Value Properties
        public virtual Int32 PictureId { get; set; }
        public virtual DateTime CreatedOnUtc { get; set; }
        public virtual DateTime ModifiedOnUtc { get; set; }
        public virtual String FileName { get; set; }
        public virtual String SEOFriendlyName { get; set; }
        public virtual String AlternateText { get; set; }
        public virtual String MimeType { get; set; }
        #endregion

        #region Navigation Properties
        public virtual ICollection<PictureInstance> PictureInstances { get; set; }
        #endregion

        #region Enumeration Properties
        /// <summary>
        /// Gets or sets the format of the picture (internal use only).
        /// </summary>
        public String FormatEnum { get; set; }

        /// <summary>
        /// Gets or sets the format of the picture
        /// </summary>
        public virtual PictureFormat Format
        {
            get { return (PictureFormat)Enum.Parse(typeof(PictureFormat), this.FormatEnum); }
            set { this.FormatEnum = value.ToString(); }
        }
        #endregion
    }
}

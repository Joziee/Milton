using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace Milton.Database.Models.Media
{
    /// <summary>
    /// Represents an instance of a picture.
    /// </summary>
    /// <remarks>
    /// Each picture is stored in multiple instances with sizes from thumbnail up to the original file.
    /// Whenever a picture is needed, the most appropriate instance for the application should be used.
    /// An instance can either be stored on disk, in the cloud, or in the database.
    /// </remarks>
    public partial class PictureInstance : BaseEntity
    {
        #region Constructor

        public PictureInstance()
        {
            this.CreatedOnUtc = DateTime.Now;
            this.ModifiedOnUtc = DateTime.Now;
            PictureBinary = new Byte[0];
        }

        #endregion

        #region Value Properties

        public virtual Int32 PictureInstanceId { get; set; }
        public virtual DateTime CreatedOnUtc { get; set; }
        public virtual DateTime ModifiedOnUtc { get; set; }
        public virtual Byte[] PictureBinary { get; set; }
        public virtual String RelativePath { get; set; }
        public virtual Int32 Width { get; set; }
        public virtual Int32 Height { get; set; }
        public virtual Int64 Size { get; set; }
        public virtual Int32 PictureId { get; set; }

        #endregion

        #region Navigation Properties

        public virtual Picture Picture { get; set; }

        #endregion

        #region Enumeration Properties
        public String StorageEnum { get; set; }
        public String PictureSizeEnum { get; set; }

        public virtual MediaStorage Storage
        {
            get { return (MediaStorage)Enum.Parse(typeof(MediaStorage), this.StorageEnum); }
            set { this.StorageEnum = value.ToString(); }
        }

        public virtual PictureSize PictureSize
        {
            get { return (PictureSize)Enum.Parse(typeof(PictureSize), this.PictureSizeEnum); }
            set { this.PictureSizeEnum = value.ToString(); }
        }

        #endregion
    }
}

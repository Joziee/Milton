using Milton.Database.Models.Configuration;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace Milton.Database.Models.Media
{
    /// <summary>
    /// Settings for media i.e. pictures, videos, documents etc.
    /// </summary>
    public class MediaSettings : ISettings
    {
        /// <summary>
        /// Set defaults
        /// </summary>
        public MediaSettings()
        {
            this.MediaStorage = Media.MediaStorage.Cloud;

            this.ButtonPictureSize = 60;
            this.ThumbnailPictureSize = 120;
            this.SmallPictureSize = 240;
            this.StandardPictureSize = 480;
            this.LargePictureSize = 640;
            this.ZoomPictureSize = 1280;

            this.StorageRootPath = "/media";
            this.PictureFolderName = "pictures";
            this.VideoFolderName = "videos";
            this.DocumentFolderName = "documents";
            this.ArchiveFolderName = "archives";
            this.MiscFolderName = "misc";

            this.PictureResizeQuality = 80;
            this.LogoPictureSize = 200;
        }

        /// <summary>
        /// Where does media get stored
        /// </summary>
        public MediaStorage MediaStorage { get; set; }

        /* The various sizes to resize pictures */
        public Int32 ButtonPictureSize { get; set; }
        public Int32 ThumbnailPictureSize { get; set; }
        public Int32 SmallPictureSize { get; set; }
        public Int32 StandardPictureSize { get; set; }
        public Int32 LargePictureSize { get; set; }
        public Int32 ZoomPictureSize { get; set; }
        public Int32 LogoPictureSize { get; set; }

        /// <summary>
        /// The relative path where media is stored
        /// </summary>
        public String StorageRootPath { get; set; }

        /* The names of folders where media is stored */
        public String PictureFolderName { get; set; }
        public String VideoFolderName { get; set; }
        public String AudioFolderName { get; set; }
        public String DocumentFolderName { get; set; }
        public String ArchiveFolderName { get; set; }
        public String MiscFolderName { get; set; }

        /// <summary>
        /// The quality to use when resizing pictures
        /// </summary>
        public Int32 PictureResizeQuality { get; set; }
    }
}

using Milton.Database.Models.Business;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace Milton.Database.Models.Media
{
    /// <summary>
	/// A downloadable item, either for purchase or supporting document/file
	/// </summary>
	public partial class Download : BaseEntity
    {
        #region Constructor
        public Download()
        {
        }
        #endregion

        #region Value Properties
        public int DownloadId { get; set; }
        public string FileName { get; set; }
        public string Extension { get; set; }
        public byte[] Binary { get; set; }
        public string RelativeFolderPath { get; set; }
        public string ContentType { get; set; }
        public long Size { get; set; }
        public string DownloadTypeEnum { get; set; }
        #endregion

        #region Enumeration Properties
        public string MediaStorageEnum { get; set; }
        public MediaStorage MediaStorage
        {
            get { return (MediaStorage)Enum.Parse(typeof(MediaStorage), this.MediaStorageEnum); }
            set { this.MediaStorageEnum = value.ToString(); }
        }

        public DownloadType DownloadType
        {
            get { return (DownloadType)Enum.Parse(typeof(DownloadType), this.DownloadTypeEnum); }
            set { this.DownloadTypeEnum = value.ToString(); }
        }
        #endregion
    }
}

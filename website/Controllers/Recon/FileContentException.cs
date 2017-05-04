using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace Milton.Website.Controllers.Recon
{
    public class FileContentException : Exception
    {
        public enum FileContentErrorCode
        {
            ValueExpected = 0,
            InvalidFormat = 1,
            TooFewRecordsFound = 2,
            TooManyRecordsFound = 3
        }

        public struct FileContentError
        {
            public int SheetNumber;
            public int RowNumber;
            public int ColumnNumber;
            public int ErrorCode;
            public string ColumnName;
            public string ErrorMessage;
        }

        public List<FileContentError> Errors { get; set; }

        public FileContentException()
        {
            this.Errors = new List<FileContentError>();
        }
    }
}

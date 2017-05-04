using System;
using System.Collections.Generic;
using System.Linq;
using System.Web;

namespace Milton.Website.Models.Batch
{
    public class BatchViewModel
    {
        public int BatchId { get; set; }
        public string AccountIds { get; set; }
        public string BorderTripIds { get; set; }
        public DateTime SubmissionDate { get; set; }
    }
}
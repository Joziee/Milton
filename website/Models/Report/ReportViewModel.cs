using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace Milton.Website.Models.Report
{
    public class ReportViewModel
    {
        public List<ReportTransportViewModel> Transport { get; set; }
        public List<ReportLodgeViewModel> Lodge { get; set; }
    }
}

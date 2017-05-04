using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace Milton.Website.Models.Report
{
    public class ReportTransportViewModel
    {
        public string Name { get; set; }
        public string Surname { get; set; }
        public string IdNumber { get; set; }
        public int Transport { get; set; }
        public decimal Total { get; set; }
    }
}

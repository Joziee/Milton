using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace Milton.Website.Models.Report
{
    public class ReportLodgeViewModel
    {
        public string Name { get; set; }
        public string Surname { get; set; }
        public string IdNumber { get; set; }
        public int Accommodation { get; set; }
        public decimal Total { get; set; }
    }
}

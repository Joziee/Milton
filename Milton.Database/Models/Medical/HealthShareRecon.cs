using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace Milton.Database.Models.Medical
{
    public class HealthShareRecon : BaseEntity
    {
        public int HealthShareReconId { get; set; }
        public DateTime InvoiceDate { get; set; }
        public DateTime CaptureDate { get; set; }
        public string InvoiceNumber { get; set; }
        public string Patient { get; set; }
        public string IdNumber { get; set; }
        public string TreatingPractitioner { get; set; }
        public string MedicalAid { get; set; }
        public string Code { get; set; }
        public string Description { get; set; }
        public int Qty { get; set; }
        public double Total { get; set; }
        public bool Paid { get; set; }
    }
}

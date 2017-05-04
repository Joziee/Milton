using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace Milton.Database.Models.Medical
{
    public class Recon : BaseEntity
    {
        public int ReconId { get; set; }
        public string InvoiceNumber { get; set; }
        public DateTime Date { get; set; }
        public Decimal Total { get; set; }
    }
}

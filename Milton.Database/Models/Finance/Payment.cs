using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace Milton.Database.Models.Finance
{
    public class Payment : BaseEntity
    {
        public Payment()
        {
            this.CreatedOnUtc = DateTime.Now;
            this.ModifiedOnUtc = DateTime.Now;
            this.ModifiedByUserId = 0;
        }

        public int PaymentId { get; set; }
        public string InvoiceNumber { get; set; }
        public decimal Amount { get; set; }
        public DateTime ActionDate { get; set; }
    }
}

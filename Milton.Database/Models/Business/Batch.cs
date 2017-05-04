using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace Milton.Database.Models.Business
{
    public class Batch : BaseEntity
    {
        public int BatchId { get; set; }
        public List<Account> Accounts { get; set; }
        public List<BorderTrip> BorderTrips { get; set; }
        public DateTime SubmissionDate { get; set; }

    }
}

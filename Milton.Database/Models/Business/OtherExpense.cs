using Milton.Database.Models.Medical;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace Milton.Database.Models.Business
{
    public class OtherExpense : BaseEntity
    {
        public int OtherExpenseId { get; set; }
        public int AccountId { get; set; }
        public Account Account { get; set; }
        public decimal Amount { get; set; }
        public string Description { get; set; }
        public DateTime Date { get; set; }
        public bool SubstractAccommodation { get; set; }
        public bool Sent { get; set; }
    }
}

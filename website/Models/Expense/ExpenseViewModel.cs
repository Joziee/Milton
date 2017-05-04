using System;
using System.Collections.Generic;
using System.Linq;
using System.Web;

namespace Milton.Website.Models.Expense
{
    public class ExpenseViewModel
    {
        public int OtherExpenseId { get; set; }
        public int AccountId { get; set; }
        public string Description { get; set; }
        public decimal Amount { get; set; }
        public string Date { get; set; }
        public bool SubstractAccommodation { get; set; }
    }
}
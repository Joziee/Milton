using System;
using System.Collections.Generic;
using System.Linq;
using System.Web;

namespace Milton.Website.Models.BorderTrip
{
    public class BorderTripViewModel
    {
        public int BorderTripId { get; set; }
        public int AccountId { get; set; }
        public string Name { get; set; }
        public string Description { get; set; }
        public string Date { get; set; }
        public decimal Amount { get; set; }
    }
}
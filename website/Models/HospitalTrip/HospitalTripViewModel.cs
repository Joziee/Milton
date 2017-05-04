using System;
using System.Collections.Generic;
using System.Linq;
using System.Web;

namespace Milton.Website.Models.HospitalTrip
{
    public class HospitalTripViewModel
    {
        public int HospitalTripId { get; set; }
        public int AccountId { get; set; }
        public string HospitalName { get; set; }
        public int HospitalId { get; set; }
        public string Date { get; set; }
        public bool ReturnTrip { get; set; }
    }
}